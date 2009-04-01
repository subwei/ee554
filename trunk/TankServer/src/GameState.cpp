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
	next_client_id = 0;

	/* Create the datagram socket */
	sockfd = socket(AF_INET, SOCK_DGRAM, 0);
	if(sockfd < 0){
		cout << "GAME STATE SERVER ERROR: opening socket" << endl;
		return;
	}

	/* The broadcasting address */
	char bc = '1';
    if(setsockopt(sockfd,SOL_SOCKET,SO_BROADCAST,&bc,sizeof(bc)) < 0) {
        cout<<"Error in setting Broadcast option";
    }
	broadcast_addr.sin_family = AF_INET;
	broadcast_addr.sin_port = RESPONSE_PORT;
	broadcast_addr.sin_addr.s_addr = INADDR_BROADCAST;
	memset(broadcast_addr.sin_zero,'\0',sizeof broadcast_addr.sin_zero);

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
		cout << "GameState Now Running" << endl;

		/* Wait for the state to change */
		this->wait(game_cond_var);

		/* Broadcast the current state */
		broadcastState();

		this->unlock();
	}
}

/***********************************************
 * Methods to start a game (only works for the 1st player)
 ***********************************************/
void GameState::startGame(Client_info client) {
	this->lock();
	cout << "GameState: Starting game" << endl;

	/* Make sure this is the first player only */
	if(activeClients.size() <= 0 || client.id != activeClients.at(0)->id) {
		return;
	}

	/* set the clients info */
	for(int i=0; i<activeClients.size(); i++) {
		client_orientation o;
		int x = 0;
		int y = 0;
		switch(i) {
		case 0:
			o = WEST;
			break;
		case 1:
			x = SCREEN_WIDTH - TANK_HEIGHT;
			y = SCREEN_HEIGHT - TANK_WIDTH;
			o = EAST;
			break;
		case 2:
			x = SCREEN_WIDTH - TANK_HEIGHT;
			o = SOUTH;
			break;
		case 3:
			y = SCREEN_HEIGHT - TANK_HEIGHT;
			o = NORTH;
			break;
		default:
			break;
		}
		activeClients.at(i)->orientation = o;
		activeClients.at(i)->health = 100;
		activeClients.at(i)->x_pos = x;
		activeClients.at(i)->y_pos = y;
	}

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
	cout << "Game State: Updating client position" << endl;

	/* Locate the client to move & adjust his location */
	for(int i=0; i<activeClients.size(); i++) {
		if(activeClients.at(i)->id == client.id) {

			/* Determine the direction to move */
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
		if(activeClients.at(index)->x_pos >= SCREEN_WIDTH) {
			activeClients.at(index)->x_pos -= dx;
		}
		if(activeClients.at(index)->y_pos >= SCREEN_HEIGHT) {
			activeClients.at(index)->y_pos -= dy;
		}
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
	cout << "GameState: Registering new client" << endl;

	Client_info* new_client = (Client_info*)malloc(sizeof(Client_info));
	new_client->health = 100;
	new_client->orientation = WEST;
	new_client->x_pos = 0;
	new_client->y_pos = 0;
	new_client->client_addr = client.client_addr;
	new_client->id = next_client_id++;

	/* Add the client to the appropriate list */
	if(isActive && activeClients.size() < 4) {
		new_client->state = ACTIVE;
		activeClients.push_back(new_client);
	} else {
		inactiveClients.push_back(new_client);
	}

	this->signal(game_cond_var);
	this->unlock();
}

/************************************************
 * Broadcast the current state to all clients
 *  - The lock should be obtained prior to use
 ***********************************************/
void GameState::broadcastState() {
	cout << "Now Broadcasting new state" << endl;

	if(activeClients.size() == 0) {
		cout << "GameState: No active members to update" << endl;
	}

	/* Build the message */
	char buf[10];
	buf[0] = 0xFF;
	buf[1] = (char)(activeClients.at(0)->x_pos >> 24)&0xFF;
	buf[2] = (char)(activeClients.at(0)->x_pos >> 16)&0xFF;
	buf[3] = (char)(activeClients.at(0)->x_pos >> 8)&0xFF;
	buf[4] = (char)(activeClients.at(0)->x_pos)&0xFF;
	buf[5] = (char)(activeClients.at(0)->y_pos >> 24)&0xFF;
	buf[6] = (char)(activeClients.at(0)->y_pos >> 16)&0xFF;
	buf[7] = (char)(activeClients.at(0)->y_pos >> 8)&0xFF;
	buf[8] = (char)(activeClients.at(0)->y_pos)&0xFF;
//	switch(activeCients.at(0)->) {
//
//	}
//	buf[9] = ; /* Orientation */

	/* Broadcast the game state for all active players */
//	if(sendto(sockfd, buf, 10 , 0,
//	        (struct sockaddr *)&broadcast_addr, sizeof broadcast_addr) == -1) {
//		cout << "GameState: Error broadcasting the state!" << endl;
//	}
}
