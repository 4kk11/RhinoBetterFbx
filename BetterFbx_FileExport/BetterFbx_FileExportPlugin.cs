using System;
using System.Collections.Generic;
using Rhino;
using Rhino.FileIO;
using Rhino.Runtime;
using Rhino.DocObjects;


namespace BetterFbx_FileExport
{

	public class BetterFbx_FileExportPlugin : Rhino.PlugIns.FileExportPlugIn
	{
		public BetterFbx_FileExportPlugin()
		{
			Instance = this;
		}

		public static BetterFbx_FileExportPlugin Instance { get; private set; }

		protected override Rhino.PlugIns.FileTypeList AddFileTypes(Rhino.FileIO.FileWriteOptions options)
		{
			var result = new Rhino.PlugIns.FileTypeList();
			result.AddFileType("BetterFbx (*.fbx)", "fbx");
			return result;
		}

		protected override Rhino.PlugIns.WriteFileResult WriteFile(string filename, int index, RhinoDoc doc, Rhino.FileIO.FileWriteOptions options)
		{
			var rhinoObjects = GetObjectsToExport(doc);
			BetterFbx_FileExportCommand.ExportMeshFBX(rhinoObjects, 0, filename);
			return Rhino.PlugIns.WriteFileResult.Success;
		}

		private IEnumerable<Rhino.DocObjects.RhinoObject> GetObjectsToExport(RhinoDoc doc)
		{
			return doc.Objects.GetSelectedObjects(false, false);
		}



	}
}