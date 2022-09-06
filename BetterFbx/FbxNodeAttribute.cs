using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace BetterFbx
{
	abstract public class FbxNodeAttribute : IDisposable
	{
		protected IntPtr m_ptr;


		public IntPtr NonConstPointer()
		{
			return m_ptr;
		}

		public FbxNodeAttribute()
		{ 
			//create FbxNodeAttribute

		}

		public void Dispose()
		{
			InternalDispose();
			GC.SuppressFinalize(this);
		}

		public void InternalDispose()
		{
			if (m_ptr != IntPtr.Zero)
			{
				//delete FbxNodeAttribute with UnsafeNativeMethod.
				UnsafeNativeMethods.FbxNodeAttribute_Delete(m_ptr);
				m_ptr = IntPtr.Zero;
			}
		}

		abstract public FbxNodeAttribute Duplicate();

	}

	public class FbxMesh : FbxNodeAttribute
	{
		private Mesh mesh;
		public FbxMesh(Mesh _mesh)
		{
			mesh = _mesh;
			IntPtr pRhinoMesh = Rhino.Runtime.Interop.NativeGeometryConstPointer(_mesh);
			m_ptr = UnsafeNativeMethods.FbxMesh_New(pRhinoMesh);
		}

		public override FbxNodeAttribute Duplicate()
		{
			return new FbxMesh(mesh);
		}
	}
}
