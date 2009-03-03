/*
 * TankServer.h
 *
 *  Created on: Feb 26, 2009
 *      Author: canichols
 */

#ifndef TANKSERVER_H_
#define TANKSERVER_H_

#include <iostream>
#include <vector>

#include "TankServer.h"
#include "Scheduler.h"
#include "TaskHandler.h"

#define MAX_TASK_HANDLERS 	4
#define MAX_PLAYERS			4
#define MSG_MOVE			0x10
#define MSG_SHOOT			0x20
#define MSG_REGISTER		0x30
#define MSG_QUIT			0x40
#define MSG_DEAD			0x50
#define MSG_JOIN			0x60

#endif /* TANKSERVER_H_ */
