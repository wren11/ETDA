#include "object.h"
#ifndef DABASE_H
#define DABASE_H

struct Point
{
	int X, Y;
};

class DABase
{
public:
	std::string Name;
	short X, Y;
	byte direction;
	UINT Serial;

	int FindPointer(int offset, HANDLE pHandle, int baseaddr, int offsets[])
	{
		int Address = baseaddr;
		int total = offset;
		for (int i = 0; i < total; i++)
		{
			ReadProcessMemory(pHandle, (LPCVOID)Address, &Address, 4, NULL);
			Address += offsets[i];
		}
		return Address;
	}

	bool CanInjectClient()
	{
		int offsets[] = { 491744 };
		int ptr = FindPointer(1, GetCurrentProcess(), 0x0075F8D8, offsets);
		return ptr == 0;
	}

	int SetCursor(Point pt)
	{
		int baseAddress = 0x008A4DE0;
		int offsets[] = { 0x20c, 0x00 };
		int baseptr = FindPointer(2, GetCurrentProcess(), baseAddress, offsets);
		int *ptr = (int*)baseptr;
		int(__thiscall *Set)(int*, int a2, int a3) = (int(__thiscall*)(int*, int a2, int a3))0x005F29C0;

		SetMouse(ptr, pt);
		return Set(ptr, pt.X, pt.Y);
	}

	int SetMouse(int* ptr, Point pt) 
	{
		int(__thiscall *SetMouse)(int*, int a2, int a3) = (int(__thiscall*)(int*, int a2, int a3))0x005F2990;
		return SetMouse(ptr, pt.X, pt.Y);
	}

	void Walk(Point pt)
	{
		void *(__thiscall *WalkTo)(void*, int a2, int a3) = (void*(__thiscall*)(void*, int a2, int a3))0x0061A1E0;

		if (SetCursor(pt) > 0)
			WalkTo(this, pt.X, pt.Y);;
	}

	typedef int(__stdcall *OnCharacterLoginEvent)(char * username, char * password); 
	OnCharacterLoginEvent OnCharacter = NULL;
};

#endif