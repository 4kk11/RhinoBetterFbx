#pragma once
#include <fbxsdk.h>


extern "C"
{
    __declspec(dllexport) void CreateManager();
    __declspec(dllexport) void DeleteManager();
    __declspec(dllexport) void ExportFBX(bool isAscii);
    __declspec(dllexport) int GetvCount(ON_3dPointArray* pts);
    __declspec(dllexport) void CreateNode(const CRhinoObject* rhinoObject);

}

