using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventory/Create New Drop Library")]
    public class DropLibrary : ScriptableObject
    {
        // [SerializeField] DropConfig[] GuaranteedDrops;
        [SerializeField] DropConfig[] potentialDrops;
        [SerializeField] float[] dropChancePercentage; // the array indeces represent levels 
        [SerializeField] int[] minDrops;
        [SerializeField] int[] maxDrops;

        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;
            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable())
                {
                    return 1;
                }
                int min = GetByLevel(minNumber, level);
                int max = GetByLevel(maxNumber, level);
                return UnityEngine.Random.Range(min, max + 1); // +1 because max is exclusive
            }
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
            public Dropped(InventoryItem item, int number = 1)
            {
                this.item = item;
                this.number = number;
            }
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            /* foreach (var drop in GuaranteedDrops)
            {
                yield return new Dropped(drop.item, drop.GetRandomNumber(level));
            } */
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }
            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        private bool ShouldRandomDrop(int level)
        {
            return UnityEngine.Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
        }

        private int GetRandomNumberOfDrops(int level)
        {
            int min = GetByLevel(minDrops, level);
            int max = GetByLevel(maxDrops, level);
            return UnityEngine.Random.Range(min, max + 1);
        }

        private Dropped GetRandomDrop(int level)
        {
            DropConfig drop = SelectRandomItem(level);
            if (drop == null) return new Dropped();
            return new Dropped(drop.item, drop.GetRandomNumber(level));
        }

        private DropConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = UnityEngine.Random.Range(0, totalChance);
            float chanceTotal = 0;
            foreach (var drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance, level);
                if (chanceTotal > randomRoll)
                {
                    return drop;
                }
                //randomRoll -= drop.relativeChance;
            }
            return null;
        }

        private float GetTotalChance(int level)
        {
            float total = 0;
            foreach (var drop in potentialDrops)
            {
                total += GetByLevel(drop.relativeChance, level);
            }
            return total;
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0) return default;
            if (level >= values.Length) return values[values.Length - 1];
            if (level <= 0) return default;
            return values[level - 1];
        }
    }
}
