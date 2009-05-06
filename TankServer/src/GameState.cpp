/*
 * GameState.cpp
 *
 *  Created on: Mar 17, 2009
 *      Author: canichols
 */

#include "GameState.h"

/**********************************
 * Constructor
 **********************************/
GameState::GameState() {
	/* Setting queuing parameters equal to zero for start*/
	sumarrival = 0;
	sumservice = 0;
	prevarrival.tv_sec = 0;
	prevarrival.tv_usec = 0;
	total_time = 0;
	task_count = 0;

	/* Create and initialize a lock for synchronization */
	this->initializeLock();

	/* Create a condition variable for synchronization */
	game_cond_var = this->createConditionVar();

	/* Initialize other Game State variables */
	gameStarted = false;
	next_client_id = 0;

	/* Create the datagram socket */
	sockfd = socket(AF_INET, SOCK_DGRAM, 0);
	if(sockfd < 0){
		cout << "GAME STATE SERVER ERROR: opening socket" << endl;
		return;
	}

	/* Get the servers address */
	bzero((char *) &server_addr, sizeof(server_addr));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = INADDR_ANY;
	server_addr.sin_port = htons(RESPONSE_PORT);

	/* Bind the socket to the address */
	if(bind(sockfd, (struct sockaddr *)&server_addr, sizeof(server_addr)) < 0){
		cout << "SERVER ERROR: binding" << endl;
		return;
	}
}

/***********************************
 * Destructor
 ***********************************/
GameState::~GameState() {
}

/**********************************************
 * This will loop forever sleeping between loops
 * while it waits for additional state changes.
 * After a state change, this will broadcast
 * the new state information.
 *********************************************/
void GameState::run() {
	while(true) {
		this->lock();

		/* Wait for the state to change */
		if(!gameStarted) this->wait(game_cond_var);

		/* Update the current state */
		updateState();

		/* Broadcast the current state */
		broadcastState();

		this->unlock();
		if(gameStarted) usleep(INTERFRAME_DELAY);
	}
}

/***********************************************
 * Methods to start a game (only works for the 1st player)
 ***********************************************/
void GameState::startGame(Client_info client) {
	this->lock();
	if(gameStarted){
		cout << "GameState: Attempt to start, but game is already started" << endl;
		this->unlock();
		return;
	}
	cout << "GameState: Starting game" << endl;

	/* Make sure this is the first player only */
	if(activeClients.size() < MIN_PLAYERS || client.id != activeClients.at(0)->id) {
		cout << "Not enough players or incorrect player attempting to start game!" << activeClients.at(0)->id << endl;
		this->unlock();
		return;
	}

	/* set the clients info */
	for(unsigned i=0; i<activeClients.size(); i++) {
		int orientation;
		int x = BORDER;
		int y = BORDER;
		switch(i) {
		case 0:
			x = BORDER;
			y = BORDER;
			orientation = WEST;
			break;
		case 1:
			x = SCREEN_WIDTH - TANK_WIDTH - BORDER;
			y = SCREEN_HEIGHT - TANK_HEIGHT - BORDER;
			orientation = EAST;
			break;
		case 2:
			x = SCREEN_WIDTH - TANK_WIDTH - BORDER;
			y = BORDER;
			orientation = SOUTH;
			break;
		case 3:
			x = BORDER;
			y = SCREEN_HEIGHT - TANK_HEIGHT - BORDER;
			orientation = NORTH;
			break;
		default:
			x=y=BORDER;
			orientation = UNK;
			break;
		}
		activeClients.at(i)->orientation = orientation;
		activeClients.at(i)->health = 100;
		activeClients.at(i)->x_pos = x;
		activeClients.at(i)->y_pos = y;
		activeClients.at(i)->nextShot = 0;
		for(int j=0; j<MAX_BULLETS; j++) {
//			activeClients.at(i).bullets[j] = (bullet *)malloc(sizeof(bullet));
			activeClients.at(i)->bullets[j].x = x;
			activeClients.at(i)->bullets[j].y = y;
			activeClients.at(i)->bullets[j].orientation = orientation;
		}
	}

	gameStarted = true;
	cout << "GameState: Started game" << endl;
	this->signal(game_cond_var);
	this->unlock();
}

