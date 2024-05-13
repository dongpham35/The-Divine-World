using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class BuyItemShop : MonoBehaviour
{
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
                StartCoroutine(postIventoryItemTable(itemInInventoryItem.inventoryID, itemInInventoryItem.itemID, itemInInventoryItem.quality));
            }
            else
            {
                itemInInventoryItem = new Inventory_Item();
                itemInInventoryItem.quality = 1;
                itemInInventoryItem.inventoryID = Inventory_Item.Instance.items[0].inventoryID;
                itemInInventoryItem.itemID = item_current_selected.itemID;

                Inventory_Item.Instance.items.Add(itemInInventoryItem);

                StartCoroutine(putInventoryItemTable(itemInInventoryItem.inventoryID, itemInInventoryItem.itemID, itemInInventoryItem.quality));
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
        string url = $"http://192.168.1.4/TheDiVWorld/api/Account?username={username}&gold={gold}";
        using (UnityWebRequest request = UnityWebRequest.Post(url, "POST"))
        {

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                #if UNITY_EDITOR
                EditorUtility.DisplayDialog("Thông báo", request.error, "Ok");
                #endif
            }
            else
            {
                Debug.Log("Cap nhat thanh cong bang Account");
            }
            request.Dispose();
        }
    }
    IEnumerator putInventoryItemTable(int inventoryID, int itemID, int quality)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Inventory_Item?inventoryID={inventoryID}&itemID={itemID}&quality={quality}";
        using (UnityWebRequest request = UnityWebRequest.Put(url, "PUT"))
        {

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Thông báo", request.error, "Ok");
#endif
                
            }
            else
            {
                Debug.Log("Them thanh con bang inventory_item");
            }
            request.Dispose();
        }
    }
    IEnumerator postIventoryItemTable(int inventoryID, int itemID, int quality)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Inventory_Item?inventoryID={inventoryID}&itemID={itemID}&quality={quality}";
        using (UnityWebRequest request = UnityWebRequest.Post(url, "POST"))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Thông báo", request.error, "Ok");
#endif
            }
            else
            {
                Debug.Log("Cap nhat thanh cong bang inventory_item");
            }
            request.Dispose();
        }
    }
}
