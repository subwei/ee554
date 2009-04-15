/*
 * ServerTest.cpp
 *
 *  Created on: Apr 13, 2009
 *      Author: canichols
 */
#include "ServerTest.h"

using namespace std;

/* Register */
void Register(int sockfd, sockaddr_in to) {
	int len = 2;
	char buf[10] = "";
    buf[0] = 0x00;
    buf[1] = MSG_REGISTER;

    /* Send request */
	if(sendto(sockfd, buf, 2 , 0, (struct sockaddr *)&to, sizeof(to)) == -1)
		cout << "Failed to register" << endl;
}

/* Start */
void Start(int id, int sockfd, sockaddr_in to) {
	int len = 2;
	char buf[2] = "";
    buf[0] = id;
    buf[1] = MSG_START;

    /* Send request */
	if(sendto(sockfd, buf, 2 , 0, (struct sockaddr *)&to, sizeof(to)) == -1)
		cout << "Failed to register" << endl;
}

/* Quit */
void Quit(int id) {

}

/* Shoot */
void Shoot(int id) {

}

/* Move */
void Move(int id, int sockfd, sockaddr_in to) {
	int len = 3;
	char buf[len];
    buf[0] = id;
    buf[1] = MSG_MOVE;
    buf[2] = SOUTH;

    /* Send request */
	if(sendto(sockfd, buf, len, 0, (struct sockaddr *)&to, sizeof(to)) == -1)
		cout << "Failed to Move" << endl;
}

/******************************************************************************
 * The main method
 *****************************************************************************/
int main() {
	int sockfd, length;
	int taskID = 0;
	char buffer[BUFFER_SIZE];
	struct sockaddr_in server_addr, my_addr;
	int id1 = 0, id2 = 0;

	/* Create the datagram socket */
	sockfd = socket(AF_INET, SOCK_DGRAM, 0);
	if(sockfd < 0) {
		cout << "SERVER ERROR: opening socket" << endl;
		return -1;
	}

	/* Get My address */
	bzero((char *) &my_addr, sizeof(my_addr));
	my_addr.sin_family = AF_INET;
	my_addr.sin_addr.s_addr = INADDR_ANY;
	my_addr.sin_port = htons(RESPONSE_PORT);

	/* Get the servers address */
	bzero((char *) &server_addr, sizeof(server_addr));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = INADDR_ANY;
	server_addr.sin_port = htons(SERVER_PORT);

	/* Bind the socket to the address */
	if(bind(sockfd, (struct sockaddr *)&my_addr, sizeof(my_addr)) < 0) {
		cout << "SERVER ERROR: binding" << endl;
		return -1;
	}

	length = sizeof(struct sockaddr_in);

	/* Register Player 1 */
	Register(sockfd, server_addr);

	/* Wait for the ID */
	cout << "Player 1 is Registering" << endl;
	int len = recvfrom( sockfd, buffer, BUFFER_SIZE, 0, (struct sockaddr *)&my_addr, (socklen_t *)&length);
	if(len < 0)	cout << "SERVER ERROR: recvfrom" << endl;
	id1 = (int)buffer[0];
	cout << "Player 1 is id # " << id1 << endl;

	/* Register player 2 */
	Register(sockfd, server_addr);

	/* Wait for the ID */
	cout << "Player 2 is Registering" << endl;
	len = recvfrom(sockfd, buffer, BUFFER_SIZE, 0, (struct sockaddr *)&my_addr, (socklen_t *)&length);
	if(len < 0)	cout << "SERVER ERROR: recvfrom" << endl;
	id2 = (int)buffer[0];
	cout << "Player 2 is id # " << id2 << endl;

	/* Start the game */
	cout << "Player 1 starts the game" << endl;
	Start(id1, sockfd, server_addr);

	/* Move player 1 */
	cout << "Player 1 is moving" << endl;
	Move(id1, sockfd, server_addr);

	/* Move Player 2 */
	cout << "" << endl;
	Move(id2, sockfd, server_addr);

	/* Shoot with player 1 */
	cout << "" << endl;
	Shoot(id1);

	/* Shoot with player 2 */
	cout << "" << endl;
	Shoot(id2);

	/* Quit with Player 1 */
	cout << "" << endl;
	Quit(id1);

	/* Quit with player 2 */
	cout << "" << endl;
	Quit(id2);

	return 0;
}
