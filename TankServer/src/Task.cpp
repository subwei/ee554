/*
 * Task.cpp
 *
 *  Created on: Mar 3, 2009
 *      Author: canichols
 */

#include "Task.h"

Task::Task() {
	type = 0;
	state = 0;
	owner = 0;
}

Task::Task(int type, int state, int owner) {
	this->type = type;
	this->state = state;
	this->owner = owner;
}

Task::~Task() {
}
