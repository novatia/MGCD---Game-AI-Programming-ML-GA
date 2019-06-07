using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

namespace Maze
{
    [CustomEditor(typeof(Brain))]
    public class BrainInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
                return;

            Brain brain = target as Brain;

            if (brain == null)
                return;

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("Alive: " + brain.isAlive);
            EditorGUILayout.LabelField("Live timer: " + brain.liveTimer.ToString("F2"));

            if (brain.dna != null)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("DNA", EditorStyles.boldLabel);
                for (int index = 0; index < brain.dna.dnaLength; ++index)
                {
                    EditorGUILayout.LabelField(index + ". " + brain.dna.GetGene(index));
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            Repaint();
        }
    }
}