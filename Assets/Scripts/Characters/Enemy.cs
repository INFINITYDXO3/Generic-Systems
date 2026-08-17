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
        Vector3 move = target.position - transform.position;
        ProcessMove(new (move.x, move.z));
        if(target != null)
        {
            SetAim(new Aim(transform.position, target.position - transform.position));
            ToggleAttack(true);
        }
    }
}
