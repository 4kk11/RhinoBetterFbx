using System;
using System.Collections.Generic;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace BetterFbx
{
	public class FbxNodeGoo : GH_Goo<FbxNode>
	{
		public FbxNodeGoo() { }

		public FbxNodeGoo(FbxNode fbxNode) : base(fbxNode)
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
			get { return "FbxNode"; }
		}

		public override string TypeName
		{
			get { return "FNode"; }
		}

		public override IGH_Goo Duplicate()
		{
			if (Value == null) return new FbxNodeGoo();
			return new FbxNodeGoo(Value.Duplicate());
		}

		public override string ToString()
		{
			if (Value == null) return "null";
			return String.Format("FbxNode[{0}]", Value.GetName());
		}

		public override bool CastFrom(object source)
		{
			if (ReferenceEquals(source, null))
				return false;

			FbxNode fbxNode = source as FbxNode;
			if (fbxNode != null)
			{
				Value = fbxNode.Duplicate();  //おそらく、FbxNodeのコンストラクタにchild等の情報を渡す仕組みでないと受け継がれない
				return true;
			}

			return false;
		}

		public override bool CastTo<TQ>(ref TQ target)
		{
			Type typeQ = typeof(TQ);
			if (typeQ.IsAssignableFrom(typeof(FbxNode)))
			{
				target = (TQ)((object)Value.Duplicate());
				return true;
			}
			return false;
		}

	}

	public class FbxNodeParameter : GH_PersistentParam<FbxNodeGoo>
	{
		public FbxNodeParameter() : base(new GH_InstanceDescription("FbxNode", "FbxNode", "", "MyTools", "BetterFbx"))
		{ }

		public static readonly Guid ParameterId = new Guid("29863442-C045-4D55-96B6-29A4772A9F0D");

		public override Guid ComponentGuid => ParameterId;

		protected override GH_GetterResult Prompt_Plural(ref List<FbxNodeGoo> values)
		{
			return GH_GetterResult.success;
		}

		protected override GH_GetterResult Prompt_Singular(ref FbxNodeGoo value)
		{
			return GH_GetterResult.success;
		}
	}
}
