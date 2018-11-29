using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class SkyDiveTesting : MonoBehaviour
{
    public SkyDivingStateENUM skyDivingState = SkyDivingStateENUM.startFreeFalling;

    [Header("**Level Designer Only**")]
	public float MaxUpAngle = 9.0f;
	public float MaxDownAngle = 45.0f;
	public float ChuteHeight = 100f;
	public float ChuteDrag = 3.65f;
	public float RotationSpeed = 145f;
    public float ForwardSpeed = 5f;//player moves forward while falling
    
    //private readonly float _CameraDistance = 10f;
    private Vector3 _LocalRotation = Vector3.zero;//starting 
    public Transform cameraPivotTransform;
    [Header("Camera Controls")]
    public float MouseSensitivity = 4.0f;
    public float ScrolSensitivity = 2.0f;
    public float OrbitDampening = 10.0f;
    public float ScrollDampening = 6.0f;

    [Range(5f, 50f)]
    public float FallingDragTuning = 30.0f;

	private Rigidbody rb;
	private float distanceToTerrain;
    private Animator anim;
    private BRS_TPCharacter playerCharacter;
    private BRS_TPController playerController;

    private void Awake()
    {
        //gather references
        if(playerCharacter == null) playerCharacter = this.gameObject.GetComponent<BRS_TPCharacter>();
        if(playerController == null) playerController = this.gameObject.GetComponent<BRS_TPController>();

        rb = gameObject.GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
    }

    void Start ()
	{
        

       // rb.AddForce(new Vector3(0,0,1), ForceMode.Impulse);
        //rb.velocity = (transform.forward * 1.5f);

  //      //normalize these variables - this permits the level Designer to be carefree about setting these
  //      if (MaxDownAngle > 89.5f)
		//  MaxDownAngle = (90 - MaxDownAngle) + 1;
		//MaxUpAngle = Mathf.Abs(MaxUpAngle);
		//MaxDownAngle = Mathf.Abs(MaxDownAngle);
	}

    private void StartFreeFalling()
    {
        TogglePlayerControls(false);//turn off player controls except for skydiving controls
        anim.SetBool("SkyDive", true);
        skyDivingState = SkyDivingStateENUM.freeFalling;
        //
    }

    private void TogglePlayerControls(bool active)
    {
        playerCharacter.enabled = active;
        playerController.enabled = active;
    }

    private void FreeFalling()
    {
      
        
    }

	private float GetDistanceToTerrain()
	{
        float distanceToLanding = 999999.0f;//just a really long distance
		RaycastHit hit;

		if (Physics.Linecast(cameraPivotTransform.position, Vector3.down, out hit))
        {
			distanceToLanding = hit.distance;
		}

        return distanceToLanding;
	}

	private float CalculateDragIncrement()
	{
        float Cosmic;
        Cosmic = 0.025f;
		return (Mathf.Abs(90f - cameraPivotTransform.rotation.eulerAngles.x) * Cosmic) * FallingDragTuning;
	}

    private void GetCameraMovement()
    {
        _LocalRotation.y = Input.GetAxis("Mouse Y") * MouseSensitivity;
        _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, -90f, 90f);//do not exceed straight up or down
        _LocalRotation.x = Input.GetAxis("Mouse X") * MouseSensitivity;
        
    }

    private void HandleCameraMovement()
    {
        //handle camera pitch
        cameraPivotTransform.Rotate(new Vector3(_LocalRotation.y * RotationSpeed * Time.deltaTime, 0, 0));
        //handle character turn
        this.transform.Rotate(new Vector3(0, _LocalRotation.x * RotationSpeed * Time.deltaTime, 0));
    }

    private void StartLanded()
    {
        //Destroy Parachute
        TogglePlayerControls(true);
        anim.SetBool("SkyDive", false);
        skyDivingState = SkyDivingStateENUM.landed;
    }
    
    private void Update()
    {
        switch (skyDivingState)
        {
            case SkyDivingStateENUM.startFreeFalling:
                StartFreeFalling();
                break;

            case SkyDivingStateENUM.freeFalling:
                GetCameraMovement();
                break;


            case SkyDivingStateENUM.startLanded:
                StartLanded();
                break;
            default:
                break;
        }
    }//end Update()

    private void FixedUpdate()
	{
        distanceToTerrain = GetDistanceToTerrain();
        switch (skyDivingState)
        {
            case SkyDivingStateENUM.startFreeFalling:
                break;

            case SkyDivingStateENUM.freeFalling:
                FreeFalling();
                break;

            default:
                break;
        }
    }

    private void LateUpdate()
    {
        switch (skyDivingState)
        {
            case SkyDivingStateENUM.startFreeFalling:
                break;

            case SkyDivingStateENUM.freeFalling:
                HandleCameraMovement();//move camera after all physics step have completed
                break;

            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Terrain"))
        {
            //you've hit the terrain
            skyDivingState = SkyDivingStateENUM.startLanded;
        }
    }

    //if PLAYERSTATEENUM == freefalling
    //freefall()
    //checkChuteDistance
    //else PLAYERSTATEENUM == parachuting
    //OpenChute()
    //checkdistance for ground 
    //else PLAYERSTATEENUM == grounded
    //disablefalling controls

    private void OpenChute()
    {
        rb.drag = ChuteDrag;
        //charFalling = false;
        //TODO 
        //animateCute
        //change controls
        //
    }
}
