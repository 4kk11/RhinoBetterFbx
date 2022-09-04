using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterFbx
{
	public class FbxNode : IDisposable
	{
		private IntPtr m_ptr;
		private string name;
		private List<FbxNode> childNodes = new List<FbxNode>();
		
		public IntPtr NonConstPointer()
		{
			return m_ptr;
		}

		public FbxNode(string nodeName, List<FbxNode> _childNodes = null)
		{
			//create FbxNode with UnsafeNativeMethod.
			m_ptr = UnsafeNativeMethods.FbxNode_New(this.name);

			//set name
			if (nodeName == null) this.name = "node0";
			else this.name = nodeName;

			//set childNodes
			if (_childNodes != null)
			{
				AddChildNodes(_childNodes);
			}
		}

		public string GetName()
		{
			return name;
		}

		public List<FbxNode> GetChildNodes()
		{
			return childNodes;
		}

		public void AddChildNode(FbxNode node)
		{
			UnsafeNativeMethods.FbxNode_AddChild(m_ptr, node.NonConstPointer());
			childNodes.Add(node);
		}

		public void AddChildNodes(List<FbxNode> nodes)
		{
			if (nodes.Count == 0) return;
			foreach (FbxNode node in nodes)
			{
				AddChildNode(node);
			}
		}

		public int GetChildCount()
		{
			return childNodes.Count;
		}

		~FbxNode()
		{
			InternalDispose();
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
				//delete FbxNode with UnsafeNativeMethod.
				UnsafeNativeMethods.FbxNode_Delete(m_ptr);
				m_ptr = IntPtr.Zero;
			}
		}

		public FbxNode Duplicate()
		{
			return new FbxNode(this.name, this.childNodes);
		}

	}
}
