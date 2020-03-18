using UnityEngine;
using Vuforia;
using System.Collections;
using System.Collections.Generic;


namespace Vuforia
{
    public class internetscropt : MonoBehaviour, ITrackableEventHandler
    {
        //------------Begin Sound----------
        public AudioSource soundTarget;
        public AudioClip clipTarget;
        private AudioSource[] allAudioSources;
        //function to stop all sounds
        void StopAllAudio()
        {
            allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource audioS in allAudioSources)
            {
                audioS.Stop();
            }
        }
        //function to play sound
        void playSound(string ss)
        {
            soundTarget.clip = clipTarget;
            soundTarget.loop = false;

            soundTarget.playOnAwake = false;
            soundTarget.Play();
        }
        //-----------End Sound------------

        #region PRIVATE_MEMBER_VARIABLES

        private TrackableBehaviour mTrackableBehaviour;

        #endregion // PRIVATE_MEMBER_VARIABLES

        #region UNTIY_MONOBEHAVIOUR_METHODS

        void Start()
        {
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this
                );
            }
            soundTarget = (AudioSource)gameObject.AddComponent<AudioSource>();
            clipTarget = (AudioClip)Resources.Load("sounds / audiosample");
        }
        #endregion // UNTIY_MONOBEHAVIOUR_METHODS

        #region PUBLIC_METHODS

        public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }

            else
            {
                OnTrackingLost();
            }
        }
        #endregion // PUBLIC_METHODS

        #region PRIVATE_METHODS

        private void OnTrackingFound()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);
            // Enable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = true;
            }
            // Enable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = true;
            }
            Debug.Log(" Trackable " + mTrackableBehaviour.TrackableName + " found");
            if (mTrackableBehaviour.TrackableName == " Aeroplane ")

            {
                playSound(" sounds / audiosample ");
            }
            if (mTrackableBehaviour.TrackableName == " unitychan ")
            {
                playSound(" sounds / audiosample ");
            }

        }

        private void OnTrackingLost()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);
            // Disable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = false;
            }
            // Disable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = false;
            }
            Debug.Log(" Trackable " + mTrackableBehaviour.TrackableName
                + " lost ");
            StopAllAudio();
        }

        #endregion // PRIVATE_METHODS
    }
}