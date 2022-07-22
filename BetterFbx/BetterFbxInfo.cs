using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace BetterFbx
{
	public class BetterFbxInfo : GH_AssemblyInfo
	{
		public override string Name => "BetterFbx";

		//Return a 24x24 pixel bitmap to represent this GHA library.
		public override Bitmap Icon => null;

		//Return a short string describing the purpose of this GHA library.
		public override string Description => "";

		public override Guid Id => new Guid("BF9C6F08-AF3B-4EDF-A5BA-8DC5CB5EC7B8");

		//Return a string identifying you or your company.
		public override string AuthorName => "";

		//Return a string representing your preferred contact details.
		public override string AuthorContact => "";
	}
}