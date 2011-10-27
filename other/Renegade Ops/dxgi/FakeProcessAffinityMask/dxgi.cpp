#include <windows.h>
#include <shlwapi.h>
#include <stdio.h>
#include <stdlib.h>

#include "dxgi.hpp"

extern "C"
{
	APIWRAPPER p_D3DKMTCloseAdapter;
	APIWRAPPER p_D3DKMTDestroyAllocation;
	APIWRAPPER p_D3DKMTDestroyContext;
	APIWRAPPER p_D3DKMTDestroyDevice;
	APIWRAPPER p_D3DKMTDestroySynchronizationObject;
	APIWRAPPER p_D3DKMTQueryAdapterInfo;
	APIWRAPPER p_D3DKMTSetDisplayPrivateDriverFormat;
	APIWRAPPER p_D3DKMTSignalSynchronizationObject;
	APIWRAPPER p_D3DKMTUnlock;
	APIWRAPPER p_D3DKMTWaitForSynchronizationObject;
	APIWRAPPER p_DXGIDumpJournal;
	APIWRAPPER p_OpenAdapter10;
	APIWRAPPER p_OpenAdapter10_2;
	APIWRAPPER p_CreateDXGIFactory1;
	APIWRAPPER p_CreateDXGIFactory;
	APIWRAPPER p_D3DKMTCreateAllocation;
	APIWRAPPER p_D3DKMTCreateContext;
	APIWRAPPER p_D3DKMTCreateDevice;
	APIWRAPPER p_D3DKMTCreateSynchronizationObject;
	APIWRAPPER p_D3DKMTEscape;
	APIWRAPPER p_D3DKMTGetContextSchedulingPriority;
	APIWRAPPER p_D3DKMTGetDeviceState;
	APIWRAPPER p_D3DKMTGetDisplayModeList;
	APIWRAPPER p_D3DKMTGetMultisampleMethodList;
	APIWRAPPER p_D3DKMTGetRuntimeData;
	APIWRAPPER p_D3DKMTGetSharedPrimaryHandle;
	APIWRAPPER p_D3DKMTLock;
	APIWRAPPER p_D3DKMTOpenAdapterFromHdc;
	APIWRAPPER p_D3DKMTOpenResource;
	APIWRAPPER p_D3DKMTPresent;
	APIWRAPPER p_D3DKMTQueryAllocationResidency;
	APIWRAPPER p_D3DKMTQueryResourceInfo;
	APIWRAPPER p_D3DKMTRender;
	APIWRAPPER p_D3DKMTSetAllocationPriority;
	APIWRAPPER p_D3DKMTSetContextSchedulingPriority;
	APIWRAPPER p_D3DKMTSetDisplayMode;
	APIWRAPPER p_D3DKMTSetGammaRamp;
	APIWRAPPER p_D3DKMTSetVidPnSourceOwner;
	APIWRAPPER p_D3DKMTWaitForVerticalBlankEvent;
	APIWRAPPER p_DXGID3D10CreateDevice;
	APIWRAPPER p_DXGID3D10CreateLayeredDevice;
	APIWRAPPER p_DXGID3D10GetLayeredDeviceSize;
	APIWRAPPER p_DXGID3D10RegisterLayers;
	APIWRAPPER p_DXGIReportAdapterConfiguration;
}

