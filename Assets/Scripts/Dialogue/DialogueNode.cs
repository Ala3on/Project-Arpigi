using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public string uniqueID;
        public string text;
        public List<string> children = new List<string>();
        public Rect rectPosition = new Rect(10, 30, 300, 100);



    }
}
