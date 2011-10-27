#include <windows.h>
#include "game.hpp"
#include "patch.hpp"

bool b_Hooked = false;

BOOL WINAPI myGetProcessAffinityMask(
	HANDLE hProcess,
	PDWORD_PTR lpProcessAffinityMask,
	PDWORD_PTR lpSystemAffinityMask)
{
	*lpProcessAffinityMask = 7;
	*lpSystemAffinityMask = 7;
	return TRUE;
}

bool GameHook()
{
	if (b_Hooked == true)
	{
		return true;
	}

	HINSTANCE dll = LoadLibraryW(L"kernel32.dll");
	if (dll == NULL)
	{
		return false;
	}

	void *function = GetProcAddress(dll, "GetProcessAffinityMask");
	if (function == NULL)
	{
		return false;
	}

	b_Hooked = true;
	PatchFunctionAbsolute((unsigned int)function, (unsigned int)&myGetProcessAffinityMask);
	return true;
}
