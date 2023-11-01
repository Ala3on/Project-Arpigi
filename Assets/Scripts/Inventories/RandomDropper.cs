using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        // CONFIG DATA
        [Tooltip("How far can the pickups be scattered from the dropper.")]
        [SerializeField] float scatterDistance = 1;
        [SerializeField] GuaranteedDrops[] guaranteedDrops;
        [SerializeField] DropLibrary dropLibrary;

        [System.Serializable]
        class GuaranteedDrops
        {
            public InventoryItem item;
            public int number;
        }

        const int ATTEMPTS = 30;

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPTS; i++)
            {

                Vector3 randomPoint = transform.position + UnityEngine.Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;
        }

        // PUBLIC

        public void RandomDrop()
        {
            if (guaranteedDrops.Length > 0)
            {

                foreach (var drop in guaranteedDrops)
                {
                    DropItem(drop.item, drop.number);
                }
            }
            BaseStats baseStats = GetComponent<BaseStats>();
            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.number);
            }
        }

        /* protected override Vector3 GetDropLocation()
        {
            CapsuleCollider collider = GetComponent<CapsuleCollider>();
            Vector3 randomPoint = collider.center;
            randomPoint += RandomPointInCapsule(collider.height, collider.radius);
            return transform.TransformPoint(randomPoint);
        }

        private Vector3 RandomPointInCapsule(float height, float radius)
        {
            Vector3 randomPoint = Vector3.zero;
            float y = UnityEngine.Random.Range(-height / 2, height / 2);
            float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
            float randomRadius = UnityEngine.Random.Range(0, radius);
            randomPoint.y = y;
            randomPoint.x = Mathf.Cos(angle) * randomRadius;
            randomPoint.z = Mathf.Sin(angle) * randomRadius;
            return randomPoint;
        } */
    }
}
