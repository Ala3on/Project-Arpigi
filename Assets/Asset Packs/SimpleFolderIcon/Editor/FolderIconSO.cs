using System.Collections.Generic;
using UnityEngine;

namespace SimpleFolderIcon.Editor
{
    [CreateAssetMenu(fileName = "New Folder Icon Settings", menuName = "FolderIcons/New Settings")]
    public class FolderIconSO : ScriptableObject
    {

        public Texture2D icon;
        public List<string> folderNames;

        public void OnValidate()
        {
            IconDictionaryCreator.BuildDictionary();
        }
    }
}

