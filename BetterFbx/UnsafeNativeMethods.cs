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
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void ExportFBX([MarshalAs(UnmanagedType.Bool)]bool isAscii, int axisSelect, string path);

		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void CreateNode(IntPtr pro);
		//public static extern void CreateNode(IntPtr pMesh, IntPtr pLayerNames, string objectName);

		//FbxNodeComponent
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern IntPtr FbxNode_New(string name);
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void FbxNode_Delete(IntPtr pFbxNode);
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void FbxNode_AddChild(IntPtr pFbxNode_parent, IntPtr pFbxNode_child);
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void FbxNode_SetAttribute(IntPtr pFbxNode, IntPtr pFbxNodeAttr);

		//FbxNodeAttributeComponent
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr FbxMesh_New(IntPtr pRhinoMesh);
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void FbxNodeAttribute_Delete(IntPtr pFbxNodeAttribute);

		//FbxExporterComponent
		[DllImport("BetterFbxLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void FbxExporter_Set(IntPtr pFbxNode);
	}
}
