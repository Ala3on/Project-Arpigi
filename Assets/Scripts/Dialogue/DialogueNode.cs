using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(10, 30, 300, 100);

        // GETTER
        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public Rect GetRect()
        {
            return rect;
        }

        // SETTER

#if UNITY_EDITOR
        public void AddChild(string childName)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childName);
        }

        public void RemoveChild(string childName)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childName);
        }

        public void SetText(string newText)
        {
            if (text != newText)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
            }
        }

        public void SetRect(Rect newRect)
        {
            rect = newRect;
        }

        public void Drag(Vector2 delta)
        {
            Undo.RecordObject(this, "Update Node Position");
            rect.position += delta;
        }
#endif




    }
}
