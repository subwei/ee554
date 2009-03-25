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

	/* Setup the socket to send out state info on */
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
		this->wait(game_cond_var);

		/* Broadcast the current state */
		broadcastState();

		this->unlock();
	}
}

/***********************************************
 * Method to change a clients position
 **********************************************/
void GameState::updateClientPosition(Client_info client, int dx, int dy) {
	this->lock();

	/* Locate the client to move */


	/* Apply the Move */


	this->unlock();
}

/*************************************************
 * A client is quitting
 ************************************************/
void GameState::clientQuit(Client_info client) {
	;
}

/*************************************************
 * A client is registering
 ************************************************/
void GameState::clientReg(Client_info client) {
	;
}

/************************************************
 * Broadcast the current state to all clients
 ***********************************************/
void GameState::broadcastState() {
	;
}
