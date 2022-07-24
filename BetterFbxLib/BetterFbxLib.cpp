#include <iostream>
#include "stdafx.h"
#include <fbxsdk.h>
#include "BetterFbxLib.h"

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
