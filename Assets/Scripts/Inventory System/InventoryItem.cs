using UnityEngine;

public abstract class Item : MonoBehaviour, IPickable, IDroppable
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
