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


void CreateNode(const CRhinoObject* pRhinoObject, const ON_SimpleArray<ON_wString>* layerNames, const wchar_t* objectName)
{
    if (pRhinoObject == NULL) return;

    CRhinoDoc* pRhinoDoc = pRhinoObject->Document();
    const ON_MappingRef* pRef = GetValidMappingRef(pRhinoObject, true);
    CRhinoTextureMappingTable& table = pRhinoDoc->m_texture_mapping_table;

    const ON_Mesh* pMesh = dynamic_cast<const ON_Mesh*>(pRhinoObject->Geometry());
    //rhinoObject->Attributes().m_rendering_attributes.
    if (!pMesh) return;

    ON_3dPointArray vertices = pMesh->DoublePrecisionVertices();
    ON_SimpleArray<ON_MeshFace> faces = pMesh->m_F;
    ON_3fVectorArray vNormals = pMesh->m_N;
    ON_3fVectorArray fNornals = pMesh->m_FN;
    ON_2fPointArray textureCoordinates = pMesh->m_T;
    int vCount = pMesh->VertexCount();
    int fCount = pMesh->FaceCount();
    
    //Create FbxMesh
    FbxMesh* IMesh = FbxMesh::Create(scene, "test_mesh");

    //Each Vertex 　Rhinoでは頂点の共有はできるが原則1頂点1ノーマル1UV座標になっている。
    IMesh->InitControlPoints(vCount);
    FbxVector4* fbx_vertices = IMesh->GetControlPoints();  //vertices
    FbxGeometryElementNormal* fbx_vNormals = IMesh->CreateElementNormal();  //normals Rhinoはnormal数=頂点数になる
    fbx_vNormals->SetMappingMode(FbxGeometryElement::eByControlPoint);
    fbx_vNormals->SetReferenceMode(FbxGeometryElement::eDirect);
    //FbxGeometryElementUV* fbx_uvMap = IMesh->CreateElementUV("UV1"); //uvMap Rhinoはuv座標数=頂点数になる
    //fbx_uvMap->SetMappingMode(FbxGeometryElement::eByControlPoint);
    //fbx_uvMap->SetReferenceMode(FbxGeometryElement::eDirect);
    for (int i=0; i < vCount; i++)
    {
        //convert to fbxsdk class and Set
        double vertX = vertices.At(0)->x;
        //set vertex
        FbxVector4 vert(vertices.At(i)->x, vertices.At(i)->y, vertices.At(i)->z);
        fbx_vertices[i] = vert;
        //set normal
        FbxVector4 nor(vNormals[i].x, vNormals[i].y, vNormals[i].z);
        fbx_vNormals->GetDirectArray().Add(nor);
        //set uvMap
        //FbxVector2 uv(textureCoordinates[i].x, textureCoordinates[i].y);
        //fbx_uvMap->GetDirectArray().Add(uv);
    }

    for (int k = 0; k < pRef->m_mapping_channels.Count(); k++)
    {
        const ON_MappingChannel& chan = pRef->m_mapping_channels[k];
        ON_UUID id = chan.m_mapping_id;
        ON_TextureMapping mapping;
        if (!table.GetTextureMapping(id, mapping)) continue;
        ON_SimpleArray<ON_2fPoint> _textureCoordinates;
        _textureCoordinates.SetCount(vCount);
        mapping.GetTextureCoordinates(*pMesh, _textureCoordinates);

        char num_char[3 + sizeof(char)];
        std::sprintf(num_char, "%d", k);
        FbxGeometryElementUV* fbx_uvMap = IMesh->CreateElementUV(num_char); //uvMap Rhinoはuv座標数=頂点数になる
        fbx_uvMap->SetMappingMode(FbxGeometryElement::eByControlPoint);
        fbx_uvMap->SetReferenceMode(FbxGeometryElement::eDirect);

        for (int i = 0; i < vCount; i++)
        {
            FbxVector2 uv(_textureCoordinates[i].x, _textureCoordinates[i].y);
            fbx_uvMap->GetDirectArray().Add(uv);
        }
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
        size_t nameSize = (wcslen(name) + 1) * 4;
        char* _name = new char[nameSize];
        //ワイド文字列(unicode 2バイト)からマルチバイト文字列(utf-8)(日本語を加味して4バイト取っておく)に変換
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

    //// オブジェクト名を拾う
    const wchar_t* text = objectName;
    size_t size = (wcslen(text) + 1) * 4;
    char* _text = new char[size];
    WideCharToMultiByte(CP_UTF8, 0, text, -1, _text, (int)size, NULL, NULL);
 
    FbxNode* INode = FbxNode::Create(scene, _text);
    delete[] _text;
    INode->SetNodeAttribute(IMesh);
    currentNode->AddChild(INode);
    
}

static const ON_MappingRef* GetValidMappingRef(const CRhinoObject* pObject, bool withChannels)
{
    // Helper function - implementation only.
    if (NULL == pObject)
        return NULL;

    const ON_ObjectRenderingAttributes& attr = pObject->Attributes().m_rendering_attributes;

    // There are no mappings at all - just get out.
    if (0 == attr.m_mappings.Count())
        return NULL;

    // Try with the current renderer first.
    const ON_MappingRef* pRef = attr.MappingRef(RhinoApp().GetDefaultRenderApp());

    // 5DC0192D-73DC-44F5-9141-8E72542E792D
    ON_UUID uuidRhinoRender = RhinoApp().RhinoRenderPlugInUUID();
    if (NULL == pRef)
    {
        //Prefer the Rhino renderer mappings next
        pRef = attr.MappingRef(uuidRhinoRender);
    }

    // Then just run through the list until we find one with some channels.
    int i = 0;
    while (NULL == pRef && withChannels && i < attr.m_mappings.Count())
    {
        pRef = attr.m_mappings.At(i++);
    }

    return pRef;
}
