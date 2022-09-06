#include <iostream>
#include <Windows.h>
#include "stdafx.h"
#include "FbxOperator.h"
#include "BetterFbxLib.h"


const ON_Mesh* mesh = nullptr;
FbxManager* manager = nullptr;
FbxScene* scene = nullptr;

void CreateManager()
{
    manager = FbxManager::Create();
    scene = FbxScene::Create(manager, "");
    
}

void DeleteManager()
{
    manager->Destroy();
    manager = nullptr;
}

void ExportFBX(bool isAscii, int axisSelect, const wchar_t* path)
{
    int pFileFormat = manager->GetIOPluginRegistry()->GetNativeWriterFormat();

    if (isAscii)
    {
        int IFormatIndex, IFormatCount = manager->GetIOPluginRegistry()->GetWriterFormatCount();
        for (IFormatIndex = 0; IFormatIndex < IFormatCount; ++IFormatIndex)
        {
            if (manager->GetIOPluginRegistry()->WriterIsFBX(IFormatIndex))
            {
                FbxString IDesc = manager->GetIOPluginRegistry()->GetWriterFormatDescription(IFormatIndex);
                const char* IASCII = "ascii";
                if (IDesc.Find(IASCII) >= 0)
                {
                    pFileFormat = IFormatIndex;
                    break;
                }
            }
        }
    }

    //default of rhino, blender (z-up, righthand)
    
    FbxAxisSystem axisSystem = FbxAxisSystem::Max;

    switch (axisSelect)
    {
        case 0:
            axisSystem = FbxAxisSystem::Max; // ZAxis, -ParityOdd, RightHanded.
            break;
        case 1:
            axisSystem = FbxAxisSystem::Motionbuilder; // YAxis, ParityOdd, RightHanded.
            break;
        case 2:
            axisSystem = FbxAxisSystem::DirectX; // YAxis, ParityOdd, LeftHanded.
            break;
        default:
            axisSystem = FbxAxisSystem::Max;
            break;
    }
    
    //TODO: cover more freedom AxisSystem
    /*
    bool is_Yup = false;
    bool is_Left = false;
    int uvecSign = 1;
    int fvecSign = 1;

    FbxAxisSystem::EUpVector upvec = FbxAxisSystem::eZAxis;
    FbxAxisSystem::EFrontVector frontvec = static_cast <FbxAxisSystem::EFrontVector>(-1);
    FbxAxisSystem::ECoordSystem coordSystem = FbxAxisSystem::eRightHanded;

    if (is_Yup) //if is (Z-up or Y-up) (false or true)
    {
        upvec = FbxAxisSystem::eYAxis;
    }

    if (is_Left) //if is (right or left) (false or true)
    {
        coordSystem = FbxAxisSystem::eLeftHanded;
    }
    
    FbxAxisSystem axisSystem(upvec,frontvec, coordSystem);
    */ 

    scene->GetGlobalSettings().SetSystemUnit(FbxSystemUnit::mm);
    scene->GetGlobalSettings().SetAxisSystem(axisSystem);

    const char* _path = wStringToChar(path);

    FbxExporter* IExporter = FbxExporter::Create(manager, "");
    if (IExporter->Initialize(_path, pFileFormat))
    {
        IExporter->Export(scene);
    }
    IExporter->Destroy();
    if (_path)
    {
        delete[] _path;
        _path = nullptr;
    }
}


void CreateNode(const CRhinoObject* pRhinoObject)
{
    if (pRhinoObject == NULL) return;

    const ON_Mesh* rhinoMesh = dynamic_cast<const ON_Mesh*>(pRhinoObject->Geometry());
    if (!rhinoMesh) return;

    //Create FbxMesh
    FbxMesh* fbxMesh = FbxMesh::Create(scene, "mesh");
    SetUpFbxMesh_Vertices(fbxMesh, rhinoMesh);
    SetUpFbxMesh_UV(fbxMesh, rhinoMesh, pRhinoObject);
    SetUpFbxMesh_Faces(fbxMesh, rhinoMesh);

    //Create Material 
    const ON_Material rhinoMaterial = pRhinoObject->ObjectMaterial();
    FbxSurfaceMaterial* fbxMaterial = CreateMaterial(scene, rhinoMaterial); //TODO: share material

    //Create LayerNode
    FbxNode* terminalNode = SetUpFbxNode_RhinoLayers(scene, pRhinoObject);

    //Create MeshNode
    FbxNode* meshNode = SetupFbxNode_MeshNode(scene, pRhinoObject, fbxMesh);
    meshNode->AddMaterial(fbxMaterial); //SetMaterial
    
    //Custom properties
    AddCustomProperty_FromRhino(fbxMesh, pRhinoObject);

    terminalNode->AddChild(meshNode);
}

//<FbxNodeComponent>
FbxNode* FbxNode_New(const wchar_t* name)
{
    FbxManager* manager_temp = FbxManager::Create();
    const char* _name = wStringToChar(name);
    FbxNode* node = FbxNode::Create(manager_temp, _name);
    delete[] _name;
    _name = nullptr;
    return node;
}

void FbxNode_AddChild(FbxNode* pFbxNode_parent, FbxNode* pFbxNode_child)
{
    pFbxNode_parent->AddChild(pFbxNode_child);
}

void FbxNode_SetAttribute(FbxNode* pFbxNode, FbxNodeAttribute* pFbxNodeAttr)
{
    pFbxNode->SetNodeAttribute(pFbxNodeAttr);
}

void FbxNode_Delete(FbxNode* pFbxNode)
{
    pFbxNode->GetFbxManager()->Destroy();
}

//</FbxNodeComponent>

//<FbxNodeAttributeComponent>

FbxMesh* FbxMesh_New(const ON_Mesh* pRhinoMesh)
{
    FbxManager* manager_temp = FbxManager::Create();
    FbxMesh* pFbxMesh = FbxMesh::Create(manager_temp, "mesh");
    SetUpFbxMesh_Vertices(pFbxMesh, pRhinoMesh);
    SetUpFbxMesh_Faces(pFbxMesh, pRhinoMesh);

    return pFbxMesh;
}

void FbxNodeAttribute_Delete(FbxNodeAttribute* pFbxNodeAttr)
{
    pFbxNodeAttr->GetFbxManager()->Destroy();
}
//</FbxNodeAttributeComponent>
