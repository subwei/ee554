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
	int next_client_id;
	int sockfd;
	struct sockaddr_in server_addr, broadcast_addr;
	vector<Client_info*> activeClients;
	vector<Client_info*> inactiveClients;

public:
	GameState();
	virtual ~GameState();

	/* Implements the abstract method from Thread */
	void run();

	/* Allow first player to initialize the game */
	void startGame(Client_info client);

	/* Perform a movement */
	void updateClientPosition(Client_info client);

	/* A client is quitting */
	void clientQuit(Client_info client);

	/* A client is registering */
	void clientReg(Client_info client, bool isActive);

	/* Broadcast the current state to all clients */
	void broadcastState();
};

#endif /* GAMESTATE_H_ */
