#pragma once
#include <vector>
#include <algorithm>
#include <thread>
#include "map.h"
#include "dabase.h"
#include "Constants.h"

class DA
{
public:
		HANDLE GameHandle;

		DA() : GameHandle(GetCurrentProcess())
		{

		}
		DWORD ProcessId;
		void* executor;		
};

class Darkages;
class Darkages : public DA
{
public:

	typedef int(*Callback)(Darkages);

	Darkages() : DA()
	{

	};

	bool Init(void* p);
	void Run();
	void CleanUp();

	static void LetsGo(Darkages& obj, Callback cb);

	DABase *base;
	Map *map;

	typedef char(__thiscall *SubWalk)(int ecx, char direction);
	SubWalk hWalk = NULL;

	typedef char(__thiscall *SetWalkPos)(int thisptr, c_walk prm);
	SetWalkPos hSetWalkPos = NULL;

	//char __thiscall sub_5EFBE0(void *this, int a2)
	typedef char(__thiscall *SetCommand)(int thisptr, int command);
	SetCommand hSetCommand = NULL;
	
};

