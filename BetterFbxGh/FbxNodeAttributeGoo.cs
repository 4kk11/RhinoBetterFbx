using System;
using System.Collections.Generic;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace BetterFbxGh
{
	public class FbxNodeAttributeGoo : GH_Goo<FbxNodeAttribute>
	{
		public FbxNodeAttributeGoo() { }

		public FbxNodeAttributeGoo(FbxNodeAttribute fbxNodeAttribute) : base(fbxNodeAttribute)
		{ }

		public override bool IsValid
		{
			get
			{
				if (Value == null) return false;
				return true;
			}
		}

		public override string IsValidWhyNot 
		{
			get
			{
				if (Value == null) return "Missing instance";
				return string.Empty;
			}
		}

		public override string TypeDescription
		{
			get { return "FbxNodeAttribute"; }
		}

		public override string TypeName
		{
			get { return "FNodeAttr"; }
		}

		public override IGH_Goo Duplicate()
		{
			if (Value == null) return new FbxNodeAttributeGoo();
			return new FbxNodeAttributeGoo(Value.Duplicate());
		}

		public override string ToString()
		{
			if (Value == null) return "null";
			return String.Format("FbxNodeAttribute[{0}]", Value.GetType().ToString());
		}

		public override bool CastFrom(object source)
		{
			if (ReferenceEquals(source, null)) return false;
			if (source is FbxNodeAttribute attr)
			{
				Value = attr.Duplicate();
				return true;
			}
			return false;
		}

		public override bool CastTo<TQ>(ref TQ target)
		{
			Type typeQ = typeof(TQ);
			if (typeQ.IsAssignableFrom(typeof(FbxNodeAttribute)))
			{
				target = (TQ)((object)Value.Duplicate());
				return true;
			}
			return false;
		}
	}

	public class FbxNodeAttributeParameter : GH_PersistentParam<FbxNodeAttributeGoo>
	{ 
		public FbxNodeAttributeParameter() : base(new GH_InstanceDescription("FbxNodeAttributeParam", "FbxNodeAttributeParam", "", "MyTools", "BetterFbx"))
		{ }

		public static readonly Guid ParameterId = new Guid("552A6815-5A73-46CE-9B77-FBAF6CF61E3C");

		public override Guid ComponentGuid => ParameterId;

		protected override GH_GetterResult Prompt_Plural(ref List<FbxNodeAttributeGoo> values)
		{
			return GH_GetterResult.success;
		}

		protected override GH_GetterResult Prompt_Singular(ref FbxNodeAttributeGoo value)
		{
			return GH_GetterResult.success;
		}

	}
}
