using Rhino;
using Rhino.Geometry;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.DocObjects;
using Rhino.Runtime;
using System;
using System.Collections.Generic;

namespace BetterFbx_FileExport
{
	public class BetterFbx_FileExportCommand : Command
	{
		public BetterFbx_FileExportCommand()
		{

			Instance = this;
		}

		public static BetterFbx_FileExportCommand Instance { get; private set; }

		public override string EnglishName => "BetterFbx_FileExportCommand";

		protected override Result RunCommand(RhinoDoc doc, RunMode mode)
		{
			return Result.Success;
		}
		public static void ExportMeshFBX(IEnumerable<RhinoObject> rhinoObjects, bool isAscii, int axisSelect, string path)
		{
			UnsafeNativeMethods.CreateManager();

			foreach (RhinoObject ro in rhinoObjects)
			{
				if (ro.ObjectType != ObjectType.Mesh)
				{
					ro.CreateMeshes(Rhino.Geometry.MeshType.Preview, CreateMeshingParameter(), true);
				}
				IntPtr pro = Interop.RhinoObjectConstPointer(ro);
				UnsafeNativeMethods.CreateNode(pro);
			}
			UnsafeNativeMethods.ExportFBX(isAscii, axisSelect, 1, path);
			UnsafeNativeMethods.DeleteManager();
		}


		public static IEnumerable<Rhino.DocObjects.RhinoObject> GetObjectsToExport(RhinoDoc doc)
		{
			return doc.Objects.GetSelectedObjects(false, false);
		}

		private static MeshingParameters CreateMeshingParameter()
		{
			double meshDetailLevel = BetterFbx_FileExportPlugin.meshDetailLevel / 10.0;
			MeshingParameters param = new MeshingParameters(meshDetailLevel);
			return param;
		}
	}
}
