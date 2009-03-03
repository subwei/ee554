g++ -pthread -c Scheduler.cpp
g++ -pthread -c TaskHandler.cpp
g++ -pthread -c Thread.cpp
g++ -pthread -o TankServer TankServer.cpp TaskHandler.o Scheduler.o Thread.o
rm -f TankServer.o TaskHandler.o Scheduler.o Thread.o

