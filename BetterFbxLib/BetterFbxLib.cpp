#include <iostream>
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


void CreateNode(const ON_Mesh* pMesh)
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
    FbxNode* INode = FbxNode::Create(scene, "test_node");
    INode->SetNodeAttribute(IMesh);
    
    scene->GetRootNode()->AddChild(INode);
}