/***********************************************
 * Method to change a clients position
 **********************************************/
void GameState::updateClientPosition(Client_info client) {
	this->lock();
	int dx = 0;
	int dy = 0;
	int index = -1;

	if(!gameStarted) {
		//cout << "Attempt to move without a running game" << endl;
		this->unlock();
		return;
	}
//	cout << "Game State: Updating client position" << endl;

	/* Locate the client to move & adjust his location */
	for(unsigned i=0; i<activeClients.size(); i++) {
		if(activeClients.at(i)->id == client.id) {

			/* Determine the direction to move */
			activeClients.at(i)->orientation = client.orientation;
			switch(client.orientation) {
			case NORTH:
				dy -= TANK_SPEED;
				break;
			case SOUTH:
				dy = TANK_SPEED;
				break;
			case EAST:
				dx = TANK_SPEED;
				break;
			case WEST:
				dx -= TANK_SPEED;
				break;
			case NORTHEAST:
				dy -= TANK_SPEED * sin(45);
				dx = TANK_SPEED * cos(45);
				break;
			case NORTHWEST:
				dy -= TANK_SPEED * sin(45);
				dx -= TANK_SPEED * cos(45);
				break;
			case SOUTHEAST:
				dy = TANK_SPEED * sin(45);
				dx = TANK_SPEED * cos(45);
				break;
			case SOUTHWEST:
				dy = TANK_SPEED * sin(45);
				dx -= TANK_SPEED * cos(45);
				break;
			default:
				cout << "Client is moving in an unknown direction???" << endl;
				break;
			}

			/* Now move the client */
			activeClients.at(i)->x_pos += dx;
			activeClients.at(i)->y_pos += dy;
			index = i;
			break;
		}
	}

	/* Verify a valid move */
	if(index >= 0) {
		if(activeClients.at(index)->x_pos >= SCREEN_WIDTH - TANK_WIDTH - BORDER) {
			activeClients.at(index)->x_pos = SCREEN_WIDTH - TANK_WIDTH - BORDER;
		}else if(activeClients.at(index)->x_pos < BORDER) {
			activeClients.at(index)->x_pos = BORDER;
		}
		if(activeClients.at(index)->y_pos >= SCREEN_HEIGHT - TANK_HEIGHT - BORDER){
			activeClients.at(index)->y_pos = SCREEN_HEIGHT - TANK_HEIGHT - BORDER;
		} else if(activeClients.at(index)->y_pos < BORDER) {
			activeClients.at(index)->y_pos = BORDER;
		}
//		cout << "Tank Position: x=" << activeClients.at(index)->x_pos << " y=" << activeClients.at(index)->y_pos << endl;
	}

	this->signal(game_cond_var);
	this->unlock();
}

/*************************************************
 * A client is shooting
 ************************************************/
void GameState::newShot(Client_info client) {
	this->lock();

	/* Ensure that we are playing a game first */
	if(!gameStarted) {
//		cout << "Attempt to shoot without a running game" << endl;
		this->unlock();
		return;
	}
//	cout << "GameState: Client " << client.id << " is shooting" << endl;

	/* Locate the client and take their shot if needed */
	int dx=0, dy=0;
	for(unsigned i=0; i<activeClients.size(); i++) {
		Client_info *ci = activeClients.at(i);
		if(ci->nextShot >= MAX_BULLETS) continue;
		if(ci->id != client.id) continue;
		switch(ci->orientation) {
		case NORTH:
			dx = TANK_WIDTH/2 - 5;
			break;
		case SOUTH:
			dy = TANK_HEIGHT;
			dx = TANK_WIDTH/2 - 5;
			break;
		case EAST:
			dx = TANK_WIDTH;
			dy = TANK_HEIGHT/2 - 5;
			break;
		case WEST:
			dy = TANK_HEIGHT/2 - 5;
			break;
		default:
			cout << "Unable to move bullet in a known direction" << endl;
			return;
		}

		ci->bullets[ci->nextShot].x = ci->x_pos + dx;
		ci->bullets[ci->nextShot].y = ci->y_pos + dy;
		ci->bullets[ci->nextShot++].orientation = ci->orientation;
	}

	this->unlock();
}

