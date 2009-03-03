/*
 * TaskHandler.cpp
 *
 *  Created on: Mar 2, 2009
 *      Author: canichols
 */

#include "TaskHandler.h"

TaskHandler::TaskHandler(): Thread() {
}

TaskHandler::~TaskHandler() {
}

void TaskHandler::run() {
	cout << "TaskHandler Thread Started" << endl;

}
