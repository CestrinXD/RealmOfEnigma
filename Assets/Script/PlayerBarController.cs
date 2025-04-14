using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    // [SerializeField] private Text valueText;

    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    public void SetValueBar(float value, float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = value;

        
    }


}
