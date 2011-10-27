#ifndef __DXGI_HPP
#define __DXGI_HPP

bool DXGIWrap();
void DXGIUnwrap();

typedef int (*APIWRAPPER)(void);

#ifdef __cplusplus
extern "C"
{
#endif
	extern APIWRAPPER p_D3DKMTCloseAdapter;
	extern APIWRAPPER p_D3DKMTDestroyAllocation;
	extern APIWRAPPER p_D3DKMTDestroyContext;
	extern APIWRAPPER p_D3DKMTDestroyDevice;
	extern APIWRAPPER p_D3DKMTDestroySynchronizationObject;
	extern APIWRAPPER p_D3DKMTQueryAdapterInfo;
	extern APIWRAPPER p_D3DKMTSetDisplayPrivateDriverFormat;
	extern APIWRAPPER p_D3DKMTSignalSynchronizationObject;
	extern APIWRAPPER p_D3DKMTUnlock;
	extern APIWRAPPER p_D3DKMTWaitForSynchronizationObject;
	extern APIWRAPPER p_DXGIDumpJournal;
	extern APIWRAPPER p_OpenAdapter10;
	extern APIWRAPPER p_OpenAdapter10_2;
	extern APIWRAPPER p_CreateDXGIFactory1;
	extern APIWRAPPER p_CreateDXGIFactory;
	extern APIWRAPPER p_D3DKMTCreateAllocation;
	extern APIWRAPPER p_D3DKMTCreateContext;
	extern APIWRAPPER p_D3DKMTCreateDevice;
	extern APIWRAPPER p_D3DKMTCreateSynchronizationObject;
	extern APIWRAPPER p_D3DKMTEscape;
	extern APIWRAPPER p_D3DKMTGetContextSchedulingPriority;
	extern APIWRAPPER p_D3DKMTGetDeviceState;
	extern APIWRAPPER p_D3DKMTGetDisplayModeList;
	extern APIWRAPPER p_D3DKMTGetMultisampleMethodList;
	extern APIWRAPPER p_D3DKMTGetRuntimeData;
	extern APIWRAPPER p_D3DKMTGetSharedPrimaryHandle;
	extern APIWRAPPER p_D3DKMTLock;
	extern APIWRAPPER p_D3DKMTOpenAdapterFromHdc;
	extern APIWRAPPER p_D3DKMTOpenResource;
	extern APIWRAPPER p_D3DKMTPresent;
	extern APIWRAPPER p_D3DKMTQueryAllocationResidency;
	extern APIWRAPPER p_D3DKMTQueryResourceInfo;
	extern APIWRAPPER p_D3DKMTRender;
	extern APIWRAPPER p_D3DKMTSetAllocationPriority;
	extern APIWRAPPER p_D3DKMTSetContextSchedulingPriority;
	extern APIWRAPPER p_D3DKMTSetDisplayMode;
	extern APIWRAPPER p_D3DKMTSetGammaRamp;
	extern APIWRAPPER p_D3DKMTSetVidPnSourceOwner;
	extern APIWRAPPER p_D3DKMTWaitForVerticalBlankEvent;
	extern APIWRAPPER p_DXGID3D10CreateDevice;
	extern APIWRAPPER p_DXGID3D10CreateLayeredDevice;
	extern APIWRAPPER p_DXGID3D10GetLayeredDeviceSize;
	extern APIWRAPPER p_DXGID3D10RegisterLayers;
	extern APIWRAPPER p_DXGIReportAdapterConfiguration;
#ifdef __cplusplus
}
#endif

#endif
