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
		public static extern void ExportFBX([MarshalAs(UnmanagedType.Bool)]bool isAscii, int axisSelect);

		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void CreateNode(IntPtr pro);
		//public static extern void CreateNode(IntPtr pMesh, IntPtr pLayerNames, string objectName);

	}
}
