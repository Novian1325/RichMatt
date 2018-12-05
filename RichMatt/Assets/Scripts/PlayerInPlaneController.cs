using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerInPlaneController : MonoBehaviour
{
	//private Transform _XForm_Camera;
	//private Transform _XForm_Parent;
	private Vector3 _LocalRotation;
    private Transform originalPivot;
    private Transform cameraPivot;//saved transform
    private Vector3 cameraStartingPosition;
    private PlaneManager planeManager;

    //private readonly float _CameraDistance = 10f;
    public float MouseSensitivity = 4.0f;
    private float orbitDistance = -50;//must be negative!
	//public float ScrollSensitivity = 2.0f;
    //TODO Add feature to scroll to zoom in and out
	public float OrbitDampening = 10.0f;
    //public float ScrollDampening = 6.0f;
    //public bool CameraDisabled = false;

    private bool isAllowedToJump = false;

    private SkyDiveTesting skyDiveController;
    private BRS_TPCharacter playerCharacter;
    private BRS_TPController playerController;
    
    public void OnDropZoneEnter()
    {
        isAllowedToJump = true;
        //show tooltip UI to player
        Debug.Log("GREEN LIGHT! GREEN LIGHT! GO GO GO! JUMP!");
    }

    public void OnEnterPlane(PlaneManager planeMan)
    {
        //initialization method to tell the camera which object to orbit around, turn off other player controls, and other things
        //disable player controls
        this.planeManager = planeMan;//will need to tell plane manager that it wants to jump
        this.cameraPivot = planeManager.GetCameraPivot();//sett the pivot to that of the plane
        this.skyDiveController = GetComponent<SkyDiveTesting>();//get handle on SkyDive controller script
        this.playerCharacter = GetComponent<BRS_TPCharacter>();//get a reference to the character to get at its model
        this.playerController = GetComponent<BRS_TPController>();
        this.enabled = true;//make sure it is turned on

        this.skyDiveController.enabled = false;//disable it, as we are not yet skydiving
        playerCharacter.ShowPlayerModel(false);//make player invisible
        playerController.TogglePlayerControls(false);

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
        cameraTransform.localPosition = new Vector3(0, 0, orbitDistance);//camera starting position
    }

    private void Update()
    {
        if (CrossPlatformInputManager.GetButton("Jump") && isAllowedToJump){
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
        planeManager.OnPlayerJump(this);
        //disable tooltip UI // redundant -- destroyed along with this
        skyDiveController.enabled = true;//turn on skydive controller and let it take control from here
        skyDiveController.BeginSkyDive();//tell player to do animations and stuff for skydiving
        this.transform.position = planeManager.GetDropSpot(); // set player to appear at planes location from wherever they were
        playerCharacter.ShowPlayerModel(true);//make the player visible again
        //playerController.TogglePlayerControls(true);//normal control does not resume until after skydiving
        Camera.main.transform.SetParent(originalPivot);//set parent back to player's pivot
        Camera.main.transform.localPosition = cameraStartingPosition;//reset
        Camera.main.transform.localRotation = Quaternion.identity;//set rotation to neutral relative to parent
        this.enabled = false; //maybe even Destroy(this);

    }

    void LateUpdate()
	{
		if(Input.GetAxis("Mouse X") !=0 || Input.GetAxis("Mouse Y") !=0)
		{
			_LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
			_LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;

			_LocalRotation.y = Mathf.Clamp(_LocalRotation.y, -30f, 90f);

			Quaternion QT = Quaternion.Euler (_LocalRotation.y, _LocalRotation.x, 0);
			this.cameraPivot.rotation = Quaternion.Lerp (this.cameraPivot.rotation, QT, Time.deltaTime * OrbitDampening);
		}
	}
}