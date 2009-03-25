/*
 * GameState.h
 *
 *  Created on: Mar 17, 2009
 *      Author: canichols
 */

#ifndef GAMESTATE_H_
#define GAMESTATE_H_

#include "Thread.h"
#include "TankServer.h"

class GameState: public Thread {
private:
	int game_cond_var;
	vector<Client_info> activeClients;
	vector<Client_info> inactiveClients;

public:
	GameState();
	virtual ~GameState();

	/* Implements the abstract method from Thread */
	void run();

	/* Perform a movement */
	void updateClientPosition(Client_info client, int dx, int dy);

	/* A client is quitting */
	void clientQuit(Client_info client);

	/* A client is registering */
	void clientReg(Client_info client);

	/* Broadcast the current state to all clients */
	void broadcastState();
};

#endif /* GAMESTATE_H_ */
