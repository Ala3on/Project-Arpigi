using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{

    [CreateAssetMenu(fileName = "New Dialogue", menuName = "RPG/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        // Negli scriptable object awake viene chiamato quando viene creato o la prima volta che viene aperto
        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("Awake from " + this.name);
            if (nodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode();
                rootNode.uniqueID = Guid.NewGuid().ToString();
                nodes.Add(rootNode);
            }
#endif
            OnValidate();
        }

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.uniqueID] = node;
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
            foreach (string childID in parentNode.children)
            {
                /* if (nodeLookup.ContainsKey(childID))
                {
                    result.Add(nodeLookup[childID]);
                } */
                // Modo più efficiente
                if (nodeLookup.TryGetValue(childID, out DialogueNode childNode))
                {
                    yield return childNode;
                }
            }
            // return result;
        }

        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = new DialogueNode();
            newNode.uniqueID = Guid.NewGuid().ToString();
            newNode.rectPosition = new Rect(parent.rectPosition.xMax + 20, parent.rectPosition.center.y, 300, 100);
            parent.children.Add(newNode.uniqueID);
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.children.Remove(nodeToDelete.uniqueID);
            }
        }
    }
}