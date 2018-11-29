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
        //the only thing that should be accepting controls is this code
        if(playerController == null)
        {
            Debug.LogError("ERROR!!!!!!!");
        }
        else
        {
            playerController.enabled = false;

        }
        playerCharacter.enabled = false;
        anim.SetBool("SkyDive", true);
        skyDivingState = SkyDivingStateENUM.freeFalling;
        //
    }

    private void FreeFalling()
    {
        // read inputs
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");



    }

    void Update()
    {
        switch (skyDivingState)
        {
            case SkyDivingStateENUM.startFreeFalling:
                StartFreeFalling();
                break;

            case SkyDivingStateENUM.freeFalling:
                ;
                break;

            default:
                break;
        }
    }//end Update()
        //anim.SetBool("SkyDive", false);

        /*
		if (Input.GetAxisRaw ("Mouse Y") > 0)
        {
			//Twist to the right
			var angle = transform.rotation.eulerAngles.x;
			if ((angle < MaxDownAngle) || (angle > 180.0f))
			  transform.Rotate (Vector3.right * RotationSpeed * Time.deltaTime);
		}
        else if (Input.GetAxisRaw ("Mouse Y") < 0)
		{
			//Twist to the left
			var angle = transform.rotation.eulerAngles.x;
			if ((angle > MaxUpAngle) && (angle < 180.0f))
			  transform.Rotate (Vector3.right * -(RotationSpeed * Time.deltaTime));
		}
            transform.Rotate(Vector3.up * (Input.GetAxisRaw("Mouse X") * 10.0f) * Time.deltaTime);
            */
    
        /*
        
        //Skydiving
        //Mouse Left and Right: = rotate character on Y axis
        if (Input.GetAxisRaw("Mouse X") > 0)
        {
            //Twist to the right
                transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }
        else if (Input.GetAxisRaw("Mouse X") < 0)
        {
            //Twist to the left
                transform.Rotate(Vector3.up * -(RotationSpeed * Time.deltaTime));
        }
        
    */

        ////Skydiving
        ////Key left, key right = rotate on the z axis
        //if (Input.GetAxisRaw("Vertical") > 0)
        //{
        //    //Dive
        //    var angle = transform.rotation.eulerAngles.x;
        //    if ((angle < MaxDownAngle) || (angle > 180.0f))
        //        transform.Rotate(Vector3.right * RotationSpeed * Time.deltaTime);
        //}
        //else if (Input.GetAxisRaw("Vertical") < 0)
        //{
        //    //Pull Back
        //    var angle = transform.rotation.eulerAngles.x;
        //    if ((angle < MaxUpAngle) && (angle < 180.0f))
        //        transform.Rotate(Vector3.right * -(RotationSpeed * Time.deltaTime));
        //       // transform.Rotate(Vector3.right * -(RotationSpeed * Time.deltaTime));
        //    //transform.Rotate((Vector3.right.x * -(RotationSpeed * Time.deltaTime)), transform.rotation.y, transform.rotation.z);
        //}

        //transform.Rotate(Vector3.up * (Input.GetAxisRaw("Vertical") * 10.0f) * Time.deltaTime);

        /*


        //Skydiving
        //Key Forward = dive
        //Key backwards = pull back
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            //Twist to the right
            //transform.Rotate(new Vector3(0, 0, (Vector3.forward.z * RotationSpeed)) * Time.deltaTime);
            //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, (Vector3.forward.z * RotationSpeed)) * Time.deltaTime);
            //transform.rotation = Quaternion.Euler(0, 0, 0);
            //transform.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime, Space.Self);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            //Twist to the left
            //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, (Vector3.forward.z * -RotationSpeed)) * Time.deltaTime);
            //transform.Rotate(Vector3.forward * -(RotationSpeed * Time.deltaTime), Space.Self);
            // transform.Rotate(new Vector3(0, 0, (Vector3.forward.z * -RotationSpeed)) * Time.deltaTime);
        }

        */


    //}

	private float GetDistanceToTerrain()
	{
        float distanceToLanding = 999999.0f;
		RaycastHit hit;

		if (Physics.Linecast(cameraPivotTransform.position, Vector3.down, out hit)) {
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

    private void HandleCameraMovement()
    {
        if (Input.GetAxis("Mouse Y") != 0)
        {
            _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;
            _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, -90f, 90f);
            Quaternion QT = Quaternion.Euler(_LocalRotation.y, 0, 0);
            cameraPivotTransform.rotation = Quaternion.Lerp(cameraPivotTransform.rotation, QT, Time.deltaTime * OrbitDampening);
        }
        if(Input.GetAxis("Mouse X") != 0)
        {
            _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
            Quaternion QT = Quaternion.Euler(0, _LocalRotation.x, 0);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, QT, Time.deltaTime * OrbitDampening);
        }
    }

    void FixedUpdate()
	{
        distanceToTerrain = GetDistanceToTerrain();
        switch (skyDivingState)
        {
            case SkyDivingStateENUM.startFreeFalling:
                break;

            case SkyDivingStateENUM.freeFalling:
                HandleCameraMovement();
                FreeFalling();
                break;

            default:
                break;
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
