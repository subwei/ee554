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
	bool gameStarted;
	struct sockaddr_in server_addr, broadcast_addr;
	vector<Client_info*> activeClients;
	vector<Client_info*> inactiveClients;

	/* Queuing stuff */
	unsigned sumarrival;
	unsigned sumservice;
	unsigned total_time;
	struct timeval prevarrival;
	int task_count;

	/* Broadcast the current state to all clients */
	void broadcastState();

	/* Update the current state of all bullets */
	void updateState();

	/* End the game & cleanup */
	void endGame();

public:
	GameState();
	virtual ~GameState();

	/* Implements the abstract method from Thread */
	void run();

	/* Allow first player to initialize the game */
	void startGame(Client_info client);

	/* Perform a movement */
	void updateClientPosition(Client_info client);

	/* Perform a Shooting */
	void newShot(Client_info client);

	/* A client is quitting */
	void clientQuit(Client_info client);

	/* A client is registering */
	void clientReg(Client_info client, bool isActive);
	/* Summing current arrival with pervious arrivals */

    /* Finished Task function will store sum times and store times using the previous function calls*/
	void FinishedTask(Client_info client);

};

#endif /* GAMESTATE_H_ */
