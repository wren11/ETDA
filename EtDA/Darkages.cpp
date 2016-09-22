#include "common.h"
#include <thread>
#include <concrt.h>
#include <iostream>
 //...
#pragma once
#include "Constants.h"
#include "GameFunctions.h"
#include <ddraw.h>
#include <vector>
#include <chrono>
#include <future>
#include <time.h>
#include <string>
#include <functional>
#include <cstdio>

class Later
{
public:
	template <class callable, class... arguments>
	Later(int after, bool async, callable&& f, arguments&&... args)
	{
		std::function<typename std::result_of<callable(arguments...)>::type()> task(std::bind(std::forward<callable>(f), std::forward<arguments>(args)...));

		if (async)
		{
			std::thread([after, task]() {
				std::this_thread::sleep_for(std::chrono::milliseconds(after));
				task();
			}).detach();
		}
		else
		{
			std::this_thread::sleep_for(std::chrono::milliseconds(after));
			task();
		}
	}

};

Darkages da;
DABase base;

typedef void (_stdcall *OnRecvEvent)(BYTE *data, unsigned int Length); OnRecvEvent Receiver = NULL;
typedef int  (__stdcall *OnSendEvent)(BYTE *data, int arg1, int arg2, char arg3); OnSendEvent Sender = NULL;

/* DirectDraw / GDI */
typedef int(__thiscall *pPaint)(int *ecx, int hdc); pPaint hPaint = NULL;
typedef HDC(__thiscall *DAGetDC)(int ecx);          DAGetDC pDAGetDC = NULL;

HDC context = NULL;
bool hud = false;

void RenderCalls()
{
	if (context == NULL)
		return;
}

int __fastcall DrawOverlay(int *ecx, int hdcptr)
{
	int ptx = { 0 };

	if (context == NULL)
	{
		void *GetDC = (void*)DA741_GETDC;

		__asm
		{
			mov edx, [hdcptr];
			mov ptx, edx

			push edx
			call[GetDC]
			mov eax, ptx
			pop ecx
		}

		context = HDC(ptx);

		GdiplusStartupInput gdiplusStartupInput;
		ULONG_PTR           gdiplusToken;
		Status st = GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

		return st;
	}	

	RenderCalls();
	hPaint(ecx, hdcptr);
}

int __stdcall OnCharacterLogin(char *username, char *password)
{
	return base.OnCharacter(username, password);
}

HWND hTargetWnd = FindWindow(nullptr, "etda");

void RedirectPacketInformation(byte *packet, int length, int type)
{
	try
	{
		if (!IsWindow(hTargetWnd))
		{
			hTargetWnd = FindWindow(nullptr, "etda");
			return;
		}

		COPYDATASTRUCT payload;
		payload.dwData = type;
		payload.cbData = sizeof(BYTE) * length;
		payload.lpData = packet;

		SendMessageTimeout(hTargetWnd, WM_COPYDATA, 
			static_cast<WPARAM>(
		    reinterpret_cast<int>(packet)), 
			reinterpret_cast<LPARAM>(
			static_cast<LPVOID>(&payload)), SMTO_NORMAL, 5,
			nullptr);
		SendMessageTimeout(hTargetWnd, WM_COPYDATA, static_cast<WPARAM>(da.ProcessId) == 0 ? GetCurrentProcessId() : da.ProcessId, (LPARAM)(LPVOID)&payload, SMTO_NORMAL, 10, NULL);
	}
	catch (exception)
	{
		return;
	}
}

int __stdcall OnPacketSend(BYTE *data, int arg1, int arg2, char arg3)
{
	__asm
	{
		pushfd
		pushad
	}

	RedirectPacketInformation(data, arg1, 2);

	__asm
	{
		popad
		popfd
	}

	return Sender(data, arg1, arg2, arg3);
}

