using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
	public Transform followTarget;
	public float smoothSpeed = 10.0f;
	public float smoothRot = 1.0f;
	public Transform followOffset;

	void LateUpdate()
	{
		//Smoothly move to stay at the offset
		Vector3 desiredPosition = followOffset.transform.position;
		Vector3 smoothedPosition = Vector3.Lerp (transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
		transform.position = smoothedPosition;

		//Smoothly rotate to stay "behind"
		Quaternion smoothedRot = Quaternion.Slerp(transform.rotation, followTarget.transform.rotation, smoothRot * Time.deltaTime);
		transform.rotation = smoothedRot;
	}
}
