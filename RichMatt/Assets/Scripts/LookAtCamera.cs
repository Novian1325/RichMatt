using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform target;

        void Start()
        {
            target = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        void Update()
        {
            this.transform.LookAt(new Vector3(target.position.x, target.position.y, target.position.z));
        }
    }

}
