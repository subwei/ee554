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
#include <map>
#include <sys/time.h>

/* Class prototypes */
class GameState;
class Scheduler;
class TaskHandler;

/* Game Constants */
#define SCHEDULING_ALGORITHMS 	1
#define MAX_TASK_HANDLERS 		4
#define MAX_PLAYERS				4
#define MIN_PLAYERS				2
#define MAX_BULLETS				1

/* Scheduling Algorithms */
#define GREEDY_ALGORITHM	1

/* Message Types */
#define MSG_MOVE			0x10
#define MSG_SHOOT			0x20
#define MSG_REGISTER		0x30
#define MSG_QUIT			0x40
#define MSG_DEAD			0x50
#define MSG_JOIN			0x60
#define MSG_START			0x70
#define MSG_BROADCAST		0xFF

/* Networking Constants */
#define RESPONSE_PORT		4800
#define SERVER_PORT			4801
#define BUFFER_SIZE			1024

/* Client & Game State Constants */
#define TANK_WIDTH 			50
#define TANK_HEIGHT			50
#define SCREEN_WIDTH		800
#define SCREEN_HEIGHT		550
#define TANK_SPEED			10
#define SHOOTING_SPEED		15
#define BORDER				50

/* Clients orientations */
#define NORTH				0x01
#define WEST				0x02
#define SOUTH				0x04
#define EAST				0x08
#define NORTHWEST			0x10
#define NORTHEAST			0x20
#define SOUTHWEST			0x40
#define SOUTHEAST			0x80
#define UNK 				0x00

/* Inter-frame delay (us) ==> Inverse gives the frame rate */
#define INTERFRAME_DELAY	50000

/* macro to determine the difference in two times */
#define TIME_DIFFERENCE(before, after, result) { \
	result.tv_sec = after.tv_sec - before.tv_sec; \
	result.tv_usec = after.tv_usec - before.tv_usec; \
}

/*****************************************************
 * Client Data
 ****************************************************/

/* Every client can be in one of the following states */
enum client_state { ACTIVE, INACTIVE };

/* Structure for bullets */
typedef struct bullet {
	int orientation;
	int x;
	int y;
}bullet;

/* Struct to contain all client state information */
typedef struct Client_info {
	int id;
	int health;
	int x_pos;
	int y_pos;
	int orientation;
	client_state state;
	struct sockaddr_in client_addr;
	struct timeval arrival;
	struct timeval serverEntry;
	bullet bullets[MAX_BULLETS];
	int nextShot;
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
