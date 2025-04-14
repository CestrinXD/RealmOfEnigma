using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float floatUpSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private TextMeshPro textMesh;

    // private TextMeshPro textMesh;
    private Color startColor;

    private void Awake()
    {
        // textMesh = GetComponent<TextMeshPro>();
        // Debug.Log(textMesh);
        startColor = textMesh.color;
    }

    public void Setup(int damageAmount)
    {
        textMesh.text = damageAmount.ToString();
    }

    private void Update()
    {
        transform.position += Vector3.up * floatUpSpeed * Time.deltaTime;

        startColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = startColor;

        if (startColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
