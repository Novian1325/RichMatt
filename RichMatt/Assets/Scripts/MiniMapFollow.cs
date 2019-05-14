using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class MiniMapFollow : MonoBehaviour
    {
        [SerializeField] private bool rotateWithPlayer = true;
        [SerializeField] private int miniMapHeight = 1000;
        [SerializeField] private Transform targetTransform;

        private Transform origParent;
        private Quaternion originalRotation;

        // Use this for initialization
        void Start()
        {
            targetTransform = targetTransform == null ? GameObject.FindGameObjectWithTag("Player").transform : targetTransform;
            origParent = transform.parent;
            originalRotation = transform.rotation;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (rotateWithPlayer)
            {
                transform.SetParent(targetTransform);//deprecate this and replace with direct reference to target orientation
                                                     //TODO orient rotation with character's if toggled
            }
            else
            {
                transform.rotation = originalRotation;
                transform.SetParent(origParent);//deprecate this when rotateWithPlayer above integrated
            }

            //move minimap
            transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y + miniMapHeight, targetTransform.position.z);
        }
    }

}
