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
    public Transform cameraPivotTransform;
    private Transform characterTransform;
    [Header("Camera Controls")]
    public float MouseXSensitivity = 4.0f;
    public float MouseYSensitivity = 4.0f;
    public float smoothTime = 5.0f;
    //public float ScrollSensitivity = 2.0f;
    //public float OrbitDampening = 10.0f;
    //public float ScrollDampening = 6.0f;

    [SerializeField] private bool smoothCamera = true;
    private bool clampVerticalRotation = true;
    private readonly float LookMinimumX = -90;
    private readonly float LookMaximumX = 90;

    [Range(5f, 50f)]
    public float FallingDragTuning = 30.0f;

	private Rigidbody rb;
	private float distanceToTerrain;
    private Animator anim;
    private BRS_TPCharacter playerCharacter;
    private BRS_TPController playerController;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    public void InitCameraRotation()
    {
        //save rotation starting values
        characterTransform = this.transform;
        m_CharacterTargetRot = characterTransform.transform.localRotation;
        m_CameraTargetRot = cameraPivotTransform.localRotation;
    }

    private void Awake()
    {
        //gather references
        if(playerCharacter == null) playerCharacter = this.gameObject.GetComponent<BRS_TPCharacter>();
        if(playerController == null) playerController = this.gameObject.GetComponent<BRS_TPController>();

        rb = gameObject.GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
        InitCameraRotation();
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

    public void BeginSkyDive()
    {
        //this is the function that is called from outside the class and starts the freefall framework
        skyDivingState = SkyDivingStateENUM.startFreeFalling;
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

		if (Physics.Raycast(this.transform.position, Vector3.down, out hit, distanceToLanding))
        {
            if (hit.collider.CompareTag("Terrain"))//verify that the ground was hit -- ex. not a parachute right below you
            {
			    distanceToLanding = hit.distance;

            }
		}

        return distanceToLanding;
	}

	private float CalculateDragIncrement()
	{
        float Cosmic;
        Cosmic = 0.025f;
		return (Mathf.Abs(90f - cameraPivotTransform.rotation.eulerAngles.x) * Cosmic) * FallingDragTuning;
	}

    private void RotateView()
    {
        float yRot = Input.GetAxis("Mouse X") * MouseXSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * MouseYSensitivity;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        
    }

    private void HandleCameraMovement()//
    {
        if (smoothCamera)
        {
            characterTransform.localRotation = Quaternion.Slerp(characterTransform.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            characterTransform.localRotation = m_CharacterTargetRot;
            cameraPivotTransform.localRotation = m_CameraTargetRot;
        }
    }//

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
                RotateView();
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

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, LookMinimumX, LookMaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
