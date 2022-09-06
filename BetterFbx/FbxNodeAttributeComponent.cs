using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace BetterFbx
{
    public class FbxNodeAttributeComponent : GH_Component
    {

        public FbxNodeAttributeComponent() : base("FbxNodeAttribute", "FbxNodeAttribute", "Description", "MyTools", "BetterFbx")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FbxNodeAttributeParameter(), "Attribute", "Attribute", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            DA.GetData("mesh", ref mesh);

            if (mesh == null) return;

            //convert mesh to FbxAttribute
            FbxNodeAttribute attr = new FbxMesh(mesh);

            DA.SetData("Attribute", new FbxNodeAttributeGoo(attr));
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("BFB816AB-A89D-4723-BD06-7B39477D4BC7");

    }
}