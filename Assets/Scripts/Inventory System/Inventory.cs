using UnityEngine;
using System;
using System.Collections.Generic;
public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> inventoryItems;

    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemRemoved;
    public event Action OnItemsChanged;

    public List<Item> InventoryItems {get => inventoryItems;}

    private void Start()
    {
        OnItemAdded += (item) => OnItemsChanged?.Invoke();
        OnItemRemoved += (item) => OnItemsChanged?.Invoke();

        if(inventoryItems == null) inventoryItems = new ();
        else
        {
            foreach(var item in inventoryItems)
            {
                OnItemAdded?.Invoke(item);
            }
        }
    }

    public void AddItem(Item item)
    {
        inventoryItems.Add(item);
        OnItemAdded?.Invoke(item);
    }
}
