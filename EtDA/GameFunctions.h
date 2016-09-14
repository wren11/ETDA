static class GameFunction
{
public:
	static int GetSenderID()
	{

		int value;

		__asm
		{
			mov eax, senderOffset
			mov value, eax
		}

		return *(int*)value;
	}

	static int SendToClient(BYTE *packet, int Length)
	{

		if (Length > 0)
		{

			__asm
			{
				pushfd
				pushad
			}


			int cave = (int)VirtualAllocEx(GetCurrentProcess(), NULL, Length, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
			memcpy((void*)cave, (void*)packet, Length);

			int Recv = recvPacketin;
			int PacketLength = Length;

			__asm
			{
				pop ecx

				mov edx, PacketLength
				push edx

				mov eax, cave
				push eax

				call Recv
				lea ecx, [ebp - 00000405]
				push ecx
			}

			__asm
			{
				popad
				popfd
			}

			VirtualFreeEx(GetCurrentProcess(), (void*)cave, 0, MEM_RELEASE);

			return 0;


		}
	}

	int __stdcall StubSender(BYTE *packet, int Length)
	{
		MessageBox(NULL, "1", "2", MB_OK);
		return 1;
	}


	static int SendToServer(BYTE *packet, int Length)
	{
		int cave = (int)VirtualAllocEx(GetCurrentProcess(), NULL, Length, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
		memcpy((void*)cave, (void*)packet, Length);

		__asm
		{
			pushfd
			pushad
		}


		int SenderID = GetSenderID();
		int Send = sendOffset;
		int PacketLength = Length;

		__asm
		{
			mov edx, PacketLength
			push edx

			mov eax, cave
			push eax

			mov ecx, [SenderID]
			call Send
		}

		__asm
		{
			popad
			popfd
		}

		VirtualFreeEx(GetCurrentProcess(), (void*)cave, 0, MEM_RELEASE);

		return 0;
	}

};

#define ASSAIL GameFunction::SendToServer(new BYTE[] { 0x13, 0x01 }, 3);
#define F5     GameFunction::SendToServer(new BYTE[] { 0x38, 0x01 }, 3);


#ifdef __cplusplus
extern "C"
{
#endif
#define DLL __declspec(dllexport)
	typedef void(__stdcall * ProgressCallback)(int);
	DLL void OnAction(ProgressCallback progressCallback);
#ifdef __cplusplus
}
#endif