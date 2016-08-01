#include "common.h"
#include "Constants.h"
#include "GameFunctions.h"

Darkages* da;

void RenderCalls(void);


DWORD WINAPI Setup(LPVOID Args)
{
	if (da->Init(Args))
	{
		if (da != nullptr)
			da->Run();

		return 1;
	}
}

DWORD WINAPI PacketRecvConsumer(LPVOID Args)
{
	while (true)
	{
		Sleep(1);

		try
		{
			if (*reinterpret_cast<int*>(0x00721000) == 1)
			{
				auto target = *reinterpret_cast<int*>(0x00721004);
				auto length = *reinterpret_cast<int*>(0x00721008);
				auto data = reinterpret_cast<unsigned char*>(0x00721012);

				if (data == 0)
					continue;
				if (length <= 0)
					continue;

				vector<byte> packet;
				for (int i = 0; i < length; i++ , data++)
					packet.push_back(*data);

				if (packet.size() == 0 || packet.size() != length)
				{
					packet.clear();
					*reinterpret_cast<int*>(0x00721000) = 0;
					continue;
				}
				else
				{
					GameFunction::SendToClient(&packet[0], length);
					packet.clear();
					*reinterpret_cast<int*>(0x00721000) = 0;
					*reinterpret_cast<int*>(0x00721012) = 0;
					WriteProcessMemory(GetCurrentProcess(), reinterpret_cast<void*>(0x00721012), 0x00, 2048, NULL);
				}
			}
		}
		catch (exception)
		{
		}
	}
	return -1;
}


DWORD WINAPI PacketConsumer(LPVOID Args)
{
	while (true)
	{
		Sleep(1);

		try
		{
			if (*((int*)0x006FD000) == 1)
			{
				int target = *((int*)0x006FD004);
				int length = *((int*)0x006FD008);
				unsigned char* data = (unsigned char*)0x006FD012;

				if (data == 0)
					continue;
				if (length <= 0)
					continue;

				vector<byte> packet;
				for (int i = 0; i < length; i++ , data++)
					packet.push_back(*data);

				if (packet.size() == 0 || packet.size() != length)
				{
					packet.clear();
					*((int*)0x006FD000) = 0;
					continue;
				}
				else
				{
					GameFunction::SendToServer(&packet[0], length);
					packet.clear();
					*((int*)0x006FD000) = 0;
					*((int*)0x006FD012) = 0;

					WriteProcessMemory(GetCurrentProcess(), (void*)0x006FD012, 0x00, 2048, NULL);
				}
			}
		}
		catch (exception)
		{
		}
	}
	return -1;
}


void Login(char* username, char* password)
{
	int login = 0x004C1B60;

	/*
		push    eax             ; char *
		lea     ecx, [ebp+var_24]
		push    ecx             ; char *
		mov     ecx, [ebp+var_2C]
		call    sub_4C1B60
	*/
}


HANDLE a, b, c;

BOOL APIENTRY DllMain(HANDLE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		{
			DisableThreadLibraryCalls(NULL);
			da = new Darkages;
			a = CreateThread(NULL, 0, Setup, 0, 0, &da->ProcessId);
			b = CreateThread(NULL, 0, PacketConsumer, 0, 0, &da->ProcessId);
			c = CreateThread(NULL, 0, PacketRecvConsumer, 0, 0, &da->ProcessId);
		}
		break;
	case DLL_PROCESS_DETACH:
		if (da->GameHandle != nullptr)
		{
			CloseHandle(a);
			CloseHandle(b);
			CloseHandle(c);
			da->CleanUp();
		}
		break;
	}
	return TRUE;
}
