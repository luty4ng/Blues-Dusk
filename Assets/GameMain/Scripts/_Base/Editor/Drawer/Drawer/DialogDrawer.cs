using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace UnityGameKit.Editor
{
    [CustomPropertyDrawer(typeof(DialogAttribute), false)]
    public class DialogDrawer : PropertyDrawer
    {
        private const string CustomOptionName = "<None>";
        private readonly string m_Name;
        private string[] m_SceneNames;
        private int m_SceneNameIndex;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent content)
        {
            DialogAttribute attr = (attribute as DialogAttribute);
            List<string> m_TempSceneNames = new List<string>
            {
                CustomOptionName
            };
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/Configs/DialogCollection.txt");
            string[] splits = textAsset.text.Split(',');
            for (int i = 0; i < splits.Length; i++)
            {
                m_TempSceneNames.Add(splits[i]);
            }

            m_SceneNames = m_TempSceneNames.ToArray();
            m_SceneNameIndex = 0;
            Draw(property, m_TempSceneNames);
        }

        private void Draw(SerializedProperty serializedProperty, List<string> tempList)
        {
            if (!string.IsNullOrEmpty(serializedProperty.stringValue))
            {
                m_SceneNameIndex = tempList.IndexOf(serializedProperty.stringValue);
                if (m_SceneNameIndex <= 0)
                {
                    m_SceneNameIndex = 0;
                    serializedProperty.stringValue = null;
                }
            }

            int selectedIndex = EditorGUILayout.Popup(fieldInfo.Name, m_SceneNameIndex, m_SceneNames);
            if (selectedIndex != m_SceneNameIndex)
            {
                m_SceneNameIndex = selectedIndex;
                serializedProperty.stringValue = selectedIndex <= 0 ? null : m_SceneNames[selectedIndex];
            }

            //EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            //{
                
            //}
            //EditorGUI.EndDisabledGroup();

        }
    }


}