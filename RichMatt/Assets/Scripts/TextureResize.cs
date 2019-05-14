using UnityEngine;
using System.Collections;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [ExecuteInEditMode]
    public class TextureResize : MonoBehaviour
    {
        public int updatesPerSecond = 5;
        public float tilingX = 50;
        public float tilingY = 10;
        private Transform relativeTransform;
        new private Renderer renderer;
        // Use this for initialization
        void Start()
        {
            Debug.Log("Start", this.gameObject);
            relativeTransform = transform;
            renderer = GetComponent<Renderer>();
            StartCoroutine(UpdateTiling2());
            //UpdateTiling();
        }

        // Update is called once per frame
        void Update()
        {
            //UpdateTiling();//DO NOT LEAVE THIS HERE! ONLY UPDATE TILING WHEN SIZE HAS CHANGED


        }

        private void UpdateTiling()
        {
            Debug.Log("Updating the tiling!", this.gameObject);
            renderer.sharedMaterial.mainTextureScale = new Vector2(tilingX * relativeTransform.lossyScale.x, tilingY);
        }

        private IEnumerator UpdateTiling2()
        {
            while (true)
            {
                UpdateTiling();
                yield return new WaitForSeconds(1 / updatesPerSecond);
            }
        }
    }

}