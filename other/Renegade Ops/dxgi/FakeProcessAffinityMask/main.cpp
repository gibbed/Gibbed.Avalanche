#include <windows.h>
#include <shlwapi.h>
#include <stdio.h>
#include <stdlib.h>

#include "game.hpp"
#include "dxgi.hpp"

int GetCoreCount()
{
	HANDLE process = GetCurrentProcess();
	
	DWORD ProcessAffinityMask;
	DWORD SystemAffinityMask;

	if (GetProcessAffinityMask(process, &ProcessAffinityMask, &SystemAffinityMask) == FALSE)
	{
		MessageBoxW(
			0,
			L"GetProcessAffinityMask() return FALSE? Strange issue! Game would crash anyway.",
			L"Error",
			MB_OK | MB_ICONERROR);
		ExitProcess(0);
	}

	int count = 0;
    DWORD mask = 2;
	for (int i = 0; i < 31; i++)
    {
      if ((mask & ProcessAffinityMask) != 0)
	  {
		  count++;
	  }

	  mask = _rotl(mask, 1);
    }

	return count;
}

BOOL APIENTRY DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	switch (fdwReason)
	{
		case DLL_PROCESS_ATTACH:
		{
			if (DXGIWrap() == false)
			{
				MessageBoxW(
					0,
					L"Real dxgi.dll appears to be missing!",
					L"Error",
					MB_OK | MB_ICONERROR);
				ExitProcess(0);
				return FALSE;
			}
			
			int cores = GetCoreCount();
			if (cores > 1)
			{
				MessageBoxW(
					0,
					L"Renegade Ops should function normally for you,\ngibbed's dxgi.dll isn't necessary!\n(and it's not going to patch your game)\n\nIf the game crashes, it's not the dual-core issue.",
					L"Error",
					MB_OK | MB_ICONWARNING);
			}
			else
			{
				if (GameHook() == false)
				{
					MessageBoxW(
						0,
						L"Failed to patch game. :(",
						L"Error",
						MB_OK | MB_ICONERROR);
					ExitProcess(0);
					return FALSE;
				}
			}
			
			break;
		}

		case DLL_PROCESS_DETACH:
		{
			DXGIUnwrap();
			break;
		}
	}

	return TRUE;
}
