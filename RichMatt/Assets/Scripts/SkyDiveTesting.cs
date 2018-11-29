using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class SkyDiveTesting : MonoBehaviour
{
	[Header("**Level Designer Only**")]
	public float MaxUpAngle = 9.0f;
	public float MaxDownAngle = 45.0f;
	public float ChuteHeight = 100f;
	public float ChuteDrag = 3.65f;
	public float RotationSpeed = 145f;

	[Range(5f, 50f)]
   public float FallingDragTuning = 30.0f;

	private Rigidbody rb;
	private float distanceToTerrain;
	private bool charFalling = true;
    private Animator anim;
    public BRS_TPCharacter playerController;

	void Start ()
	{
		rb = gameObject.GetComponent<Rigidbody> ();
        anim = gameObject.GetComponent<Animator>();

       // rb.AddForce(new Vector3(0,0,1), ForceMode.Impulse);
        //rb.velocity = (transform.forward * 1.5f);

        //normalize these variables - this permits the level Designer to be carefree about setting these
        if (MaxDownAngle > 89.5f)
		  MaxDownAngle = (90 - MaxDownAngle) + 1;
		MaxUpAngle = Mathf.Abs(MaxUpAngle);
		MaxDownAngle = Mathf.Abs(MaxDownAngle);
	}

	void Update ()
	{
        anim.SetBool("SkyDive", charFalling);

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

        //Skydiving
        //Key left, key right = rotate on the z axis
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            //Dive
            var angle = transform.rotation.eulerAngles.x;
            if ((angle < MaxDownAngle) || (angle > 180.0f))
                transform.Rotate(Vector3.right * RotationSpeed * Time.deltaTime);
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            //Pull Back
            var angle = transform.rotation.eulerAngles.x;
            if ((angle < MaxUpAngle) && (angle < 180.0f))
                transform.Rotate(Vector3.right * -(RotationSpeed * Time.deltaTime));
               // transform.Rotate(Vector3.right * -(RotationSpeed * Time.deltaTime));
            //transform.Rotate((Vector3.right.x * -(RotationSpeed * Time.deltaTime)), transform.rotation.y, transform.rotation.z);
        }

        transform.Rotate(Vector3.up * (Input.GetAxisRaw("Vertical") * 10.0f) * Time.deltaTime);

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


    }

	private float GetDistanceToTerrain()
	{
        float distanceToLanding = 999999.0f;
		RaycastHit hit;

		if (Physics.Linecast(transform.position, Vector3.down, out hit)) {
			distanceToLanding = hit.distance;
		}

        return distanceToLanding;
	}

	private float CalculateDragIncrement()
	{
        float Cosmic;
        Cosmic = 0.025f;
		return (Mathf.Abs(90f - transform.rotation.eulerAngles.x) * Cosmic) * FallingDragTuning;
	}

  void FixedUpdate()
	{
        distanceToTerrain = GetDistanceToTerrain();

		if (charFalling)
        {
		  if (distanceToTerrain <= ChuteHeight)
            {
                OpenChute();
			  
		  }
          else
          {
            
		    rb.drag = CalculateDragIncrement();
		  }
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
        charFalling = false;
        //TODO 
        //animateCute
        //change controls
        //
    }
}