/*************************************************
 * A client is quitting
 ************************************************/
void GameState::clientQuit(Client_info client) {
	this->lock();
	cout << "GameState: Client is quitting" << endl;

	/* Locate the client & free up resources */
	for(unsigned i=0; i<activeClients.size(); i++) {
		if(activeClients.at(i)->id == client.id) {
			Client_info *c = activeClients.at(i);
			activeClients.erase(activeClients.begin() + i);
			/* Free up resources allocated to clients */
//			for(int j=0; j<MAX_BULLETS; j++) {
//				free(c->bullets[j]);
//			}
			free(c);
			break;
		}
	}
	if(activeClients.size() < MIN_PLAYERS) {
		endGame();
		gameStarted = false;
	}
	next_client_id--;

	this->signal(game_cond_var);
	this->unlock();
}

/*************************************************
 * A client is registering
 ************************************************/
void GameState::clientReg(Client_info client, bool isActive) {
	this->lock();
	if(gameStarted) {
		cout << "GameState: registration is closed" << endl;
		this->unlock();
		return;
	}
	cout << "GameState: Registering new client" << endl;

	Client_info* new_client = (Client_info*)malloc(sizeof(Client_info));
	new_client->health = 100;
	new_client->orientation = WEST;
	new_client->x_pos = 0;
	new_client->y_pos = 0;
	new_client->client_addr = client.client_addr;
	new_client->id = next_client_id++;
	new_client->client_addr = client.client_addr;

	/* Add the client to the appropriate list */
	if(isActive && activeClients.size() < MAX_PLAYERS) {
		new_client->state = ACTIVE;
		activeClients.push_back(new_client);
	} else {
		inactiveClients.push_back(new_client);
	}

	/* Send the id to the client */
	char buf[1];
	buf[0] = new_client->id;
	if(sendto(sockfd, buf, 1, 0,
			(struct sockaddr *)&new_client->client_addr, sizeof(new_client->client_addr)) != -1)
		cout << "GameState: sending reg confirmation to " << new_client->id << endl;
//	this->signal(game_cond_var);
	this->unlock();
}

/************************************************
 * Broadcast the current state to all clients
 *  - The lock should be obtained prior to use
 ***********************************************/
