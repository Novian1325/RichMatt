using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextureResize : MonoBehaviour
{
    public float tilingX = 50;
    public float tilingY = 10;
    private Transform relativeTransform;
    new private Renderer renderer;
    // Use this for initialization
    void Start()
    {
        Debug.Log("Start");
        relativeTransform = transform;
        renderer = GetComponent<Renderer>();
        UpdateTiling();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateTiling();//DO NOT LEAVE THIS HERE! ONLY UPDATE TILING WHEN SIZE HAS CHANGED


    }

    private void UpdateTiling()
    {
        Debug.Log("Updating the tiling!");
        renderer.sharedMaterial.mainTextureScale = new Vector2(tilingX * relativeTransform.lossyScale.x, tilingY);
    }
}