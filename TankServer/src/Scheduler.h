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

#include "Thread.h"

using namespace std;

class Scheduler: public Thread {
public:
	Scheduler();
	virtual ~Scheduler();

	/* Implements the abstract method from Thread */
	void run();
};

#endif /* SCHEDULER_H_ */
