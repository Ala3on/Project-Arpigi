using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
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
        Equipment equipment;
        float damageTakenPercentage = 0;

        LazyValue<float> healthPoints;


        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            equipment = GetComponent<Equipment>();
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
            if (equipment) equipment.equipmentUpdated += HandleEquipmentUpdate;
        }
        private void OnDisable()
        {
            baseStats.onLevelUp -= RestoreHp;
            if (equipment) equipment.equipmentUpdated -= HandleEquipmentUpdate;
        }

        private void HandleEquipmentUpdate()
        {
            float maxHp = GetMaxHp();
            float currentHp = maxHp - (maxHp * damageTakenPercentage / 100);
            healthPoints.value = Mathf.Min(currentHp, maxHp);
        }

        public bool IsDead { get { return isDead; } }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            UpdateDamagePercentage();
            if (healthPoints.value <= 0)
            {
                onDie.Invoke(damage);
                if (gameObject.tag != "Player")
                {
                    AwardExperience(instigator);
                }
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

        public void UpdateDamagePercentage()
        {
            damageTakenPercentage = MathF.Min(100 * ((GetMaxHp() - healthPoints.value) / GetMaxHp()), 100);
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
            UpdateDamagePercentage();
        }

        private void RestoreHp()
        {
            healthPoints.value = GetMaxHp();
            UpdateDamagePercentage();

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
        struct HealthSaveData
        {
            public float healthPoints;
            public float damageTakenPercentage;
        }

        public JToken CaptureState()
        {
            HealthSaveData data = new HealthSaveData
            {
                healthPoints = healthPoints.value,
                damageTakenPercentage = damageTakenPercentage
            };
            return JToken.FromObject(data);
        }

        public void RestoreState(JToken state)
        {
            var data = state.ToObject<HealthSaveData>();
            healthPoints.value = data.healthPoints;
            damageTakenPercentage = data.damageTakenPercentage;

            if (healthPoints.value <= 0)
            {
                Die();
            }

        }
    }
}

