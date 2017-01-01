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
			if (*reinterpret_cast<int*>(RecvConsumerPacketAvailable) == 1)
			{
				auto target = *reinterpret_cast<int*>(RecvConsumerPacketType);
				auto length = *reinterpret_cast<int*>(RecvConsumerPacketLength);
				auto data = reinterpret_cast<unsigned char*>(RecvConsumerPacketData);

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
					*reinterpret_cast<int*>(RecvConsumerPacketAvailable) = 0;
					continue;
				}
				else
				{
					GameFunction::SendToClient(&packet[0], length);
					packet.clear();
					*reinterpret_cast<int*>(RecvConsumerPacketAvailable) = 0;
					*reinterpret_cast<int*>(RecvConsumerPacketData) = 0;
					WriteProcessMemory(GetCurrentProcess(), reinterpret_cast<void*>(RecvConsumerPacketData), 0x00, 2048, NULL);
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
			if (*reinterpret_cast<int*>(SendConsumerPacketAvailable) == 1)
			{
				auto target = *reinterpret_cast<int*>(SendConsumerPacketType);
				auto length = *reinterpret_cast<int*>(SendConsumerPacketLength);
				auto data = reinterpret_cast<unsigned char*>(SendConsumerPacketData);

				if (data == 0)
					continue;
				if (length <= 0)
					continue;

				if (data[0] == 0x94)
				{
					//5EFFE0((void *)v6, v10, v9, a2, 1);
					//char __thiscall sub_5EFFE0(void *this, int a2, int a3, char a4, char a5);

					char(__thiscall *E)(void*, DAPoint, char steps, char direction) 
						= (char(__thiscall*)(void*, DAPoint, char steps, char direction))0x005EFFE0;

					int thisptr = *(int*)0x00882E68;
				}

				vector<byte> packet;
				for (int i = 0; i < length; i++, data++)
					packet.push_back(*data);

				if (packet.size() == 0 || packet.size() != length)
				{
					packet.clear();
					*reinterpret_cast<int*>(SendConsumerPacketAvailable) = 0;
					continue;
				}
				else
				{
					GameFunction::SendToServer(&packet[0], length);
					packet.clear();
					*reinterpret_cast<int*>(SendConsumerPacketAvailable) = 0;
					*reinterpret_cast<int*>(SendConsumerPacketData) = 0;
					WriteProcessMemory(GetCurrentProcess(), reinterpret_cast<void*>(SendConsumerPacketData), 0x00, 2048, NULL);
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
