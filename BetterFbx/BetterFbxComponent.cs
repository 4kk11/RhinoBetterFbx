using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
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
			pManager.AddBooleanParameter("button", "button", "", GH_ParamAccess.item);
		}


		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
		}


		protected override void SolveInstance(IGH_DataAccess DA)
		{
			bool button = false;
			DA.GetData("button", ref button);
			if (button)
			{
				Program_test.test();
			}
		}


		protected override System.Drawing.Bitmap Icon => null;

		public override Guid ComponentGuid => new Guid("4F20AB28-A245-4971-91F6-B52F7E15D506");
	}

	internal class Program_test
	{
		[DllImport("BetterFbxLibrary.dll")]
		static extern void SetNum(int n);
		[DllImport("BetterFbxLibrary.dll")]
		static extern int GetNum();

		static public void test()
		{
			SetNum(2);
			int retNum = GetNum();
			MessageBox.Show(retNum.ToString());
		}
	}
}