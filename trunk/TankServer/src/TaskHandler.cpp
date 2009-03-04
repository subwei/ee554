/*
 * TaskHandler.cpp
 *
 *  Created on: Mar 2, 2009
 *      Author: canichols
 */

#include "TaskHandler.h"

TaskHandler::TaskHandler(Scheduler *scheduler): Thread() {
	this->scheduler = scheduler;
}

TaskHandler::~TaskHandler() {
}

void TaskHandler::run() {
	cout << "TaskHandler Thread Started" << endl;

}
