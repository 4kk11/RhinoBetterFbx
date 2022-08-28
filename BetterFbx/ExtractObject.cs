using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;
using Rhino.DocObjects;
using Rhino;

namespace BetterFbx
{
	class ExtractObject
	{
		public static string[] GetParentLayerNames(RhinoDoc doc, RhinoObject ro)
		{
			int layerIndex = ro.Attributes.LayerIndex;
			Guid layerId = doc.Layers.FindIndex(layerIndex).Id;
			List<string> layerNames = new List<string>();
			GetParentLayerNames_Recursion(layerId, layerNames);
			layerNames.Reverse();
			return layerNames.ToArray();
		}
		private static void GetParentLayerNames_Recursion(Guid id, List<string> layerNames)
		{
			Layer nowLayer = Rhino.RhinoDoc.ActiveDoc.Layers.FindId(id);
			layerNames.Add(nowLayer.Name);
			Guid parentId = nowLayer.ParentLayerId;
			if (parentId != Guid.Empty) GetParentLayerNames_Recursion(parentId, layerNames);
		}

		public static string GetObjectName(RhinoDoc doc, RhinoObject ro)
		{
			if (ro.Name == null)
			{
				return "object";
			}
			return ro.Name;
		}
	}
}
