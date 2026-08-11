using UnityEngine;

public class Enemy : CharactersHandler
{
    Transform target;
    void Start()
    {
        target = FindAnyObjectByType<PlayerHandler>().transform;
    }

    void Update()
    {
        if(target != null)
        {
            SetAim(new Aim(transform.position, target.position - transform.position));
            ToggleAttack(true);
        }
    }
}
