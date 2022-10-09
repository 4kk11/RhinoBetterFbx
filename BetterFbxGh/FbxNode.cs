using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterFbxGh
{
	public class FbxNode : IDisposable
	{
		private IntPtr m_ptr;
		private string name;
		private List<FbxNode> childNodes = new List<FbxNode>();
		private FbxNodeAttribute attribute;
		
		public IntPtr NonConstPointer()
		{
			return m_ptr;
		}

		public FbxNode(string nodeName, List<FbxNode> _childNodes , FbxNodeAttribute attr )
		{
			//set name
			if (nodeName == null) this.name = "node0";
			else this.name = nodeName;

			//create FbxNode with UnsafeNativeMethod.
			m_ptr = UnsafeNativeMethods.FbxNode_New(this.name);

			//set childNodes
			if (_childNodes != null)
			{
				AddChildNodes(_childNodes);
			}

			//set Attribute
			if (attr != null) SetAttribtue(attr);
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
			if (node == null) return;
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

		public void SetAttribtue(FbxNodeAttribute attr)
		{
			UnsafeNativeMethods.FbxNode_SetAttribute(m_ptr, attr.NonConstPointer());
			attribute = attr;
		}
		
		public FbxNodeAttribute GetAttribute()
		{
			return this.attribute;
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
			return new FbxNode(this.name, this.childNodes, this.attribute);
		}

	}
}
