using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class DoorHandler : BRS_Interactable
    {
        [Header("Door Handler")]

        [Tooltip("Current state the door is in.")]
        [SerializeField] bool doorOpen = false;

        [Tooltip("Can the door open in either direction?")]
        [SerializeField] bool doorOpensBackward = false;

        private Animator animator;//animator attached to this object
        private Transform xform;//cached for performance
        private Vector3 actorDirection;//direction the actor is facing

        // Use this for initialization
        void Start()
        {

            animator = this.GetComponent<Animator>();
            xform = this.transform;//cache for performance

        }

        new private void Update()
        {
            base.Update();

            if (doorOpen)
            {
                OpenDoor();

            }
            else
            {
                CloseDoor();
            }

        }

        public override void Interact(BRS_InteractionManager interactingObject)
        {
            //actual interaction stuff here
            doorOpen = !doorOpen;

            if (doorOpensBackward)
            {
                actorDirection = interactingObject.transform.forward;
            }
        }

        private void CloseDoor()
        {
            animator.SetBool("DoorOpen", false);
        }

        private void OpenDoor()
        {
            //doors always open forwards unless they can be opened from the back, and the player is behind the doors
            animator.SetBool("DoorOpen", true);
            animator.SetBool("OpenBackward", DetermineDoorOpenDirection());
        }

        /// <summary>
        /// Which direction shall the door open?
        /// </summary>
        /// <returns>Whether or not the door is opening backwards.</returns>
        private bool DetermineDoorOpenDirection()
        {
            if (!doorOpensBackward) return false;

            bool openDoorBackwards = true;
            float angleOfPlayerToDoor = Vector3.Angle(actorDirection, xform.forward);

            //is the player standing behind or in front of the door?
            if (angleOfPlayerToDoor < 90)
            {
                openDoorBackwards = false;
            }
            else
            {
                openDoorBackwards = true;
            }

            return openDoorBackwards;
        }

    }


}
