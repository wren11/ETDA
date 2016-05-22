
#ifndef OBJECT_H
#define OBJECT_H
class Object
{
public:
	std::string Name;
	short X, Y;
	byte direction;
	UINT Serial;
	int Timer;
	clock_t LastDionSeen;
};
#endif