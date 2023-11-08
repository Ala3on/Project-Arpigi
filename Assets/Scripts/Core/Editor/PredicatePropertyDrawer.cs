using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG.Quests;
using System.Linq;
using GameDevTV.Inventories;

namespace RPG.Core.Editor
{
    [CustomPropertyDrawer(typeof(Condition.Predicate))]
    public class PredicatePropertyDrawer : PropertyDrawer
    {
        Dictionary<string, Quest> quests;
        Dictionary<string, InventoryItem> items;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty predicate = property.FindPropertyRelative("predicate");
            SerializedProperty parameters = property.FindPropertyRelative("parameters");
            SerializedProperty negate = property.FindPropertyRelative("negate");
            float propHeight = EditorGUI.GetPropertyHeight(predicate);
            position.height = propHeight;
            EditorGUI.PropertyField(position, predicate);

            EPredicate selectedPredicate = (EPredicate)predicate.enumValueIndex;

            if (selectedPredicate == EPredicate.Select) return; //Stop drawing if there's no predicate
            while (parameters.arraySize < 2)
            {
                parameters.InsertArrayElementAtIndex(0);
            }
            SerializedProperty parameterZero = parameters.GetArrayElementAtIndex(0);
            SerializedProperty parameterOne = parameters.GetArrayElementAtIndex(1); //Edit: was accidentally 0 in first draft
            if (selectedPredicate == EPredicate.HasQuest || selectedPredicate == EPredicate.CompletedQuest || selectedPredicate == EPredicate.CompletedObjective)
            {
                position.y += propHeight;
                DrawQuest(position, parameterZero);
            }
            if (selectedPredicate == EPredicate.CompletedObjective)
            {
                position.y += propHeight;
                DrawObjective(position, parameterOne, parameterZero);
            }
            if (selectedPredicate == EPredicate.HasItem || selectedPredicate == EPredicate.HasItems || selectedPredicate == EPredicate.HasItemEquipped)
            {
                position.y += propHeight;
                DrawInventoryItemList(position, parameterZero, selectedPredicate == EPredicate.HasItems, selectedPredicate == EPredicate.HasItemEquipped);
            }
            if (selectedPredicate == EPredicate.HasItems)
            {
                position.y += propHeight;
                DrawIntSlider(position, "Qty Needed", parameterOne, 1, 100);
            }
            if (selectedPredicate == EPredicate.HasLevel)
            {
                position.y += propHeight;
                DrawIntSlider(position, "Level Required", parameterZero, 1, 100);
            }
            /*   if (selectedPredicate == EPredicate.MinimumTrait)
             {
                 position.y += propHeight;
                 DrawTrait(position, parameterZero);
                 position.y += propHeight;
                 DrawIntSlider(position, "Minimum", parameterOne, 1,100);
             } */
            position.y += propHeight;
            EditorGUI.PropertyField(position, negate);
        }

