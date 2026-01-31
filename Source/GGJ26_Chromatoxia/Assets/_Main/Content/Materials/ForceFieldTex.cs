using UnityEngine;

public class ForceFieldScroll : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2(0f, 1f);
    Material mat;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        Vector2 offset = mat.mainTextureOffset;
        offset += scrollSpeed * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}