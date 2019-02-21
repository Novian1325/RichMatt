using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [RequireComponent(typeof(Collider))]
    public class BRS_Interactable : MonoBehaviour
    {
        [Header("---Interactable---")]
        [Tooltip("UI Tooltip Prompt that gets displayed to Player.")]
        [SerializeField] protected GameObject toolTipObject; //protected means derived classes can use it like private

        private BRS_Trackable trackable;
        protected bool playerIsLookingAtObject = false;

        // Use this for initialization
        void Start()
        {
            trackable = gameObject.GetComponent<BRS_Trackable>() as BRS_Trackable; //may or may not exist
        }

        protected virtual void HandleTooltip()
        {
            //toggle tooltip
            if (toolTipObject) ToggleTooltip(playerIsLookingAtObject);
        }

        // Update is called once per frame
        protected void Update()
        {
            //Update must be called from derived class!

            //Handle Tooltips
            HandleTooltip();
            //set to false to verfiy next frame
            playerIsLookingAtObject = false;
        }

        protected void RemoveTrackableFromCompass()
        {
            if (trackable)
            {
                trackable.RemoveTrackable();
            }
        }

        /// <summary>
        /// Base interact method. Sends log to Console if not overridden by derived class.
        /// </summary>
        /// <param name="actor">Object, probably player or AI, that is the actor.</param>
        public virtual void Interact(BRS_InteractionManager actor)
        {
            //this method should probably be overridden by derived class, ie a vehicle should do something that an item does not
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            stringBuilder.Append(actor.gameObject.name);
            stringBuilder.Append(" is interacting with ");
            stringBuilder.Append(this.gameObject.name);

            Debug.Log(stringBuilder.ToString());
        }

        public virtual void PlayerIsLookingAtObject(bool b)
        {
            playerIsLookingAtObject = b;
        }

        public virtual bool GetPlayerIsLookingAtObject()
        {
            return playerIsLookingAtObject;
        }

        public virtual void ToggleTooltip(bool active)
        {
            if (toolTipObject) toolTipObject.SetActive(active);
        }

    }


}
