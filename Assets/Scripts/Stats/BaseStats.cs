using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Stats
{

    public class BaseStats : MonoBehaviour
    {
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect = null;
        [SerializeField] bool shouldUseModifier = false;
        [SerializeField] int currentLevel = 1;

        public event Action onLevelUp;


        public float GetStat(Stat stat)
        {
            if (!shouldUseModifier)
            {
                return GetBaseStat(stat);
            }
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, currentLevel);
        }

        private float GetAdditiveModifier(Stat stat)
        {
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifiers in provider.GetAdditiveModifiers(stat))
                {
                    total += modifiers;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifiers in provider.GetPercentageModifiers(stat))
                {
                    total += modifiers;
                }
            }
            return total;
        }

        public void TryLevelUp(float exp)
        {
            int oldLevel = currentLevel;
            CalculateLevel(exp);

            if (oldLevel < currentLevel)
            {
                LevelUpEffect();
                onLevelUp();
            }
        }

        public void CalculateLevel(float exp)
        {
            if (currentLevel >= progression.GetMaxLevel(characterClass)) return;

            if (exp >= progression.GetStat(Stat.ExperienceToLevelUp, characterClass, currentLevel + 1))
            {
                currentLevel++;
                CalculateLevel(exp);
            }


        }


        /* public int GetCourseLevel()
        {
            float currentXP = GetComponent<Experience>().GetCurrentExp();
            int penultimateLevel = progression.GetMaxLevel(characterClass);
            for (int level = 1; level < penultimateLevel; level++)
            {
                float expToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, currentLevel);
                if (expToLevelUp > currentXP)
                {
                    return currentLevel;
                }
            }
            return penultimateLevel + 1;
        } */

        public int GetLevel()
        {
            return currentLevel;
        }

        private void LevelUpEffect()
        {
            if (levelUpEffect != null)
            {
                GameObject levelUpFX = Instantiate(levelUpEffect, transform);
                Destroy(levelUpFX, 3f);
            }

        }

    }

}