#pragma once
#include <fbxsdk.h>


extern "C"
{
    __declspec(dllexport) void CreateManager();
    __declspec(dllexport) void DeleteManager();
    __declspec(dllexport) void ExportFBX(bool isAscii);
    __declspec(dllexport) void CreateNode(const CRhinoObject* rhinoObject);

}

