/*
 * Scheduler.cpp
 *
 *  Created on: Mar 2, 2009
 *      Author: canichols
 */

#include "Scheduler.h"

/**********************************
 * Constructor
 **********************************/
Scheduler::Scheduler(vector<TaskHandler*> *taskHandlers): Thread() {
	/* Add the taskHandlers */
	this->free_TaskHandlers = taskHandlers;
	this->busy_TaskHandlers = new vector<TaskHandler*>();

	/* Create and initialize a lock for synchronization */
	this->initializeLock();

	/* Create a condition variable for synchronization */
	schedule_cond_var = this->createConditionVar();

	/* Set the default algorithm */
	scheduling_algorithm = GREEDY_ALGORITHM;
}

/***********************************
 * Destructor
 ***********************************/
Scheduler::~Scheduler() {
}

/**********************************************
 * The actual method where the thread begins
 *  Loops forever waiting to perform tasks &
 *  assign them to specific task handlers
 *********************************************/
void Scheduler::run() {
	/* Schedule forever, but sleep when out of tasks */
	while(true) {
		this->lock();
		if(queuedTasks.size() > 0 && free_TaskHandlers->size() > 0)
		{
			switch(scheduling_algorithm) {
			case GREEDY_ALGORITHM:
				greedySchedule();
				break;
			default:
				greedySchedule();
				break;
			}
		}
		else
		{
//			cout << "Scheduler: Nothing to do" << endl;
			this->wait(schedule_cond_var);
		}
		this->unlock();
	}
}

/*****************************************************************
 * Set the scheduling algorithm
 ****************************************************************/
void Scheduler::SetAlgorithm(int algorithm) {
	this->lock();
	scheduling_algorithm = algorithm;
	this->unlock();
}

/*******************************************************************
 * Scheduling Algorithms
 *******************************************************************/

void Scheduler::greedySchedule() {
	if(free_TaskHandlers->size() > 0) {
		/* Get the task handler & task of interest */
		TaskHandler *th = free_TaskHandlers->at(0);
		Task t = queuedTasks.at(0);

		/* Add queued task to running list */
		runningTasks.push_back(t);

		/* Add the Task handler to the busy list */
		busy_TaskHandlers->push_back(th);

		/* Give the task to the first free task handler - round robin */
		free_TaskHandlers->at(0)->performTask(t);

		/* Remove the task from the queue */
		queuedTasks.erase(queuedTasks.begin());

		/* Remove the task handler from the free list */
		free_TaskHandlers->erase(free_TaskHandlers->begin());
	} else
		cout << "Scheduler: No free task handler" << endl;
}

/*******************************************************************
 * Incoming Messages to wake up the scheduler and give it more work
 *******************************************************************/

void Scheduler::addTask(Task task) {
	this->lock();
	queuedTasks.push_back(task);
	this->signal(schedule_cond_var);
	this->unlock();
}

void Scheduler::finishedTask(TaskHandler *taskHandler, Task task) {
	this->lock();

	/* Locate and mark this task handler as free */
	for(unsigned i=0; i<busy_TaskHandlers->size(); i++) {
		if(taskHandler == busy_TaskHandlers->at(i)) {
			free_TaskHandlers->push_back(busy_TaskHandlers->at(i));
			busy_TaskHandlers->erase(busy_TaskHandlers->begin() + i);
			break;
		}
	}

	/* Locate, erase the task, and signal the scheduler */
	for(unsigned i=0; i < runningTasks.size(); i++) {
		if(runningTasks.at(i).id == task.id) {
			runningTasks.erase(runningTasks.begin() + i);
			this->signal(schedule_cond_var);
			break;
		}
	}

	this->unlock();
}
