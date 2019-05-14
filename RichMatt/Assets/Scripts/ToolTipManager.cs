using System.Collections.Generic;
using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class ToolTipManager : MonoBehaviour
    {

        private static ToolTipManager toolTipManagerInstance;//for singleton pattern

        [SerializeField] private GameObject skydivePrompt;
        [SerializeField] private GameObject deployParachutePrompt;
        private List<GameObject> toolTips = new List<GameObject>();

        private void InitToolTips()
        {
            //verify that each prompt exists. If it does, add it to the list for tracking
            if (skydivePrompt) toolTips.Add(skydivePrompt);
            if (deployParachutePrompt) toolTips.Add(deployParachutePrompt);

        }

        private void Awake()
        {
            //singleton pattern
            SingletonPattern(this); //ensures only one exists, if any

            //verify and init tooltips
            InitToolTips();

            //all tooltips should start disabled
            DisableAllToolTips();
        }

        // Use this for initialization
        void Start()
        {

        }

        private static void SingletonPattern(ToolTipManager TTM_instance)
        {
            if (!toolTipManagerInstance)
            {
                toolTipManagerInstance = TTM_instance;
            }
            else
            {
                Destroy(TTM_instance.gameObject);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DisableAllToolTips()
        {
            foreach (GameObject tip in toolTips)
            {
                tip.SetActive(false);
            }
        }

        public void ShowToolTip(ToolTipENUM toolTip, bool active)
        {
            switch (toolTip)
            {
                case ToolTipENUM.DEPLOYPARACHUTE:
                    if (deployParachutePrompt) deployParachutePrompt.SetActive(active);
                    break;
                case ToolTipENUM.SKYDIVE:
                    if (skydivePrompt) skydivePrompt.SetActive(active);
                    break;
                default:
                    break;
            }
        }

    }


}
