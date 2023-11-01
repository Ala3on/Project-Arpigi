using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "Health Potion", menuName = "RPG/Inventory/Consumable/New Health Potion")]
    public class HealthPotion : ActionItem
    {


        [SerializeField] float amountToHeal;
        [SerializeField] bool isPercentage;

        public override void Use(GameObject user)
        {
            Health player = user.GetComponent<Health>();
            if (player == null) return;
            player.Heal(amountToHeal, isPercentage);
        }
    }
}
