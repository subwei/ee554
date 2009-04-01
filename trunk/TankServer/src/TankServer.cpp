//============================================================================
// Name        : TankServer.cpp
// Author      : Corey Nichols
// Version     :
// Copyright   : Your copyright notice
// Description : C++, Ansi-style server running a basic Tank game.
//============================================================================

#include "TankServer.h"

using namespace std;

/******************************************************************************
 * Parse an incoming message
 *  Return the Task
 *****************************************************************************/
Task ParseMsg(char* buffer, int length) {
	Task task;
	task.id = -1;
	task.client.orientation = UNK;

	/* First parse the message */
	if(length >= 1) {
		task.id = 0;
		task.client.id = buffer[0]&0x0F;
		task.type = buffer[0]&0xF0;
		task.state = IDLE;

		/* Check if we need to gather the direction from the buffer */
		if(task.type != MSG_MOVE && task.type != MSG_SHOOT && length > 1) {
			switch(buffer[1]) {
			case 1:
				task.client.orientation = NORTH;
				break;
			case 2:
				task.client.orientation = SOUTH;
				break;
			case 3:
				task.client.orientation = EAST;
				break;
			case 4:
				task.client.orientation = WEST;
				break;
			case 5:
				task.client.orientation = NORTHEAST;
				break;
			case 6:
				task.client.orientation = NORTHWEST;
				break;
			case 7:
				task.client.orientation = SOUTHEAST;
				break;
			case 8:
				task.client.orientation = SOUTHWEST;
				break;
			default:
				cout << "Unknown direction" << endl;
				break;
			}
		} else if(task.type & MSG_REGISTER) {
			/* Anything special for a registration??? */
		}
	}
	else {
		cout << "SERVER ERROR: ParseMsg failed" << endl;
	}

	return task;
}

/******************************************************************************
 * Receives all of the UDP messages ==> Essentially this is the server
 *****************************************************************************/
void RunServer(Scheduler *scheduler) {
	int sockfd, length;
	int taskID = 0;
	char buffer[BUFFER_SIZE];
	struct sockaddr_in server_addr, client_addr;

	cout << "Starting to run the server" << endl;

	/* Create the datagram socket */
	sockfd = socket(AF_INET, SOCK_DGRAM, 0);
	if(sockfd < 0)
	{
		cout << "SERVER ERROR: opening socket" << endl;
		return;
	}

	/* Get the servers address */
	bzero((char *) &server_addr, sizeof(server_addr));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = INADDR_ANY;
	server_addr.sin_port = htons(SERVER_PORT);

	/* Bind the socket to the address */
	if(bind(sockfd, (struct sockaddr *)&server_addr, sizeof(server_addr)) < 0)
	{
		cout << "SERVER ERROR: binding" << endl;
		return;
	}

	length = sizeof(struct sockaddr_in);
	while(true) {
		int len = recvfrom( sockfd, buffer, BUFFER_SIZE, 0, (struct sockaddr *)&client_addr, (socklen_t *)&length);
		if(len < 0)
		{
			cout << "SERVER ERROR: recvfrom" << endl;
			continue;
		}

//	buffer[0] = 0x30;

		/* Parse the message & send the task to the scheduler */
		if(scheduler) {
			Task t = ParseMsg((char *)buffer, 1);
			if(t.id >= 0) {
				t.id = taskID++;
				scheduler->addTask(t);
			}
			else {
				cout << "Server is unable to parse incoming message" << endl;
			}
		}
		else
			cout << "Scheduler is down!" << endl;
	}
}

/******************************************************************************
 * The main method
 *****************************************************************************/
int main() {
	cout << "Starting the server" << endl;
	vector<TaskHandler*> *taskHandlerList = new vector<TaskHandler*>();
	Scheduler *scheduler = new Scheduler(taskHandlerList);
	GameState *gameState = new GameState();

	/* Find out how many Task handlers the user wants */
	int taskHandlers = 0;
	while( taskHandlers <= 0 || taskHandlers > MAX_TASK_HANDLERS ) {
		cout << "How many task handler threads (1-4)? " << endl;
		cin >> taskHandlers;
	}

	/* Find out which Algorithm to Run */
	int menuItem = 0;
	while(menuItem <= 0 || menuItem > SCHEDULING_ALGORITHMS) {
		cout << "Select a scheduling algorithm:" << endl;
		cout << "\t1) Greedy Algorithm" << endl;
		cin >> menuItem;
	}

	/* Set the scheduling algorithm */
	scheduler->SetAlgorithm(menuItem);

	/* Start the GameState thread first */
	gameState->start();

	/* Start the scheduler */
	scheduler->start();

	/* Create the Task Handlers & start them */
	for(int i=0; i<taskHandlers; i++)
	{
		taskHandlerList->push_back(new TaskHandler(scheduler, gameState));
		taskHandlerList->at(i)->start();
	}

	/* begin the server method */
	RunServer(scheduler);

	/* Wait for the worker threads to complete - This should be the last thread */
//	if( scheduler && scheduler->getThread())
//		pthread_join( *(scheduler->getThread()), NULL);
//	for(int i=0; i<taskHandlerList->size(); i++)
//		if( taskHandlerList->size() > 0 && taskHandlerList->at(i)->getThread() )
//			pthread_join( *(taskHandlerList->at(i)->getThread()), NULL);

	/* Free up Resources & exit */
	cout << "Server Terminated!" << endl;
	if(gameState) delete gameState;
	if(scheduler) delete scheduler;
	if(taskHandlerList) {
		taskHandlerList->clear();
		delete taskHandlerList;
	}
	return 0;
}
