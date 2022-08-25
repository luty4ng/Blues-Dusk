﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameKit;
using GameKit.DataTable;
using UnityEditor;
using UnityGameKit.Runtime;

namespace UnityGameKit.Editor
{
    [CustomEditor(typeof(DataTableComponent))]
    internal sealed class DataTableComponentInspector : GameKitInspector
    {
        private SerializedProperty m_CachedBytesSize = null;

        private HelperInfo<DataTableHelperBase> m_DataTableHelperInfo = new HelperInfo<DataTableHelperBase>("DataTable");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DataTableComponent t = (DataTableComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_DataTableHelperInfo.Draw();
                EditorGUILayout.PropertyField(m_CachedBytesSize);
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Data Table Count", t.Count.ToString());
                EditorGUILayout.LabelField("Cached Bytes Size", t.CachedBytesSize.ToString());

                DataTableBase[] dataTables = t.GetAllDataTables();
                foreach (DataTableBase dataTable in dataTables)
                {
                    DrawDataTable(dataTable);
                }
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void OnEnable()
        {
            m_CachedBytesSize = serializedObject.FindProperty("m_CachedBytesSize");

            m_DataTableHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        private void DrawDataTable(DataTableBase dataTable)
        {
            EditorGUILayout.LabelField(dataTable.FullName, Utility.Text.Format("{0} Rows", dataTable.Count));
        }

        private void RefreshTypeNames()
        {
            m_DataTableHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
    }
}