using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Inventory ID: " + Inventory.Instance.inventoryID);
        Debug.Log("items.Coutns: " + Inventory_Item.Instance.items.Count);
        for(int i = 0; i < Inventory_Item.Instance.items.Count; i++)
        {
            Item item = Item.Instance.items.FirstOrDefault(it => it.itemID == Inventory_Item.Instance.items[i].itemID);
            Debug.Log("ItemID: " + item.itemID);
            Debug.Log("Name: " + item.name);
        }
    }
}
