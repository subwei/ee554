g++ -pthread -c Scheduler.cpp
g++ -pthread -c TaskHandler.cpp
g++ -pthread -o TankServer TankServer.cpp TaskHandler.o Scheduler.o
rm -f TankServer.o TaskHandler.o Scheduler.o

