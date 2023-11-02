using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        Vector2 scrollPosition;
        Vector2 windowSize = new Vector2(1000, 1000);

        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;

        const float backgroundSize = 50f;

        [MenuItem("Window/RPG/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            //var window = GetWindow<DialogueEditor>();
            var window = (DialogueEditor)GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");

            // window.titleContent = new GUIContent("Dialogue Editor");
            // window.Show();
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue == null) return false;
            ShowEditorWindow();

            return true;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            // EditorGUILayout.LabelField(new Rect(10, 10, 200, 200), "Hello", EditorStyles.boldLabel);
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected", EditorStyles.boldLabel);

            }
            else
            {
                ProcessEvents();
                ProcessDragCanvas();

                // TITLE
                EditorGUILayout.LabelField(selectedDialogue.name, EditorStyles.boldLabel);

                // START SCROLL VIEW
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(windowSize.x, windowSize.y);

                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                Rect textureCoords = new Rect(0, 0, canvas.width / backgroundSize, canvas.height / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();
                // END SCROLL VIEW

                if (creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                    CalculateScrollViewSize();
                }
                if (deletingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Delete Dialogue Node");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                    CalculateScrollViewSize();
                }

            }


        }


        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetRootAtPoint(Event.current.mousePosition + scrollPosition);
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Update Node Position");
                draggingNode.rectPosition.position += Event.current.delta;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                // CalculateScrollViewSize();
                CalculateScrollViewSize();
                draggingNode = null;
            }

        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rectPosition, nodeStyle);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node: " + node.uniqueID, EditorStyles.boldLabel);
            string newText = EditorGUILayout.TextField(node.text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                node.text = newText;
            }

            // BUTTONS
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }
            DrawLinkButtons(node);
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.children.Contains(node.uniqueID))
            {
                if (GUILayout.Button("Unlink"))
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                    linkingParentNode.children.Remove(node.uniqueID);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                    linkingParentNode.children.Add(node.uniqueID);
                    linkingParentNode = null;
                }

            }
        }

        private void DrawConnections(DialogueNode node)
        {
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector2 startPosition = new Vector2(node.rectPosition.xMax, node.rectPosition.center.y);
                Vector2 endPosition = new Vector2(childNode.rectPosition.xMin, childNode.rectPosition.center.y);
                Vector2 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.5f;

                Handles.DrawBezier(
                    startPosition,
                    endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    Color.white,
                    null,
                    4f
                );
            }
        }

        private DialogueNode GetRootAtPoint(Vector2 mousePosition)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.rectPosition.Contains(mousePosition))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }

        private void CalculateScrollViewSize()
        {
            Vector2 maxSize = new Vector2();
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                maxSize.x = node.rectPosition.xMax > maxSize.x ? node.rectPosition.xMax : maxSize.x;
                maxSize.y = node.rectPosition.yMax > maxSize.y ? node.rectPosition.yMax : maxSize.y;
            }
            windowSize = maxSize + new Vector2(100, 100);
            GUI.changed = true;
        }

        private void ProcessDragCanvas()
        {
            if (draggingNode == null && Event.current.type == EventType.MouseDrag)
            {
                scrollPosition -= Event.current.delta;
                GUI.changed = true;
            }
        }


    }
}


