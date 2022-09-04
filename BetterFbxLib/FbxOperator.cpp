#include "stdafx.h"
#include "FbxOperator.h"
#include <stack>


void SetUpFbxMesh_Vertices(FbxMesh* pFbxMesh, const ON_Mesh* pRhinoMesh)
{
    //get mesh info from RhinoMesh
    ON_3dPointArray vertices = pRhinoMesh->DoublePrecisionVertices();
    ON_3fVectorArray vNormals = pRhinoMesh->m_N;
    int vCount = pRhinoMesh->VertexCount();
    
    //get vertices from FbxMesh
    pFbxMesh->InitControlPoints(vCount);
    FbxVector4* fbx_vertices = pFbxMesh->GetControlPoints();  

    //get normals from FbxMesh 
    FbxGeometryElementNormal* fbx_vNormals = pFbxMesh->CreateElementNormal();  
    fbx_vNormals->SetMappingMode(FbxGeometryElement::eByControlPoint);   //Rhinoはnormal数=頂点数になる
    fbx_vNormals->SetReferenceMode(FbxGeometryElement::eDirect);

    for (int i = 0; i < vCount; i++)
    {
        //convert to fbxsdk class and Set
        double vertX = vertices.At(0)->x;
        //set vertex
        FbxVector4 vert(vertices.At(i)->x, vertices.At(i)->y, vertices.At(i)->z);
        fbx_vertices[i] = vert;
        //set normal
        FbxVector4 nor(vNormals[i].x, vNormals[i].y, vNormals[i].z);
        fbx_vNormals->GetDirectArray().Add(nor);
    }
}


void CreateUVmap(FbxMesh* pFbxMesh, const ON_SimpleArray<ON_2fPoint>& texCoords, const char* uvName)
{
    int vCount = texCoords.Count();

    FbxGeometryElementUV* fbx_uvMap = pFbxMesh->CreateElementUV(uvName); 
    fbx_uvMap->SetMappingMode(FbxGeometryElement::eByControlPoint); //Rhinoはuv座標数=頂点数になる
    fbx_uvMap->SetReferenceMode(FbxGeometryElement::eDirect);

    for (int i = 0; i < vCount; i++)
    {
        //set texture coordinate to uvMap in fbx
        FbxVector2 uv(texCoords[i].x, texCoords[i].y);
        fbx_uvMap->GetDirectArray().Add(uv);
    }
}

void SetUpFbxMesh_UV(FbxMesh* pFbxMesh, const ON_Mesh* pRhinoMesh, const CRhinoObject* pRhinoObject)
{
    //get mesh info from RhinoMesh
    int vCount = pRhinoMesh->VertexCount();

    //get rhinoDoc info and rhino texture table
    //デフォルトの状態(マッピングチャンネルに名前がない等)だと、pObject->Attributes().m_rendering_attributes.m_mappings.Count() = 0になる。
    // つまりpRefがNULLになってしまう。(TextureCoordinatesがマッピングに反映されていない?)
    //なので、0のとき(マッピングチャンネルが1つだけでかつデフォルト状態)では、ON_Mesh.m_TからTextureCoordinatesを取る。
    CRhinoDoc* pRhinoDoc = pRhinoObject->Document();
    const ON_MappingRef* pRef = GetValidMappingRef(pRhinoObject, true);
    
    if (pRef != NULL) //Get TextureCoordinates via mapping channels
    {
        CRhinoTextureMappingTable& table = pRhinoDoc->m_texture_mapping_table;
        //repeat for number of mappings
        for (int k = 0; k < pRef->m_mapping_channels.Count(); k++)
        {
            //get texture coordinate from mapping channel
            const ON_MappingChannel& chan = pRef->m_mapping_channels[k];
            ON_UUID id = chan.m_mapping_id;
            ON_TextureMapping mapping;
            if (!table.GetTextureMapping(id, mapping)) continue;
            ON_SimpleArray<ON_2fPoint> textureCoordinates;
            textureCoordinates.SetCount(vCount);
            mapping.GetTextureCoordinates(*pRhinoMesh, textureCoordinates);

            char num_char[3 + sizeof(char)];
            std::sprintf(num_char, "%d", k);
            //create uvMap in fbx  
            CreateUVmap(pFbxMesh, textureCoordinates, num_char); //TODO: fix uvName
        }
    }
    else //Get TextureCoordinates via ON_Mesh
    {
        ON_2fPointArray textureCoordinates = pRhinoMesh->m_T;
        CreateUVmap(pFbxMesh, textureCoordinates, "uvmap_0");
    }
}

