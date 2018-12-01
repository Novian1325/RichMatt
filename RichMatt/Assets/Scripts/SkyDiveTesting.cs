using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDiveTesting : MonoBehaviour
{
    #region Variables

    public SkyDivingStateENUM skyDivingState = SkyDivingStateENUM.startFreeFalling;

    [Header("SkyDiving Settings")]
    public float SlowDrag = 0.35f;
    public float FallDrag = 0.25f;
    public float SwoopDrag = 0.01f;
    public float ChuteDrag = 0.5f;
    public float ChuteHeight = 100f;
    public float SwoopChangeSpeed = 145f;
    public float rollChangeSpeed = 5f;
    public float ForwardSpeed = 5f;//player moves forward while falling
    private float dragSmooth = 10f;

    private readonly float minSwoopAngle = -15f;
    private readonly float maxSwoopAngle =  30f;
    //private readonly float minSwoopAngle = (-15f / 360f);//deprecated
    //private readonly float maxSwoopAngle = (30f / 360f);//deprecated

    private readonly float minRollRotation = -45f;
    private readonly float maxRollRotation =  45f;
    //private readonly float minRollRotation = (-45f / 360f);
    //private readonly float maxRollRotation = (45f / 360f);

    //private readonly float _CameraDistance = 10f;
    public Transform cameraPivotTransform; //camera look
    private Transform characterTransform;//this object
    public Transform characterSwoopTransform; //used for pitch
    public Transform characterRollAxis;//used for rolling


    [Header("Camera Controls")]
    public float MouseXSensitivity = 4.0f;
    public float MouseYSensitivity = 4.0f;
    public float smoothTime = 5.0f;
    //public float ScrollSensitivity = 2.0f;
    //public float OrbitDampening = 10.0f;
    //public float ScrollDampening = 6.0f;

    //Camera settings
    [SerializeField] private bool smoothCamera = true;
    private readonly bool clampVerticalRotation = true;
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
        if (playerCharacter == null) playerCharacter = this.gameObject.GetComponent<BRS_TPCharacter>();
        if (playerController == null) playerController = this.gameObject.GetComponent<BRS_TPController>();

        rb = gameObject.GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
        InitRotationTransforms();
    }

    void Start()
    {
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
        rb.drag = FallDrag;
        Debug.Log("start freefalling" + rb.drag + " " + FallDrag);
        skyDivingState = SkyDivingStateENUM.freeFalling;
    }

    private void TogglePlayerControls(bool active)
    {
        playerCharacter.enabled = active;
        playerController.enabled = active;
    }

    private void FreeFalling()
    {
        //calculate distance to ground
        //deploy chute if too low
        //increment state


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
        float cameraRotationX = Input.GetAxis("Mouse Y") * MouseYSensitivity;
        float characterRotationX = Input.GetAxis("Vertical") * SwoopChangeSpeed;
        float characterRotationY = Input.GetAxis("Mouse X") * MouseXSensitivity;
        float characterRotationZ = Input.GetAxis("Horizontal") * rollChangeSpeed;

        float charRoll = characterRollAxis.localRotation.z;
        float charSwoop = characterSwoopTransform.localRotation.x;

        //unwind swoop amount
        //if input in deadzone and swoop axis not at identity
        if (System.Math.Abs(characterRotationX) < Mathf.Epsilon && System.Math.Abs(charSwoop) > .01f)
        {
            characterRotationX = charSwoop > 0 ? -1 : 1;
        }

        #region Clamp Roll
        //unwind
        //if input in deadzone and roll axis not at identity
        if (System.Math.Abs(characterRotationZ) < Mathf.Epsilon && System.Math.Abs(charRoll) > .01f)
        {
            characterRotationZ = charRoll > 0 ? -1 : 1;
            
        }
        //else if (charRoll > maxRollRotation)
        //{
        //    Debug.Log(charRoll + " / " + maxRollRotation);
        //    if (characterRotationZ > 0)
        //        characterRotationZ = 0;
        //}
        //else if (charRoll < minRollRotation)
        //{
        //    if (characterRotationZ < 0)
        //        characterRotationZ = 0;
        //}
        #endregion

        //set target rotations for axes
        m_CharacterSwoopTargetRot *= Quaternion.Euler(characterRotationX, 0f, 0f);//pitch
        m_CharacterTargetRot *= Quaternion.Euler(0f, characterRotationY, 0f);//yaw
        m_CharacterRollTargetRot *= Quaternion.Euler(0f, 0f, -characterRotationZ);//roll

        m_CameraTargetRot *= Quaternion.Euler(-cameraRotationX, 0f, 0f);//camera pitch

        //CLAMP 'EM ALL!
        if (clampVerticalRotation)
        {
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot, LookMinimumX, LookMaximumX);
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
        float myRot = characterSwoopTransform.localRotation.x;
        float GoalDrag;

        //Debug.Log(myRot + " : " + minSwoopAngle + " / " + maxSwoopAngle);

        GoalDrag = myRot > 0 ? -SwoopDrag : SlowDrag;
        //Debug.Log(GoalDrag);

        //anglePercent = Mathf.Abs(myRot) / GoalDrag;
        //Debug.Log(anglePercent);

        //rb.drag = Mathf.Lerp(rb.drag, GoalDrag * anglePercent, 1);
        rb.drag = myRot / GoalDrag;
        
        #region clamp drag
        if (myRot > .01f)
        {
            rb.drag = Mathf.Clamp(rb.drag, SwoopDrag, FallDrag);
        }
        else if (myRot < .01f)
        {
            //drag is a negative number here and will always be clamped to fall drag
            rb.drag = Mathf.Clamp(rb.drag, FallDrag, SlowDrag);

        }
        //else
        //{
        //    //rb.drag = -rb.drag * Time.deltaTime;
        //}
        #endregion

        
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
                RotateView();
                HandlePlayerMovement();
                HandleDrag();
                break;

            case SkyDivingStateENUM.startparachute:
                break;

            case SkyDivingStateENUM.parachuting:
                HandleDrag();
                break;

            case SkyDivingStateENUM.startLanded:
                StartLanded();
                break;
            default:
                break;
        }
    }

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