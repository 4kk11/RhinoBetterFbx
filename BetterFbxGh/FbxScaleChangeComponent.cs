using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace BetterFbxGh
{
    public class FbxScaleChangeComponent : GH_Component
    {

        public FbxScaleChangeComponent(): base("FbxScaleChanger", "FbxScaleChanger", "Description", "MyTools", "BetterFbx")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("FbxPath_Input", "FbxPath_Input", "", GH_ParamAccess.item);
            pManager.AddTextParameter("FbxPath_Output", "FbxPath_Output", "", GH_ParamAccess.item);
            //pManager.AddTextParameter("Unit_Source", "Unit_source", "", GH_ParamAccess.item, "mm");
            pManager.AddTextParameter("Unit_Target", "Unit_Target", "", GH_ParamAccess.item, "mm");
            pManager.AddBooleanParameter("isAscii", "isAscii", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("button", "button", "", GH_ParamAccess.item);
            pManager[2].Optional = true;
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

                string path_intput = null;
                string path_output = null;
                string unit_target = null;
                bool isAscii = false;
                DA.GetData("isAscii", ref isAscii);

                DA.GetData("FbxPath_Input", ref path_intput);
                DA.GetData("FbxPath_Output", ref path_output);
                DA.GetData("Unit_Target", ref unit_target);

                UnsafeNativeMethods.CreateManager();
                UnsafeNativeMethods.ImportFBX(path_intput);

                int unitSelect = 0;
                switch (unit_target)
                {
                    case "mm":
                        unitSelect = 1;
                        break;
                    case "cm":
                        unitSelect = 2;
                        break;
                    case "m":
                        unitSelect = 3;
                        break;
                }

                UnsafeNativeMethods.ExportFBX(isAscii, 0, unitSelect, path_output);

                UnsafeNativeMethods.DeleteManager();
            }
        }

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("3FB401B1-B460-4C57-8359-ACE5E4F48158");

    }
}