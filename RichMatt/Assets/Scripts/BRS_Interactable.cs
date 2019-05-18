using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [RequireComponent(typeof(Collider))]
    public class BRS_Interactable : MonoBehaviour
    {
        [Header("---Interactable---")]
        [Tooltip("UI Tooltip Prompt that gets displayed to Player.")]
        [SerializeField] protected GameObject toolTipObject; //protected means derived classes can use it like private

        /// <summary>
        /// Trackable behavior that may be attached to this gameObject.
        /// </summary>
        private BRS_Trackable trackable;

        /// <summary>
        /// Used to display ToolTip.
        /// </summary>
        protected bool playerIsLookingAtObject = false;

        // Use this for initialization
        void Start()
        {
            trackable = gameObject.GetComponent<BRS_Trackable>() as BRS_Trackable; //may or may not exist
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

        /// <summary>
        /// Turns Tooltip Object on or off depending on it being looked at by the Player.
        /// </summary>
        protected virtual void HandleTooltip()
        {
            //toggle tooltip
            if (toolTipObject) toolTipObject.SetActive(playerIsLookingAtObject);
        }

        /// <summary>
        /// Tells Compass to stop tracking this object
        /// </summary>
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
            var stringBuilder = new System.Text.StringBuilder();

            stringBuilder.Append(actor.gameObject.name);
            stringBuilder.Append(" is interacting with ");
            stringBuilder.Append(this.gameObject.name);

            Debug.Log(stringBuilder.ToString());
        }

        /// <summary>
        /// Tells this object whether or not the Player is pointing at it.
        /// </summary>
        /// <param name="b"></param>
        public virtual void PlayerIsLookingAtObject(bool b)
        {
            playerIsLookingAtObject = b;
        }

        /// <summary>
        /// Whether or not the Player is looking at this Object.
        /// </summary>
        /// <returns></returns>
        public virtual bool GetPlayerIsLookingAtObject()
        {
            return playerIsLookingAtObject;
        }
        
    }//end class declaration
}//end namespace
