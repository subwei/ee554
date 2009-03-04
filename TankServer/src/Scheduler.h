/*
 * Scheduler.h
 *
 *  Created on: Mar 2, 2009
 *      Author: canichols
 */

#ifndef SCHEDULER_H_
#define SCHEDULER_H_

#include <pthread.h>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <vector>

#include "TankServer.h"

using namespace std;

class Scheduler: public Thread {
private:
	int schedule_cond_var;
	vector<Task> queuedTasks;
	vector<Task> runningTasks;
	vector<TaskHandler*> *free_TaskHandlers;

public:
	Scheduler(vector<TaskHandler*> *);
	virtual ~Scheduler();

	/* Implements the abstract method from Thread */
	void run();

	/* Adds a task to be scheduled */
	void addTask(Task task);
};

#endif /* SCHEDULER_H_ */
