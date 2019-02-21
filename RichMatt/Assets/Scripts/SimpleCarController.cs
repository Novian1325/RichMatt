using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class SimpleCarController : MonoBehaviour
    {
        private float m_horizontalInput;
        private float m_verticalInput;
        private float m_steeringAngle;

        private Rigidbody rb;

        public WheelCollider frontDriverW, frontPassengerW;
        public WheelCollider rearDriverW, rearPassengerW;
        public Transform frontDriverT, frontPassengerT;
        public Transform rearDriverT, rearPassengerT;
        public float maxSteerAngle = 30;
        public float motorForce = 50;

        public void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void GetInput()
        {
            m_horizontalInput = Input.GetAxis("Horizontal");
            m_verticalInput = Input.GetAxis("Vertical");
        }

        private void Steer()
        {
            m_steeringAngle = maxSteerAngle * m_horizontalInput;
            frontDriverW.steerAngle = m_steeringAngle;
            frontPassengerW.steerAngle = m_steeringAngle;
        }

        private void Accelerate()
        {
            frontDriverW.motorTorque = m_verticalInput * motorForce;
            frontPassengerW.motorTorque = m_verticalInput * motorForce;
        }

        private void UpdateWheelPoses()
        {
            UpdateWheelPose(frontDriverW, frontDriverT);
            UpdateWheelPose(frontPassengerW, frontPassengerT);
            UpdateWheelPose(rearDriverW, rearDriverT);
            UpdateWheelPose(rearPassengerW, rearPassengerT);
        }

        private static void UpdateWheelPose(WheelCollider _collider, Transform _transform)
        {
            Vector3 _pos = _transform.position;
            Quaternion _quat = _transform.rotation;

            _collider.GetWorldPose(out _pos, out _quat);

            _transform.position = _pos;
            _transform.rotation = _quat;

        }

        private void FixedUpdate()
        {
            GetInput();
            Steer();
            Accelerate();
            UpdateWheelPoses();
        }

        public void OnPlayerExit()
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            frontDriverW.motorTorque = 0;
            frontPassengerW.motorTorque = 0;
            //maybe also freeze all rb.Constraints to avoid sliding downhill
        }

    }


}
