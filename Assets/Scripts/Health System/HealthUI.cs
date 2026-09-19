using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Slider slider;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;

    private float ui_health;
    private Image sliderImage;

    private void Awake()
    {
        if(canvas != null) canvas.worldCamera = Camera.main;
        if(slider != null) sliderImage = slider.fillRect.GetComponent<Image>();
        if(healthSystem != null) SetUIHealth(healthSystem.Health);
    }

    private void LateUpdate()
    {
        if(healthSystem == null) return;

        if(Mathf.Abs(ui_health - healthSystem.Health) > 0.001f)
        {
            SetUIHealth(healthSystem.Health);
        }
    }

    private void SetUIHealth(float health)
    {
        ui_health = health;
        if(slider != null) slider.value = ui_health/healthSystem.MaxHealth;
        if(sliderImage != null) sliderImage.color = Color.Lerp(lowHealthColor, fullHealthColor, ui_health/healthSystem.MaxHealth);
    }
}
