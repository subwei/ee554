/*
 * Thread.cpp
 *
 *  Created on: Mar 3, 2009
 *      Author: canichols
 */

#include "Thread.h"

static void *beginThread(void *arg) {
	Thread *t = (Thread *)arg;
	t->run();
	return NULL;
}

Thread::Thread() {
	newID = 0;
}

Thread::~Thread() {
	this->terminate();
}

/*******************************************************************
 * Spawn the new thread & start it running
 ******************************************************************/
void Thread::start() {
	pthread_create(&thread, NULL, beginThread, this);
}

/*******************************************************************
 * Kill this thread
 *  Requires the cleanup of all variables
 ******************************************************************/
void Thread::terminate() {
   pthread_mutex_destroy(&mutex_lock);
   for(int i=0; i<conditions.size(); i++)
	   pthread_cond_destroy(&conditions.at(i));
   conditions.clear();
   newID = 0;
   pthread_exit(NULL);
}

/********************************************************************
 *  Synchronization Methods
 *******************************************************************/

void Thread::initializeLock() {
	if(pthread_mutex_init(&mutex_lock, NULL))
		cout << "THREAD ERROR: unable to initialize lock" << endl;
}

void Thread::lock() {
	if(pthread_mutex_lock(&mutex_lock))
		cout << "THREAD ERROR: unable to get lock" << endl;
}

void Thread::unlock() {
	if(pthread_mutex_unlock(&mutex_lock))
		cout << "THREAD ERROR: unable to release lock" << endl;
}

/********************************************************************
 *  Condition Variable Methods
 *******************************************************************/

int Thread::createConditionVar() {
	pthread_cond_t newCondition;
	conditions.push_back(newCondition);
	pthread_cond_init(&conditions.at(newID), NULL);
	return newID++;
}

void Thread::destroyConditionVar(int id) {
	if(id < newID) {
		newID--;
		pthread_cond_destroy(&conditions.at(id));
		conditions.erase(conditions.begin() + id);
	}
}

void Thread::wait(int id) {
	this->lock();
	if(id < newID)
		pthread_cond_wait(&conditions.at(id), &mutex_lock);
	this->unlock();
}

void Thread::signal(int id) {
	this->lock();
	if(id < newID)
		pthread_cond_signal(&conditions.at(id));
	this->unlock();
}

void Thread::broadcast(int id) {
	this->lock();
	if(id < newID)
		pthread_cond_broadcast(&conditions.at(id));;
	this->unlock();
}
