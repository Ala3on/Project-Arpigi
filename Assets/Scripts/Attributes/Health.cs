using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] DeathEvent onDie;

        // workaround per usare un valore dinamico passato all'evento su Invoke()
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        [System.Serializable]
        public class DeathEvent : UnityEvent<float>
        {
        }

        bool isDead = false;
        BaseStats baseStats;

        LazyValue<float> healthPoints;


        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        private void Start()
        {
            healthPoints.ForceInit();

        }

        private void OnEnable()
        {
            baseStats.onLevelUp += RestoreHp;
        }
        private void OnDisable()
        {
            baseStats.onLevelUp -= RestoreHp;
        }

        public bool IsDead { get { return isDead; } }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            if (healthPoints.value <= 0)
            {
                onDie.Invoke(damage);
                AwardExperience(instigator);
                Die();
            }
            else
            {
                takeDamage.Invoke(damage);
            }

        }

        public float GetMaxHp()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetPercentageHealth()
        {
            return 100 * healthPoints.value / GetMaxHp();
        }
        public float GetHpFraction()
        {
            return healthPoints.value / GetMaxHp();
        }

        public float GetCurrentHp()
        {
            return healthPoints.value;
        }

        public void Heal(float healtToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healtToRestore, GetMaxHp());
        }

        private void RestoreHp()
        {
            healthPoints.value = GetMaxHp();
        }


        private void Die()
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience[] experiences = FindObjectsOfType<Experience>();
            foreach (Experience experience in experiences)
            {
                experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
            }

        }

        /* private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        } */

        // SAVE DATA

        public JToken CaptureState()
        {
            return JToken.FromObject(healthPoints.value);
        }

        public void RestoreState(JToken state)
        {
            healthPoints.value = state.ToObject<float>();

            if (healthPoints.value <= 0)
            {
                Die();
            }

        }

    }
}