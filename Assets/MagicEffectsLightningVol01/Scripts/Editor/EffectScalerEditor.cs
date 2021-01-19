// (c) Copyright 2014 Luke Light&Magic. All rights reserved.

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EffectScaler))]
public class EffectScalerEditor : Editor {
  private SerializedObject scaleObject;
  private GUIStyle commentStyle;

  private SerializedProperty scaleProperty;
  private SerializedProperty scaleLightProperty;
  private SerializedProperty scaleTrailEffectProperty;


  void Awake() {
    scaleObject = new SerializedObject(target);
    scaleProperty = scaleObject.FindProperty("EffectScale");
    scaleLightProperty = scaleObject.FindProperty("ScaleLight");
    scaleTrailEffectProperty = scaleObject.FindProperty("ScaleTrailEffect");
  }

  public override void OnInspectorGUI() {
    if (commentStyle == null) {
      commentStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
      commentStyle.font = EditorStyles.miniFont;
      commentStyle.alignment = TextAnchor.UpperLeft;
    }

    if (scaleObject.targetObject == target)
      scaleObject.Update();

    GUILayout.Label("EffectScaler Settings", EditorStyles.boldLabel);
    EditorGUILayout.Space();
    EditorGUILayout.PropertyField(scaleProperty, new GUIContent("Scale"));
    GUILayout.BeginHorizontal();
    GUILayout.Space(105.0f);
    GUILayout.Box("Only work in editor mode. Scaling is completed, you can remove the script does not affect the final result.", commentStyle, GUILayout.ExpandWidth(true));
    GUILayout.EndHorizontal();

    EditorGUILayout.Space();
    EditorGUILayout.Space();

    EditorGUILayout.PropertyField(scaleLightProperty, new GUIContent("Scale Lights Range"));
    EditorGUILayout.PropertyField(scaleTrailEffectProperty, new GUIContent("Scale Trail&Beam Effects"));

    scaleObject.ApplyModifiedProperties();

  }

}
