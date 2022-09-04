using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace BetterFbx
{
    public class FbxNodeComponent : GH_Component
    {

        public FbxNodeComponent(): base("FbxNode", "FbxNode", "Description", "MyTools", "BetterFbx")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "Name", "", GH_ParamAccess.item);
            pManager.AddParameter(new FbxNodeParameter(), "ChildNode", "ChildNode", "", GH_ParamAccess.list);
            
            pManager[1].Optional = true;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FbxNodeParameter(), "Node", "Node", "", GH_ParamAccess.item);

            //test
            pManager.AddIntegerParameter("ChildCount", "ChildCount", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string nodeName = null;
            DA.GetData("Name", ref nodeName);

            List<FbxNode> childNodes = new List<FbxNode>();
            DA.GetDataList("ChildNode", childNodes);

            FbxNode fbxNode = new FbxNode(nodeName, childNodes);

            DA.SetData("Node", new FbxNodeGoo(fbxNode));

            //test
            if (fbxNode.GetChildCount() != 0)
            {
                FbxNode childNode = fbxNode.GetChildNodes()[0];
                DA.SetData("ChildCount", childNode.GetChildCount());
            }
        }

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("7308E169-7409-452C-A17E-2BABE71089EC");

    }
}