void GameState::broadcastState() {
	if(activeClients.size() == 0) {
		cout << "GameState: No active members to update" << endl;
	}

	/* Build the message */
	char buf[BUFFER_SIZE] = {0};
	int j = 0, bullet_count = 0;
	unsigned i = 0;
	buf[j++] = 0xFF;
	buf[j++] = activeClients.size()&0xFF;
	for(i=0; i<activeClients.size(); i++) {
		bullet_count += activeClients.at(i)->nextShot;
	}
	buf[j++] = (char)bullet_count&0xFF;
	for(i=0; i<activeClients.size(); i++) {
		Client_info *ci = activeClients.at(i);
		buf[j++] = (char)(ci->id)&0xff;
		buf[j++] = (char)(ci->x_pos >> 24)&0xFF;
		buf[j++] = (char)(ci->x_pos >> 16)&0xFF;
		buf[j++] = (char)(ci->x_pos >> 8)&0xFF;
		buf[j++] = (char)(ci->x_pos)&0xFF;
		buf[j++] = (char)(ci->y_pos >> 24)&0xFF;
		buf[j++] = (char)(ci->y_pos >> 16)&0xFF;
		buf[j++] = (char)(ci->y_pos >> 8)&0xFF;
		buf[j++] = (char)(ci->y_pos)&0xFF;
		buf[j++] = (char)(ci->orientation)&0xFF;
	}
	for(i=0; i<activeClients.size(); i++) {
		Client_info *ci = activeClients.at(i);
		for(int k=0; k<ci->nextShot; k++) {
			buf[j++] = (char)(i)&0xFF;
			buf[j++] = (char)(ci->bullets[k].x >> 24)&0xFF;
			buf[j++] = (char)(ci->bullets[k].x >> 16)&0xFF;
			buf[j++] = (char)(ci->bullets[k].x >> 8)&0xFF;
			buf[j++] = (char)(ci->bullets[k].x)&0xFF;
			buf[j++] = (char)(ci->bullets[k].y >> 24)&0xFF;
			buf[j++] = (char)(ci->bullets[k].y >> 16)&0xFF;
			buf[j++] = (char)(ci->bullets[k].y >> 8)&0xFF;
			buf[j++] = (char)(ci->bullets[k].y)&0xFF;
		}
	}

	/* Broadcast the game state for all active players */
	for(unsigned i=0; i<activeClients.size(); i++) {
		if(sendto(sockfd, buf, j, 0,
				(struct sockaddr *)&activeClients.at(i)->client_addr, sizeof(activeClients.at(i)->client_addr)) == -1)
			cout << "GameState: Error broadcasting the state to client " << i << "!" << endl;
	}

	/* Broadcast the game state for all inactive players */
	for(unsigned i=0; i<inactiveClients.size(); i++) {
		if(sendto(sockfd, buf, j, 0,
				(struct sockaddr *)&inactiveClients.at(i)->client_addr, sizeof(inactiveClients.at(i)->client_addr)) == -1)
			cout << "GameState: Error broadcasting the state to client " << i << "!" << endl;
	}
}

/* Sbar = sum of s/#of trials
 * Lambda = ( last arrival - gamestart)/N
 *N = Counted in Gamestate.cpp
 *Wbar
 *Wq
 */
void GameState::FinishedTask(Client_info client){
	this->lock();
	struct timeval tv1, tv2;

	/* Find total inter-arrival time */
	if(prevarrival.tv_usec == 0 && prevarrival.tv_sec == 0)
		prevarrival = client.arrival;
	else
	{
		TIME_DIFFERENCE(prevarrival, client.arrival, tv1);
		sumarrival += tv1.tv_sec * 1000000 + tv1.tv_usec;
		prevarrival = client.arrival;
	}

	/* Determine the total service time */
	gettimeofday(&tv2, 0);
	TIME_DIFFERENCE(client.serverEntry, tv2, tv1)
	sumservice += tv1.tv_sec * 1000000 + tv1.tv_usec;

	/* Determine the total system time */
	TIME_DIFFERENCE(client.arrival, tv2, tv1);
	total_time += tv1.tv_sec * 1000000 + tv1.tv_usec;
//	cout << "Time in system = " << tv1.tv_sec * 1000000 + tv1.tv_usec << endl;

	/* Count the number of tasks processed */
	task_count++;
	this->unlock();
}

/************************************************
 * Update the current state to all clients
 *  - The lock should be obtained prior to use
 ***********************************************/
