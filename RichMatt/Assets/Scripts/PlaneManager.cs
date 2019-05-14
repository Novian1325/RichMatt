using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class PlaneManager : MonoBehaviour
    {
        [SerializeField] private int airspeed = 100;
        [SerializeField] private GameObject cargo_Supplies;
        [SerializeField] private PlayerInPlaneController[] cargo_Players;
        [SerializeField] private GameObject planeCameraPivot;//tells player camera which object to pivot/orbit around
        [SerializeField] private Transform dropSpot;//where does the player appear when they jump out of the plane?

        //for debugging statements
        [SerializeField] private bool DEBUG = false;
        //to which drop zone is the plane headed?
        private GameObject targetDropZone;

        private static int planeCounter = 0;

        private bool CheckIfPlayerOnBoard()
        {
            //returns true if there is at least one player in cargo
            bool playerIsOnBoard = false;
            if (cargo_Players != null)//if the array has been initialized
            {
                foreach (PlayerInPlaneController player in cargo_Players)//look through array and see
                {
                    if (player != null)//is any slot still active?
                    {
                        playerIsOnBoard = true;//one case makes it true
                    }
                }
            }
            return playerIsOnBoard;
        }

        public Transform GetDropSpot()
        {
            return dropSpot;
        }

        //this is basically the constructor class
        public void InitPlane(GameObject incomingTargetDropZone, GameObject[] incomingPlayers = null, GameObject incomingSupplies = null, int incomingAirSpeed = 100)
        {
            if (DEBUG)
            {
                //gives this plane a name for tracking purposes
                this.gameObject.name = "Plane Number: " + ++planeCounter;
                Debug.Log("Plane: " + this.gameObject.name + " heading towards DZ: " + incomingTargetDropZone);
            }

            //initialize member variables
            this.targetDropZone = incomingTargetDropZone;
            this.airspeed = incomingAirSpeed;

            //tell player to reaact to being placed on plane
            if (incomingPlayers != null && incomingPlayers.Length > 0) ConfigurePlayersOnPlane(incomingPlayers);

            if (incomingSupplies != null) this.cargo_Supplies = incomingSupplies; //TODO if incoming supplies is deleted or cleared, does that affect this.cargo_Supplies?

        }

        private void ConfigurePlayersOnPlane(GameObject[] incomingPlayers)
        {
            cargo_Players = new PlayerInPlaneController[incomingPlayers.Length];
            if (incomingPlayers != null)//if the array has been initialized
            {
                for (int i = 0; i < incomingPlayers.Length; ++i)//look through array and see
                {
                    if (incomingPlayers[i] != null)//is any slot still active?
                    {
                        //add a component to the gameobject and keep a reference to it;
                        cargo_Players[i] = incomingPlayers[i].AddComponent<PlayerInPlaneController>() as PlayerInPlaneController;
                        cargo_Players[i].OnEnterPlane(this);//message player that he is now on an airplane, and the player will handle everything else from there
                                                            //the playerhandler will need a reference to this plane manager to know when it has entered the drop zone
                    }
                }
            }

        }

        public Transform GetCameraPivot()
        {
            return this.planeCameraPivot.transform;
        }
        // Use this for initialization
        void Start()
        {


        }

        // Update is called once per frame
        void Update()
        {
            //move the plane forward using translation
            transform.position += transform.forward * Time.deltaTime * airspeed;

        }

        public void OnPlayerJump(PlayerInPlaneController player)
        {
            //remove playerController from list
            for (int i = 0; i < cargo_Players.Length; ++i)
            {
                if (cargo_Players[i] == player)
                {
                    cargo_Players[i] = null;
                }
            }
        }

        private void DropSupplies()
        {
            if (DEBUG) Debug.Log("GET OUT OF MY PLANE, " + cargo_Supplies.gameObject.name);
            Instantiate(cargo_Supplies, dropSpot.position, this.transform.rotation);
        }

        private void ForceOutPlayers()
        {
            for (int i = 0; i < cargo_Players.Length; ++i)
            {
                if (cargo_Players[i] != null)
                {
                    if (DEBUG) Debug.Log("GET OUT OF MY PLANE, " + cargo_Players[i].gameObject.name);
                    cargo_Players[i].ForceJump();
                }
            }

        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            if (otherCollider.gameObject == targetDropZone)
            {
                if (DEBUG) Debug.Log("Now Entering Target Drop Zone: " + otherCollider.name);
                foreach (PlayerInPlaneController player in cargo_Players)
                {
                    player.OnDropZoneEnter();
                }

                //drop supplies
                if (cargo_Supplies) DropSupplies();
            }
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            //if leaving target dropzone
            if (otherCollider.gameObject == targetDropZone)
            {
                if (DEBUG) Debug.Log("Now Leaving Target Drop Zone: " + otherCollider.name);

                //Force All Players out (if there are any)
                if (this.cargo_Players != null && cargo_Players.Length > 0) ForceOutPlayers();

            }

            //Destroy when leaving boundary
            else if (otherCollider.gameObject.CompareTag("MapBounds"))
            {
                if (DEBUG)
                {
                    System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

                    stringBuilder.Append("Plane ");
                    stringBuilder.Append(this.name);
                    stringBuilder.Append(" leaving boundary. Destroying....");

                    Debug.Log(stringBuilder.ToString());

                    this.gameObject.SetActive(false);
                }
                else Destroy(this.gameObject);
            }

        }
    }


}
