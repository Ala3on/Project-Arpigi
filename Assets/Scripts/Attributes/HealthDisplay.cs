using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            // GetComponent<TextMeshProUGUI>().SetText("{0:0}%", health.GetPercentageHealth());
            //GetComponent<TextMeshProUGUI>().SetText(GetComponent<TextMeshProUGUI>().text + " " + health.GetCurrentHp());
            GetComponent<TextMeshProUGUI>().SetText(health.GetCurrentHp() + "/" + health.GetMaxHp());

        }

    }
}