/*
 * ServerTest.h
 *
 *  Created on: Apr 13, 2009
 *      Author: canichols
 */

#ifndef SERVERTEST_H_
#define SERVERTEST_H_

/* System Includes */
#include <iostream>
#include <math.h>
#include <stdio.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <vector>
#include <time.h>

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
#define MSG_START			0x70
#define MSG_BROADCAST		0xFF

/* Networking Constants */
#define RESPONSE_PORT		4802
#define SERVER_PORT			4801
#define BUFFER_SIZE			1024

/* Client & Game State Constants */
#define TANK_WIDTH 			50
#define TANK_HEIGHT			50
#define SCREEN_WIDTH		600
#define SCREEN_HEIGHT		600
#define TANK_SPEED			25
#define SHOOTING_SPEED		50

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

/*****************************************************
 * Client Data
 ****************************************************/

/* Every client can be in one of the following states */
enum client_state { ACTIVE, INACTIVE };

/* Struct to contain all client state information */
typedef struct Client_info {
	int id;
	int health;
	int x_pos;
	int y_pos;
	int orientation;
	client_state state;
	struct sockaddr_in client_addr;
	time_t arrival;
	time_t serverEntry;
	time_t exit;
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

#endif /* SERVERTEST_H_ */