void __stdcall OnPacketRecv(BYTE *data, unsigned int Length)
{
	if (data[0] == 0x19 && *reinterpret_cast<int*>(OptionA) == 1)
	{
		return;
	}

	if (data[0] == 0x13 && *reinterpret_cast<int*>(OptionB) == 1)
	{
		return;
	}

	if (data[0] == 0x29 && *reinterpret_cast<int*>(OptionC) == 1)
	{
		short animation = (data[9] << 8) | data[10];
		if (animation == 33)
			return;
		if (animation == 245)
			return;
		if (animation == 7)
			return;
		if (animation == 245)
			return;
		if (animation == 239)
			return;
		if (animation == 62)
			return;
	}


	__asm
	{
		pushfd
		pushad
	}

	try
	{
		if (data[0] == 0x07)
		{
			USHORT entity_count = (USHORT)((data[1] << 8) + data[2]);
			int index = 0;

			for (int i = 0; i < entity_count; i++)
			{
				USHORT xcord = (USHORT)((data[index + 3] << 8) + data[index + 4]);
				USHORT ycord = (USHORT)((data[index + 5] << 8) + data[index + 6]);
				USHORT sprite = (USHORT)((data[index + 11] << 8) + data[index + 12]);



				if (sprite > 0x8000 && sprite < 0x9000)
				{
					if (sprite == 0 || sprite == 32000)
					{
						data[index + 11] = 0x93;
						data[index + 12] = 0x00;
					}
					else if (sprite == 32908 && *((int*)OptionD) == 1)
					{
						data[index + 11] = 0x90;
						data[index + 12] = 0x00;
					}

					index += 13;
				}
				else
				{
					if (sprite < 0x8000 && *((int*)OptionE) == 1)
					{
						data[index + 11] = 0x40;
						data[index + 12] = 14;
					}

					int TYPE = (USHORT)((data[index + 18] << 8) + data[index + 19]);

					if (TYPE == 0x0001 || TYPE == 0x0000)
					{
						index += 17;
					}
					else
					{
						int name_length = (BYTE)data[index + 20];
						index += 18 + name_length;
					}
				}
			}
		}
	}
	catch (exception)
	{

	}

	RedirectPacketInformation(data, Length, 1);

	__asm
	{
		popad
		popfd
	}

	Receiver(data, Length);
}

bool Darkages::Init(void *hModule)
{
	this->executor = new Executor;
	return true;
};


int CallBack(Darkages da)
{
	da = da;
	da.ProcessId = GetCurrentProcessId();
	base = *da.base;

	base.OnCharacter = (DABase::OnCharacterLoginEvent)DetourFunction((PBYTE)hOnCharacter, (PBYTE)OnCharacterLogin);
	return 1;
}

void Darkages::Run()
{
	Receiver = (OnRecvEvent)DetourFunction((PBYTE)recvPacketin, (PBYTE)OnPacketRecv);
	Sender = (OnSendEvent)DetourFunction((PBYTE)sendPacketout, (PBYTE)OnPacketSend);
	hPaint = (pPaint)DetourFunction((PBYTE)hPaintPtr, (PBYTE)DrawOverlay);

	LetsGo(*this, &CallBack);
}

void Darkages::LetsGo(Darkages& obj, Callback cb)
{
	char *name = { 0 };

	__asm
	{
		mov eax, userNameoffset
		mov name, eax
	}

	obj.base = new DABase();
	obj.base->Name = name;
	obj.ProcessId = GetCurrentProcessId();
	cb(obj);


	//typedef int func(BYTE*, int);
	//func* f = (func*)&GameFunction::StubSender;
	//WriteProcessMemory(GetCurrentProcess(), (void*)0x0085C000, (void*)&f, 4, NULL);
}

void Darkages::CleanUp()
{
	DetourRemove((PBYTE)Receiver, (PBYTE)OnPacketRecv);
	DetourRemove((PBYTE)Sender, (PBYTE)OnPacketSend);
}