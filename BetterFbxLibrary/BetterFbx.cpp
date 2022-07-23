#include <iostream>

#include "BetterFbxLibrary.h"
#include <fbxsdk.h>

void SetNum(int n)
{
    num += 1;
}

int GetNum()
{
    FbxManager* fbx_manager = FbxManager::Create();
    fbx_manager->Destroy();
    return num;
}
