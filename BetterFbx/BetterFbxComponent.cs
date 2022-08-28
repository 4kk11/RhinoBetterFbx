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
using System.Linq;
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
			pManager.AddScriptVariableParameter("guid", "guid", "", GH_ParamAccess.list);
			pManager.AddBooleanParameter("button", "button", "", GH_ParamAccess.item);
			
			pManager.AddIntegerParameter("AxisSelect", "AxisSelect", "", GH_ParamAccess.item, 0);
			pManager.AddTextParameter("Path", "Path", "", GH_ParamAccess.item);
			pManager[0].Optional = true;
			pManager[1].Optional = true;
			pManager[2].Optional = true;
			pManager[3].Optional = true;
			
		}


		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{

		}
		
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			List<Guid> ids = new List<Guid>();
			DA.GetDataList("guid", ids);

			var doc = Rhino.RhinoDoc.ActiveDoc;

			IEnumerable<RhinoObject> rhinoObjects = ids.Select(k => doc.Objects.Find(k));

			if (!rhinoObjects.All(obj => obj != null))
			{
				this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not Found RhinoObject!");
				return;
			}
			

			bool button = false;
			int axisSelect = 0;
			string path = null;
			
			DA.GetData("button", ref button);
			DA.GetData("AxisSelect", ref axisSelect);
			DA.GetData("Path", ref path);

			if (path == null) path = "D:/Users/akiak/Desktop/export/test.fbx";

			if (button)
			{
				ExportMeshFBX(rhinoObjects, axisSelect, path);
			}
		}


		protected override System.Drawing.Bitmap Icon => null;

		public override Guid ComponentGuid => new Guid("4F20AB28-A245-4971-91F6-B52F7E15D506");


		static public void ExportMeshFBX(IEnumerable<RhinoObject> rhinoObjects, int axisSelect, string path)
		{
			UnsafeNativeMethods.CreateManager();

			foreach (RhinoObject ro in rhinoObjects)
			{
				IntPtr pro = Interop.RhinoObjectConstPointer(ro);
				UnsafeNativeMethods.CreateNode(pro);
			}

			UnsafeNativeMethods.ExportFBX(false, axisSelect, path);
			UnsafeNativeMethods.DeleteManager();
		}

	}


}