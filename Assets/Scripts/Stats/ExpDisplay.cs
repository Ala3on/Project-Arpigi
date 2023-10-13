using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{

    public class ExpDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI experienceText;
        [SerializeField] TextMeshProUGUI levelText;
        Experience experience;
        BaseStats stats;

        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            stats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            experienceText.SetText(experience.GetCurrentExp().ToString());
            levelText.SetText(stats.GetLevel().ToString());

        }

    }

}