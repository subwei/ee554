//============================================================================
// Name        : TankServer.cpp
// Author      : Corey Nichols
// Version     :
// Copyright   : Your copyright notice
// Description : Hello World in C++, Ansi-style
//============================================================================

#include "TankServer.h"

using namespace std;

/******************************************************************************
 * Receives all of the UDP messages ==> Essentially this is the server
 *****************************************************************************/
void RunServer() {
	int sockfd, length;
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
		if(len < 0) cout << "SERVER ERROR: recvfrom" << endl;

		/* Parse the message & save it into the client list if appropriate */

		/* Build the task */

		/* Send the task to the scheduler */
	}
}

/******************************************************************************
 *
 *****************************************************************************/
int main() {
	cout << "Starting the server" << endl;
	Scheduler *scheduler = new Scheduler();
	vector<TaskHandler*> *taskHandlerList = new vector<TaskHandler*>();

	/* Find out how many Task handlers the user wants */
	int taskHandlers = 0;
	while( taskHandlers <= 0 || taskHandlers > MAX_TASK_HANDLERS ) {
		cout << "How many task handler threads (1-4)? " << endl;
		cin >> taskHandlers;
	}

	/* Start the scheduler */
	scheduler->start();

	/* Create the Task Handlers & start them */
	for(int i=0; i<taskHandlers; i++)
	{
		taskHandlerList->push_back(new TaskHandler());
		taskHandlerList->at(i)->start();
	}

	/* begin the server thread */
	RunServer();

	/* Wait for the worker threads to complete - This should be the last thread */
//	if( scheduler && scheduler->getThread())
//		pthread_join( *(scheduler->getThread()), NULL);
//	for(int i=0; i<taskHandlerList->size(); i++)
//		if( taskHandlerList->size() > 0 && taskHandlerList->at(i)->getThread() )
//			pthread_join( *(taskHandlerList->at(i)->getThread()), NULL);

	/* Free up Resources & exit */
	cout << "Server Terminated!" << endl;
	if(scheduler) delete scheduler;
	if(taskHandlerList) {
		taskHandlerList->clear();
		delete taskHandlerList;
	}
	return 0;
}
