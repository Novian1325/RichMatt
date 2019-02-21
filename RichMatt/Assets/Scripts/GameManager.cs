using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class GameManager : MonoBehaviour
    {
        [Header("GameSettings")]
        [Tooltip("Enable to have the Players start in the airplane. Disable to allow them to start on ground.")]
        [SerializeField] private bool startInPlane = false;

        [Tooltip("Enable to have the Players start in the skydiving state up in the air.")]
        [SerializeField] private bool startSkyDiving = false;
        [Tooltip("Players in the game. If not a single player is found, a search will be done for all objects tagged \"Player\" in scene.")]
        [SerializeField] private GameObject[] players;

        [Tooltip("The object that will calculate the randomized flight path.")]
        [SerializeField] private BRS_PlanePathManager planePathManager;

        [Header("SkyDiving")]
        [SerializeField] private SkyDiveHandler skyDiveController;
        [Tooltip("This is the height the Player will start at if \"startSkyDiving\" is true.")]
        [SerializeField] private int skyDiveTestHeight = 500;

        private void Awake()
        {
            VerifyReferences();
        }

        private void DeployPlayersInPlane()
        {
            planePathManager.InitPlaneDrop(players);


        }

        // Use this for initialization
        void Start()
        {

            //populate loot
            //determine mission
            //spawn certain enemies and specific locations


        }

        // Update is called once per frame
        void Update()
        {
            if (startInPlane)
            {
                startInPlane = false;//immediately set flag to false
                DeployPlayersInPlane();
            }

            else if (startSkyDiving)
            {
                startSkyDiving = false;
                //if you're lower than 100 feet, raise it up to a default value
                if (skyDiveController.transform.position.y < skyDiveTestHeight)
                    skyDiveController.transform.position = new Vector3(skyDiveController.transform.position.x,
                        skyDiveTestHeight, skyDiveController.transform.position.z);

                skyDiveController.BeginSkyDive();

            }

        }

        private bool VerifyReferences()
        {
            bool allReferencesOkay = true;
            //verify players
            if (players.Length < 1)//if no players loaded
            {
                players = GameObject.FindGameObjectsWithTag("Player");
                if (players.Length < 1)//if no players tagged
                {

                    Debug.LogError("ERROR! No Players found in scene. Tag one or double check BRS TPC.");
                    allReferencesOkay = false;
                }
                else
                {
                }
            }

            //verify skyDiveController
            if (skyDiveController == null)
            {
                if (players.Length < 1)
                {
                    Debug.LogError("ERROR! cannot auto set skyDiveController without any players.");
                    allReferencesOkay = false;
                }
                else
                {
                    skyDiveController = players[0].GetComponent<SkyDiveHandler>();
                }

            }
            return allReferencesOkay;

        }
    }


}