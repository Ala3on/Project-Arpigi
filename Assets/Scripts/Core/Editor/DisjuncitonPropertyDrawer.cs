using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Core
{

    [CustomPropertyDrawer(typeof(Condition.Disjunction))]
    public class DisjunctionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.MiddleCenter;
            SerializedProperty or = property.FindPropertyRelative("or");
            float propHeight = EditorGUIUtility.singleLineHeight;
            Rect upPosition = position;
            upPosition.width -= EditorGUIUtility.labelWidth;
            upPosition.x = position.xMax - upPosition.width;
            upPosition.width /= 3.0f;
            upPosition.height = propHeight;
            Rect downPosition = upPosition;
            downPosition.x += upPosition.width;
            Rect deletePosition = upPosition;
            deletePosition.x = position.xMax - deletePosition.width;
            var enumerator = or.GetEnumerator();
            int idx = 0;
            int itemToRemove = -1;
            int itemToMoveUp = -1;
            int itemToMoveDown = -1;
            while (enumerator.MoveNext())
            {
                if (idx > 0)
                {
                    if (GUI.Button(downPosition, "v")) itemToMoveDown = idx - 1;
                    EditorGUI.DropShadowLabel(position, "-------OR-------", style);
                    position.y += propHeight;
                }
                SerializedProperty p = enumerator.Current as SerializedProperty;
                position.height = EditorGUI.GetPropertyHeight(p);
                EditorGUI.PropertyField(position, p);
                position.y += position.height;
                position.height = propHeight;
                upPosition.y = deletePosition.y = downPosition.y = position.y;
                if (GUI.Button(deletePosition, "Remove")) itemToRemove = idx;
                if (idx > 0 && GUI.Button(upPosition, "^")) itemToMoveUp = idx;
                position.y += propHeight;
                idx++;
            }

            if (itemToRemove >= 0)
            {
                or.DeleteArrayElementAtIndex(itemToRemove);
            }

            if (itemToMoveUp >= 0)
            {
                or.MoveArrayElement(itemToMoveUp, itemToMoveUp - 1);
            }

            if (itemToMoveDown >= 0)
            {
                or.MoveArrayElement(itemToMoveDown, itemToMoveDown + 1);
            }
            if (GUI.Button(position, "Add Predicate"))
            {
                or.InsertArrayElementAtIndex(idx);
            }

        }



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float result = 0;
            float propHeight = EditorGUIUtility.singleLineHeight;
            IEnumerator enumerator = property.FindPropertyRelative("or").GetEnumerator();
            bool multiple = false;
            while (enumerator.MoveNext())
            {
                SerializedProperty p = enumerator.Current as SerializedProperty;
                result += EditorGUI.GetPropertyHeight(p) + propHeight;
                if (multiple) result += propHeight;
                multiple = true;
            }
            return result + propHeight * 1.5f;
        }
    }

}
