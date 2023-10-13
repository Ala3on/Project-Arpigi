using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Attributes
{

    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        private void Start()
        {
            foreground.localScale = new Vector3(1, 1, 1);
            rootCanvas.enabled = false;
        }

        public void HandleHealthChanged()
        {
            float fraction = healthComponent.GetHpFraction();

            foreground.localScale = new Vector3(fraction, 1, 1);

            if (fraction < 1 && fraction > 0)
            {
                rootCanvas.enabled = true;
            }
            else
            {
                rootCanvas.enabled = false;

            }
        }
    }
}
