using Assets.Scripts.Models;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

public class BuyItemShop : MonoBehaviour
{
    DatabaseReference databaseReference;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void buyItemInShop()
    {
        var name_item_current_selected = GetComponentInChildren<TMP_Text>().text;
        var item_current_selected = Item.Instance.items.FirstOrDefault(item => item.name.Equals(name_item_current_selected));
        if (item_current_selected.cost <= Account.Instance.gold)
        {
            var itemInInventoryItem = Inventory_Item.Instance.items.FirstOrDefault(item => item.itemID == item_current_selected.itemID);
            if(itemInInventoryItem != null)
            {
                itemInInventoryItem.quality++;
                StartCoroutine(setInventoryItemTable(itemInInventoryItem.inventoryID, itemInInventoryItem.itemID, itemInInventoryItem.quality));
            }
            else
            {
                itemInInventoryItem = new Inventory_Item();
                itemInInventoryItem.quality = 1;
                itemInInventoryItem.inventoryID = Inventory.Instance.inventoryID;
                itemInInventoryItem.itemID = item_current_selected.itemID;

                Inventory_Item.Instance.items.Add(itemInInventoryItem);

                StartCoroutine(setInventoryItemTable(itemInInventoryItem.inventoryID, itemInInventoryItem.itemID, itemInInventoryItem.quality));
            }
            Account.Instance.gold -= item_current_selected.cost;
            StartCoroutine(postAccountTable_gold(Account.Instance.username, Account.Instance.gold));
        }
        else
        {
            #if UNITY_EDITOR
            EditorUtility.DisplayDialog("Thông báo", "Số tiền của bạn không đủ", "Ok");
            #endif

        }

    }

    IEnumerator postAccountTable_gold(string username, int gold)
    {
        var data = new Dictionary<string, object>
        {
            {"level",   Account.Instance.level},
            {"avatar",   Account.Instance.avatar},
            {"password", Account.Instance.password },
            {"email", Account.Instance.email },
            {"gold", gold},
            {"levelID",  Account.Instance.levelID},
            {"class", Account.Instance.classname },
            {"exp",  Account.Instance.experience_points}
        };

        var task = databaseReference.Child("Account").Child(username).SetValueAsync(data);

        yield return new WaitUntil(predicate: () => task.IsCompleted);
    }
    IEnumerator setInventoryItemTable(int inventoryID, int itemID, int quality)
    {
        var data = new Dictionary<string, object>
        {
            {"quality", quality }
        };

        var task = databaseReference.Child("Inventory_Item").Child(inventoryID.ToString()).Child(itemID.ToString()).SetValueAsync(data);
        yield return new WaitUntil(predicate : () => task.IsCompleted);
    }
}
