using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        // STATE
        private List<Pickup> droppedItems = new List<Pickup>();
        // private List<DropRecord> otherSceneDroppedItems = new List<DropRecord>();
        private List<otherSceneDropRecord> otherSceneDrops = new List<otherSceneDropRecord>();

        class otherSceneDropRecord
        {
            public string id;
            public int number;
            public Vector3 location;
            public int scene;
        }



        // PUBLIC

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        /// <param name="number">
        /// The number of items contained in the pickup. Only used if the item
        /// is stackable.
        /// </param>
        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, GetDropLocation(), number);
        }

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation(), 1);
        }

        // PROTECTED

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }
        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Quaternion GetDropRotation()
        {
            return transform.rotation;
        }

        // PRIVATE
        // TODO: sistemare rotation del drop 
        public void SpawnPickup(InventoryItem item, Vector3 spawnLocation, int number)
        {
            var pickup = item.SpawnPickup(spawnLocation, GetDropRotation(), number);
            droppedItems.Add(pickup);
        }

        /*  private struct DropRecord
         {
             public string itemID;
             public JToken position;
             public int number;
             public int sceneBuildIndex;
         }

         public JToken CaptureState()
         {
             RemoveDestroyedDrops();
             // var droppedItemsList = new DropRecord[droppedItems.Count];
             var droppedItemsList = new List<DropRecord>();
             int buildIndex = SceneManager.GetActiveScene().buildIndex;
             foreach (Pickup pickup in droppedItems)
             {
                 var droppedItem = new DropRecord();
                 droppedItem.itemID = pickup.GetItem().GetItemID();
                 droppedItem.position = pickup.transform.position.ToToken();
                 droppedItem.number = pickup.GetNumber();
                 droppedItem.sceneBuildIndex = buildIndex;
                 droppedItemsList.Add(droppedItem);
             }
             droppedItemsList.AddRange(otherSceneDroppedItems);
             return JToken.FromObject(droppedItemsList);
         }

         public void RestoreState(JToken state)
         {
             var droppedItemsList = state.ToObject<List<DropRecord>>();
             int buildIndex = SceneManager.GetActiveScene().buildIndex;
             otherSceneDroppedItems.Clear();
             foreach (var item in droppedItemsList)
             {
                 if (item.sceneBuildIndex != buildIndex)
                 {
                     otherSceneDroppedItems.Add(item);
                     continue;
                 }
                 var pickupItem = InventoryItem.GetFromID(item.itemID);
                 Vector3 position = item.position.ToVector3();
                 int number = item.number;
                 SpawnPickup(pickupItem, position, number);
             }
         } */



        List<otherSceneDropRecord> MergeDroppedItemsWithOtherSceneDrops()
        {
            List<otherSceneDropRecord> result = new List<otherSceneDropRecord>();
            result.AddRange(otherSceneDrops);
            foreach (var item in droppedItems)
            {
                otherSceneDropRecord drop = new otherSceneDropRecord();
                drop.id = item.GetItem().GetItemID();
                drop.number = item.GetNumber();
                drop.location = item.transform.position;
                drop.scene = SceneManager.GetActiveScene().buildIndex;
                result.Add(drop);
            }
            return result;
        }

        public JToken CaptureState()
        {
            RemoveDestroyedDrops();
            var drops = MergeDroppedItemsWithOtherSceneDrops();
            JArray state = new JArray();
            IList<JToken> stateList = state;
            foreach (var drop in drops)
            {
                JObject dropState = new JObject();
                IDictionary<string, JToken> dropStateDict = dropState;
                dropStateDict["id"] = JToken.FromObject(drop.id);
                dropStateDict["number"] = drop.number;
                dropStateDict["location"] = drop.location.ToToken();
                dropStateDict["scene"] = drop.scene;
                stateList.Add(dropState);
            }

            return state;
        }

        private void ClearExistingDrops()
        {
            foreach (var oldDrop in droppedItems)
            {
                if (oldDrop != null) Destroy(oldDrop.gameObject);
            }

            otherSceneDrops.Clear();
        }

        public void RestoreState(JToken state)
        {
            if (state is JArray stateArray)
            {
                int currentScene = SceneManager.GetActiveScene().buildIndex;
                IList<JToken> stateList = stateArray;
                ClearExistingDrops();
                foreach (var entry in stateList)
                {
                    if (entry is JObject dropState)
                    {
                        IDictionary<string, JToken> dropStateDict = dropState;
                        int scene = dropStateDict["scene"].ToObject<int>();
                        InventoryItem item = InventoryItem.GetFromID(dropStateDict["id"].ToObject<string>());
                        int number = dropStateDict["number"].ToObject<int>();
                        Vector3 location = dropStateDict["location"].ToVector3();
                        if (scene == currentScene)
                        {
                            SpawnPickup(item, location, number);
                        }
                        else
                        {
                            var otherDrop = new otherSceneDropRecord();
                            otherDrop.id = item.GetItemID();
                            otherDrop.number = number;
                            otherDrop.location = location;
                            otherDrop.scene = scene;
                            otherSceneDrops.Add(otherDrop);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }
    }
}