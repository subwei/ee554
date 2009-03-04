/*
 * TaskHandler.h
 *
 *  Created on: Mar 2, 2009
 *      Author: canichols
 */

#ifndef TASKHANDLER_H_
#define TASKHANDLER_H_

#include <pthread.h>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>

#include "TankServer.h"

using namespace std;

class TaskHandler: public Thread {
private:
	Scheduler *scheduler;
	int task_cond_var;
	Task currentTask;

public:
	TaskHandler(Scheduler *);
	virtual ~TaskHandler();

	/* Implements the abstract method from Thread
	 * This will be run when the thread is started */
	void run();

	/* Adds a task that needs to be performed */
	void performTask(Task);
};

#endif /* TASKHANDLER_H_ */
