using System;
using UnityEditor;

public static class CustomEditorUtilities
{
    public static void NumberField(SerializedProperty property, string label = null)
    {
        EditorGUILayout.BeginHorizontal();
        if (label != null)
            EditorGUILayout.LabelField(label);
        
        switch (property.propertyType)
        {
            case SerializedPropertyType.Float:
                property.floatValue = EditorGUILayout.FloatField(property.floatValue);
                break;
            case SerializedPropertyType.Integer:
                property.intValue = EditorGUILayout.IntField(property.intValue);
                break;
        }
        
        EditorGUILayout.EndHorizontal();
    }

}