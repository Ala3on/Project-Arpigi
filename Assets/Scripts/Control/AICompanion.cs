using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

// remove this comment
namespace RPG.Control
{
    public class AICompanion : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;
        NavMeshAgent navMeshAgent;
        CombatTarget closestTarget = null;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindWithTag("Player");
        }

        private void Update()
        {
            if (health.IsDead) return;
            if (fighter == null) return;

            if (DistanceToPlayer() > 8)
            {
                fighter.Cancel();
                closestTarget = null;
                mover.StartMoveAction(player.transform.position, 1f);
                return;
            }

            if (closestTarget == null || closestTarget.GetComponent<Health>().IsDead)
            {
                closestTarget = GetTargetInChaseRange();
            }

            if (closestTarget != null && DistanceToPlayer() <= 8)
            {
                fighter.Attack(closestTarget.gameObject);
            }
            else
            {
                mover.StartMoveAction(player.transform.position, 1f);
            }
        }

        /* private CombatTarget GetClosestTarget()
        {
            CombatTarget[] targets = FindObjectsOfType<CombatTarget>();
            CombatTarget closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (CombatTarget target in targets)
            {
                if (target.GetComponent<Health>().IsDead) continue;

                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    closest = target;
                    closestDistance = distance;
                }
            }

            return closest;
        } */

        private CombatTarget GetTargetInChaseRange()
        {
            CombatTarget[] targets = FindObjectsOfType<CombatTarget>();
            CombatTarget closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (CombatTarget target in targets)
            {
                if (target.enabled == false) continue; // skip targets that are not enabled (e.g. friendly NPCs)
                if (target.GetComponent<Health>().IsDead) continue;

                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < closestDistance && distance <= chaseDistance)
                {
                    closest = target;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private float DistanceToPlayer()
        {
            return Vector3.Distance(transform.position, player.transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}