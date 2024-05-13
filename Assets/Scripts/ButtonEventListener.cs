using Assets.Scripts.Models;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ButtonEventListener : MonoBehaviour
{
    public void AttachedItem()
    {
        if (Item_Attached.Instance.num_item_attached == 6)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Thông báo", "Không thể mặc thêm đồ", "Ok");
#endif
        }
        else
        {
            for (int i = 0; i < Item_Attached.Instance.item_attacheds.Count; i++)
            {
                if (Item_Attached.Instance.item_attacheds[i] == 0)
                {
                    var itemName_selected = GetComponentInChildren<TMP_Text>().text;
                    var item = Item.Instance.items.FirstOrDefault(i => i.name.Equals(itemName_selected));
                    Inventory_Item item_Selected = Inventory_Item.Instance.items.FirstOrDefault(i => i.itemID == item.itemID);
                    item_Selected.quality--;
                    Item_Attached.Instance.item_attacheds[i] = item.itemID;
                    if (item_Selected.quality == 0)
                    {
                        StartCoroutine(deleteInventoryItemTable(item_Selected.inventoryID, item_Selected.itemID));
                    }
                    else
                    {
                        StartCoroutine(postIventoryItemTable(item_Selected.inventoryID, item_Selected.itemID, item_Selected.quality));
                    }
                    StartCoroutine(postItemAttachedTable(Item_Attached.Instance.username, Item_Attached.Instance.item_attacheds[0],
                        Item_Attached.Instance.item_attacheds[1], Item_Attached.Instance.item_attacheds[2], Item_Attached.Instance.item_attacheds[3]
                        , Item_Attached.Instance.item_attacheds[4], Item_Attached.Instance.item_attacheds[5]));

                    if (item.type.Equals("amor"))
                    {
                        Property.Instance.amor += item.value;
                        StartCoroutine(postPropertyTable_amor(Property.Instance.username, Property.Instance.amor));
                    }
                    else if (item.type.Equals("attack"))
                    {
                        Property.Instance.attack_damage += item.value;
                        StartCoroutine(postPropertyTable_attackdamage(Property.Instance.username, Property.Instance.attack_damage));
                    }
                    else if (item.type.Equals("blood"))
                    {
                        Property.Instance.blood += item.value;
                        StartCoroutine(postPropertyTable_blood(Property.Instance.username, Property.Instance.attack_damage));
                    }
                    else if (item.type.Equals("critical_rate"))
                    {
                        Property.Instance.critical_rate = Mathf.Clamp(Property.Instance.critical_rate + item.value, 0, 100);
                        StartCoroutine(postPropertyTable_criticalrate(Property.Instance.username, Property.Instance.critical_rate));
                    }
                    else if (item.type.Equals("speed"))
                    {
                        Property.Instance.speed += item.value;
                        StartCoroutine(postPropertyTable_speed(Property.Instance.username, Property.Instance.speed));
                    }
                    else if (item.type.Equals("amor_penetraction"))
                    {
                        Property.Instance.amor_penetraction = Mathf.Clamp(Property.Instance.amor_penetraction + item.value, 0, 50);
                        StartCoroutine(postPropertyTable_amorpenetraction(Property.Instance.username, Property.Instance.amor_penetraction));
                    }
                    else if (item.type.Equals("weapon"))
                    {
                        Property.Instance.blood += item.value;
                        Property.Instance.attack_damage += item.value;
                        Property.Instance.amor += item.value;
                        Property.Instance.speed += item.value;
                        Property.Instance.amor_penetraction = Mathf.Clamp(Property.Instance.amor_penetraction + item.value, 0, 50);
                        Property.Instance.critical_rate = Mathf.Clamp(Property.Instance.critical_rate + item.value, 0, 100);
                        StartCoroutine(postPropertyTable(Property.Instance.username, Property.Instance.blood, Property.Instance.attack_damage,
                            Property.Instance.amor, Property.Instance.critical_rate, Property.Instance.speed, Property.Instance.amor_penetraction));
                    }
                    break;
                }
            }
            Item_Attached.Instance.num_item_attached++;
        }

    }

    public void UnAttachedItem()
    {
        string name_slot_selected = gameObject.name;
        int slot_selected = Convert.ToInt16(name_slot_selected[name_slot_selected.Length - 1].ToString());
        
        int itemID_selected = Item_Attached.Instance.item_attacheds[slot_selected-1];
        Item_Attached.Instance.item_attacheds[slot_selected-1] = 0;
        Item_Attached.Instance.num_item_attached--;
        Inventory_Item item = Inventory_Item.Instance.items.FirstOrDefault(i => i.itemID == itemID_selected);
        Item itemSelected = Item.Instance.items.FirstOrDefault(i => i.itemID == itemID_selected);
        if(item != null)
        {
            item.quality++;
            StartCoroutine(postIventoryItemTable(item.inventoryID, item.itemID, item.quality));
            StartCoroutine(postItemAttachedTable(Item_Attached.Instance.username, Item_Attached.Instance.item_attacheds[0],
                Item_Attached.Instance.item_attacheds[1], Item_Attached.Instance.item_attacheds[2],
                Item_Attached.Instance.item_attacheds[3], Item_Attached.Instance.item_attacheds[4], Item_Attached.Instance.item_attacheds[5]));
        }
        else
        {
            item = new Inventory_Item();
            item.itemID = itemID_selected;
            item.inventoryID = Inventory.Instance.inventoryID;
            item.quality = 1;
            Inventory_Item.Instance.items.Add(item);
            StartCoroutine(putInventoryItemTable(item.inventoryID, item.itemID, item.quality));
            StartCoroutine(postItemAttachedTable(Item_Attached.Instance.username, Item_Attached.Instance.item_attacheds[0],
                Item_Attached.Instance.item_attacheds[1], Item_Attached.Instance.item_attacheds[2],
                Item_Attached.Instance.item_attacheds[3], Item_Attached.Instance.item_attacheds[4], Item_Attached.Instance.item_attacheds[5]));
        }

        if (itemSelected.type.Equals("amor"))
        {
            Property.Instance.amor -= itemSelected.value;
            StartCoroutine(postPropertyTable_amor(Property.Instance.username, Property.Instance.amor));
        }
        else if (itemSelected.type.Equals("attack"))
        {
            Property.Instance.attack_damage -= itemSelected.value;
            StartCoroutine(postPropertyTable_attackdamage(Property.Instance.username, Property.Instance.attack_damage));
        }
        else if (itemSelected.type.Equals("blood"))
        {
            Property.Instance.blood -= itemSelected.value;
            StartCoroutine(postPropertyTable_blood(Property.Instance.username, Property.Instance.attack_damage));
        }
        else if (itemSelected.type.Equals("critical_rate"))
        {
            
            Property.Instance.critical_rate = Mathf.Clamp(Property.Instance.critical_rate + itemSelected.value, 0, 100);
            StartCoroutine(postPropertyTable_criticalrate(Property.Instance.username, Property.Instance.critical_rate));
        }
        else if (itemSelected.type.Equals("speed"))
        {
            Property.Instance.speed -= itemSelected.value;
            StartCoroutine(postPropertyTable_speed(Property.Instance.username, Property.Instance.speed));
        }
        else if (itemSelected.type.Equals("amor_penetraction"))
        {
            Property.Instance.amor_penetraction = Mathf.Clamp(Property.Instance.amor_penetraction + itemSelected.value, 0, 50);
            StartCoroutine(postPropertyTable_amorpenetraction(Property.Instance.username, Property.Instance.amor_penetraction));
        }


    }

    
    IEnumerator postItemAttachedTable(string username, int itemID1, int itemID2, int itemID3, int itemID4, int itemID5, int itemID6)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Item_Attached?username={username}&itemID1={itemID1}&itemID2={itemID2}" +
            $"&itemID3={itemID3}&itemID4={itemID4}&itemID5={itemID5}&itemID6={itemID6}";
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
                Debug.Log("Cap nhat thanh cong bang item attached");
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
    IEnumerator deleteInventoryItemTable(int inventoryID, int itemID)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Inventory_Item?inventoryID={inventoryID}&itemID={itemID}";
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
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
                Debug.Log("CXoa thanh cong bang inventory_item");
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

    IEnumerator postPropertyTable_amor(string username,int amor)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Property?username={username}&amor={amor}";
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
                Debug.Log("Cap nhat thanh cong bang Property");
            }
            request.Dispose();
        }
    }

    IEnumerator postPropertyTable_attackdamage(string username, int attack_damage)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Property?username={username}&amor={attack_damage}";
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
                Debug.Log("Cap nhat thanh cong bang Property");
            }
            request.Dispose();
        }
    }

    IEnumerator postPropertyTable_blood(string username, int blood)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Property?username={username}&blood={blood}";
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
                Debug.Log("Cap nhat thanh cong bang Property");
            }
            request.Dispose();
        }
    }
    IEnumerator postPropertyTable_criticalrate(string username, int critical_rate)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Property?username={username}&crittical_rate={critical_rate}";
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
                Debug.Log("Cap nhat thanh cong bang Property");
            }
            request.Dispose();
        }
    }

    IEnumerator postPropertyTable_speed(string username, int speed)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Property?username={username}&speed={speed}";
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
                Debug.Log("Cap nhat thanh cong bang Property");
            }
            request.Dispose();
        }
    }
    IEnumerator postPropertyTable_amorpenetraction(string username, int amor_penetraction)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Property?username={username}&amor_penetraction={amor_penetraction}";
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
                Debug.Log("Cap nhat thanh cong bang Property");
            }
            request.Dispose();
        }
    }

    IEnumerator postPropertyTable(string username, int blood, int attack_damage, int amor, int critical_rate, int speed, int amor_penetraction)
    {
        string url = $"http://192.168.1.4/TheDiVWorld/api/Property?username={username}&blood={blood}&attack_damage={attack_damage}&amor={amor}" +
            $"&critical_rate={critical_rate}&speed={speed}&amor_penetraction={amor_penetraction}";
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
                Debug.Log("Cap nhat thanh cong bang Property");
            }
            request.Dispose();
        }
    }
}
