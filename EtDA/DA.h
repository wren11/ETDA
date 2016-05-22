#pragma once
#include <vector>
#include <algorithm>
#include <thread>
#include "map.h"
#include "dabase.h"


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
};

