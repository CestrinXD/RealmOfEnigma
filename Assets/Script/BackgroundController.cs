using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos, length;
    private float startPosY, height;

    public GameObject cam;
    public float parallaxEffect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // startPos = transform.position.x;
        // length = GetComponent<SpriteRenderer>().bounds.size.x;
        startPos = transform.position.x;
        startPosY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;

    }

    // Update is called once per frame
    void LateUpdate()
{
    float distX = cam.transform.position.x * parallaxEffect;
    float distY = cam.transform.position.y * parallaxEffect;

    float moveX = cam.transform.position.x * (1 - parallaxEffect);
    float moveY = cam.transform.position.y * (1 - parallaxEffect);

    float newX = Mathf.Round((startPos + distX) * 100f) / 100f;
    float newY = Mathf.Round((startPosY + distY) * 100f) / 100f;

    transform.position = new Vector3(newX, newY, transform.position.z);

    if (moveX > startPos + length)
    {
        startPos += length;
    }
    else if (moveX < startPos - length)
    {
        startPos -= length;
    }

    if (moveY > startPosY + height)
    {
        startPosY += height;
    }
    else if (moveY < startPosY - height)
    {
        startPosY -= height;
    }
}

}
