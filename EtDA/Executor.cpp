#include "common.h"

Executor::Executor()
{ 

}

Executor::~Executor()
{

}

void* Executor::HookFunction(DWORD address, DWORD jumpto)
{
	return (void*)DetourFunction((PBYTE)address, (PBYTE)jumpto);
}