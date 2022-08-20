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

namespace BetterFbx
{
	class UnsafeNativeMethods
	{
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void CreateManager();
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void DeleteManager();
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ExportFBX();
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetvCount(IntPtr pts);
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void CreateNode(IntPtr pro, IntPtr pLayerNames, string objectName);
		//public static extern void CreateNode(IntPtr pMesh, IntPtr pLayerNames, string objectName);

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

	}
}
