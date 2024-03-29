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
Task ParseMsg(char* buffer, int length, sockaddr_in client_addr) {
	Task task;
	task.id = -1;
	task.client.orientation = UNK;
	task.client.client_addr = client_addr;

	/* First parse the message */
	if(length >= 1) {
		task.id = 0;
		task.client.id = buffer[0]&0x0F;
		task.type = buffer[1]&0xF0;
		task.state = IDLE;
		gettimeofday(&(task.client.arrival), 0);

		/* Check if we need to gather the direction from the buffer */
		if(task.type == MSG_MOVE || task.type == MSG_SHOOT) {
			task.client.orientation = buffer[2];
		} else if(task.type == MSG_REGISTER) {}
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

	cout << "Starting the server" << endl;

	/* Create the datagram socket */
	sockfd = socket(AF_INET, SOCK_DGRAM, 0);
	if(sockfd < 0){
		cout << "SERVER ERROR: opening socket" << endl;
		return;
	}

	/* Get the servers address */
	bzero((char *) &server_addr, sizeof(server_addr));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = INADDR_ANY;
	server_addr.sin_port = htons(SERVER_PORT);

	/* Bind the socket to the address */
	if(bind(sockfd, (struct sockaddr *)&server_addr, sizeof(server_addr)) < 0) {
		cout << "SERVER ERROR: binding" << endl;
		return;
	}

	length = sizeof(struct sockaddr_in);
	while(true) {
		int len = recvfrom( sockfd, buffer, BUFFER_SIZE, 0, (struct sockaddr *)&client_addr, (socklen_t *)&length);
		if(len < 0)	{
			cout << "SERVER ERROR: recvfrom" << endl;
			continue;
		}

		/* Parse the message & send the task to the scheduler */
		if(scheduler) {
			Task t = ParseMsg((char *)buffer, 1, client_addr);
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
