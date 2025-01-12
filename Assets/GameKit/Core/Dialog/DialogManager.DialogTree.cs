using System.Collections.Generic;
using GameKit.DataStructure;
using GameKit.DataNode;

namespace GameKit.Dialog
{
    internal sealed partial class DialogManager : GameKitModule, IDialogManager
    {
        public sealed class DialogTree : IDialogTree, IReference
        {
            private string m_Name;
            private IDataNode m_RootNode;
            private IDataNode m_CurrentNode;
            private IDataNode m_StartNode;
            private IDataNodeManager m_DataNodeManager;
            private Dictionary<string, bool> m_LocalConditions;

            public string Name
            {
                get
                {
                    return m_Name;
                }
            }

            public IDataNode RootNode
            {
                get
                {
                    return m_RootNode;
                }
            }

            public IDataNode CurrentNode
            {
                get
                {
                    return m_CurrentNode;
                }
                set
                {
                    m_CurrentNode = value;
                }
            }

            public IDataNode StartNode
            {
                get
                {
                    return m_StartNode;
                }
            }

            public IDataNodeManager DataNodeManager
            {
                get
                {
                    return m_DataNodeManager;
                }
            }

            public Dictionary<string, bool> LocalConditions
            {
                get
                {
                    return m_LocalConditions;
                }
                set
                {
                    m_LocalConditions = value;
                }
            }

            public DialogTree()
            {
                m_Name = "<Default>";
                m_RootNode = null;
                m_StartNode = null;
                m_CurrentNode = null;
                m_DataNodeManager = null;
            }

            public static DialogTree Create(string name, IDataNode rootNode)
            {
                DialogTree tree = ReferencePool.Acquire<DialogTree>();
                tree.m_DataNodeManager = GameKitModuleCenter.GetModule<IDataNodeManager>();
                tree.m_Name = name;
                tree.m_RootNode = rootNode;
                tree.m_StartNode = tree.m_RootNode;
                tree.m_CurrentNode = tree.m_RootNode;
                tree.m_LocalConditions = new Dictionary<string, bool>();
                return tree;
            }

            public static DialogTree Create(string name)
            {
                DialogTree tree = ReferencePool.Acquire<DialogTree>();
                tree.m_DataNodeManager = GameKitModuleCenter.GetModule<IDataNodeManager>();
                tree.m_Name = name;
                tree.m_RootNode = tree.DataNodeManager.GetOrAddNode(name);
                tree.m_StartNode = tree.m_RootNode;
                tree.m_CurrentNode = tree.m_RootNode;
                tree.m_LocalConditions = new Dictionary<string, bool>();
                return tree;
            }

            public void Reset()
            {
                m_CurrentNode = m_StartNode;
                // Utility.Debugger.LogSuccess("Reset Tree {0}", m_Name);
            }

            public IDataNode GetChildNode(int index = 0)
            {
                if (m_CurrentNode.IsLeaf || index < 0 || index >= m_CurrentNode.ChildCount)
                    return null;
                return  m_CurrentNode.GetChild(index) as IDataNode;
            }

            public IDataNode[] GetAllChildNodes()
            {
                if (m_CurrentNode.IsLeaf)
                    return null;
                return m_CurrentNode.GetAllChild();;
            }

            public void SetCondition(string predicate, bool status)
            {
                m_LocalConditions[predicate] = status;
            }


            public void Update(float elapseSeconds, float realElapseSeconds)
            {

            }

            public void Clear()
            {
                m_Name = "<Default>";
                m_RootNode = null;
                m_StartNode = null;
                m_CurrentNode = null;
                m_LocalConditions.Clear();
            }

            public void Release()
            {
                ReferencePool.Release(this);
            }
        }
    }

}