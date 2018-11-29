using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
	//private Transform _XForm_Camera;
	private Transform _XForm_Parent;
	private Vector3 _LocalRotation;

    private Transform playerPivot;//saved transform

	//private readonly float _CameraDistance = 10f;
	public float MouseSensitivity = 4.0f;
	public float ScrollSensitivity = 2.0f;
	public float OrbitDampening = 10.0f;
	public float ScrollDampening = 6.0f;
	//public bool CameraDisabled = false;


	void Start()
	{
        //this._XForm_Camera = this.transform;
        this.playerPivot = this.transform.parent;//start pivoting this
        _XForm_Parent = this.playerPivot;//save this for later
	}

    public void FollowPlane(Transform newPivot)
    {
        _XForm_Parent = newPivot;
        this.enabled = true;
    }

    public void FollowPlayer()
    {
        _XForm_Parent = this.playerPivot;
        this.enabled = false;
    }

	void LateUpdate()
	{
		if(Input.GetAxis("Mouse X") !=0 || Input.GetAxis("Mouse Y") !=0)
		{
			_LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
			_LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;

			_LocalRotation.y = Mathf.Clamp(_LocalRotation.y, -30f, 90f);

			Quaternion QT = Quaternion.Euler (_LocalRotation.y, _LocalRotation.x, 0);
			this._XForm_Parent.rotation = Quaternion.Lerp (this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);
		}
	}
}