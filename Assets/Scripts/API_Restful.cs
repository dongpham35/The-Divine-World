using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class API_Restful : MonoBehaviour
{
    private string URL = "http://192.168.1.4/TheDiVWorld/api/";

    private void Awake()
    {
        StartCoroutine(getFriend(Account.Instance.username));
        StartCoroutine(getInventory(Account.Instance.username));
        StartCoroutine(getItem());
        StartCoroutine(getProperty(Account.Instance.username));
        StartCoroutine(getSessions(Account.Instance.username));
        StartCoroutine(getUp_level());
        StartCoroutine(getItem_Attached());
    }


    IEnumerator getFriend(string username)
    {
        string url = URL + "Friend?username=" + username;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                if (stats == null)
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "Danh sách rỗng", "Ok");
#endif
                }
                else
                {
                    for(int i=0;i<stats.Count;i++)
                    {
                        Friend f = new Friend();
                        f.friendID = int.Parse(stats[i]["friendID"]);
                        f.username2ID = stats[i]["username2ID"].ToString().Replace('"', ' ').Replace(" ", "");
                        f.username = username;
                        Friend.Instance.friends.Add(f);
                    }
                }

            }
            request.Dispose();
        }

    }

    IEnumerator getInventory(string username)
    {
        string url = URL + "Inventory?username=" + username;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                if (stats == null)
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "Không tìm thấy túi đồ", "Ok");
#endif
                }
                else
                {
                    Inventory.Instance.inventoryID = int.Parse(stats["inventoryID"]);
                    Inventory.Instance.username = stats["username"].ToString().Replace('"', ' ').Replace(" ", "");
                }

            }
            StartCoroutine(getInventory_Item(Inventory.Instance.inventoryID));

            request.Dispose();
        }

    }

    IEnumerator getInventory_Item(int inventoryID)
    {

        string url = URL + "Inventory_Item?inventoryID=" + inventoryID;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                if (stats == null)
                {
                }
                else
                {
                    for(int i = 0; i< stats.Count; i++)
                    {
                        Inventory_Item inventory_Item = new Inventory_Item();
                        inventory_Item.inventoryID = inventoryID;
                        inventory_Item.itemID = int.Parse(stats[i]["itemID"]);
                        inventory_Item.quality = int.Parse(stats[i]["quality"]);
                        Inventory_Item.Instance.items.Add(inventory_Item);
                    }
                }

            }
            request.Dispose();
        }
    }

    IEnumerator getItem()
    {
        string url = URL + "Item";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                if (stats == null)
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "Lỗi", "Ok");
#endif
                }
                else
                {
                    for (int i = 0; i < stats.Count; i++)
                    {
                        Item item = new Item();
                        item.itemID = int.Parse(stats[i]["itemID"]);
                        item.image = stats[i]["image"].ToString().Replace('"', ' ').Replace(" ", "");
                        item.name = stats[i]["name"].ToString().Replace('"', ' ').Replace(" ", "");
                        item.description = stats[i]["description"].ToString().ToString().Replace('"', ' ').Replace(" ", "");
                        item.type = stats[i]["type"].ToString().Replace('"', ' ').Replace(" ", "");
                        item.value = int.Parse(stats[i]["value"]);
                        item.cost = int.Parse(stats[i]["cost"]);
                        Item.Instance.items.Add(item);
                    }
                }

            }
            request.Dispose();
        }
    }

    IEnumerator getProperty(string username)
    {
        string url = URL + "Property?username=" + username;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                if (stats == null)
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "Lỗi", "Ok");
#endif
                }
                else
                {
                    Property.Instance.propertyID = int.Parse(stats["propertyID"]);
                    Property.Instance.username = username;
                    Property.Instance.blood = int.Parse(stats["blood"]);
                    Property.Instance.attack_damage = int.Parse(stats["attack_damage"]);
                    Property.Instance.amor = int.Parse(stats["amor"]);
                    Property.Instance.critical_rate = int.Parse(stats["critical_rate"]);
                    Property.Instance.speed = int.Parse(stats["speed"]);
                    Property.Instance.amor_penetraction = int.Parse(stats["amor_penetraction"]);
                }

            }
            request.Dispose();
        }
    }

    IEnumerator getSessions(string username)
    {
        string format = "yyyy-MM-ddTHH:mm:ss";
        string url = URL + "Sessions?username=" + username;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                if (stats == null)
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "Lịch sử trống", "Ok");
#endif
                }
                else
                {
                    for(int i = 0; i < stats.Count; i++)
                    {
                        Sessions session = new Sessions();
                        session.sessionID = int.Parse(stats[i]["sessionID"]);
                        session.username = username;
                        session.username2ID = stats[i]["username2ID"].ToString().Replace('"', ' ').Replace(" ", "");
                        string starttime = stats[i]["start_time"].ToString().Replace('"', ' ').Replace(" ", "");
                        session.start_time = DateTime.ParseExact(starttime, format, System.Globalization.CultureInfo.InvariantCulture);
                        string endtime = stats[i]["end_time"].ToString().Replace('"', ' ').Replace(" ", "");
                        session.end_time = DateTime.ParseExact(endtime, format, System.Globalization.CultureInfo.InvariantCulture);
                        session.winnerID = stats[i]["winnerID"].ToString().Replace('"', ' ').Replace(" ", "");
                        Sessions.Instance.sessions.Add(session);
                    }
                }

            }
            request.Dispose();
        }
    }

    IEnumerator getUp_level()
    {
        string url = URL + "Up_level";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                if (stats == null)
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "Lỗi", "Ok");
#endif
                }
                else
                {
                    for (int i = 0; i < stats.Count; i++)
                    {
                        Up_level up_level = new Up_level();
                        up_level.levelID = int.Parse(stats[i]["levelID"]);
                        up_level.@class = stats[i]["class"].ToString().Replace('"', ' ').Replace(" ", "");
                        up_level.blood = int.Parse(stats[i]["blood"]);
                        up_level.attack_damage = int.Parse(stats[i]["attack_damage"]);
                        up_level.amor = int.Parse(stats[i]["amor"]);
                        up_level.speed = int.Parse(stats[i]["speed"]);
                        Up_level.Instance.up_levels.Add(up_level);
                    }
                }

            }
            request.Dispose();
        }
    }

    IEnumerator getItem_Attached()
    {
        string url = URL + "Item_Attached?username=" + Account.Instance.username;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                if (stats == null)
                {
                }
                else
                {
                    Item_Attached.Instance.item_attachedID = int.Parse(stats["item_attachedID"]);
                    if (stats["itemID1"] != null)
                    {
                        Item_Attached.Instance.item_attacheds[0] = int.Parse(stats["itemID1"]);
                    }
                    if (stats["itemID2"] != null)
                    {
                        Item_Attached.Instance.item_attacheds[1] = int.Parse(stats["itemID2"]);
                    }
                    if (stats["itemID3"] != null)
                    {
                        Item_Attached.Instance.item_attacheds[2] = int.Parse(stats["itemID3"]);
                    }
                    if (stats["itemID4"] != null)
                    {
                        Item_Attached.Instance.item_attacheds[3] = int.Parse(stats["itemID4"]);
                    }
                    if (stats["itemID5"] != null)
                    {
                        Item_Attached.Instance.item_attacheds[4] = int.Parse(stats["itemID5"]);
                    }
                    if (stats["itemID6"] != null)
                    {
                        Item_Attached.Instance.item_attacheds[5] = int.Parse(stats["itemID6"]);
                    }
                    Item_Attached.Instance.num_item_attached = Item_Attached.Instance.item_attacheds.Where(i => i != 0f).ToList().Count;
                    Item_Attached.Instance.username = Account.Instance.username;
                }

            }
            request.Dispose();
        }
    }

}