void __declspec(naked) h_D3DKMTCloseAdapter() { _asm{ jmp p_D3DKMTCloseAdapter } }
void __declspec(naked) h_D3DKMTDestroyAllocation() { _asm{ jmp p_D3DKMTDestroyAllocation } }
void __declspec(naked) h_D3DKMTDestroyContext() { _asm{ jmp p_D3DKMTDestroyContext } }
void __declspec(naked) h_D3DKMTDestroyDevice() { _asm{ jmp p_D3DKMTDestroyDevice } }
void __declspec(naked) h_D3DKMTDestroySynchronizationObject() { _asm{ jmp p_D3DKMTDestroySynchronizationObject } }
void __declspec(naked) h_D3DKMTQueryAdapterInfo() { _asm{ jmp p_D3DKMTQueryAdapterInfo } }
void __declspec(naked) h_D3DKMTSetDisplayPrivateDriverFormat() { _asm{ jmp p_D3DKMTSetDisplayPrivateDriverFormat } }
void __declspec(naked) h_D3DKMTSignalSynchronizationObject() { _asm{ jmp p_D3DKMTSignalSynchronizationObject } }
void __declspec(naked) h_D3DKMTUnlock() { _asm{ jmp p_D3DKMTUnlock } }
void __declspec(naked) h_D3DKMTWaitForSynchronizationObject() { _asm{ jmp p_D3DKMTWaitForSynchronizationObject } }
void __declspec(naked) h_DXGIDumpJournal() { _asm{ jmp p_DXGIDumpJournal } }
void __declspec(naked) h_OpenAdapter10() { _asm{ jmp p_OpenAdapter10 } }
void __declspec(naked) h_OpenAdapter10_2() { _asm{ jmp p_OpenAdapter10_2 } }
void __declspec(naked) h_CreateDXGIFactory1() { _asm{ jmp p_CreateDXGIFactory1 } }
void __declspec(naked) h_CreateDXGIFactory() { _asm{ jmp p_CreateDXGIFactory } }
void __declspec(naked) h_D3DKMTCreateAllocation() { _asm{ jmp p_D3DKMTCreateAllocation } }
void __declspec(naked) h_D3DKMTCreateContext() { _asm{ jmp p_D3DKMTCreateContext } }
void __declspec(naked) h_D3DKMTCreateDevice() { _asm{ jmp p_D3DKMTCreateDevice } }
void __declspec(naked) h_D3DKMTCreateSynchronizationObject() { _asm{ jmp p_D3DKMTCreateSynchronizationObject } }
void __declspec(naked) h_D3DKMTEscape() { _asm{ jmp p_D3DKMTEscape } }
void __declspec(naked) h_D3DKMTGetContextSchedulingPriority() { _asm{ jmp p_D3DKMTGetContextSchedulingPriority } }
void __declspec(naked) h_D3DKMTGetDeviceState() { _asm{ jmp p_D3DKMTGetDeviceState } }
void __declspec(naked) h_D3DKMTGetDisplayModeList() { _asm{ jmp p_D3DKMTGetDisplayModeList } }
void __declspec(naked) h_D3DKMTGetMultisampleMethodList() { _asm{ jmp p_D3DKMTGetMultisampleMethodList } }
void __declspec(naked) h_D3DKMTGetRuntimeData() { _asm{ jmp p_D3DKMTGetRuntimeData } }
void __declspec(naked) h_D3DKMTGetSharedPrimaryHandle() { _asm{ jmp p_D3DKMTGetSharedPrimaryHandle } }
void __declspec(naked) h_D3DKMTLock() { _asm{ jmp p_D3DKMTLock } }
void __declspec(naked) h_D3DKMTOpenAdapterFromHdc() { _asm{ jmp p_D3DKMTOpenAdapterFromHdc } }
void __declspec(naked) h_D3DKMTOpenResource() { _asm{ jmp p_D3DKMTOpenResource } }
void __declspec(naked) h_D3DKMTPresent() { _asm{ jmp p_D3DKMTPresent } }
void __declspec(naked) h_D3DKMTQueryAllocationResidency() { _asm{ jmp p_D3DKMTQueryAllocationResidency } }
void __declspec(naked) h_D3DKMTQueryResourceInfo() { _asm{ jmp p_D3DKMTQueryResourceInfo } }
void __declspec(naked) h_D3DKMTRender() { _asm{ jmp p_D3DKMTRender } }
void __declspec(naked) h_D3DKMTSetAllocationPriority() { _asm{ jmp p_D3DKMTSetAllocationPriority } }
void __declspec(naked) h_D3DKMTSetContextSchedulingPriority() { _asm{ jmp p_D3DKMTSetContextSchedulingPriority } }
void __declspec(naked) h_D3DKMTSetDisplayMode() { _asm{ jmp p_D3DKMTSetDisplayMode } }
void __declspec(naked) h_D3DKMTSetGammaRamp() { _asm{ jmp p_D3DKMTSetGammaRamp } }
void __declspec(naked) h_D3DKMTSetVidPnSourceOwner() { _asm{ jmp p_D3DKMTSetVidPnSourceOwner } }
void __declspec(naked) h_D3DKMTWaitForVerticalBlankEvent() { _asm{ jmp p_D3DKMTWaitForVerticalBlankEvent } }
void __declspec(naked) h_DXGID3D10CreateDevice() { _asm{ jmp p_DXGID3D10CreateDevice } }
void __declspec(naked) h_DXGID3D10CreateLayeredDevice() { _asm{ jmp p_DXGID3D10CreateLayeredDevice } }
void __declspec(naked) h_DXGID3D10GetLayeredDeviceSize() { _asm{ jmp p_DXGID3D10GetLayeredDeviceSize } }
void __declspec(naked) h_DXGID3D10RegisterLayers() { _asm{ jmp p_DXGID3D10RegisterLayers } }
void __declspec(naked) h_DXGIReportAdapterConfiguration() { _asm{ jmp p_DXGIReportAdapterConfiguration } }

