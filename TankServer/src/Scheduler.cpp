/*
 * Scheduler.cpp
 *
 *  Created on: Mar 2, 2009
 *      Author: canichols
 */

#include "Scheduler.h"

Scheduler::Scheduler(): Thread() {
}

Scheduler::~Scheduler() {
}

void Scheduler::run() {
	cout << "Perform Scheduling" << endl;
}
