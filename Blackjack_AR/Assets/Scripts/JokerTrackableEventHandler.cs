using UnityEngine;


namespace Vuforia
{
    public class JokerTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
    {

        [SerializeField] bool isTracked;

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

        private void Update()
        {
            if (isTracked)
            {
                JokersCall();
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

        public void JokersCall()
        {
            FindObjectOfType<GameMaster>().SetJokerAppeared(true);
        }

        private void OnTrackingFound()
        {
            isTracked = true;
            //timer = StartCoroutine(CountTime());
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
            isTracked = false;
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