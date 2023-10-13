using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{

    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoint = 0;

        // public delegate void ExperienceGainedDelegate();
        // public event ExperienceGainedDelegate onExperienceGained;
        // se non serve passare parametri o avere valori di ritorno 
        // si può omettere di dichiarare un delegate e dichiarare direttamente un Action 
        public event Action onExperienceGained;

        public void GainExperience(float exp)
        {
            experiencePoint += exp;
            GetComponent<BaseStats>().TryLevelUp(experiencePoint);
            // Ho implementato questa cosa diversamente
            // onExperienceGained();
        }

        public float GetCurrentExp()
        {
            return experiencePoint;
        }

        public JToken CaptureState()
        {
            return JToken.FromObject(experiencePoint);
        }

        public void RestoreState(JToken state)
        {
            experiencePoint = state.ToObject<float>();
            GetComponent<BaseStats>().CalculateLevel(experiencePoint);

        }
    }

}