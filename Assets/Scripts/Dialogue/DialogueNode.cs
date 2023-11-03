using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] Speaker speaker;
        [SerializeField][TextArea] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(10, 30, 300, 100);

        // construct a Dictionary to lookup the values in OnEnable, and then assign the style from the Dictionary.
        //Dictionary<Speaker, GUIStyle> speakerStyle = new Dictionary<Speaker, GUIStyle>();

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

        public bool IsPlayerSpeaking()
        {
            return speaker == Speaker.Player;
        }

        public Speaker GetSpeaker()
        {
            return speaker;
        }


        // SETTER

#if UNITY_EDITOR
        public void AddChild(string childName)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childName);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childName)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childName);
            EditorUtility.SetDirty(this);

        }

        public void SetText(string newText)
        {
            if (text != newText)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
                EditorUtility.SetDirty(this);

            }
        }

        public void SetRect(Rect newRect)
        {
            rect = newRect;
            EditorUtility.SetDirty(this);

        }

        public void Drag(Vector2 delta)
        {
            Undo.RecordObject(this, "Update Node Position");
            rect.position += delta;
            EditorUtility.SetDirty(this);
        }


        public void SetSpeaker(Speaker newSpeaker)
        {
            if (speaker != newSpeaker)
            {
                Undo.RecordObject(this, "Change Dialogue Speaker");
                speaker = newSpeaker;
                EditorUtility.SetDirty(this);
            }
        }


#endif




    }
}
