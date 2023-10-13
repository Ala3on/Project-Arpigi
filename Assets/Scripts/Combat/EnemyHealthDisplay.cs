using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{

    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            Health health = fighter.GetTargetHealthComponent();
            if (health == null)
            {
                GetComponent<TextMeshProUGUI>().SetText("N/A");
            }
            else
            {
                //GetComponent<TextMeshProUGUI>().SetText("{0:0}%", health.GetPercentageHealth());
                GetComponent<TextMeshProUGUI>().SetText("{0:0}/{1:0}", health.GetCurrentHp(), health.GetMaxHp());

            }
        }
    }

}