void SetUpFbxMesh_Faces(FbxMesh* pFbxMesh, const ON_Mesh* pRhinoMesh)
{
    //get mesh info from RhinoMesh
    ON_SimpleArray<ON_MeshFace> faces = pRhinoMesh->m_F;
    int fCount = pRhinoMesh->FaceCount();

    //create material element
    FbxGeometryElementMaterial* IMaterialElement = pFbxMesh->CreateElementMaterial();
    IMaterialElement->SetMappingMode(FbxGeometryElement::eByPolygon);
    IMaterialElement->SetReferenceMode(FbxGeometryElement::eIndexToDirect);

    for (int i = 0; i < fCount; i++)
    {
        //get rhinomesh face
        ON_MeshFace mf = faces[i];

        int iteCount = 0;
        if (mf.IsQuad()) iteCount = 4;
        else iteCount = 3;

        //create fbxmesh face
        pFbxMesh->BeginPolygon(0, -1, false);
        for (int j = 0; j < iteCount; j++)
        {
            pFbxMesh->AddPolygon(mf.vi[j]);
        }
        pFbxMesh->EndPolygon();
    }
}



FbxSurfacePhong* CreateMaterial(FbxScene* scene, const ON_Material& onMaterial)
{
    char* matName = wStringToChar(onMaterial.Name().Array());
    //TODO: adjust parameters
    FbxDouble3 Black(0.0, 0.0, 0.0);
    ON_Color onEmission = onMaterial.Emission();
    FbxDouble3 IEmissiveColor(onEmission.FractionRed(), onEmission.FractionGreen(), onEmission.FractionBlue());
    ON_Color onAmbient = onMaterial.Ambient();
    FbxDouble3 IAmbientColor(onAmbient.FractionRed(), onAmbient.FractionGreen(), onAmbient.FractionBlue());
    ON_Color onDiffuse = onMaterial.Diffuse();
    FbxDouble3 IDiffuseColor(onDiffuse.FractionRed(), onDiffuse.FractionGreen(), onDiffuse.FractionBlue());
    //FbxDouble ITransparencyFactor(onMaterial.Transparency());
    FbxSurfacePhong* IMaterial = FbxSurfacePhong::Create(scene, matName);
    IMaterial->Emissive.Set(IEmissiveColor);
    IMaterial->Ambient.Set(IAmbientColor);
    IMaterial->Diffuse.Set(IDiffuseColor);
    //IMaterial->TransparencyFactor.Set(ITransparencyFactor);
    IMaterial->DiffuseFactor.Set(1.0);
    IMaterial->AmbientFactor.Set(1.0);

    IMaterial->Specular.Set(Black);
    IMaterial->SpecularFactor.Set(0.3);
    IMaterial->ShadingModel.Set("Phong");

    if (matName)
    {
        delete[] matName;
        matName = nullptr;
    }

    CreateTexture(scene, IMaterial, onMaterial);

    return IMaterial;
}

void CreateTexture(FbxScene* scene, FbxSurfacePhong* fbxMaterial , const ON_Material& onMaterial)
{
    ON_ObjectArray<ON_Texture> textures = onMaterial.m_textures;
    for (int i = 0; i < textures.Count(); ++i)
    {
        ON_Texture tex = textures[i];
        const wchar_t* texpath = tex.m_image_file_reference.FullPath().Array();
        FbxFileTexture* ITexture = nullptr;

        if (tex.m_type == ON_Texture::TYPE::diffuse_texture)
        {
            ITexture = FbxFileTexture::Create(scene, "Diffuse Texture");
        }
        else if (tex.m_type == ON_Texture::TYPE::bump_texture)
        {
            ITexture = FbxFileTexture::Create(scene, "Bump Texture");
        }
        else if (tex.m_type == ON_Texture::TYPE::opacity_texture)
        {
            ITexture = FbxFileTexture::Create(scene, "Opacity Texture");
        }
        else continue;

        char* texName = wStringToChar(texpath);
        ITexture->SetFileName(texName);
        ITexture->SetTextureUse(FbxTexture::eStandard);
        ITexture->SetMappingType(FbxTexture::eUV);
        ITexture->SetMaterialUse(FbxFileTexture::eModelMaterial);
        ITexture->SetSwapUV(false);
        ITexture->SetDefaultAlpha(1.0);
        ITexture->SetTranslation(0.0, 0.0);
        ITexture->SetScale(1.0, 1.0);
        ITexture->SetRotation(0.0, 0.0);

        if (fbxMaterial)
        {
            if (tex.m_type == ON_Texture::TYPE::diffuse_texture)
            {
                fbxMaterial->Diffuse.ConnectSrcObject(ITexture);
            }
            else if (tex.m_type == ON_Texture::TYPE::bump_texture)
            {
                fbxMaterial->Bump.ConnectSrcObject(ITexture);
            }
            else if (tex.m_type == ON_Texture::TYPE::opacity_texture)
            {
                fbxMaterial->Ambient.ConnectSrcObject(ITexture);
            }
            else continue;
        }

        if (texName)
        {
            delete[] texName;
            texName = nullptr;
        }
    }
}

