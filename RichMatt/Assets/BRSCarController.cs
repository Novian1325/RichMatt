﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles;

public class BRSCarController : MonoBehaviour
{
    public GameObject ExitPoint;
    public GameObject CameraPoint;
    private UnityStandardAssets.Vehicles.Car.CarController CarController;
    private UnityStandardAssets.Vehicles.Car.CarUserControl CarUserControl;
    private UnityStandardAssets.Vehicles.Car.CarAudio CarAudio;
    private Collider CarZone;

    public GameObject Player;

    public bool InCar;
    public bool Trap;

	// Use this for initialization
	void Start ()
    {
        CarController = gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>();
        CarUserControl = gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarUserControl>();
        CarAudio = gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarAudio>();
        CarZone = gameObject.GetComponent<BoxCollider>();

        CarController.enabled = false;
        CarUserControl.enabled = false;
        CarAudio.enabled = false;
        CameraPoint.SetActive(false);

        //Start the game with the Car turned off
        InCar = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(InCar && Input.GetKeyDown("u"))
        {
            GetOutCar();
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == Player.name)
        {
           Debug.Log(other.gameObject);

            GetInCar();
        }
    }

    public void GetInCar()
    {
        InCar = true;

        Player.SetActive(false);

        CarController.enabled = true;
        CarUserControl.enabled = true;
        CarAudio.enabled = false;
        CameraPoint.SetActive(true);
    }

    public void GetOutCar()
    {
        InCar = false;

        Player.SetActive(true);
        Player.transform.position = ExitPoint.transform.position;

        CarController.enabled = false;
        CarUserControl.enabled = false;
        CarAudio.enabled = false;
        CameraPoint.SetActive(false);
    }
}