using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 6;
        NavMeshAgent navMeshAgent;
        Health health;

        void Awake()
        {
            health = GetComponent<Health>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update()

        {
            navMeshAgent.enabled = !health.IsDead; // permette di passare sopra i cadaveri senza aggirarli
            UpdateAnimator();
        }


        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        struct MoverSaveData
        {
            public JToken position;
            public JToken rotation;
        }

        public JToken CaptureState()
        {
            MoverSaveData data = new MoverSaveData
            {
                position = transform.position.ToToken(),
                rotation = transform.eulerAngles.ToToken()
            };
            return JToken.FromObject(data);
        }

        public void RestoreState(JToken state)
        {
            MoverSaveData data = state.ToObject<MoverSaveData>();
            navMeshAgent.Warp(data.position.ToVector3());
            transform.eulerAngles = data.rotation.ToVector3();
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }


    }
}
