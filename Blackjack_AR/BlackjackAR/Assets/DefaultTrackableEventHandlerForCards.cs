using UnityEngine;
using Vuforia;
using System.Collections;
using System.Collections.Generic;


namespace Vuforia
{
    public class DefaultTrackableEventHandlerForCards : MonoBehaviour, ITrackableEventHandler
    {
        public int id;
        public bool tracked;

        [Header("Tracking time info")]
        public float presentTime;
        [SerializeField] Coroutine timer;
        

        #region PRIVATE_MEMBER_VARIABLES

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
        }
        #endregion // UNTIY_MONOBEHAVIOUR_METHODS

        #region PUBLIC_METHODS

        public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,TrackableBehaviour.Status newStatus)
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

        IEnumerator CountTime()
        {
            while (true)
            {
                presentTime += Time.deltaTime;
                yield return null;
            }
        }

        private void OnTrackingFound()
        {
            tracked = true;
            timer = StartCoroutine(CountTime());
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
            Debug.Log(" Trackable " +mTrackableBehaviour.TrackableName +" found");
        
        }


        private void OnTrackingLost()
        {
            tracked = false;
            presentTime = 0;
            if (timer != null) StopCoroutine(timer);
            Renderer[] rendererComponents = GetComponentsInChildren <Renderer> (true);
            Collider[] colliderComponents = GetComponentsInChildren < Collider > (true);
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
            Debug.Log(" Trackable " +mTrackableBehaviour.TrackableName
                + " lost ");
        }

        #endregion // PRIVATE_METHODS
    }
}