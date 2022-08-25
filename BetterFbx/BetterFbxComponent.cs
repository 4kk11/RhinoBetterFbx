using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Rhino.Runtime;
using Rhino.DocObjects;

using System.Windows.Forms;

namespace BetterFbx
{
	public class BetterFbxComponent : GH_Component
	{


		public BetterFbxComponent() : base("BetterFbx", "BetterFbx", "Description", "MyTools", "BetterFbx")
		{
			
		}


		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddScriptVariableParameter("guid", "guid", "", GH_ParamAccess.item);
			pManager.AddBooleanParameter("button", "button", "", GH_ParamAccess.item);
			pManager[0].Optional = true;
			pManager[1].Optional = true;
		}


		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddGenericParameter("vertices", "vertices", "", GH_ParamAccess.list);
			pManager.AddGenericParameter("faces", "faces", "", GH_ParamAccess.list);
			pManager.AddPointParameter("out", "out", "", GH_ParamAccess.list);
		}

		public MeshVertexList vertices { get; private set; }
		public MeshFaceList faces { get; private set; }

		public MeshVertexNormalList vNormals { get; private set; }
		public MeshFaceNormalList fNormals { get; private set; }
		
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			
			
			Guid id = default(Guid);
			DA.GetData("guid", ref id);

			var doc = Rhino.RhinoDoc.ActiveDoc;
			RhinoObject rhinoObject = doc.Objects.Find(id);
			if (rhinoObject == null) return;

			//Get mesh
			Mesh mesh = rhinoObject.Geometry as Mesh;
			if (mesh == null) return;
			vertices = mesh.Vertices;
			faces = mesh.Faces;
			vNormals = mesh.Normals;
			fNormals = mesh.FaceNormals;
			DA.SetDataList(0, vertices);
			DA.SetDataList(1, faces);

			//Get Layer
			string[] layerNames = ExtractObject.GetParentLayerNames(doc, rhinoObject);
			string objectName = ExtractObject.GetObjectName(doc, rhinoObject);

			bool button = false;
			DA.GetData("button", ref button);
			if (button)
			{

				//ExportMeshFBX(mesh, layerNames, objectName);
				ExportMeshFBX(rhinoObject, layerNames, objectName);
			}
		}


		protected override System.Drawing.Bitmap Icon => null;

		public override Guid ComponentGuid => new Guid("4F20AB28-A245-4971-91F6-B52F7E15D506");


		static public void ExportMeshFBX(RhinoObject ro, string[] layerNames, string objectName)
		{
			IntPtr pro = Interop.RhinoObjectConstPointer(ro);

			var string_array = new Rhino.Runtime.InteropWrappers.ClassArrayString();
			foreach (string str in layerNames)
			{
				string_array.Add(str);
			}
			IntPtr pLayerNames = string_array.ConstPointer();
			UnsafeNativeMethods.CreateManager();
			UnsafeNativeMethods.CreateNode(pro, pLayerNames, objectName);
			UnsafeNativeMethods.ExportFBX(false);
			UnsafeNativeMethods.DeleteManager();

			string_array.Dispose();
		}

	}


}