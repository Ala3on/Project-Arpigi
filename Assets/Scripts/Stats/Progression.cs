using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] charachterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];

            if (levels.Length < level)
            {
                return 0;
            }

            return levels[level - 1];
        }

        /* public int GetLevels(Stat stat, CharacterClass characterClass)
        // Metodo usato nel corso
        {
            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        } */

        public int GetMaxLevel(CharacterClass characterClass)
        {
            BuildLookup();
            return lookupTable[characterClass][Stat.ExperienceToLevelUp].Length;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in charachterClasses)
            {
                Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
            // public float[] health;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}