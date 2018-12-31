using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles;

public class BRSCarController : MonoBehaviour
{
    public GameObject ExitPoint;
    public GameObject CameraPoint;
    private UnityStandardAssets.Vehicles.Car.CarController CarController;
    private SimpleCarController SCC;
    private BRS_TPController TPCon;
    private BRS_TPCharacter TPChar;
    private UnityStandardAssets.Vehicles.Car.CarUserControl CarUserControl;
    private UnityStandardAssets.Vehicles.Car.CarAudio CarAudio;
    private Collider CarZone;
    private Camera cam;
    private CameraFollowController CFC;
    private Quaternion PreviousCameraQuaternion;
    private Transform originalParent;
    private Vector3 originalPosition;

    public GameObject Visuals;

    public GameObject Player;

    public bool InCar;
    public bool InVehicleRange;

	// Use this for initialization
	void Start ()
    {
        Visuals.SetActive(false);
        cam = Camera.main;
        CFC = cam.GetComponent<CameraFollowController>();
        SCC = this.GetComponent<SimpleCarController>();
        TPCon = Player.GetComponent<BRS_TPController>();
        TPChar = Player.GetComponent<BRS_TPCharacter>();
        //CarController = gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>();
        //CarUserControl = gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarUserControl>();
        //CarAudio = gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarAudio>();
        //CarZone = gameObject.GetComponent<BoxCollider>();

        //CarController.enabled = false;
        //CarUserControl.enabled = false;
        //CarAudio.enabled = false;
        //CameraPoint.SetActive(false);
        CFC.enabled = false;
        SCC.enabled = false;

        //Start the game with the Car turned off
        InCar = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Interact") && InVehicleRange)
        {
            if (InCar)
            {
                GetOutCar();
            }
            else
            {
                GetInCar();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           InVehicleRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InVehicleRange = false;
        }
    }

    public void GetInCar()
    {
        InCar = true;
        originalPosition = cam.transform.localPosition;
        originalParent = cam.transform.parent.transform;
        PreviousCameraQuaternion = cam.transform.localRotation;
        cam.transform.SetParent(this.transform);

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
        Visuals.SetActive(true);
    }

    public void GetOutCar()
    {
        InCar = false;

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
        Visuals.SetActive(false);

        cam.transform.SetParent(originalParent);
        cam.transform.localPosition = originalPosition;
        cam.transform.localRotation = PreviousCameraQuaternion;
    }
}