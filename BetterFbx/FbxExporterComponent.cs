using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace BetterFbx
{
    public class FbxExporterComponent : GH_Component
    {
        public FbxExporterComponent() : base("FbxExporter", "FbxExporter", "Description", "MyTools", "BetterFbx")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new FbxNodeParameter(), "Node", "Node", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("button", "button", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("AxisSelect", "AxisSelect", "", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Path", "Path", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("isAscii", "isAscii", "", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            FbxNode fbxNode = null;
            DA.GetData("Node", ref fbxNode);
            if (fbxNode == null) return;

            bool button = false;
            int axisSelect = 0;
            string path = null;
            bool isAscii = false;

            DA.GetData("button", ref button);
            DA.GetData("AxisSelect", ref axisSelect);
            DA.GetData("Path", ref path);
            DA.GetData("isAscii", ref isAscii);

            if (path == null) path = "D:/Users/akiak/Desktop/export/test.fbx";

            if (button)
            {
                //export fbx
                ExportFbx(fbxNode, isAscii, axisSelect, path);
            }
        }

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("F3B04087-F7CE-4FCD-9AE5-092B996ACF56");


        private void ExportFbx(FbxNode fbxNode, bool isAscii, int axisSelect, string path)
        {
            UnsafeNativeMethods.CreateManager();

            //set node to scene
            UnsafeNativeMethods.FbxExporter_Set(fbxNode.NonConstPointer());

            //export fbx
            UnsafeNativeMethods.ExportFBX(isAscii, axisSelect, 1, path);
            

            UnsafeNativeMethods.DeleteManager();
        }
    }
}