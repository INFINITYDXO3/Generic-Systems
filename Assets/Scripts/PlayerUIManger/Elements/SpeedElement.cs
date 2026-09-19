using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedElement : PlayerUIElement
{
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private Image speedometer;

    private float value;

    public override void SetValue<T>(T value)
    {
        if(value is float speed)
        {
            this.value = speed;
            UpdateUI();
        }    
    }

    protected override void UpdateUI()
    {
        speedText.text = value.ToString();
        speedometer.fillAmount = value/35;
    }

    protected override void Awake()
    {
        SetElementType(ElementType.Speed);
    }
}
