/*
 * TaskHandler.cpp
 *
 *  Created on: Mar 2, 2009
 *      Author: canichols
 */

#include "TaskHandler.h"

TaskHandler::TaskHandler(Scheduler *scheduler): Thread() {
	this->scheduler = scheduler;

	/* Initialize the lock */
	this->initializeLock();

	/* Create a condition variable */
	task_cond_var = this->createConditionVar();
}

TaskHandler::~TaskHandler() {
}

void TaskHandler::run() {
	cout << "TaskHandler Thread Started" << endl;

	/* Wait until there is a task to perform */
	while(true) {
		this->lock();
		this->wait(task_cond_var);
		scheduler->finishedTask(currentTask);
		this->unlock();
	}
}

void TaskHandler::performTask(Task task) {
	cout << "TaskHandler will perform task " << task.id << endl;
	this->lock();
	currentTask = task;
	this->signal(task_cond_var);
	this->unlock();
}
