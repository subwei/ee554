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

#include "Thread.h"

using namespace std;

class TaskHandler: public Thread {
public:
	TaskHandler();
	virtual ~TaskHandler();

	/* Implements the abstract method from Thread
	 * This will be run when the thread is started */
	void run();
};

#endif /* TASKHANDLER_H_ */
