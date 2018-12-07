using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDiveTesting : MonoBehaviour
{
    #region Variables

    public SkyDivingStateENUM skyDivingState = SkyDivingStateENUM.startFreeFalling;

    [Header("SkyDiving Settings")]
    [SerializeField] private float slowDrag = 0.6f; //target drag when slowing
    [SerializeField] private float fallDrag = 0.5f;//normal drag
    [SerializeField] private float swoopDrag = 0.01f;//drag when swooping (pitching)
    [SerializeField] private float chuteDrag = 1.2f;// drag while chute is deployed
    [SerializeField] private float rollFactor = .25f;
    [SerializeField] private int forceParachuteHeight = 100; //height at which parachute auto deploys
    [SerializeField] private int deployParachuteLimit = 250; //character must be at least this distance to ground before being able to deploy 'chute
    [SerializeField] private float attitudeChangeSpeed = 5f;//roll, yaw, pitch speed
    [SerializeField] private float forwardMomentum = 5f;//player moves forward while falling not straight down "forward momentum"
    [SerializeField] private float parachuteStallModifier = 1.3f;//modifies the glide that occurs when pulling back while parachute deployed
    [SerializeField] private float terminalVelocity = -20f;//maximum velocity a body can achieve in a freefall state /
    [SerializeField] private float parachuteTerminalVelocityModifier = 1.5f;//maximum velocity a body can achieve in a parachute state /
    [SerializeField] private Parachute Parachute;
    //MUST BE NEGATIVE! Gets inverted if above 0

    //private readonly float _CameraDistance = 10f;
    public Transform cameraPivotTransform; //camera look
    public Transform characterSwoopTransform; //used for pitch
    public Transform characterRollAxis;//used for rolling
    private Transform characterTransform;//this object used for yaw
    
    [Header("Camera Controls")]
    public float MouseXSensitivity = 4.0f;
    public float MouseYSensitivity = 4.0f;
    public float smoothTime = 5.0f;

    //Camera settings
    [SerializeField] private bool smoothCamera = true;
    private readonly bool clampVerticalRotation = true;
    private readonly float cameraMinPitch = -90;
    private readonly float cameraMaxPitch = 90;

    [Range(5f, 50f)]
    public float FallingDragTuning = 30.0f;
    private float distanceToTerrain;

    //component references
    private Rigidbody rb;
    private Animator anim;
    private BRS_TPController playerController;
    
    //clamp limits
    private readonly float minSwoopAngle = -15f;
    private readonly float maxSwoopAngle = 30f;
    private readonly float minRollRotation = -45f;
    private readonly float maxRollRotation = 45f;

    //target rotations
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private Quaternion m_CharacterSwoopTargetRot;
    private Quaternion m_CharacterRollTargetRot;

    #endregion

    public void InitRotationTransforms()
    {
        //save rotation starting values
        characterTransform = this.transform;
        m_CharacterTargetRot = characterTransform.transform.localRotation;
        m_CameraTargetRot = cameraPivotTransform.localRotation;
        m_CharacterSwoopTargetRot = characterSwoopTransform.localRotation;
        m_CharacterRollTargetRot = characterRollAxis.localRotation;
    }

    private void Awake()
    {
        //gather references
        if (playerController == null) playerController = this.gameObject.GetComponent<BRS_TPController>();

        rb = gameObject.GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
        InitRotationTransforms();
    }

    void Start()
    {
        if(terminalVelocity > 0)
        {
            terminalVelocity *= -1;//invert if above 0
        }
    }

    public void BeginSkyDive()
    {
        //Debug.Log("BEGIN SKYDIVE()!");
        //this is the function that is called from outside the class and starts the freefall framework
        this.enabled = true;//turn this script on so its Update() recurring functions will be called
        skyDivingState = SkyDivingStateENUM.startFreeFalling;
    }

    private void StartFreeFalling()
    {
        //Debug.Log("StartFreeFalling()");
        playerController.TogglePlayerControls(false);//turn off player controls except for skydiving controls

        anim.SetBool("SkyDive", true);
        //anim.SetBool("OnGround", false);
        rb.drag = fallDrag;
        //Debug.Log("start freefalling" + rb.drag + " " + FallDrag);
        skyDivingState = SkyDivingStateENUM.freeFalling;
    }

    private void FreeFalling()
    {
        //limit downward velocity to terminal velocity, or something
        SetTargetRotations();
        HandlePlayerMovement();
        HandleDrag();
        if (PPBRS_Utility.GetDistanceToTerrain(this.transform.position) <= forceParachuteHeight)//pull parachute if too close to ground
            skyDivingState = SkyDivingStateENUM.startparachute;//put in state to pull parachute
    }

    private void StartParachute()
    {
        DeployParachute();
        //if chute pulled, velocity limited further
        skyDivingState = SkyDivingStateENUM.parachuting;
    }

    private void Parachuting()
    {
        SetTargetRotations();
        HandlePlayerMovement();//rotate character model;//maybe handle drag differently here?

    }

    private void StartLanded()
    {
        Parachute.DestroyParachute();//parachute class handles destroying itself (playing anims, whatever)
        playerController.TogglePlayerControls(true);
        anim.SetBool("SkyDive", false);
        skyDivingState = SkyDivingStateENUM.landed;
        this.enabled = false;//this script is finished unless activated again
    }

    private void SetTargetRotations()
    {
        float cameraRotationX = Input.GetAxis("Mouse Y") * MouseYSensitivity;//get camera pitch input
        float characterRotationX = Input.GetAxis("Vertical") * attitudeChangeSpeed;//get swoop input
        float characterRotationY = Input.GetAxis("Mouse X") * MouseXSensitivity;//get yaw input
        float characterRotationZ = (rollFactor * characterRotationY) * attitudeChangeSpeed;//get roll input, also adding a portion of the yaw input means the char rolls into turns

        float charRoll = characterRollAxis.localRotation.z;//cache
        float charSwoop = characterSwoopTransform.localRotation.x;//cache

        //restrict rolling to freefalling only, for now.  Can swing when parachuting
        characterRotationZ = (skyDivingState == SkyDivingStateENUM.freeFalling) ? characterRotationZ : 0;//force to zero roll if not skydiving


        #region unwind to center if no input
        //unwind swoop amount
        //if input in deadzone and swoop axis not at identity
        if (System.Math.Abs(characterRotationX) < Mathf.Epsilon && System.Math.Abs(charSwoop) > .01f)
        {
            characterRotationX = charSwoop > 0 ? -1 : 1;
        }
        
        //unwind roll
        //if input in deadzone and roll axis not at identity
        if (System.Math.Abs(characterRotationZ) < Mathf.Epsilon && System.Math.Abs(charRoll) > .01f)
        {
            characterRotationZ = charRoll > 0 ? .8f : -.8f;
        }
        #endregion

        //only roll if skydiving, otherwise reset rotation
         m_CharacterRollTargetRot *= Quaternion.Euler(0f, 0f, -characterRotationZ);

        //set target rotations for axes
        m_CharacterSwoopTargetRot *= Quaternion.Euler(characterRotationX, 0f, 0f);//pitch
        m_CharacterTargetRot *= Quaternion.Euler(0f, characterRotationY, 0f);//yaw
        //only roll when freefalling, not when 'chute deployed
        if(skyDivingState == SkyDivingStateENUM.freeFalling) m_CharacterRollTargetRot *= Quaternion.Euler(0f, 0f, -characterRotationZ);//roll

        m_CameraTargetRot *= Quaternion.Euler(-cameraRotationX, 0f, 0f);//camera pitch

        //CLAMP 'EM ALL!
        if (clampVerticalRotation)
        {
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot, cameraMinPitch, cameraMaxPitch);
        }
        m_CharacterSwoopTargetRot = ClampRotationAroundXAxis(m_CharacterSwoopTargetRot, minSwoopAngle, maxSwoopAngle);
        m_CharacterRollTargetRot = ClampRotationAroundZAxis(m_CharacterRollTargetRot, minRollRotation, maxRollRotation);
    }

    private void HandlePlayerMovement()
    {
        //move character pitch
        characterSwoopTransform.localRotation = Quaternion.Slerp(characterSwoopTransform.localRotation,
            m_CharacterSwoopTargetRot,
            smoothTime * Time.deltaTime);

        //move character roll
        characterRollAxis.localRotation = Quaternion.Slerp(characterRollAxis.localRotation,
            m_CharacterRollTargetRot,
            smoothTime * Time.deltaTime);
        
        float currentSwoopAngle = GetCurrentSwoopAngle();

        //are we swooping forward or backward (slowing, reeling)? what's the max distance we can go in that direction?
        float localMaxAngle = currentSwoopAngle > 0 ? maxSwoopAngle : minSwoopAngle;

        //if parachuting, pulling back increases forward momentum
        if (skyDivingState == SkyDivingStateENUM.parachuting)
        {
            //pulling back has a different effect than pushing forward
            localMaxAngle = localMaxAngle > 0 ? maxSwoopAngle : -minSwoopAngle;
        }

        //drag varies inversely with swoopAngle: y = k/x.           
        //where x is the ratio of our currentSwoop angle to maxSwoop angle
        //if we swoop a little bit, we want the drag to change a little bit
        float targetForwardMove = 1 + (forwardMomentum * (1 - (currentSwoopAngle / localMaxAngle)));
        //should not be totally zero....

        //if parachuting, pulling back increases forward drastically
        if(skyDivingState == SkyDivingStateENUM.parachuting)
        {
            targetForwardMove *= parachuteStallModifier;//elongate arc when pulling back
        }

        //move character forward a bit
        characterTransform.Translate(Vector3.forward * targetForwardMove * Time.deltaTime);
    }

    private void HandleCameraMovement()
    {
        if (smoothCamera)
        {
            //move whole character (inluding cam) yaw
            characterTransform.localRotation = Quaternion.Slerp(characterTransform.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            //move camera pitch
            cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            characterTransform.localRotation = m_CharacterTargetRot;//yaw
            cameraPivotTransform.localRotation = m_CameraTargetRot;//camera pitch
        }
    }

    private float GetCurrentSwoopAngle()
    {
        Quaternion q = characterSwoopTransform.localRotation;
        //normalize
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        //hooray trigonometry!
        return 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

    }

    private void HandleDrag()
    {
        //convert rotation to angle!

        float currentSwoopAngle = GetCurrentSwoopAngle();
        
        //are we swooping forward or backward (slowing, reeling)? what's the max distance we can go in that direction?
        float localMaxAngle = currentSwoopAngle > 0 ? maxSwoopAngle : -minSwoopAngle;
        
        //drag varies inversely with swoopAngle: y = k/x.           
        //where x is the ratio of our currentSwoop angle to maxSwoop angle
        //if we swoop a little bit, we want the drag to change a little bit
        float targetDrag = fallDrag * (1 - (currentSwoopAngle / localMaxAngle));

        //level out drag if level swoop angle
        targetDrag = Mathf.Abs(currentSwoopAngle) < 1f ? fallDrag : targetDrag;//set to fall drag if no pitch

        //clamp to limits
        targetDrag = Mathf.Clamp(targetDrag, swoopDrag, slowDrag);//clamp

        //modify drag if chute is deployed
        targetDrag = skyDivingState == SkyDivingStateENUM.parachuting ? chuteDrag * targetDrag : targetDrag;

        //set drag; calcs complete
        rb.drag = targetDrag;
        
        //sets velocity cap based on state
        float velocityCap = skyDivingState == SkyDivingStateENUM.parachuting ? terminalVelocity * parachuteTerminalVelocityModifier : terminalVelocity;

        //clamp downward velocity to terminalVelocity
        rb.velocity = rb.velocity.y < velocityCap ? new Vector3(rb.velocity.x, velocityCap, rb.velocity.z) : rb.velocity;
        
        //Debug.Log("State: " + skyDivingState + " drag: " + rb.drag + ", pitch: " + currentSwoopAngle + ", velocity: " + rb.velocity.y);
    }

    private void CheckForRipCord()
    {
        //should only be called in Update()
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Interact"))
        {
            if(PPBRS_Utility.GetDistanceToTerrain(this.transform.position) <= deployParachuteLimit)
            {
                skyDivingState = SkyDivingStateENUM.startparachute;
            }
            else
            {
                //Debug.Log("Cannot deploy parachute: too high above limit");
            }
        }
    }
    
    private void Update()
    {
        switch (skyDivingState)
        {
            case SkyDivingStateENUM.startFreeFalling:
                break;

            case SkyDivingStateENUM.freeFalling:
                FreeFalling();
                break;

            case SkyDivingStateENUM.startparachute:
                StartParachute();
                break;

            case SkyDivingStateENUM.parachuting:
                Parachuting();
                break;

            case SkyDivingStateENUM.startLanded:
                StartLanded();
                break;
            case SkyDivingStateENUM.landed:
            default:
                break;
        }
    }

    private void FixedUpdate()
	{
        distanceToTerrain = PPBRS_Utility.GetDistanceToTerrain(this.transform.position);
        switch (skyDivingState)
        {
            case SkyDivingStateENUM.startFreeFalling:
                StartFreeFalling();
                break;

            case SkyDivingStateENUM.freeFalling:
                CheckForRipCord();
                HandleDrag();
                break;

            case SkyDivingStateENUM.startparachute:
                HandleDrag();
                break;

            case SkyDivingStateENUM.parachuting:
                HandleDrag();
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

            case SkyDivingStateENUM.parachuting:
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
            //reset rigid body
            rb.velocity = Vector3.zero;
            rb.drag = 0;
            skyDivingState = SkyDivingStateENUM.startLanded;
        }
    }
    
    private void DeployParachute()
    {
        characterRollAxis.localRotation = Quaternion.identity;//stand straight up and down when 'chute deployed -- no roll
        anim.SetBool("Parachuting", true);
        Parachute.DeployParachute();
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q, float min, float max)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, min, max);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
    private Quaternion ClampRotationAroundZAxis(Quaternion q, float min, float max)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, min, max);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }

}