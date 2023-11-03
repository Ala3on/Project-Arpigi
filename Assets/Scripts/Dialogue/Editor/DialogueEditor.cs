using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        Vector2 scrollPosition;
        Vector2 windowSize = new Vector2(1000, 1000);

        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] GUIStyle playerNodeStyle;
        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;
        [NonSerialized] DialogueNode selectedNode = null;
        [NonSerialized] Dictionary<Speaker, GUIStyle> speakerStyle = new Dictionary<Speaker, GUIStyle>();

        const float backgroundSize = 50f;

        void Awake()
        {
            for (int i = 0; i < Resources.FindObjectsOfTypeAll<Texture2D>().Length; i++)
            {
                Debug.Log(Resources.FindObjectsOfTypeAll<Texture2D>()[i].name);
            }
        }

        [MenuItem("Window/RPG/Dialogue Editor")]
        public static DialogueEditor ShowEditorWindow()
        {
            //var window = GetWindow<DialogueEditor>();
            return (DialogueEditor)GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");

            // window.titleContent = new GUIContent("Dialogue Editor");
            // window.Show();
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue == null) return false;
            DialogueEditor editor = ShowEditorWindow();
            editor.selectedDialogue = dialogue;
            editor.CalculateScrollViewSize();
            //editor.OnSelectionChanged();

            return true;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            SetupNodeStyles();

        }

        private void SetupNodeStyles()
        {
            // GLOBAL STYLE
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.active.background = EditorGUIUtility.Load("node0 on") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            nodeStyle.fixedWidth = 300;
            //nodeStyle.fixedHeight = 150;
            // speakerStyle.Add(Speaker.Speaker2, nodeStyle);

            // PLAYER STYLE
            playerNodeStyle = new GUIStyle(nodeStyle);
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.active.background = EditorGUIUtility.Load("node1 on") as Texture2D;
            speakerStyle.Add(Speaker.Player, playerNodeStyle);

            // COMPANION STYLE
            GUIStyle companionStyle = new GUIStyle(nodeStyle);
            companionStyle.normal.background = EditorGUIUtility.Load("node6") as Texture2D;
            companionStyle.active.background = EditorGUIUtility.Load("node6 on") as Texture2D;
            speakerStyle.Add(Speaker.Companion, companionStyle);

            // GUIStyle speaker1Style = new GUIStyle(nodeStyle);
            // speaker1Style.normal.background = EditorGUIUtility.Load("node5") as Texture2D;
            // speaker1Style.active.background = EditorGUIUtility.Load("node5 on") as Texture2D;

            // speakerStyle.Add(Speaker.Speaker1, speaker1Style);
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                CalculateScrollViewSize();
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

                // START SCROLL VIEW
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(windowSize.x, windowSize.y);
                //Debug.Log("width: " + position.width + " height: " + position.height + "canvas width: " + canvas.width + " canvas height: " + canvas.height);

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

                // TITLE
                EditorGUI.DropShadowLabel(new Rect(10, 10, 200, 20), selectedDialogue.name, EditorStyles.boldLabel);
                if (GUI.Button(new Rect(10, 30, 20, 20), "+"))
                {
                    selectedDialogue.CreateNode(null);
                    CalculateScrollViewSize();
                }

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                    CalculateScrollViewSize();
                }
                if (deletingNode != null)
                {
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
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    Selection.activeObject = draggingNode;
                    selectedNode = draggingNode;
                }
                else
                {
                    Selection.activeObject = selectedDialogue;
                    selectedNode = null;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.Drag(Event.current.delta);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                CalculateScrollViewSize();
                draggingNode = null;
            }

        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(mousePosition))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            if (speakerStyle.ContainsKey(node.GetSpeaker()))
            {
                style = speakerStyle[node.GetSpeaker()];
            }
            // GUIStyle style = SetNodeStyle(node);
            Texture2D normalTexture = style.normal.background;
            if (node == selectedNode)
            {
                style.normal.background = style.active.background;
            }
            // Trick per cambiare colore a un elemento GUI
            // basta cambiare colore prima della sua creazione e poi ripristinare il colore di default
            // Color defaultColor = GUI.color;
            // GUI.color = Color.cyan;
            GUI.Label(new Rect(node.GetRect().xMin + 10, node.GetRect().yMin - 15, 100, 15), node.GetSpeaker().ToString(), EditorStyles.boldLabel);
            GUILayout.BeginArea(node.GetRect(), style);


            GUILayout.BeginHorizontal();
            // ripristina il colore di default
            // GUI.color = defaultColor;
            GUILayout.BeginVertical();

            // TEXT INPUT
            node.SetText(EditorGUILayout.TextField(node.GetText()));

            // BUTTONS
            //GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }
            // if (node.IsPlayerSpeaking())
            // {
            //     if (GUILayout.Button("Player"))
            //     {
            //         node.SetSpeaker(Speaker.Speaker1);
            //         //style = nodeStyle;
            //     }
            // }
            // else
            // {
            //     if (GUILayout.Button("NPC"))
            //     {
            //         node.SetSpeaker(Speaker.Player);
            //         //style = playerNodeStyle;
            //     }
            // }
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();
            DrawLinkButtons(node);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
            style.normal.background = normalTexture;
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                // TODO è la strada giusta
                //if (GUI.Button(new Rect(node.rectPosition.width - 16, (node.rectPosition.height / 2) - 8, 16, 16), "Link"))
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
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }

            }
        }

        private void DrawConnections(DialogueNode node)
        {
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector2 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
                Vector2 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
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


        private void CalculateScrollViewSize()
        {
            Vector2 maxSize = new Vector2();
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                maxSize.x = node.GetRect().xMax > maxSize.x ? node.GetRect().xMax : maxSize.x;
                //maxSize.y = node.rectPosition.yMax >  ? node.rectPosition.yMax : maxSize.y;
                maxSize.y = node.GetRect().yMax > maxSize.y ? Mathf.Max(node.GetRect().yMax, position.height) : Mathf.Max(maxSize.y, position.height);
                // Debug.Log("maxSize.x: " + maxSize.x + " node.rectPosition.yMax: " + node.rectPosition.yMax + " position.height: " + position.height);
            }
            windowSize = maxSize + new Vector2(100, 0);
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

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        private Texture2D MakeTex(int width, int height, Color textureColor, RectOffset border, Color bordercolor)
        {
            int widthInner = width;
            width += border.left;
            width += border.right;

            Color[] pix = new Color[width * (height + border.top + border.bottom)];



            for (int i = 0; i < pix.Length; i++)
            {
                if (i < (border.bottom * width))
                    pix[i] = bordercolor;
                else if (i >= ((border.bottom * width) + (height * width)))  //Border Top
                    pix[i] = bordercolor;
                else
                { //Center of Texture

                    if ((i % width) < border.left) // Border left
                        pix[i] = bordercolor;
                    else if ((i % width) >= (border.left + widthInner)) //Border right
                        pix[i] = bordercolor;
                    else
                        pix[i] = textureColor;    //Color texture
                }
            }

            Texture2D result = new Texture2D(width, height + border.top + border.bottom);
            result.SetPixels(pix);
            result.Apply();


            return result;

        }

        private Texture2D CreateRoundedRectangleTexture(int width, int height, int borderRadius, Color backgroundColor)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Calcola la distanza dal centro
                    float dx = Mathf.Abs(x - width / 2);
                    float dy = Mathf.Abs(y - height / 2);
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);

                    if (distance < borderRadius)
                    {
                        pixels[x + y * width] = backgroundColor;
                    }
                    else
                    {
                        pixels[x + y * width] = Color.clear; // Imposta i pixel al di fuori del bordo arrotondato come trasparenti
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }




    }
}


