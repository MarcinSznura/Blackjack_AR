using UnityEngine;
using Vuforia;
using System.Collections;
using System.Collections.Generic;


namespace Vuforia
{
    public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
    {
        [SerializeField] int cardValue;
        [SerializeField] bool countedInThisRound;
        [SerializeField] string cardName;


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
            Debug.Log(" Trackable " +mTrackableBehaviour.TrackableName +" found");
            if (!countedInThisRound)
            {
                countedInThisRound = true;
                FindObjectOfType<GameMaster>().IncreasePlayerScore(cardValue);
            }

            if (cardName == "Joker")
            {
                FindObjectOfType<GameMaster>().ChangeState(5);
                var allCard = FindObjectsOfType<DefaultTrackableEventHandler>();
                foreach (var card in allCard)
                {
                    card.countedInThisRound = false;
                }
                
            }

        }

        private void OnTrackingLost()
        {
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
            //StopAllAudio();
        }

        #endregion // PRIVATE_METHODS
    }
}