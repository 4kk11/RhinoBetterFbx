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

void ImportFBX(const wchar_t* path)
{
    //FbxImporter
    FbxImporter* importer = FbxImporter::Create(manager, "");
    const char* _path = wStringToChar(path);
    if (importer->Initialize(_path))
    {
        importer->Import(scene);
    }
    importer->Destroy();
    if (_path)
    {
        delete[] _path;
        _path = nullptr;
    }
}

void ExportFBX(bool isAscii, int axisSelect, int unitSelect, const wchar_t* path)
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
            break;
        case 1:
            axisSystem = FbxAxisSystem::Max; // ZAxis, -ParityOdd, RightHanded.
            scene->GetGlobalSettings().SetAxisSystem(axisSystem);
            break;
        case 2:
            axisSystem = FbxAxisSystem::Motionbuilder; // YAxis, ParityOdd, RightHanded.
            scene->GetGlobalSettings().SetAxisSystem(axisSystem);
            break;
        case 3:
            axisSystem = FbxAxisSystem::DirectX; // YAxis, ParityOdd, LeftHanded.
            scene->GetGlobalSettings().SetAxisSystem(axisSystem);
            break;
        default:
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

    //TODO: ユニットスケールについての詳しい検証
    switch (unitSelect)
    {
        case 0:
            break;
        case 1: //mm
            //FbxSystemUnit::mm.ConvertScene(scene);
            scene->GetGlobalSettings().SetSystemUnit(FbxSystemUnit::mm);
            break;
        case 2: //cm
            FbxSystemUnit::cm.ConvertScene(scene);
            scene->GetGlobalSettings().SetSystemUnit(FbxSystemUnit::cm);
            break;
        case 3: //m
            FbxSystemUnit::m.ConvertScene(scene);
            scene->GetGlobalSettings().SetSystemUnit(FbxSystemUnit::m);
            break;
        default:
            break;
    }

    
   

    const char* _path = wStringToChar(path);

    FbxExporter* IExporter = FbxExporter::Create(manager, "");

    /*
    FbxNode* root = scene->GetRootNode();
    FbxNode* parent = root->GetChild(0);
    FbxNode* child1 = parent->GetChild(0);
    FbxNode* child2 = child1->GetChild(0);
    FbxMesh* mesh = child2->GetMesh();
    const char* name = mesh->GetName();
    */
    
    FbxIOSettings* ios = FbxIOSettings::Create(manager, IOSROOT);
    ios->SetBoolProp(EXP_FBX_EMBEDDED, true);

    if (IExporter->Initialize(_path, pFileFormat, ios))
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
    const ON_Mesh* rhinoMesh;
    if (pRhinoObject->ObjectType() == ON::mesh_object)
    {
        rhinoMesh = dynamic_cast<const ON_Mesh*>(pRhinoObject->Geometry());
    }
    else
    {
        ON_SimpleArray<const ON_Mesh*> meshes;
        pRhinoObject->GetMeshes(ON::preview_mesh, meshes);
        rhinoMesh = meshes[0];
    }

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
    pFbxNode->RemoveNodeAttributeByIndex(0);
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

void FbxNodeAttribute_Delete(FbxMesh* pFbxNodeAttr)
{
    if (!pFbxNodeAttr) return;
    FbxManager* manager = pFbxNodeAttr->GetFbxManager();
    if (manager) manager->Destroy();
    else pFbxNodeAttr->Destroy();
    
}
//</FbxNodeAttributeComponent>

//<FbxExporterCompornent>
void FbxExporter_Set(FbxNode* pFbxNode)
{
    FbxCloneManager cloneManager;
    FbxNode* clone = (FbxNode*)cloneManager.Clone(pFbxNode);
    //RootNodeに入れると何らかの原因でこわれる？ルートの一個下のノードしか残らない...
    // 最初の一回目は正常だが、二回目に壊れている。
    //cloneを生成・入力することで一応回避できる。。
    FbxNode* rootNode = scene->GetRootNode();
    rootNode->AddChild(clone);
}

//</FbxExporterComponent>
