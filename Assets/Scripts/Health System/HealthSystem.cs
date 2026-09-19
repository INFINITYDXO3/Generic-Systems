using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    

    private float health;
    public float Health => health;
    public float MaxHealth => maxHealth;

    public bool IsDead => health == 0;

    private void Awake()
    {
        health = maxHealth;
    }


    public void Damage(float damage)
    {
        if(IsDead) return;
        health = Mathf.Clamp(health - damage, 0, health);
    }

    public void Heal(float heal)
    {
        if(IsDead) return;
        health = Mathf.Clamp(health + heal, 0, maxHealth);
    }
    
}
