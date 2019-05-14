using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_InteractionManager : MonoBehaviour
    {
        [Tooltip("The player's 'reach'. The minimum distance one must be in order to interact with anything.")]
        [SerializeField] private int interactionRaycastLimit = 5;

        private Transform playerCameraXform;

        private BRS_Interactable interactablePlayerIsLookingAt;

        [SerializeField] private bool DEBUG = false;
        // Use this for initialization
        void Start()
        {

            playerCameraXform = Camera.main.transform;

        }

        // Update is called once per frame
        void Update()
        {
            //show tooltip to player if w/n range and looking at model
            HandleToolTipRaycasting();

            //handle interactions
            if (Input.GetButtonDown("Interact"))
            {
                HandleInteraction();

            }

        }

        /// <summary>
        /// Handle calling interaction on object, or not.
        /// </summary>
        private void HandleInteraction()
        {
            if (interactablePlayerIsLookingAt)
            {
                interactablePlayerIsLookingAt.Interact(this);
            }
            else
            {
                if (DEBUG) Debug.Log("Interaction button pressed. Player is not looking at anything interactable.");
            }

        }

        /// <summary>
        /// Shows a tooltip if the player is looking at an Interactable object.
        /// </summary>
        private void HandleToolTipRaycasting()
        {
            interactablePlayerIsLookingAt = WhatIsPlayerLookingAt();

            if (interactablePlayerIsLookingAt)
            {
                //this frame, the player is looking at this object
                interactablePlayerIsLookingAt.PlayerIsLookingAtObject(true);

            }

            else
            {
                //there is nothing nearby for the player to look at
                if (DEBUG) Debug.Log("Player not looking at an Interactable.");
            }

        }

        /// <summary>
        /// Returns a reference to an Interactable object if a raycast can be drawn between the two.
        /// </summary>
        /// <returns>Reference to the Interactable the player is looking at.</returns>
        private BRS_Interactable WhatIsPlayerLookingAt()
        {
            BRS_Interactable targetInteractable = null;

            //check to see if player is looking at interactable object's model
            RaycastHit hitInfo;

            //shoot a raycast from the cameras position forward, store the info, limit the ray to this length, use normal raycast layers, ignore triggerColliders
            if (Physics.Raycast(new Ray(playerCameraXform.position, playerCameraXform.forward), out hitInfo, interactionRaycastLimit, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (DEBUG) Debug.Log("Raycast hit: " + hitInfo.collider.gameObject.name);
                //is the player looking at the item model?
                if (hitInfo.collider.CompareTag("Interactable"))
                {
                    targetInteractable = hitInfo.collider.GetComponent<BRS_Interactable>();
                }
                else
                {
                    if (DEBUG) Debug.Log("Raycast did not hit an Interactable.");
                }
            }
            else
            {
                if (DEBUG) Debug.Log("Raycast hit nothing.");
            }

            return targetInteractable;

        }
    }

}
