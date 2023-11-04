using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{

    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] Fighter[] fighters = null;
        [SerializeField] bool activateOnStart = false;

        public void Start()
        {
            Activate(activateOnStart);
        }

        public void Activate(bool shouldActivate)
        {
            foreach (Fighter fighter in fighters)
            {

                CombatTarget combatTarget = fighter.GetComponent<CombatTarget>();
                if (combatTarget != null)
                {
                    combatTarget.enabled = shouldActivate;
                }
                fighter.enabled = shouldActivate;
            }
        }

    }
}
