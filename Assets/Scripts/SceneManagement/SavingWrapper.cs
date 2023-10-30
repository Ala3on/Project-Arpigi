using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        [SerializeField] float fadeInTime = 0.5f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(Load());
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        public IEnumerator Load()
        {
            yield return GetComponent<SavingSystem>().Load(defaultSaveFile);
        }
        /* public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        } */

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }
        public void Delete()
        {

            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
