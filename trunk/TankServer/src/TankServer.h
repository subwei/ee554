/*
 * TankServer.h
 *
 *  Created on: Feb 26, 2009
 *      Author: canichols
 */

#ifndef TANKSERVER_H_
#define TANKSERVER_H_

/* System Includes */
#include <iostream>
#include <math.h>
#include <stdio.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <vector>

/* Class prototypes */
class GameState;
class Scheduler;
class TaskHandler;

/* Game Constants */
#define SCHEDULING_ALGORITHMS 	1
#define MAX_TASK_HANDLERS 		4
#define MAX_PLAYERS				4

/* Scheduling Algorithms */
#define GREEDY_ALGORITHM	1

/* Message Types */
#define MSG_MOVE			0x10
#define MSG_SHOOT			0x20
#define MSG_REGISTER		0x30
#define MSG_QUIT			0x40
#define MSG_DEAD			0x50
#define MSG_JOIN			0x60

/* Networking Constants */
#define RESPONSE_PORT		4800
#define SERVER_PORT			4801
#define BUFFER_SIZE			1024

/* Client & Game State Constants */
#define TANK_WIDTH 			50
#define TANK_HEIGHT			50
#define SCREEN_WIDTH		600
#define SCREEN_HEIGHT		600
#define TANK_SPEED			25
#define SHOOTING_SPEED		50

/*****************************************************
 * Client Data
 ****************************************************/

/* Every client can be in one of the following states */
enum client_state { ACTIVE, INACTIVE };

/* Clients orientations */
enum client_orientation { NORTH, WEST, SOUTH, EAST, NORTHWEST, NORTHEAST, SOUTHWEST, SOUTHEAST, UNK };

/* Struct to contain all client state information */
typedef struct Client_info {
	int id;
	int health;
	int x_pos;
	int y_pos;
	client_orientation orientation;
	client_state state;
	struct sockaddr_in client_addr;
}Client_info;

/*********************************************
 * Task Data
 ********************************************/

/* All tasks can have the following states */
enum task_state { RUNNING, IDLE };

/* Struct to contain all task information */
typedef struct Task {
	int id;
	int type;
	Client_info client;
	task_state state;
}Task;

/* Local includes */
#include "Thread.h"
#include "TaskHandler.h"
#include "GameState.h"
#include "Scheduler.h"

#endif /* TANKSERVER_H_ */
