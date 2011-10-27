#ifndef __PATCH_HPP
#define __PATCH_HPP

void PatchBytes(unsigned int address, void *data, int length);
void PatchCode(unsigned int address, void *data, int length);
void PatchFunction(unsigned int address, unsigned int target);
void PatchFunctionAbsolute(unsigned int address, unsigned int target);

#endif