HINSTANCE h_Original;

bool DXGIWrap()
{
	wchar_t path[MAX_PATH];
	GetSystemDirectoryW(path, MAX_PATH);
	PathAppend(path, L"dxgi.dll");

	h_Original = LoadLibraryW(path);
	if (h_Original == NULL)
	{
		return false;
	}

	p_D3DKMTCloseAdapter = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTCloseAdapter");
	p_D3DKMTDestroyAllocation = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTDestroyAllocation");
	p_D3DKMTDestroyContext = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTDestroyContext");
	p_D3DKMTDestroyDevice = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTDestroyDevice");
	p_D3DKMTDestroySynchronizationObject = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTDestroySynchronizationObject");
	p_D3DKMTQueryAdapterInfo = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTQueryAdapterInfo");
	p_D3DKMTSetDisplayPrivateDriverFormat = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTSetDisplayPrivateDriverFormat");
	p_D3DKMTSignalSynchronizationObject = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTSignalSynchronizationObject");
	p_D3DKMTUnlock = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTUnlock");
	p_D3DKMTWaitForSynchronizationObject = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTWaitForSynchronizationObject");
	p_DXGIDumpJournal = (APIWRAPPER)GetProcAddress(h_Original, "DXGIDumpJournal");
	p_OpenAdapter10 = (APIWRAPPER)GetProcAddress(h_Original, "OpenAdapter10");
	p_OpenAdapter10_2 = (APIWRAPPER)GetProcAddress(h_Original, "OpenAdapter10_2");
	p_CreateDXGIFactory1 = (APIWRAPPER)GetProcAddress(h_Original, "CreateDXGIFactory1");
	p_CreateDXGIFactory = (APIWRAPPER)GetProcAddress(h_Original, "CreateDXGIFactory");
	p_D3DKMTCreateAllocation = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTCreateAllocation");
	p_D3DKMTCreateContext = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTCreateContext");
	p_D3DKMTCreateDevice = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTCreateDevice");
	p_D3DKMTCreateSynchronizationObject = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTCreateSynchronizationObject");
	p_D3DKMTEscape = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTEscape");
	p_D3DKMTGetContextSchedulingPriority = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTGetContextSchedulingPriority");
	p_D3DKMTGetDeviceState = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTGetDeviceState");
	p_D3DKMTGetDisplayModeList = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTGetDisplayModeList");
	p_D3DKMTGetMultisampleMethodList = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTGetMultisampleMethodList");
	p_D3DKMTGetRuntimeData = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTGetRuntimeData");
	p_D3DKMTGetSharedPrimaryHandle = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTGetSharedPrimaryHandle");
	p_D3DKMTLock = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTLock");
	p_D3DKMTOpenAdapterFromHdc = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTOpenAdapterFromHdc");
	p_D3DKMTOpenResource = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTOpenResource");
	p_D3DKMTPresent = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTPresent");
	p_D3DKMTQueryAllocationResidency = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTQueryAllocationResidency");
	p_D3DKMTQueryResourceInfo = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTQueryResourceInfo");
	p_D3DKMTRender = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTRender");
	p_D3DKMTSetAllocationPriority = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTSetAllocationPriority");
	p_D3DKMTSetContextSchedulingPriority = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTSetContextSchedulingPriority");
	p_D3DKMTSetDisplayMode = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTSetDisplayMode");
	p_D3DKMTSetGammaRamp = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTSetGammaRamp");
	p_D3DKMTSetVidPnSourceOwner = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTSetVidPnSourceOwner");
	p_D3DKMTWaitForVerticalBlankEvent = (APIWRAPPER)GetProcAddress(h_Original, "D3DKMTWaitForVerticalBlankEvent");
	p_DXGID3D10CreateDevice = (APIWRAPPER)GetProcAddress(h_Original, "DXGID3D10CreateDevice");
	p_DXGID3D10CreateLayeredDevice = (APIWRAPPER)GetProcAddress(h_Original, "DXGID3D10CreateLayeredDevice");
	p_DXGID3D10GetLayeredDeviceSize = (APIWRAPPER)GetProcAddress(h_Original, "DXGID3D10GetLayeredDeviceSize");
	p_DXGID3D10RegisterLayers = (APIWRAPPER)GetProcAddress(h_Original, "DXGID3D10RegisterLayers");
	p_DXGIReportAdapterConfiguration = (APIWRAPPER)GetProcAddress(h_Original, "DXGIReportAdapterConfiguration");

	return
		p_D3DKMTCloseAdapter != NULL &&
		p_D3DKMTDestroyAllocation != NULL &&
		p_D3DKMTDestroyContext != NULL &&
		p_D3DKMTDestroyDevice != NULL &&
		p_D3DKMTDestroySynchronizationObject != NULL &&
		p_D3DKMTQueryAdapterInfo != NULL &&
		p_D3DKMTSetDisplayPrivateDriverFormat != NULL &&
		p_D3DKMTSignalSynchronizationObject != NULL &&
		p_D3DKMTUnlock != NULL &&
		p_D3DKMTWaitForSynchronizationObject != NULL &&
		p_DXGIDumpJournal != NULL &&
		p_OpenAdapter10 != NULL &&
		p_OpenAdapter10_2 != NULL &&
		p_CreateDXGIFactory1 != NULL &&
		p_CreateDXGIFactory != NULL &&
		p_D3DKMTCreateAllocation != NULL &&
		p_D3DKMTCreateContext != NULL &&
		p_D3DKMTCreateDevice != NULL &&
		p_D3DKMTCreateSynchronizationObject != NULL &&
		p_D3DKMTEscape != NULL &&
		p_D3DKMTGetContextSchedulingPriority != NULL &&
		p_D3DKMTGetDeviceState != NULL &&
		p_D3DKMTGetDisplayModeList != NULL &&
		p_D3DKMTGetMultisampleMethodList != NULL &&
		p_D3DKMTGetRuntimeData != NULL &&
		p_D3DKMTGetSharedPrimaryHandle != NULL &&
		p_D3DKMTLock != NULL &&
		p_D3DKMTOpenAdapterFromHdc != NULL &&
		p_D3DKMTOpenResource != NULL &&
		p_D3DKMTPresent != NULL &&
		p_D3DKMTQueryAllocationResidency != NULL &&
		p_D3DKMTQueryResourceInfo != NULL &&
		p_D3DKMTRender != NULL &&
		p_D3DKMTSetAllocationPriority != NULL &&
		p_D3DKMTSetContextSchedulingPriority != NULL &&
		p_D3DKMTSetDisplayMode != NULL &&
		p_D3DKMTSetGammaRamp != NULL &&
		p_D3DKMTSetVidPnSourceOwner != NULL &&
		p_D3DKMTWaitForVerticalBlankEvent != NULL &&
		p_DXGID3D10CreateDevice != NULL &&
		p_DXGID3D10CreateLayeredDevice != NULL &&
		p_DXGID3D10GetLayeredDeviceSize != NULL &&
		p_DXGID3D10RegisterLayers != NULL &&
		p_DXGIReportAdapterConfiguration != NULL;
}

void DXGIUnwrap()
{
	FreeLibrary(h_Original);
	h_Original = NULL;
}
