//============================================================================
// Name        : TankServer.cpp
// Author      : Corey Nichols
// Version     :
// Copyright   : Your copyright notice
// Description : Hello World in C++, Ansi-style
//============================================================================

#include "TankServer.h"

using namespace std;

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

	/* Wait for the worker threads to complete */
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
