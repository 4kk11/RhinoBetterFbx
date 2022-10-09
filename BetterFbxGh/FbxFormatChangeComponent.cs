using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace BetterFbxGh
{
    public class FbxFormatChangeComponent : GH_Component
    {

        public FbxFormatChangeComponent(): base("FbxFormatChanger", "FbxFormatChanger", "Description", "MyTools", "BetterFbx")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("FbxPath", "FbxPath", "", GH_ParamAccess.item); 
            pManager.AddBooleanParameter("isAscii", "isAscii", "", GH_ParamAccess.item);
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
                string path = null;

                DA.GetData("FbxPath", ref path);

                bool isAscii = false;
                DA.GetData("isAscii", ref isAscii);

                UnsafeNativeMethods.CreateManager();
                UnsafeNativeMethods.ImportFBX(path);


                UnsafeNativeMethods.ExportFBX(isAscii, 0, 0, path);

                UnsafeNativeMethods.DeleteManager();
            }
        }

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("88FBEB4A-2A27-454D-B5E9-8751B0A52EB5"); 

    }
}