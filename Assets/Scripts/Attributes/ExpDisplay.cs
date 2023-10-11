using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using TMPro;
using UnityEngine;

public class ExpDisplay : MonoBehaviour
{
    Experience experience;

    private void Awake()
    {
        experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
    }

    private void Update()
    {
        GetComponent<TextMeshProUGUI>().SetText(experience.GetCurrentExp().ToString());

    }

}
