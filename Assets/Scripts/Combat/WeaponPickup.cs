using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using RPG.Movement;
using UnityEngine;

// TODO: Da rimuovere non più usato

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] WeaponConfig companionWeapon = null;
        [SerializeField] float healtToRestore = 0;
        [SerializeField] float respawnTime = 10;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if (weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healtToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healtToRestore);
            }
            if (companionWeapon != null)
            {
                AICompanion companion = FindObjectOfType<AICompanion>();
                companion.GetComponent<Fighter>().EquipWeapon(companionWeapon);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);
                //Pickup(callingController.GetComponent<Fighter>());
            }
            return true;
        }



        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }


    }
}
