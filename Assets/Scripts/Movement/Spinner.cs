using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField] float xSpeed = 0f;
        [SerializeField] float ySpeed = 0f;
        [SerializeField] float zSpeed = 0f;
        [SerializeField] int rotationDirection = 1;

        void Update()
        {
            float xValue = Time.deltaTime * xSpeed * rotationDirection;
            float yValue = Time.deltaTime * ySpeed * rotationDirection;
            float zValue = Time.deltaTime * zSpeed * rotationDirection;
            transform.Rotate(xValue, yValue, zValue);
        }
    }
}