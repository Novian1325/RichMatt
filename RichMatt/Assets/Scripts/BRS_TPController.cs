using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [RequireComponent(typeof(BRS_TPCharacter))]
    public class BRS_TPController : MonoBehaviour
    {
        //mouse camera stuff
        public float m_MouseOrbitSensitivityX = .5f;
        public float m_MousePitchSensitivityY = 4f;
        public Transform m_CameraPitchPivotTransform;

        //camera target rotations
        private Quaternion m_CameraPitchTargetQuat;
        private Quaternion m_CameraOrbitTargetQuat;

        private BRS_TPCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.


        private void Start()
        {
            //init rotations:
            m_CameraPitchTargetQuat = m_CameraPitchPivotTransform.localRotation;


            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<BRS_TPCharacter>();
        }//end Start()


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            SetCameraPitch();
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            //TODO Integrate strafing!
            float strafe = CrossPlatformInputManager.GetAxis("Horizontal");//keyboard L/R
            float hor = CrossPlatformInputManager.GetAxis("Mouse X");//mouse L/R
            float vert = CrossPlatformInputManager.GetAxis("Vertical");//keyboard F/B
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = vert * m_CamForward + hor * m_Cam.right * m_MouseOrbitSensitivityX + strafe * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = vert * Vector3.forward + hor * Vector3.right;
            }
#if !MOBILE_INPUT
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }

        private void LateUpdate()
        {
            HandleCameraPitch();
        }


        public void TogglePlayerControls(bool active)
        {
            //m_Character.enabled = active;
            this.enabled = active;
        }

        private void HandleCameraPitch()
        {
            //set pitch via SLERP smooth and wavy
            //m_CameraPitchPivotTransform.localRotation = Quaternion.Slerp(m_CameraPitchPivotTransform.localRotation, m_CameraPitchTargetQuat, Time.deltaTime);

            //set Pitch via hard set
            m_CameraPitchPivotTransform.localRotation = m_CameraPitchTargetQuat;
        }

        private void SetCameraPitch()
        {
            float cameraPitch = Input.GetAxis("Mouse Y") * m_MousePitchSensitivityY;//get camera pitch
            m_CameraPitchTargetQuat *= Quaternion.Euler(-cameraPitch, 0f, 0f);//pitch

            //clamp pitch
            m_CameraPitchTargetQuat = ClampPitch(m_CameraPitchTargetQuat);
        }

        private Quaternion ClampPitch(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, -90, 90);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }//end class


}