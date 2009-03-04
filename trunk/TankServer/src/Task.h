/*
 * Task.h
 *
 *  Created on: Mar 3, 2009
 *      Author: canichols
 */

#ifndef TASK_H_
#define TASK_H_

class Task {
private:
	int type;
	int state;
	int owner;

public:
	Task();
	Task(int type, int state, int owner);
	virtual ~Task();

	/********************************************
	 * Getters/Setters
	 *******************************************/
    int getType() const { return type; }
    void setType(int type) { this->type = type; }
    int getState() const { return state; }
    void setState(int state) { this->state = state; }
    int getOwner() const { return owner; }
    void setOwner(int owner) { this->owner = owner; }
};

#endif /* TASK_H_ */
