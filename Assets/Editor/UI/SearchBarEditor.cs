using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(SearchBar))]
public class SearchBarEditor : UnityEditor.UI.SliderEditor
{

    public override void OnInspectorGUI()
    {
        SearchBar component = (SearchBar)target;

        base.OnInspectorGUI();

        SerializedProperty onSelect = this.serializedObject.FindProperty("onBarSelect");
        EditorGUILayout.PropertyField(onSelect);

        SerializedProperty onReleased = this.serializedObject.FindProperty("onBarReleased");
        EditorGUILayout.PropertyField(onReleased);

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
