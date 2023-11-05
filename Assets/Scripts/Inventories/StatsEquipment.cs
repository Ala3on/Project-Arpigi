using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                if (slot == EquipLocation.DragonWeapon) continue;
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;
                foreach (var modifier in item.GetAdditiveModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                if (slot == EquipLocation.DragonWeapon) continue;
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;
                foreach (var modifier in item.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}
