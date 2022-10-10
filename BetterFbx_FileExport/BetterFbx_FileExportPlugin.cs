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
			Eto.Forms.DialogResult result = exportOptionDialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);
			if (result != Eto.Forms.DialogResult.Ok)
			{
				return WriteFileResult.Cancel;
			}

			int axisSelect = MapRhinoZToFbxY ? 0 : 1;
			bool isAscii = isAsciiFormat;

			BetterFbx_FileExportCommand.ExportMeshFBX(rhinoObjects, isAscii, axisSelect, filename);
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

		private const string isAsciiFormat_Key = "isAscii";
		public const bool isAsciiFormat_Default = false;
		public static bool isAsciiFormat
		{
			get => Instance.Settings.GetBool(isAsciiFormat_Key, isAsciiFormat_Default);
			set => Instance.Settings.SetBool(isAsciiFormat_Key, value);
		}

		private const string meshDetailLevel_Key = "MeshLevel";
		private const int meshDetailLevel_Default = 0;
		public static int meshDetailLevel
		{
			get => Instance.Settings.GetInteger(meshDetailLevel_Key, meshDetailLevel_Default);
			set => Instance.Settings.SetInteger(meshDetailLevel_Key, value);

		}

		#endregion
	}
}