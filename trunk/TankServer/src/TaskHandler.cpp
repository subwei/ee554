/*
 * TaskHandler.cpp
 *
 *  Created on: Mar 2, 2009
 *      Author: canichols
 */

#include "TaskHandler.h"

TaskHandler::TaskHandler(Scheduler *scheduler, GameState *gameState): Thread() {
	this->scheduler = scheduler;
	this->gameState = gameState;
	cout << "Creating the task Handler" << endl;

	/* Initialize the lock */
	this->initializeLock();

	/* Create a condition variable */
	task_cond_var = this->createConditionVar();
}

TaskHandler::~TaskHandler() {
}

/**********************************************
 * The actual method where the thread begins
 *  Loops forever waiting to perform tasks
 *********************************************/
void TaskHandler::run() {
	cout << "TaskHandler Thread Started" << endl;

	/* Wait until there is a task to perform */
	while(true) {
		this->lock();
		this->wait(task_cond_var);

		/* Perform the task based on the message type */
		currentTask.client.serverEntry = time(NULL);
		switch(currentTask.type) {
		case MSG_MOVE:
			cout << "TH: Moving" << endl;
			gameState->updateClientPosition(currentTask.client);
			break;
		case MSG_SHOOT:
			break;
		case MSG_REGISTER:
			cout << "TH: Register" << endl;
			gameState->clientReg(currentTask.client, true);
			break;
		case MSG_START:
			cout << "TH: Starting Game" << endl;
			gameState->startGame(currentTask.client);
			break;
		case MSG_QUIT:
			cout << "TH: Quitting Game" << endl;
			gameState->clientQuit(currentTask.client);
			break;
		case MSG_DEAD:
			break;
		case MSG_JOIN:
			break;
		default:
			cout << "Unknown Task" << endl;
			break;
		}

		scheduler->finishedTask(this, currentTask);
		this->unlock();
	}
}

/**********************************************
 * Called by the scheduler to assign a task to
 * this task handler.
 *********************************************/
void TaskHandler::performTask(Task task) {
	cout << "TaskHandler will perform task " << task.id << endl;
	this->lock();
	currentTask = task;
	this->signal(task_cond_var);
	this->unlock();
}
