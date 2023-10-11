using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Attributes
{

    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoint = 0;

        public void GainExperience(float exp)
        {
            experiencePoint += exp;
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


        }
    }

}