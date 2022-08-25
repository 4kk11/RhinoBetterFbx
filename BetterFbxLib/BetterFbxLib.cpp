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

void ExportFBX(bool isAscii)
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

    FbxExporter* IExporter = FbxExporter::Create(manager, "");
    if (IExporter->Initialize("D:/Users/akiak/Desktop/export/test.fbx", pFileFormat))
    {
        IExporter->Export(scene);
    }
    IExporter->Destroy();
}

int GetvCount(ON_3dPointArray* pts)
{
    //FbxManager* fbx_manager = FbxManager::Create();
    //fbx_manager->Destroy();
    
    //pts[0] = mesh->m_V[0];
    //pts[1] = mesh->m_V[1];
    int vCount = mesh->VertexCount();
    //const ON_3fPoint* _pts = mesh->m_V.Array();
    
    ON_3dPointArray _pts = mesh->DoublePrecisionVertices();
    
    pts->Append(_pts.Count(), _pts.Array());
    
    
    return vCount;
}

void CreateNode(const CRhinoObject* pRhinoObject, const ON_SimpleArray<ON_wString>* layerNames, const wchar_t* objectName)
{
    if (pRhinoObject == NULL) return;

    const ON_Mesh* pMesh = dynamic_cast<const ON_Mesh*>(pRhinoObject->Geometry());
    if (!pMesh) return;

    //Create FbxMesh
    FbxMesh* IMesh = FbxMesh::Create(scene, "test_mesh");
    SetUpFbxMesh_Vertices(IMesh, pMesh);
    SetUpFbxMesh_UV(IMesh, pMesh, pRhinoObject);
    SetUpFbxMesh_Faces(IMesh, pMesh);

    //Create Material
    const ON_Material onMaterial = pRhinoObject->ObjectMaterial();
    FbxSurfaceMaterial* IMaterial = CreateMaterial(scene, onMaterial);

    //Create Node & Set Mesh
    FbxNode* rootNode = scene->GetRootNode();
    FbxNode* currentNode = rootNode;
    int nodeCount = layerNames->Count();
    for (int i = 0; i < nodeCount; i++)
    {
        char* _name = wStringToChar(layerNames->At(i)->Array());
        FbxNode* nextNode = currentNode->FindChild(_name);
        if (nextNode == nullptr)
        {
            nextNode = FbxNode::Create(scene, _name);
            currentNode->AddChild(nextNode);
            currentNode = nextNode;
        }
        else
        {
            currentNode = nextNode;
        }
        delete[] _name;
        _name = nullptr;
    }
    


    /// オブジェクト名を拾う
    char* _text = wStringToChar(objectName);
    /// 末端のノードを作成し、メッシュを格納する
    FbxNode* INode = FbxNode::Create(scene, _text);
    delete[] _text;
    _text = nullptr;
    INode->SetNodeAttribute(IMesh);
    INode->AddMaterial(IMaterial); //SetMaterial
    currentNode->AddChild(INode);
    

    //Custom properties
    FbxPropertyT<FbxString> IProperty = FbxProperty::Create(INode, FbxStringDT, "PropOnNode");
    IProperty.ModifyFlag(FbxPropertyFlags::eUserDefined, true);
    IProperty.ModifyFlag(FbxPropertyFlags::eAnimatable, true);
    IProperty.Set("test prop");

    
    
}



