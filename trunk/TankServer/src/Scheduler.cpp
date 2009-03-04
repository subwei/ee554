/*
 * Scheduler.cpp
 *
 *  Created on: Mar 2, 2009
 *      Author: canichols
 */

#include "Scheduler.h"

Scheduler::Scheduler(vector<TaskHandler*> *taskHandlers): Thread() {
	/* Add the taskHandlers */
	this->free_TaskHandlers = taskHandlers;

	/* Create a condition variable for synchronization */
	schedule_cond_var = this->createConditionVar();

	/* Create and initialize a lock for synchronization */
	this->initializeLock();
}

Scheduler::~Scheduler() {
}

void Scheduler::run() {
	cout << "Perform Scheduling" << endl;

	/* Schedule forever, but sleep when out of tasks */
	while(true) {
		this->lock();
		if(queuedTasks.size() > 0 && free_TaskHandlers->size() > 0)
		{
			/* Decide which TaskHandler to use for the front task */

		}
		else
		{
			this->wait(schedule_cond_var);
		}
		this->unlock();
	}
}

void Scheduler::addTask(Task task) {
	this->lock();
	queuedTasks.push_back(task);
	this->signal(schedule_cond_var);
	this->unlock();
}
