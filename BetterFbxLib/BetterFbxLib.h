#pragma once
#include <fbxsdk.h>

extern "C"
{
    const ON_Mesh* mesh = nullptr;
    FbxManager* manager = nullptr;
    FbxScene* scene = nullptr;
    
    __declspec(dllexport) void CreateManager();
    __declspec(dllexport) void DeleteManager();
    __declspec(dllexport) void ExportFBX();
    __declspec(dllexport) int GetvCount(ON_3dPointArray* pts);
    //__declspec(dllexport) void CreateNode(const ON_Mesh* pMesh, const ON_SimpleArray<ON_wString>* layerNames, const wchar_t* objectName);
    __declspec(dllexport) void CreateNode(const CRhinoObject* rhinoObject, const ON_SimpleArray<ON_wString>* layerNames, const wchar_t* objectName);

    static const ON_MappingRef* GetValidMappingRef(const CRhinoObject* pObject, bool withChannels);
}