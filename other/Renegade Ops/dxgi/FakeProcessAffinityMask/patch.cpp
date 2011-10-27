#include <windows.h>

void PatchBytes(unsigned int address, void *data, int length)
{
	DWORD old, junk;
	VirtualProtect((void *)address, length, PAGE_EXECUTE_READWRITE, &old);
	memcpy((void *)address, data, length);
	VirtualProtect((void *)address, length, old, &junk);
}

void PatchCode(unsigned int address, void *data, int length)
{
	PatchBytes(address, data, length);
	FlushInstructionCache(GetCurrentProcess(), (void *)address, length);
}

void PatchFunction(unsigned int address, unsigned int target)
{
	unsigned char jump[5];

	jump[0] = 0xE9;
	*(DWORD *)(&jump[1]) = target;

	PatchCode(address, jump, 5);
}

void PatchFunctionAbsolute(unsigned int address, unsigned int target)
{
	PatchFunction(address, target - address - 5);
}
