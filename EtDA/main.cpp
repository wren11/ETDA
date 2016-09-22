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

				//this is a custom packet format. to override the games walking
				//with ETDA's own walking function.
				if (data[0] == 0x95)
				{					
					int Hook = 0x005F0C40;
					int magicTokenPointer = 0x00882E68;
					int magicToken = { 0 };

					void* memory = malloc(sizeof(char));
					memory = (void*)data[1];
					ReadProcessMemory(GetCurrentProcess(), (void*)magicTokenPointer, &magicToken, 4, NULL);

					__asm
					{
						mov eax, memory
						push eax
						mov ecx, [magicToken]
						call Hook
					}
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
