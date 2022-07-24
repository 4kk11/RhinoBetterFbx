using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
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
		}

		public MeshVertexList vertices { get; private set; }
		public MeshFaceList faces { get; private set; }

		public MeshVertexNormalList vNormals { get; private set; }
		public MeshFaceNormalList fNormals { get; private set; }
		
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			
			bool button = false;
			DA.GetData("button", ref button);
			if (button)
			{
				Program_test.test();
			}

			//Get mesh object from RhinoDocument.
			Guid id = default(Guid);
			DA.GetData("guid", ref id);
			var doc = Rhino.RhinoDoc.ActiveDoc;
			Mesh mesh = doc.Objects.Find(id)?.Geometry as Mesh;
			if (mesh == null) return;
			

			//Get mesh info.
			vertices = mesh.Vertices;
			faces = mesh.Faces;
			vNormals = mesh.Normals;
			fNormals = mesh.FaceNormals;

			//Set output.
			DA.SetDataList(0, vertices);
			DA.SetDataList(1, faces);
		}


		protected override System.Drawing.Bitmap Icon => null;

		public override Guid ComponentGuid => new Guid("4F20AB28-A245-4971-91F6-B52F7E15D506");
	}

	internal class Program_test
	{
		[DllImport("BetterFbxLib.dll")]
		static extern void SetNum(int n);
		[DllImport("BetterFbxLib.dll")]
		static extern int GetNum();

		static public void test()
		{
			SetNum(2);
			int retNum = GetNum();
			MessageBox.Show(retNum.ToString());
		}

		static public void CreateFbxNode(Mesh mesh)
		{
			MeshVertexList vertices = mesh.Vertices;
			MeshFaceList faces = mesh.Faces;
			MeshVertexNormalList vNormals = mesh.Normals;
			MeshFaceNormalList fNormals = mesh.FaceNormals;
			int vCount = vertices.Count;
			int fCount = faces.Count;


		}
	}
}