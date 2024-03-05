using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.Events;

[CustomPropertyDrawer(typeof(Task))]
public class TaskDrawerUIE : PropertyDrawer
{
    private Task task;

    float lineHeight = 20.0f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty totalHeight = property.FindPropertyRelative("totalHeight");
        SerializedProperty variableHeight = property.FindPropertyRelative("variableHeight");
        int lines = 1;


        var typeValue = property.FindPropertyRelative("taskType");
        TaskType display = (TaskType)typeValue.enumValueIndex;
        task = attribute as Task;
        EditorGUI.BeginProperty(position, label, property);

        Rect taskTypePopupRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        display = (TaskType)EditorGUI.EnumPopup(taskTypePopupRect, "Task Type:", display);
        typeValue.enumValueIndex = (int)display;
        position.y += lineHeight;
        lines++;

        if (typeValue.enumValueIndex == (int)TaskType.Kill)
        {
            var enemyTypeValue = property.FindPropertyRelative("enemyType");
            EnemyType enemy = (EnemyType)enemyTypeValue.enumValueIndex;
            Rect enemyTypePopupRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            enemy = (EnemyType)EditorGUI.EnumPopup(enemyTypePopupRect, "Enemy Type:", enemy);
            enemyTypeValue.enumValueIndex = (int)enemy;
            position.y += lineHeight;
            lines++;


            var numValue = property.FindPropertyRelative("num");
            int num = numValue.intValue;
            Rect numRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            num = EditorGUI.IntField(numRect, "Number of Enemies:", num);
            numValue.intValue = num;
            position.y += lineHeight;
            lines++;

        }
        else if (typeValue.enumValueIndex == (int)TaskType.Fetch)
        {
            var collTypeValue = property.FindPropertyRelative("collType");
            Collectible coll = (Collectible)collTypeValue.objectReferenceValue;
            Rect collTypePopupRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            coll = (Collectible)EditorGUI.ObjectField(collTypePopupRect, "Ingredient:", coll, typeof(Collectible), true);
            collTypeValue.objectReferenceValue = (Collectible)coll;
            position.y += lineHeight;
            lines++;

            var numValue = property.FindPropertyRelative("num");
            int num = numValue.intValue;
            Rect numRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            num = EditorGUI.IntField(numRect, "Number of Enemies:", num);
            numValue.intValue = num;
            position.y += lineHeight;
            lines++;
        }
        else if (typeValue.enumValueIndex == (int)TaskType.TalkTo)
        {
            var speakerTypeValue = property.FindPropertyRelative("speaker");
            DialogueTrigger speaker = (DialogueTrigger)speakerTypeValue.objectReferenceValue;
            Rect speakerTypePopupRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            speaker = (DialogueTrigger)EditorGUI.ObjectField(speakerTypePopupRect, "Speaker:", speaker, typeof(DialogueTrigger), true);
            speakerTypeValue.objectReferenceValue = (DialogueTrigger)speaker;
            position.y += lineHeight;
            lines++;
        }

        //for function
        if(typeValue.enumValueIndex == (int)TaskType.Kill || typeValue.enumValueIndex == (int)TaskType.Fetch)
        {
            SerializedProperty unityEventProp = property.FindPropertyRelative("toCall");
            Rect unityEventPropRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * 4);
            EditorGUI.PropertyField(position, unityEventProp);
            position.y += lineHeight;
            lines += 4;
        }


        EditorGUI.EndProperty();

        float newHeight = lineHeight * lines;
        if (totalHeight.floatValue != newHeight)
            totalHeight.floatValue = newHeight;

    }

    //look into foldouts
    //https://answers.unity.com/questions/1610550/how-can-propertydrawer-with-fouldouts-to-draw-at-t.html




    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty totalHeight = property.FindPropertyRelative("totalHeight");

        if (totalHeight != null)
        {
            float height = Mathf.Max(20, totalHeight.floatValue); // Height of 0 will freeze the editor

            return height;
        }

        return base.GetPropertyHeight(property, label);
    }
}
