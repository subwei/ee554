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
	bool unknown = false;

	/* Wait until there is a task to perform */
	while(true) {
		this->lock();
		this->wait(task_cond_var);

		/* Perform the task based on the message type */
		gettimeofday(&(currentTask.client.serverEntry), 0);
		switch(currentTask.type) {
		case MSG_MOVE:
			gameState->updateClientPosition(currentTask.client);
			break;
		case MSG_SHOOT:
			gameState->newShot(currentTask.client);
			break;
		case MSG_REGISTER:
			gameState->clientReg(currentTask.client, true);
			break;
		case MSG_START:
//			cout << "start" << endl;
			gameState->startGame(currentTask.client);
			break;
		case MSG_QUIT:
			gameState->clientQuit(currentTask.client);
			break;
		case MSG_DEAD:
			break;
		case MSG_JOIN:
			break;
		default:
//			cout << "Unknown Task" << endl;
			unknown = true;
			break;
		}

		scheduler->finishedTask(this, currentTask);
		if(!unknown) {
			/* Calling Finished Task */
			gameState->FinishedTask(currentTask.client);
		}
		unknown = false;
		this->unlock();
	}
}

/**********************************************
 * Called by the scheduler to assign a task to
 * this task handler.
 *********************************************/
void TaskHandler::performTask(Task task) {
	this->lock();
	currentTask = task;
	this->signal(task_cond_var);
	this->unlock();
}
