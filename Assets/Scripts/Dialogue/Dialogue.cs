using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{

    [CreateAssetMenu(fileName = "New Dialogue", menuName = "RPG/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        // Negli scriptable object awake viene chiamato quando viene creato o la prima volta che viene aperto
        /*  private void Awake()
         {
 #if UNITY_EDITOR

 #endif
             // OnValidate();
         } */

        private void OnValidate()
        {

            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {

            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            // List<DialogueNode> result = new List<DialogueNode>();
            foreach (string childID in parentNode.GetChildren())
            {
                // if (nodeLookup.ContainsKey(childID))
                // {
                //    result.Add(nodeLookup[childID]);
                // } 
                // Modo più efficiente
                if (nodeLookup.TryGetValue(childID, out DialogueNode childNode))
                {
                    yield return childNode;
                }
            }
            // return result;
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }
        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

#if UNITY_EDITOR

        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            if (parent != null)
            {
                newNode.SetRect(new Rect(parent.GetRect().xMax + 20, parent.GetRect().center.y, 300, 100));
                newNode.SetSpeaker(parent.IsPlayerSpeaking() ? Speaker.Speaker1 : Speaker.Player);
                parent.AddChild(newNode.name);
            }
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                Undo.RecordObject(this, "Added Dialogue Node");
            }
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Delete Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }

#endif

        public bool IsRootNode(DialogueNode node)
        {
            return node == nodes[0];
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                CreateNode(null);
            }
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {

        }


    }
}