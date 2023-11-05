using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class AIStatsEquipment : MonoBehaviour, IModifierProvider
    {
        Fighter fighter;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            WeaponConfig weapon = fighter.GetCurrentWeapon();
            if (weapon != null)
            {
                foreach (var modifier in weapon.GetAdditiveModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            WeaponConfig weapon = fighter.GetCurrentWeapon();
            if (weapon != null)
            {
                foreach (var modifier in weapon.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}
