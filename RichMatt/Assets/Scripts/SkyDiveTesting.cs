using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDiveTesting : MonoBehaviour
{
    #region Variables

    [SerializeField] private SkyDivingStateENUM skyDivingState = SkyDivingStateENUM.startFreeFalling;
    
    [Header("SkyDiving Settings")]
    [SerializeField] private float slowDrag = 0.6f; //target drag when slowing
    [SerializeField] private float fallDrag = 0.5f;//normal drag
    [SerializeField] private float swoopDrag = 0.01f;//drag when swooping (pitching)
    [SerializeField] private float chuteDrag = 1.2f;// drag while chute is deployed
    [SerializeField] private float rollFactor = .25f;
    [SerializeField] private int forceParachuteHeight = 100; //height at which parachute auto deploys
    [SerializeField] private int deployParachuteLimit = 250; //character must be at least this distance to ground before being able to deploy 'chute
    [SerializeField] private float attitudeChangeSpeed = 5f;//roll, yaw, pitch speed
    [SerializeField] private float forwardMomentum = 10f;//player moves forward while falling not straight down "forward momentum"
    [SerializeField] private float parachuteMometumModifier = .8f;
    [SerializeField] private float terminalVelocity = -20f;//maximum velocity a body can achieve in a freefall state /
    [SerializeField] private float parachuteTerminalVelocityModifier = 1.5f;//maximum velocity a body can achieve in a parachute state /
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

    //component references
    private Rigidbody rb;
    private Animator anim;
    private BRS_TPController playerController;
    [SerializeField] private Parachute Parachute;

    //clamp limits
    private readonly float minSwoopAngle = -15f;
    private readonly float maxSwoopAngle = 85f;
    private readonly float minRollRotation = -45f;
    private readonly float maxRollRotation = 45f;
    private readonly float returnToNeutralSpeed = .5f;//used for unwinding when no input

    //target rotations
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private Quaternion m_CharacterSwoopTargetRot;
    private Quaternion m_CharacterRollTargetRot;

    //momentum stuff
    private float targetForwardMomentum = 0f;

    //camera zoom during parachute deploy and reset after landing
    [SerializeField] private Transform zoomPoint;
    [SerializeField] private Transform cameraTransformBeforeZoom;
    [SerializeField] private float zoomSpeed;
    private float zoomStartTime;
    private float zoomLength;

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
        this.enabled = false;//enable by calling BeginSkyDiving();
       
        //verify developer input
        if (terminalVelocity > 0)
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
        anim.SetBool("OnGround", false);
        rb.drag = fallDrag;
        //Debug.Log("start freefalling" + rb.drag + " " + FallDrag);
        skyDivingState = SkyDivingStateENUM.freeFalling;
    }

    private void FreeFalling()
    {
        //Debug.Log("FreeFalling()");
        CheckForRipCord();
        SetTargetRotations();//get input and do calculations
        HandlePlayerMovement();
        if (PPBRS_Utility.GetDistanceToTerrain(this.transform.position) <= forceParachuteHeight)//pull parachute if too close to ground
            skyDivingState = SkyDivingStateENUM.startparachute;//put in state to pull parachute
    }

    private void StartParachute()
    {
        //Debug.Log("StartParachuting()");
        DeployParachute();
        //if chute pulled, velocity limited further
        skyDivingState = SkyDivingStateENUM.parachuting;
    }

    private void Parachuting()
    {
        //Debug.Log("Parachuting()");
        SetTargetRotations();
        HandlePlayerMovement();//rotate character model;//maybe handle drag differently here?

    }

    private void StartLanded()
    {
        //Debug.Log("StartLanded()");
        //camera zoom stuff
        zoomStartTime = Time.time;//reset zoom timer
        zoomLength = Vector3.Distance(Camera.main.transform.localPosition, cameraTransformBeforeZoom.localPosition);

        Parachute.DestroyParachute();//parachute class handles destroying itself (playing anims, whatever)
        playerController.TogglePlayerControls(true);
        anim.SetBool("OnGround", true);
        anim.SetBool("SkyDive", false);
        anim.SetBool("Parachuting", false);
        skyDivingState = SkyDivingStateENUM.landed;
    }

    private float GetTargetForwardMomentum(float verticalInput)
    {
        //calculate the distance the player will travel forward based on pitch
        float targetFM = 0;
        if (Mathf.Abs(verticalInput) > .01f)
        {
            float maxFM = skyDivingState == SkyDivingStateENUM.parachuting ? forwardMomentum * parachuteMometumModifier : forwardMomentum;

            if (verticalInput > 0)
            {
                //if freefalling, camera pitch affects forward move
                float swoopEffect = (skyDivingState == SkyDivingStateENUM.freeFalling) ? (1 - (PPBRS_Utility.GetPitch(cameraPivotTransform.localRotation) / maxSwoopAngle)) : 1;
                targetFM = Mathf.Lerp(targetForwardMomentum, maxFM * swoopEffect, Time.deltaTime * returnToNeutralSpeed); //if swooping
            }

            else if (skyDivingState == SkyDivingStateENUM.parachuting && verticalInput < 0)
            {
                
                //can move backwards when parachuting
                targetFM = Mathf.Lerp(targetForwardMomentum, -maxFM, Time.deltaTime * returnToNeutralSpeed); //if swooping
            }
        }

        else
        {
            //do the normal stuff
            targetFM = Mathf.Lerp(targetForwardMomentum, 0, Time.deltaTime * returnToNeutralSpeed); //if not swooping
               
        }

        return targetFM;
    }

    private void SetTargetRotations()
    {
        //cache rotations for comparisions
        float camPitch = PPBRS_Utility.GetPitch(cameraPivotTransform.localRotation);
        float charPitch = PPBRS_Utility.GetPitch(characterSwoopTransform.localRotation);
        float charRoll = characterRollAxis.localRotation.z;//cache

        float cameraRotationX = Input.GetAxis("Mouse Y") * MouseYSensitivity;//get camera pitch input
        //float characterRotationX = Input.GetAxis("Vertical") * Vector3.Angle(Camera.main.transform.forward, Vector3.forward);//get swoop input
        float verticalInput = Input.GetAxis("Vertical");
        float characterRotationX = verticalInput * camPitch;//get swoop input //character pitch
        float characterRotationY = Input.GetAxis("Mouse X") * MouseXSensitivity;//get yaw input
        float characterRotationZ = (rollFactor * characterRotationY) * attitudeChangeSpeed;//get roll input, also adding a portion of the yaw input means the char rolls into turns

        
        //restrict rolling to freefalling only, for now.  Can swing when parachuting
        characterRotationZ = (skyDivingState == SkyDivingStateENUM.freeFalling) ? characterRotationZ : 0;//force to zero roll if not skydiving

        ////if the player is swooping, steadily increase forward speed based on how 'level' the player is. if the player is looking straight down, no forward speed; otherwise, steadily return to zero.
        targetForwardMomentum = GetTargetForwardMomentum(verticalInput);

        #region unwind to center if no input
        //unwind swoop amount
        //if input in deadzone and swoop axis not at identity
        if (System.Math.Abs(characterRotationX) < 0.01f && System.Math.Abs(charPitch) > .01f)
        {
            //override input to reset char to zero rotation
            characterRotationX = charPitch > 0 ? -returnToNeutralSpeed : returnToNeutralSpeed;
        }       

        //unwind roll
        //if input in deadzone and roll axis not at identity
        if (System.Math.Abs(characterRotationZ) < Mathf.Epsilon && System.Math.Abs(charRoll) > .01f)
        {
            characterRotationZ = charRoll > 0 ? returnToNeutralSpeed : -returnToNeutralSpeed;
        }
        #endregion

        //set target rotations for axes
        //apply rotation
        m_CharacterRollTargetRot *= Quaternion.Euler(0f, 0f, -characterRotationZ);//roll

        //if parachuting, char cannot pitch forward
        if (skyDivingState == SkyDivingStateENUM.parachuting) characterRotationX = 0;

        m_CharacterSwoopTargetRot = Quaternion.Euler(characterRotationX, 0f, 0f);//pitch

        m_CharacterTargetRot *= Quaternion.Euler(0f, characterRotationY, 0f);//yaw
        //only roll when freefalling, not when 'chute deployed
        if(skyDivingState == SkyDivingStateENUM.freeFalling) m_CharacterRollTargetRot *= Quaternion.Euler(0f, 0f, -characterRotationZ);//roll

        m_CameraTargetRot *= Quaternion.Euler(-cameraRotationX, 0f, 0f);//camera pitch

        //CLAMP 'EM ALL!
        if (clampVerticalRotation)
        {
            m_CameraTargetRot = PPBRS_Utility.ClampRotationAroundXAxis(m_CameraTargetRot, cameraMinPitch, cameraMaxPitch);
        }
        m_CharacterSwoopTargetRot = PPBRS_Utility.ClampRotationAroundXAxis(m_CharacterSwoopTargetRot, 0, maxSwoopAngle);
        m_CharacterRollTargetRot = PPBRS_Utility.ClampRotationAroundZAxis(m_CharacterRollTargetRot, minRollRotation, maxRollRotation);

    }

    private void HandleCameraZoomOut()
    {
        //TODO
        //Camera may need to orbit forward over the canopy when in parachute mode
        //zoom out when chute is deployed
        Transform cameraXform = Camera.main.transform;
        //Transform cameraXform = cameraPivotTransform;
        float journeyedPercent = ((Time.time - zoomStartTime) * zoomSpeed) / zoomLength;
        if (journeyedPercent >= .95f) return; //stop after the camera gets close enough
        cameraXform.position = Vector3.Lerp(cameraXform.position, zoomPoint.position, journeyedPercent);
        //cameraXform.transform.LookAt(cameraTransformBeforeZoom);

    }

    private void HandleCameraZoomIn()
    {
        Transform cameraXform = Camera.main.transform;
        float journeyedPercent = ((Time.time - zoomStartTime) * zoomSpeed) / zoomLength;
        if (journeyedPercent >= .95f){
            this.enabled = false;//TURN OFF DISABLE THIS SCRIPT. CAMERA ZOOM IN IS FINAL THING.
        }

        else
        {
            //Debug.Log(journeyedPercent);
            cameraXform.localPosition = Vector3.Lerp(cameraXform.localPosition, cameraTransformBeforeZoom.localPosition, journeyedPercent);

        }

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

        //move player
        characterTransform.Translate(Vector3.forward * targetForwardMomentum * Time.deltaTime);
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

    private void HandleDrag()
    {
        //convert rotation to angle!

        float currentSwoopAngle = PPBRS_Utility.GetPitch(characterSwoopTransform.localRotation);
        
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
        
        //Debug.Log("State: " + skyDivingState + " drag: " + rb.drag + ", pitch: " + currentSwoopAngle + ", velocity: " + rb.velocity);
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
        switch (skyDivingState)
        {
            case SkyDivingStateENUM.startFreeFalling:
                StartFreeFalling();
                break;

            case SkyDivingStateENUM.freeFalling:
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
                HandleCameraZoomOut();

                break;

            case SkyDivingStateENUM.startLanded:
                break;

            case SkyDivingStateENUM.landed:
                HandleCameraZoomIn();
                break;
            default:
                break;
        }
    }

    private void DeployParachute()
    {
        characterRollAxis.localRotation = Quaternion.identity;//stand straight up and down when 'chute deployed -- no roll
        anim.SetBool("Parachuting", true);
        Parachute.DeployParachute();//tell parachute class to do its thing

        //for zooming camera out
        zoomStartTime = Time.time;//start zooming camera out to see canopy
        zoomLength = Vector3.Distance(Camera.main.transform.position, zoomPoint.position);
    }

    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.CompareTag("Terrain"))
        {
            
            //you've hit the terrain
            //reset rigid body
            rb.velocity = Vector3.zero;
            rb.drag = 0;
            if (skyDivingState != SkyDivingStateENUM.landed) skyDivingState = SkyDivingStateENUM.startLanded;
        }
    }
  
}