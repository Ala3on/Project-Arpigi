using System;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController weaponAnimatorOverrideController = null;
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] Weapon shieldPrefab = null;
        [SerializeField] float range = 2f;
        [SerializeField] float damage = 10f;
        [SerializeField] float percentageDamageBonus = 0;

        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";
        const string shieldName = "Shield";

        public Weapon SpawnWeapon(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;

            if (weaponPrefab != null)
            {
                Transform handTransform = isRightHanded ? rightHand : leftHand;
                weapon = Instantiate(weaponPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }
            if (shieldPrefab != null)
            {
                Weapon shield = Instantiate(shieldPrefab, leftHand);
                shield.gameObject.name = shieldName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (weaponAnimatorOverrideController != null)
            {
                animator.runtimeAnimatorController = weaponAnimatorOverrideController;
            }
            else if (overrideController != null)
            {
                // set to default
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;

        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            Transform oldShield = rightHand.Find(shieldName);

            // Check Right Hand
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon != null)
            {
                oldWeapon.name = "Destroyed";
                Destroy(oldWeapon.gameObject);

            }
            // Check Left
            if (oldShield == null)
            {
                oldShield = leftHand.Find(shieldName);
            }
            if (oldShield != null)
            {
                oldShield.name = "ShieldDestroyed";
                Destroy(oldShield.gameObject);

            }

        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            Projectile projectileInstance = Instantiate(projectile, handTransform.position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetWeaponDamage()
        {
            return damage;
        }

        public float GetPercentageDamageBonus()
        {
            return percentageDamageBonus;
        }

        public float GetWeaponRange()
        {
            return range;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }


    }
}