void GameState::updateState() {
	int dy = 0;
	int dx = 0;
	vector<int> dead_bullets, dead_clients;

	/* update bullet positions */
//	cout << "\nupdatestate" << endl;
	for(unsigned i=0; i<activeClients.size(); i++) {
		Client_info *ci = activeClients.at(i);
		for(int j=0; j<ci->nextShot; j++) {
			/* Remove bullets that have hit someone???
			 * If it hits them, then the tank is DEAD!!!! */
			for(unsigned k=0; k<activeClients.size(); k++) {
				if(k == i) continue;
				Client_info *ci2 = activeClients.at(k);
				if(ci->bullets[j].x >= ci2->x_pos + 3 &&
				   ci->bullets[j].x <= ci2->x_pos + TANK_WIDTH - 3){
					if(ci->bullets[j].y >= ci2->y_pos + 3 &&
					   ci->bullets[j].y <= ci2->y_pos + TANK_WIDTH - 3){
						dead_clients.push_back(k);
						dead_bullets.push_back(j);
					}
				}
			}

			/* Determine the orientation & movement */
			dx = 0;
			dy = 0;
			switch(ci->bullets[j].orientation) {
			case NORTH:
				dy -= SHOOTING_SPEED;
				break;
			case SOUTH:
				dy = SHOOTING_SPEED;
				break;
			case EAST:
				dx = SHOOTING_SPEED;
				break;
			case WEST:
				dx -= SHOOTING_SPEED;
				break;
			case NORTHEAST:
				dy -= SHOOTING_SPEED * sin(45);
				dx = SHOOTING_SPEED * cos(45);
				break;
			case NORTHWEST:
				dy -= SHOOTING_SPEED * sin(45);
				dx -= SHOOTING_SPEED * cos(45);
				break;
			case SOUTHEAST:
				dy = SHOOTING_SPEED * sin(45);
				dx = SHOOTING_SPEED * cos(45);
				break;
			case SOUTHWEST:
				dy = SHOOTING_SPEED * sin(45);
				dx -= SHOOTING_SPEED * cos(45);
				break;
			default:
				cout << "Unable to move bullet in a known direction" << endl;
				break;
			}

			/* Now move the bullet */
			ci->bullets[j].x += dx;
			ci->bullets[j].y += dy;

			/* Remove bullets that have left the screen */
			if((ci->bullets[j].x >= SCREEN_WIDTH + TANK_WIDTH || ci->bullets[j].x < -TANK_WIDTH) ||
			   (ci->bullets[j].y >= SCREEN_HEIGHT + TANK_HEIGHT || ci->bullets[j].y < -TANK_HEIGHT))
				dead_bullets.push_back(j);
		}
		if(dead_bullets.size() > 0) {
			if(--(ci->nextShot) < 0) ci->nextShot = 0;
		}
		if(activeClients.size() < MIN_PLAYERS) {
			endGame();
		}
		dead_bullets.clear();
	}

	/* Handle clients who have died */
	for(unsigned k=0; k<dead_clients.size(); k++) {
		Client_info* ci = activeClients.at(dead_clients.at(k));
		activeClients.erase(activeClients.begin() + dead_clients.at(k));
		inactiveClients.push_back(ci);
		//break; // To ensure no seg fault occurs
	}

	/* Check if it is the end of a game */
}

/************************************************
 * End the Game
 *  - The lock should be obtained prior to use
 ***********************************************/
void GameState::endGame() {
	cout << "********************************************************" << endl;
	cout << "Game Has Completed!" << endl;

	/* Find mean service time (s) */
	double mean_s = (double)sumservice / (double)task_count;
	cout << "Total time spent servicing = " << sumservice << " us" << endl;
	cout << "Mean service time = " << mean_s << " us" << endl;
	cout << "Mean service rate = " << 1 / mean_s << " per us" << endl;

	/* Find mean arrival rate (lambda) */
	double mean_arrival = (double)sumarrival / (double)task_count;
	cout << "Mean arrival time = " << mean_arrival << " us" << endl;
	cout << "Mean arrival rate = " << 1 / mean_arrival << " per us" << endl;

	/* Find the utilization factor */
	double utilization = mean_s / mean_arrival;
	cout << "Utilization factor = " << utilization << endl;

	/* Task Count */
	cout << "Number of tasks = " << task_count << endl;

	/* Turn around time & queue time */
	cout << "Mean turn around time = " << (double)total_time / (double)task_count << " us" << endl;
	cout << "Mean waiting time = " << (double)(total_time - sumservice) / (double)task_count << " us" << endl;

	/* Determine if there is a winner & broadcast the state one last time */
	if(activeClients.size() == 1)
		broadcastState();

	/* Cleanup & put the game into an initial state */
	gameStarted = false;
	sumarrival = 0;
	sumservice = 0;
	total_time = 0;
	prevarrival.tv_sec = 0;
	prevarrival.tv_usec = 0;
	task_count = 0;
	activeClients.clear();
	inactiveClients.clear();
	next_client_id = 0;
}
