using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextureResize : MonoBehaviour
{
    public Transform relativeTransform;

    new private Renderer renderer;
    public float tilingX = 50;
    public float tilingY = 10;
    Material mat;
    // Use this for initialization
    void Start()
    {
        Debug.Log("Start");
        renderer = GetComponent<Renderer>();
        UpdateTiling();
    }

    // Update is called once per frame
    void Update()
    {
       
        

    }

    private void UpdateTiling()
    {
        Debug.Log("Updating the tiling!");
        renderer.sharedMaterial.mainTextureScale = new Vector2(tilingX * relativeTransform.localScale.x, tilingY);
    }
}