using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class PlayerInPlaneController : MonoBehaviour
    {
        //private Transform _XForm_Camera;
        //private Transform _XForm_Parent;
        private Transform originalPivot;
        private Transform cameraPivot;//saved transform
        private Transform playerTransform;
        private Transform originalParent;//used for moving player in plane
        private Vector3 _LocalRotation;
        private Vector3 cameraStartingPosition;
        private PlaneManager planeManager;


        //private readonly float _CameraDistance = 10f;
        public float MouseSensitivity = 4.0f;
        private readonly float orbitDistance = -50;//must be negative!
                                                   //public float ScrollSensitivity = 2.0f;
                                                   //TODO Add feature to scroll to zoom in and out
        public float OrbitDampening = 10.0f;
        //public float ScrollDampening = 6.0f;
        //public bool CameraDisabled = false;

        //tooltip Manager
        [SerializeField] private ToolTipManager toolTipManager;


        private bool isAllowedToJump = false;

        //references to other components
        private SkyDiveHandler skyDiveController;
        private BRS_TPCharacter playerCharacter;
        private BRS_TPController playerController;
        private Rigidbody rb;

        private void ShowJumpPrompt(bool active)
        {
            if (toolTipManager != null)
            {
                toolTipManager.ShowToolTip(ToolTipENUM.SKYDIVE, active);
            }
            else
            {
                Debug.LogError("ERROR! ToolTip Manager not found in scene.");
            }

        }

        public void OnDropZoneEnter()
        {
            isAllowedToJump = true;//set flag so script will accept player input
            ShowJumpPrompt(true);
            //Debug.Log("GREEN LIGHT! GREEN LIGHT! GO GO GO! JUMP!");
        }

        public void OnEnterPlane(PlaneManager planeMan)
        {
            //initialization method to tell the camera which object to orbit around, turn off other player controls, and other things
            //disable player controls
            this.planeManager = planeMan;//will need to tell plane manager that it wants to jump
            this.cameraPivot = planeManager.GetCameraPivot();//sett the pivot to that of the plane
            this.skyDiveController = GetComponent<SkyDiveHandler>();//get handle on SkyDive controller script
            this.playerCharacter = GetComponent<BRS_TPCharacter>();//get a reference to the character to get at its model
            this.playerController = GetComponent<BRS_TPController>();
            this.playerTransform = this.transform;
            this.originalParent = this.transform.parent;//player will be parented to plane, and then re-parented back here
            this.rb = GetComponent<Rigidbody>();
            this.enabled = true;//make sure it is turned on

            this.skyDiveController.enabled = false;//disable it, as we are not yet skydiving

            //handle player init
            playerCharacter.ShowPlayerModel(false);//make player invisible
            playerController.TogglePlayerControls(false);

            playerTransform.SetParent(planeManager.transform);//set as child to easily handle movement
            playerTransform.localPosition = Vector3.zero;//origin relative to parent
            rb.isKinematic = true;//body will now be moved by the plane (using translation) and not by physical forces
            rb.useGravity = false; //turn off gravity so player doesn't fall out of the sky

            //init camera
            InitCamera();

        }

        private void InitCamera()
        {
            //sets initial distances, rotations, and parents the camera to the plane's pivot point
            Transform cameraTransform = Camera.main.transform;//cache reference

            originalPivot = cameraTransform.parent.transform;//get your parent's transform and cache it for later
            cameraStartingPosition = cameraTransform.localPosition;//cache starting orientation to player 
            cameraTransform.SetParent(planeManager.GetCameraPivot());//change the parent transform to this spot on the plane
            cameraTransform.localRotation = Quaternion.identity;//remove all rotation
                                                                //_LocalRotation = cameraTransform.localEulerAngles;//mayvbe?
            cameraTransform.localPosition = new Vector3(0, 0, orbitDistance);//camera starting position
        }

        private void Start()
        {
            toolTipManager = GameObject.FindGameObjectWithTag("ToolTipManager").GetComponent<ToolTipManager>();

        }

        private void Update()
        {
            if (CrossPlatformInputManager.GetButton("Jump") && isAllowedToJump)
            {
                //JUMP!
                JumpFromPlane();
            }
        }

        public void ForceJump()
        {
            JumpFromPlane();
        }

        private void JumpFromPlane()
        {
            Transform camTrans = Camera.main.transform;

            planeManager.OnPlayerJump(this);//tell the plane that this player has left
            ShowJumpPrompt(false);//disable tooltip UI
            skyDiveController.enabled = true;//turn on skydive controller and let it take control from here
            skyDiveController.BeginSkyDive();//tell player to do animations and stuff for skydiving

            //handle player stuff
            playerTransform.SetParent(originalParent);
            playerTransform.position = planeManager.GetDropSpot().position; // set player to appear at planes location from wherever they were
            playerTransform.rotation = camTrans.rotation;//player faces the same direction camera was facing when in plane
            rb.isKinematic = false;//body will now be controlled by physics forces
            rb.useGravity = true;//turn gravity back on for player
            playerCharacter.ShowPlayerModel(true);//make the player visible again

            //playerController.TogglePlayerControls(true);//normal control does not resume until after skydiving

            camTrans.SetParent(originalPivot);//set parent back to player's pivot
            camTrans.localPosition = cameraStartingPosition;//reset
            camTrans.localRotation = Quaternion.identity;//set rotation to neutral relative to parent
            Destroy(this);//remove this component  //this.enabled = false; //maybe even Destroy(this);

        }

        void LateUpdate()
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
                _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;

                _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, -30f, 90f);

                Quaternion cameraTargetRotation = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
                cameraPivot.rotation = Quaternion.Lerp(cameraPivot.rotation, cameraTargetRotation, Time.deltaTime * OrbitDampening);

                //rotate the player to update minimap facing and show orientation to other players
                Quaternion playerTargetRotation = Quaternion.Euler(0, _LocalRotation.x, 0);
                playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, playerTargetRotation, Time.deltaTime * OrbitDampening);

            }
        }

    }
}
