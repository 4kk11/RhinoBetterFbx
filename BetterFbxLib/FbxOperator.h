#pragma once
#include "stdafx.h"
#include <fbxsdk.h>

void SetUpFbxMesh_Vertices(FbxMesh* pFbxMesh, const ON_Mesh* pRhinoMesh);
void SetUpFbxMesh_UV(FbxMesh* pFbxMesh, const ON_Mesh* pRhinoMesh, const CRhinoObject* pRhinoObject);
void SetUpFbxMesh_Faces(FbxMesh* pFbxMesh, const ON_Mesh* pRhinoMesh);
FbxSurfacePhong* CreateMaterial(FbxScene* scene, const ON_Material& onMaterial);
void CreateTexture(FbxScene* scene, FbxSurfacePhong* IMaterial, const ON_Material& onMaterial);
FbxNode* SetUpFbxNode_RhinoLayers(FbxScene* scene, const CRhinoObject* pRhinoObject);
FbxNode* SetupFbxNode_MeshNode(FbxScene* scene, const CRhinoObject* pRhinoObject, FbxMesh* fbxMesh);

static const ON_MappingRef* GetValidMappingRef(const CRhinoObject* pObject, bool withChannels);
char* wStringToChar(const wchar_t* wstr);