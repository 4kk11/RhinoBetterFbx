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

			bool button = false;
			DA.GetData("button", ref button);
			if (button)
			{

				//Point3d[] pts = Program_test.UnsafeNativeMethods.test(mesh);
				//DA.SetDataList(2, pts);
				//DA.SetData(2, pts);
				Program_test.UnsafeNativeMethods.test_2(mesh);
			}
		}


		protected override System.Drawing.Bitmap Icon => null;

		public override Guid ComponentGuid => new Guid("4F20AB28-A245-4971-91F6-B52F7E15D506");
	}

	internal class Program_test
	{
		public static class UnsafeNativeMethods
		{
			[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
			static extern void CreateManager();
			[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
			static extern void DeleteManager();
			[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
			static extern void ExportFBX();
			[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
			static extern int GetvCount(IntPtr pts);
			[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
			static extern void CreateNode(IntPtr pMesh);

			static public Point3d[] test(Mesh mesh)
			{
				IntPtr inPtr = Interop.NativeGeometryConstPointer(mesh);

				//SetMesh(inPtr);

				var points_array = new Rhino.Runtime.InteropWrappers.SimpleArrayPoint3d();
				var ptr_points_array = points_array.NonConstPointer();

				int Num = GetvCount(ptr_points_array);

				Point3d[] pts = points_array.ToArray();
				points_array.Dispose();
				//MessageBox.Show(retNum.ToString());
				return pts;
			}

			static public void test_2(Mesh mesh)
			{
				IntPtr inPtr = Interop.NativeGeometryConstPointer(mesh);
				CreateManager();
				CreateNode(inPtr);
				ExportFBX();
				DeleteManager();
			}
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