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
        [SerializeField] RectTransform foreground = null;
        [SerializeField] TextMeshProUGUI healthText = null;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            // GetComponent<TextMeshProUGUI>().SetText("{0:0}%", health.GetPercentageHealth());
            //GetComponent<TextMeshProUGUI>().SetText(GetComponent<TextMeshProUGUI>().text + " " + health.GetCurrentHp());
            healthText.SetText("{0:0}/{1:0}", health.GetCurrentHp(), health.GetMaxHp());
            float fraction = health.GetHpFraction();

            foreground.localScale = new Vector3(fraction, 1, 1);

        }

    }
}