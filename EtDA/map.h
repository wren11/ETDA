#ifndef MAP_H
#define MAP_H
#include "dabase.h"
#include <array>



using namespace std;
class Map
{
public:
	byte Width;
	byte Height;
	short Number;
	std::string MapName;

	Map(DABase *context, short Number, byte Width, byte Height, std::string message)
		: Number(Number), Width(Width), Height(Height), MapName(message)
	{
		base = context;
	}

	bool Read();


protected:
	DABase *base;
};
#endif