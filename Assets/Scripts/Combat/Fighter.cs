using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable/* , IModifierProvider */
    {
        [SerializeField] float timeBetweenAttacks = 2f;

        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        // [SerializeField] string defaultWeaponName = "Unarmed";

        Equipment equipment;
        Health target;
        Mover mover;
        float timeSinceLastAttack = 0;
        LazyValue<WeaponConfig> currentWeaponConfig;
        Weapon currentWeapon;

        private void Awake()
        {
            equipment = GetComponent<Equipment>();
            mover = GetComponent<Mover>();
            currentWeaponConfig = new LazyValue<WeaponConfig>(SetupDefaultWeapon);

            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
                equipment.equipmentUpdated += UpdateCompanionWeapon;
            }
        }

        private WeaponConfig SetupDefaultWeapon()
        {
            if (equipment != null)
            {
                UpdateWeapon();
                UpdateCompanionWeapon();
                return currentWeaponConfig.value;
            }

            currentWeapon = AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            currentWeaponConfig.ForceInit();

            timeSinceLastAttack = timeBetweenAttacks;

        }


        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead) return;

            if (!GetIsInRange(target.transform))
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }

        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                GetComponent<Animator>().ResetTrigger("stopAttack");
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0;
            }
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.value.GetWeaponRange();
        }


        public bool CanAttackTaget(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (mover == null) return false;
            if (!mover.CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform))
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead;

        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
            target = null;
            mover.Cancel();
        }

        /*   public IEnumerable<float> GetAdditiveModifiers(Stat stat)
          {
              if (stat == Stat.Attack)
              {
                  yield return currentWeaponConfig.value.GetWeaponDamage();
              }
          }

          public IEnumerable<float> GetPercentageModifiers(Stat stat)
          {
              if (stat == Stat.Attack)
              {
                  yield return currentWeaponConfig.value.GetPercentageDamageBonus();
              }
          } */

        // Animation Event
        public void Hit()
        {
            if (target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Attack);


            if (currentWeaponConfig.value.HasProjectile())
            {
                currentWeaponConfig.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                if (currentWeapon != null)
                {
                    currentWeapon.OnHit();
                }
                target.TakeDamage(gameObject, damage);
            }
        }

        public void Shoot()
        {
            Hit();
        }

        public Health GetTargetHealthComponent()
        {
            return target;
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig.value = weapon;
            if (weapon == null) return;
            currentWeapon = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            WeaponConfig weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }
        // TODO: Refactor this
        private void UpdateCompanionWeapon()
        {
            Fighter companion = GameObject.FindGameObjectWithTag("Companion").GetComponent<Fighter>();
            WeaponConfig weapon = equipment.GetItemInSlot(EquipLocation.DragonWeapon) as WeaponConfig;
            if (weapon == null)
            {
                companion.EquipWeapon(companion.GetDefaultWeapon());
            }
            else
            {
                companion.EquipWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.SpawnWeapon(rightHandTransform, leftHandTransform, animator);
        }
        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig.value;
        }

        public WeaponConfig GetDefaultWeapon()
        {
            return defaultWeapon;
        }

        public JToken CaptureState()
        {
            return JToken.FromObject(currentWeaponConfig.value.name);
        }

        public void RestoreState(JToken state)
        {
            string weaponName = state.ToObject<string>();
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            if (weapon != null)
            {
                EquipWeapon(weapon);
            }
        }




    }
}