FbxNode* SetUpFbxNode_RhinoLayers(FbxScene* scene, const CRhinoObject* pRhinoObject)
{
    //get layerTable from RhinoDoc
    CRhinoDoc* doc = pRhinoObject->Document();
    const CRhinoLayerTable& layerTable = doc->m_layer_table;

    //get object layer Index
    int layerIndex = pRhinoObject->Attributes().m_layer_index;

    //get parent layer names
    std::stack<const wchar_t*> stack;
    for (int i = 0; layerIndex != -1; ++i)
    {
        const CRhinoLayer& layer = layerTable[layerIndex];

        const wchar_t* wlayerName = layer.Name().Array();
        stack.push(wlayerName);

        ON_UUID parentLayerId = layer.ParentId();
        int parentLayerIndex = layerTable.FindLayerFromId(parentLayerId, true, false, -1);

        layerIndex = parentLayerIndex;
    }

    //get terminal node (make it if it doesnt exist)
    FbxNode* rootNode = scene->GetRootNode();
    FbxNode* terminalNode = rootNode;
    for (int j = 0; !stack.empty(); ++j)
    {
        const wchar_t* wlayerName = stack.top();
        char* layerName = wStringToChar(wlayerName);
        FbxNode* nextNode = terminalNode->FindChild(layerName);
        if (nextNode == nullptr)
        {
            nextNode = FbxNode::Create(scene, layerName);
            terminalNode->AddChild(nextNode);
        }
        
        terminalNode = nextNode;
        if (layerName)
        {
            delete[] layerName;
            layerName = nullptr;
        }
        stack.pop();
    }

    return terminalNode;
    
}

FbxNode* SetupFbxNode_MeshNode(FbxScene* scene, const CRhinoObject* pRhinoObject, FbxMesh* fbxMesh)
{
    //get rhinoObject name
    const wchar_t* wObjectName = pRhinoObject->Name().Array();
    if (!wObjectName) wObjectName = L"object";
    char* objectName = wStringToChar(wObjectName);

    //create meshNode
    FbxNode* meshNode = FbxNode::Create(scene, objectName);

    if (objectName)
    {
        delete[] objectName;
        objectName = nullptr;
    }

    //set fbxMesh and fbxMat to meshNode
    meshNode->SetNodeAttribute(fbxMesh);

    return meshNode;
}

void AddCustomProperty_FromRhino(FbxObject* pFbxObject, const CRhinoObject* pRhinoObject)
{
    CRhinoObjectAttributes att = pRhinoObject->Attributes();
    int usCount = att.UserStringCount();
    if (usCount == 0)  return;

    ON_ClassArray<ON_wString> keys;
    att.GetUserStringKeys(keys);

    for (int i = 0; i < usCount; ++i)
    {
        //get value
        ON_wString onwstr;
        wchar_t* key_w = keys.At(i)->Array();
        att.GetUserString(key_w, onwstr);
        wchar_t* value_w = onwstr.Array();
        //change wchar to char
        const char* key = wStringToChar(key_w);
        const char* value = wStringToChar(value_w);

        //create userdef property
        FbxPropertyT<FbxString> fbxProperty = FbxProperty::Create(pFbxObject, FbxStringDT, key);
        fbxProperty.ModifyFlag(FbxPropertyFlags::eUserDefined, true);
        fbxProperty.ModifyFlag(FbxPropertyFlags::eAnimatable, true);
        fbxProperty.Set(value);

        if (key)
        {
            delete[] key;
            key = nullptr;
        }
        if (value)
        {
            delete[] value;
            value = nullptr;
        }
    }
    



}


char* wStringToChar(const wchar_t* wchar)
{
    if (!wchar) return nullptr;
    size_t nameSize = (wcslen(wchar) + 1) * 4;
    char* _char = new char[nameSize];
    //ワイド文字列(unicode 2バイト)からマルチバイト文字列(utf-8)(日本語を加味して4バイト取っておく)に変換
    WideCharToMultiByte(CP_UTF8, 0, wchar, -1, _char, (int)nameSize, NULL, NULL);
    return _char;
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