using System;
using System.Collections.Generic;
using Rhino;
using Rhino.PlugIns;
using Rhino.FileIO;
using Rhino.UI;
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
			var rhinoObjects = BetterFbx_FileExportCommand.GetObjectsToExport(doc);

			
			ExportOptionDialog exportOptionDialog = new ExportOptionDialog();
			exportOptionDialog.RestorePosition();
			exportOptionDialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);
			int axisSelect = 1;
			if (MapRhinoZToFbxY)
			{
				axisSelect = 0;
			}
			BetterFbx_FileExportCommand.ExportMeshFBX(rhinoObjects, axisSelect, filename);
			return Rhino.PlugIns.WriteFileResult.Success;
		}

		#region Settings

		private const string mapRhinoZToFbx_Key = "MapZupToYup";
		public const bool MapRhinoZToFbxY_Default = false;
		public static bool MapRhinoZToFbxY
		{
			get => Instance.Settings.GetBool(mapRhinoZToFbx_Key, MapRhinoZToFbxY_Default);
			set => Instance.Settings.SetBool(mapRhinoZToFbx_Key, value);
		}

		#endregion
	}
}