using System.Collections.Generic;
using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class SupplyDropManager : MonoBehaviour
    {
        //only one supplydropmanager should exist
        public static SupplyDropManager supplyDropManagerInstance;

        [Header("Supply Drop")]

        [Tooltip("Prefab of the Supply Drop. Should have a SupplyDrop component attached.")]
        [SerializeField] private GameObject supplyDropPrefab;

        [Tooltip("Used to create the Plane.")]
        [SerializeField] private BRS_PlanePathManager planePathManager;

        [Header("Spawn Settings")]

        [Tooltip("Should Supply Drops be Deployed on a random timeline? If false, relies on outside code.")]
        [SerializeField] private bool randomTimedDrops = true;

        [Tooltip("This many seconds will pass before the first supply drop is sent.")]
        [SerializeField] private int initialDelayInSeconds = 60;

        [Tooltip("Wait at least this long before queing next Supply Drop")]
        [SerializeField] private int minSpawnTime = 30;

        [Tooltip("Max amount of time between Supply Drops")]
        [SerializeField] private int maxSpawnTime = 60;

        private float nextSupplyDropSpawnTime;//at what time will the next supply drop happen?

        //keep track of the amount of supply drops in the scene;
        private List<SupplyDrop> supplyDropList = new List<SupplyDrop>();

        // Use this for initialization
        void Start()
        {
            InitSingletonPattern(this);

            nextSupplyDropSpawnTime = Time.time + initialDelayInSeconds;

            //weird things happen if max spawn time is less than 2
            if (maxSpawnTime < 2)
            {
                maxSpawnTime = 2;
            }
        }

        /// <summary>
        /// Initializes a singleton pattern with this obeject. Destroys this object if singleton already exists.
        /// </summary>
        /// <param name="SDM_instance"></param>
        private static void InitSingletonPattern(SupplyDropManager SDM_instance)
        {
            //this pattern enforces only one of these things ever exists
            if (!supplyDropManagerInstance)
            {
                supplyDropManagerInstance = SDM_instance;
            }
            else
            {
                Destroy(SDM_instance.gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (randomTimedDrops && Time.time > nextSupplyDropSpawnTime)
            {
                DeploySupplyDrop();
                nextSupplyDropSpawnTime += Random.Range(minSpawnTime, maxSpawnTime);
            }
        }

        public void AddSupplyDrop(SupplyDrop newSupplyDrop)
        {
            supplyDropList.Add(newSupplyDrop);
        }

        public void RemoveSupplyDrop(SupplyDrop supplyDrop)
        {
            supplyDropList.Remove(supplyDrop);
        }

        [ContextMenu("DeploySupplyDrop()")]//can call this function from the ComponentMenu (gear icon in the top-right corner of a Component)
        public void DeploySupplyDrop()
        {
            planePathManager.InitPlaneDrop(supplyDropPrefab); //can catch plane manager and track it
        }
    }
}
