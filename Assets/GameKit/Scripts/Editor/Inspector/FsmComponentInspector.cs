﻿

using GameKit;
using GameKit.Fsm;
using UnityEditor;
using UnityGameKit.Runtime;

namespace UnityGameKit.Editor
{
    [CustomEditor(typeof(FsmComponent))]
    internal sealed class FsmComponentInspector : GameKitInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            FsmComponent t = (FsmComponent)target;

            if (IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("FSM Count", t.Count.ToString());

                FsmBase[] fsms = t.GetAllFsms();
                foreach (FsmBase fsm in fsms)
                {
                    DrawFsm(fsm);
                }
            }

            Repaint();
        }

        private void OnEnable()
        {
        }

        private void DrawFsm(FsmBase fsm)
        {
            EditorGUILayout.LabelField(fsm.Name, fsm.IsRunning ? Utility.Text.Format("{0}, {1:F1} s", fsm.CurrentStateName, fsm.CurrentStateTime) : (fsm.IsDestroyed ? "Destroyed" : "Not Running"));
        }
    }
}
