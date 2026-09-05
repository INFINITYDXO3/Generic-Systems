using UnityEngine;
using Mirror;

public abstract class Item : NetworkBehaviour, IPickable, IDroppable
{
    protected bool canBePicked;


    public virtual void Drop()
    {
        transform.SetParent(null);
        
        canBePicked = true;
    }

    public virtual void Pickup(Inventory inventory)
    {
        if(!canBePicked) return;

        
        canBePicked = false;
    }

    public abstract void UseItem();
}