        // COMMON

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty predicate = property.FindPropertyRelative("predicate");
            float propHeight = EditorGUI.GetPropertyHeight(predicate) + EditorGUIUtility.standardVerticalSpacing;
            EPredicate selectedPredicate = (EPredicate)predicate.enumValueIndex;
            switch (selectedPredicate)
            {
                case EPredicate.Select: // No parameters, we only want the bare enum. 
                    return propHeight;
                case EPredicate.HasLevel: // All of these take 1 parameter
                case EPredicate.CompletedQuest:
                case EPredicate.HasQuest:
                case EPredicate.HasItem:
                case EPredicate.HasItemEquipped:
                    return propHeight * 3.0f; // Predicate + one parameter + negate
                case EPredicate.CompletedObjective: // All of these take 2 parameters
                case EPredicate.HasItems:
                case EPredicate.MinimumTrait:
                    return propHeight * 4.0f; // Predicate + 2 parameters + negate;
            }
            return propHeight * 2.0f;
        }

        private static void DrawIntSlider(Rect position, string caption, SerializedProperty intParameter, int minLevel = 1, int maxLevel = 100)
        {
            EditorGUI.BeginProperty(position, new GUIContent(caption), intParameter);
            if (!int.TryParse(intParameter.stringValue, out int value))
            {
                value = 1;
            }
            EditorGUI.BeginChangeCheck();
            int result = EditorGUI.IntSlider(position, caption, value, minLevel, maxLevel);
            if (EditorGUI.EndChangeCheck())
            {
                intParameter.stringValue = $"{result}";
            }
            EditorGUI.EndProperty();
        }

        // QUESTS

        void BuildQuestList()
        {
            if (quests != null) return;
            quests = new Dictionary<string, Quest>();
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                quests[quest.name] = quest;
            }
        }

        private void DrawQuest(Rect position, SerializedProperty element)
        {
            BuildQuestList();
            var names = quests.Keys.ToList();
            int index = names.IndexOf(element.stringValue);

            EditorGUI.BeginProperty(position, new GUIContent("Quest:"), element);
            int newIndex = EditorGUI.Popup(position, "Quest:", index, names.ToArray());
            if (newIndex != index)
            {
                element.stringValue = names[newIndex];
            }

            EditorGUI.EndProperty();
        }

        void DrawObjective(Rect position, SerializedProperty element, SerializedProperty selectedQuest)
        {
            string questName = selectedQuest.stringValue;
            if (!quests.ContainsKey(questName))
            {
                EditorGUI.HelpBox(position, "Please Select A Quest", MessageType.Error);
                return;
            }

            List<string> references = new List<string>();
            List<string> descriptions = new List<string>();
            foreach (Quest.Objective objective in quests[questName].GetObjectives())
            {
                references.Add(objective.reference);
                descriptions.Add(objective.description);
            }
            int index = references.IndexOf(element.stringValue);
            EditorGUI.BeginProperty(position, new GUIContent("objective"), element);
            int newIndex = EditorGUI.Popup(position, "Objective:", index, descriptions.ToArray());
            if (newIndex != index)
            {
                element.stringValue = references[newIndex];
            }
            EditorGUI.EndProperty();
        }

        // ITEMS

        void BuildInventoryItemsList()
        {
            if (items != null) return;
            items = new Dictionary<string, InventoryItem>();
            foreach (InventoryItem item in Resources.LoadAll<InventoryItem>(""))
            {
                items[item.GetItemID()] = item;
            }
        }

        void DrawInventoryItemList(Rect position, SerializedProperty element, bool stackable = false, bool equipment = false)
        {
            BuildInventoryItemsList();
            List<string> ids = items.Keys.ToList();
            //ids = ids.Where( //SQL like method to used to filter.
            //   s=> //This tells the compiler that for the rest of the Where, s is an individual element of type string
            //   items[s].IsStackable()) //iterates over each element in ids testing this expression
            //   .ToList(); //converts the result to a list (default is IEnumerable<string>)
            if (stackable) ids = ids.Where(s => items[s].IsStackable()).ToList();
            if (equipment) ids = ids.Where(s => items[s] is EquipableItem e).ToList();
            List<string> displayNames = new List<string>();
            foreach (string id in ids)
            {
                displayNames.Add(items[id].GetDisplayName());
            }
            int index = ids.IndexOf(element.stringValue);
            EditorGUI.BeginProperty(position, new GUIContent("items"), element);
            int newIndex = EditorGUI.Popup(position, "Item", index, displayNames.ToArray());
            if (newIndex != index)
            {
                element.stringValue = ids[newIndex];
            }
            EditorGUI.EndProperty();
        }

        // TRAITS

        /* void DrawTrait(Rect position, SerializedProperty element)
        {
            if (!Enum.TryParse(element.stringValue, out Trait trait))
            {
                trait = Trait.Strength;
            }
            EditorGUI.BeginProperty(position,new GUIContent("Trait"), element);
            Trait newTrait = (Trait)EditorGUI.EnumPopup(position, "Trait:", trait);
            if (newTrait != trait)
            {
                element.stringValue = $"{newTrait}";
            }
            EditorGUI.EndProperty();
        } */

    }
}
