using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TimeScalerComponent))]
public class TimeScalerComponentInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TimeScalerComponent timeScalerComponent = (TimeScalerComponent)target;

        if (timeScalerComponent == null)
            return;

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);

        float currentTimeScale = Time.timeScale;
        float multiplier = timeScalerComponent.currentTimeScaleMultiplier;

        EditorGUILayout.LabelField("Time scale: " + currentTimeScale + ".");
        EditorGUILayout.LabelField("[Current multiplier: " + multiplier + ".]");

        EditorGUILayout.EndVertical();

        Repaint(); // Repaint each frame to show updated data.
    }
}