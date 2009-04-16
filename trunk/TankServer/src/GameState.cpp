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

		/* Broadcast the current state */
		broadcastState();

		this->unlock();
		if(gameStarted) sleep(5);
	}
}

/***********************************************
 * Methods to start a game (only works for the 1st player)
 ***********************************************/
void GameState::startGame(Client_info client) {
	this->lock();
	cout << "GameState: Starting game" << endl;
	if(gameStarted){
		this->unlock();
		return;
	}

	/* Make sure this is the first player only */
	if(activeClients.size() <= 0 || client.id != activeClients.at(0)->id) {
		cout << "Not the first player --> player #" << client.id << " First = " << activeClients.at(0)->id << endl;
		this->unlock();
		return;
	}

	/* set the clients info */
	for(int i=0; i<activeClients.size(); i++) {
		int orientation;
		int x = 0;
		int y = 0;
		switch(i) {
		case 0:
			orientation = WEST;
			break;
		case 1:
			x = SCREEN_WIDTH - TANK_HEIGHT;
			y = SCREEN_HEIGHT - TANK_WIDTH;
			orientation = EAST;
			break;
		case 2:
			x = SCREEN_WIDTH - TANK_HEIGHT;
			orientation = SOUTH;
			break;
		case 3:
			y = SCREEN_HEIGHT - TANK_HEIGHT;
			orientation = NORTH;
			break;
		default:
			orientation = UNK;
			break;
		}
		activeClients.at(i)->orientation = orientation;
		activeClients.at(i)->health = 100;
		activeClients.at(i)->x_pos = x;
		activeClients.at(i)->y_pos = y;
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
		cout << "Attempt to move without a running game" << endl;
		this->unlock();
		return;
	}
	cout << "Game State: Updating client position" << endl;

	/* Locate the client to move & adjust his location */
	for(int i=0; i<activeClients.size(); i++) {
		if(activeClients.at(i)->id == client.id) {

			/* Determine the direction to move */
//			printf("Dir = %x\n", client.orientation);
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
		if(activeClients.at(index)->x_pos >= SCREEN_WIDTH || activeClients.at(index)->x_pos < 0) {
			activeClients.at(index)->x_pos -= dx;
		}
		if(activeClients.at(index)->y_pos >= SCREEN_HEIGHT || activeClients.at(index)->y_pos < 0) {
			activeClients.at(index)->y_pos -= dy;
		}
		cout << "Tank Position: x=" << activeClients.at(index)->x_pos << " y=" << activeClients.at(index)->y_pos << endl;
	}

	this->signal(game_cond_var);
	this->unlock();
}

/*************************************************
 * A client is quitting
 ************************************************/
void GameState::clientQuit(Client_info client) {
	this->lock();
	cout << "GameState: Client is quitting" << endl;

	/* Locate the client & free up resources */
	for(int i=0; i<activeClients.size(); i++) {
		if(activeClients.at(i)->id == client.id) {
			Client_info *c = activeClients.at(i);
			activeClients.erase(activeClients.begin() + i);
			free(c);
			break;
		}
	}

	this->signal(game_cond_var);
	this->unlock();
}

/*************************************************
 * A client is registering
 ************************************************/
void GameState::clientReg(Client_info client, bool isActive) {
	this->lock();
	if(gameStarted) return;
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
	if(isActive && activeClients.size() < 4) {
		new_client->state = ACTIVE;
		activeClients.push_back(new_client);
	} else {
		inactiveClients.push_back(new_client);
	}

	/* Send the id to the client */
	char buf[1];
	buf[0] = new_client->id;
	if(sendto(sockfd, buf, 1, 0,
			(struct sockaddr *)&new_client->client_addr, sizeof(new_client->client_addr)) == -1)
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
	char buf[BUFFER_SIZE];
	int j = 0;
	buf[j++] = 0xFF;
	buf[j++] = activeClients.size()&0xFF;
	for(int i=0; i<activeClients.size(); i++) {
		buf[j++] = (char)(activeClients.at(i)->x_pos >> 24)&0xFF;
		buf[j++] = (char)(activeClients.at(i)->x_pos >> 16)&0xFF;
		buf[j++] = (char)(activeClients.at(i)->x_pos >> 8)&0xFF;
		buf[j++] = (char)(activeClients.at(i)->x_pos)&0xFF;
		buf[j++] = (char)(activeClients.at(i)->y_pos >> 24)&0xFF;
		buf[j++] = (char)(activeClients.at(i)->y_pos >> 16)&0xFF;
		buf[j++] = (char)(activeClients.at(i)->y_pos >> 8)&0xFF;
		buf[j++] = (char)(activeClients.at(i)->y_pos)&0xFF;
		buf[j++] = (char)(activeClients.at(i)->orientation)&0xFF;
	}
//	memset((void *)&buf[j], '\0', BUFFER_SIZE - j);

	/* Broadcast the game state for all active players */
	for(int i=0; i<activeClients.size(); i++) {
		if(sendto(sockfd, buf, j, 0,
				(struct sockaddr *)&activeClients.at(i)->client_addr, sizeof(activeClients.at(i)->client_addr)) == -1)
			cout << "GameState: Error broadcasting the state to client " << i << "!" << endl;
	}

	/* Broadcast the game state for all inactive players */
	for(int i=0; i<inactiveClients.size(); i++) {
		if(sendto(sockfd, buf, j, 0,
				(struct sockaddr *)&inactiveClients.at(i)->client_addr, sizeof(inactiveClients.at(i)->client_addr)) == -1)
			cout << "GameState: Error broadcasting the state to client " << i << "!" << endl;
	}
}
