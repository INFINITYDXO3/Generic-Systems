using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthElement : PlayerUIElement
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;   
    [SerializeField] private TMP_Text maxHealthText;
    [SerializeField] private Image healthBox;


    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    
    private HealthSystem value;
    private Color currentColor;
    private Image healthSliderFillImage;

    private float health;
    private float healthPercentage;

    public override void SetValue<T>(T value)
    {
        if(value is HealthSystem healthSystem)
        {
            this.value = healthSystem;            
        }

    }

    private void Update()
    {
        if(value == null) return;
        
        if(health != value.Health || healthSlider.value != healthPercentage)
        {
            health = value.Health;
            healthPercentage = health / value.MaxHealth;

            UpdateUI();
        }
        
    }

    protected override void UpdateUI()
    {
        currentColor = Color.Lerp(lowHealthColor, fullHealthColor, healthPercentage);
        
        healthBox.color = currentColor;
        healthText.color = currentColor;
        if(healthSliderFillImage != null) healthSliderFillImage.color = currentColor;

        healthSlider.value = Mathf.LerpUnclamped(healthSlider.value, healthPercentage, Time.deltaTime * 2);

        healthText.text = value.Health.ToString();
    }

    protected override void Awake()
    {
        SetElementType(ElementType.Health);
        if(healthSlider != null) healthSliderFillImage = healthSlider.fillRect.GetComponent<Image>();
    }
}
