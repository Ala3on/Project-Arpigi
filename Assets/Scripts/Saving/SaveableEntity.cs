using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdetifier = "";

        // CACHED STATE
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdetifier;
        }

        public JToken CaptureState()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            foreach (ISaveable jsonSaveable in GetComponents<ISaveable>())
            {
                JToken token = jsonSaveable.CaptureState();
                string component = jsonSaveable.GetType().ToString();
                Debug.Log($"{name} Capture {component} = {token.ToString()}");
                stateDict[jsonSaveable.GetType().ToString()] = token;
            }
            return state;
        }



        public void RestoreState(JToken s)
        {
            JObject state = s.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = state;
            foreach (ISaveable jsonSaveable in GetComponents<ISaveable>())
            {
                string component = jsonSaveable.GetType().ToString();
                if (stateDict.ContainsKey(component))
                {
                    Debug.Log($"{name} Restore {component} =>{stateDict[component].ToString()}");
                    jsonSaveable.RestoreState(stateDict[component]);
                }
            }

        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdetifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }

        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;
            if (globalLookup[candidate] == this) return true;
            if (globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }
            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }
            return false;
        }
#endif

    }
}
