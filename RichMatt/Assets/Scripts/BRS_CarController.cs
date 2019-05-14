using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_CarController : BRS_Interactable
    {
        [Header("BRSCarController")]
        [Tooltip("Point in space where Player appears when exiting vehicle.")]
        [SerializeField] private GameObject ExitPoint;

        [Tooltip("things like a player sitting on an ATV, players sitting in car seats")]
        [SerializeField] private GameObject Visuals; //things like a player sitting on an ATV, players sitting in car seats

        [SerializeField] private GameObject Player;

        private UnityStandardAssets.Vehicles.Car.CarController CarController;
        private SimpleCarController SCC;
        private BRS_TPController TPCon;
        private BRS_TPCharacter TPChar;
        private UnityStandardAssets.Vehicles.Car.CarUserControl CarUserControl;
        private UnityStandardAssets.Vehicles.Car.CarAudio CarAudio;
        private Transform cameraXform;
        private CameraFollowController CFC;
        private Quaternion PreviousCameraQuaternion;
        private Transform originalParent;
        private Vector3 originalPosition;

        private bool playerInVehicle;
        private BRS_InteractionManager playerIM;

        // Use this for initialization
        void Start()
        {
            if (!Player)
            {
                Player = GameObject.FindGameObjectWithTag("Player");
            }

            if (Visuals) Visuals.SetActive(false);
            cameraXform = Camera.main.transform;
            CFC = cameraXform.GetComponent<CameraFollowController>();
            SCC = this.GetComponent<SimpleCarController>();
            TPCon = Player.GetComponent<BRS_TPController>();
            TPChar = Player.GetComponent<BRS_TPCharacter>();
            //CarController = gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>();
            //CarUserControl = gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarUserControl>();
            //CarAudio = gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarAudio>();

            //CarController.enabled = false;
            //CarUserControl.enabled = false;
            //CarAudio.enabled = false;
            //CameraPoint.SetActive(false);
            CFC.enabled = false;
            SCC.enabled = false;

            //Start the game with the Car turned off
            playerInVehicle = false;

        }

        // Update is called once per frame
        new void Update()
        {
            if (playerInVehicle)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    //get out of vehicle
                    Interact(playerIM);
                }
            }
            
            else
            {
                base.Update();
            }
        }

        override public void Interact(BRS_InteractionManager im)
        {
            playerIM = im;
            if (playerInVehicle)
            {
                im.enabled = true;
                playerIM = null;
                ExitVehicle();
            }
            else
            {
                im.enabled = false;
                EnterVehicle();
            }
        }

        public void EnterVehicle()
        {
            //turn tooltip off
            if (toolTipObject) toolTipObject.SetActive(false);

            playerInVehicle = true;
            originalPosition = cameraXform.transform.localPosition;
            originalParent = cameraXform.transform.parent.transform;
            PreviousCameraQuaternion = cameraXform.localRotation;
            cameraXform.SetParent(this.transform);

            TPCon.TogglePlayerControls(false);
            TPChar.ShowPlayerModel(false);

            //CarController.enabled = true;
            //CarUserControl.enabled = true;
            //CarAudio.enabled = false;
            //CameraPoint.SetActive(true);
            CFC.enabled = true;
            CFC.objectToFollow = this.transform;
            SCC.enabled = true;
            TPCon.enabled = false;
            if (Visuals) Visuals.SetActive(true);
        }

        public void ExitVehicle()
        {
            playerInVehicle = false;

            //Player.SetActive(true);
            Player.transform.position = ExitPoint.transform.position;
            Player.transform.rotation = ExitPoint.transform.rotation;

            //CarController.enabled = false;
            //CarUserControl.enabled = false;
            //CarAudio.enabled = false;
            //CameraPoint.SetActive(false);
            CFC.enabled = false;
            SCC.OnPlayerExit();
            SCC.enabled = false;
            TPCon.TogglePlayerControls(true);
            TPChar.ShowPlayerModel(true);
            if (Visuals) Visuals.SetActive(false);

            cameraXform.SetParent(originalParent);
            cameraXform.localPosition = originalPosition;
            cameraXform.localRotation = PreviousCameraQuaternion;
        }
    }

}
