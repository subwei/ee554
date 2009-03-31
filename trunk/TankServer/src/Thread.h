/*
 * Thread.h
 *
 *  Created on: Mar 3, 2009
 *      Author: canichols
 */

#ifndef THREAD_H_
#define THREAD_H_

#include <pthread.h>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <vector>

using namespace std;

class Thread {
private:
	pthread_t thread;
	pthread_mutex_t mutex_lock;
	vector<pthread_cond_t> conditions;
	int newID;

public:
	Thread();
	virtual ~Thread();

	/* Abstract method to carry out the inheriting
	 * classes functionality */
	virtual void run() = 0;

	/* Method to start a new thread running */
	void start();

	/* Method to kill/terminate this thread */
	void terminate();

	/* Synchronization Methods */
	void initializeLock();
	void lock();
	void unlock();

	/* Give access to the thread */
	pthread_t *getThread() { return &thread; }

	/* Condition Variable Methods */
	int createConditionVar();
	void destroyConditionVar(int);
	void wait(int);
	void signal(int);
	void broadcast(int);
};

#endif /* THREAD_H_ */
