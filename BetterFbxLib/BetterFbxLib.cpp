#include <iostream>
#include <Windows.h>
#include "stdafx.h"
#include "BetterFbxLib.h"

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

void ExportFBX()
{
    FbxExporter* IExporter = FbxExporter::Create(manager, "");
    if (IExporter->Initialize("D:/Users/akiak/Desktop/export/test.fbx"))
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


void CreateNode(const ON_Mesh* pMesh, const ON_SimpleArray<ON_wString>* layerNames)
{
    if (!pMesh) return;

    ON_3dPointArray vertices = pMesh->DoublePrecisionVertices();
    ON_SimpleArray<ON_MeshFace> faces = pMesh->m_F;
    ON_3fVectorArray vNormals = pMesh->m_N;
    ON_3fVectorArray fNornals = pMesh->m_FN;
    int vCount = pMesh->VertexCount();
    int fCount = pMesh->FaceCount();
    
    //Create FbxMesh
    FbxMesh* IMesh = FbxMesh::Create(scene, "test_mesh");

    //Each Vertex
    IMesh->InitControlPoints(vCount);
    FbxVector4* fbx_vertices = IMesh->GetControlPoints();
    FbxGeometryElementNormal* fbx_vNormals = IMesh->CreateElementNormal();
    fbx_vNormals->SetMappingMode(FbxGeometryElement::eByControlPoint);
    fbx_vNormals->SetReferenceMode(FbxGeometryElement::eDirect);
    for (int i=0; i < vCount; i++)
    {
        //convert to fbxsdk class and Set
        double vertX = vertices.At(0)->x;
        FbxVector4 vert(vertices.At(i)->x, vertices.At(i)->y, vertices.At(i)->z);
        fbx_vertices[i] = vert;
        FbxVector4 nor(vNormals[i].x, vNormals[i].y, vNormals[i].z);
        fbx_vNormals->GetDirectArray().Add(nor);

    }

    //Each Face
    for (int i = 0; i < fCount; i++)
    {
        ON_MeshFace mf = faces[i];

        int iteCount = 0;
        if (mf.IsQuad()) iteCount = 4;
        else iteCount = 3;

        IMesh->BeginPolygon(-1, -1, false);
        for (int j = 0; j < iteCount; j++)
        {
            IMesh->AddPolygon(mf.vi[j]);
        }
        IMesh->EndPolygon();
    }

    //Create Node & Set Mesh
    FbxNode* rootNode = scene->GetRootNode();
    FbxNode* currentNode = rootNode;
    int nodeCount = layerNames->Count();
    for (int i = 0; i < nodeCount; i++)
    {
        const wchar_t* name = layerNames->At(i)->Array();
        /*
        size_t ksize = layerNames->At(i)->Length()*6 + 1;
        char* _name = new char[ksize];
        size_t len = wcstombs(_name, name, ksize);
        */
        size_t nameSize = (wcslen(name) + 1) * 4;
        char* _name = new char[nameSize];
        WideCharToMultiByte(CP_UTF8, 0, name, -1, _name, (int)nameSize, NULL, NULL);
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
    
    wchar_t* text = L"オブジェクト名表";
    size_t size = (wcslen(text) + 1) * 4;
    char* _text = new char[size];
    WideCharToMultiByte(CP_UTF8, 0, text, -1, _text, (int)size, NULL, NULL);
    FbxNode* INode = FbxNode::Create(scene, _text);
    delete[] _text;
    INode->SetNodeAttribute(IMesh);
    currentNode->AddChild(INode);
    
    //scene->GetRootNode()->AddChild(layer);
}
