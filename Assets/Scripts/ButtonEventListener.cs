using Assets.Scripts.Models;
using Firebase.Database;
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

    DatabaseReference databaseReference;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void AttachedItem()
    {
        var itemName_selected = GetComponentInChildren<TMP_Text>().text;
        var item = Item.Instance.items.FirstOrDefault(i => i.name.Equals(itemName_selected));
        
        Inventory_Item item_Selected = Inventory_Item.Instance.items.FirstOrDefault(i => i.itemID == item.itemID);
        if (item.type.Equals("weapon"))
        {
            item_Selected.quality--;
            Property.Instance.blood += item.value;
            Property.Instance.attack_damage += item.value;
            Property.Instance.amor += item.value;
            Property.Instance.speed += item.value;
            Property.Instance.amor_penetraction = Mathf.Clamp(Property.Instance.amor_penetraction + item.value, 0, 50);
            Property.Instance.critical_rate = Mathf.Clamp(Property.Instance.critical_rate + item.value, 0, 100);
            StartCoroutine(updatePropertyTable(Property.Instance.username, Property.Instance.blood, Property.Instance.attack_damage,
                Property.Instance.amor, Property.Instance.speed, Property.Instance.critical_rate, Property.Instance.amor_penetraction));
        }
        else
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
                        item_Selected.quality--;
                        Item_Attached.Instance.item_attacheds[i] = item.itemID;
                        StartCoroutine(updateItemAttachedTable(Item_Attached.Instance.username, Item_Attached.Instance.item_attacheds[0],
                            Item_Attached.Instance.item_attacheds[1], Item_Attached.Instance.item_attacheds[2], Item_Attached.Instance.item_attacheds[3]
                            , Item_Attached.Instance.item_attacheds[4], Item_Attached.Instance.item_attacheds[5]));

                        if (item.type.Equals("amor"))
                        {
                            Property.Instance.amor += item.value;
                        }
                        else if (item.type.Equals("attack"))
                        {
                            Property.Instance.attack_damage += item.value;
                        }
                        else if (item.type.Equals("blood"))
                        {
                            Property.Instance.blood += item.value;
                        }
                        else if (item.type.Equals("critical_rate"))
                        {
                            Property.Instance.critical_rate = Mathf.Clamp(Property.Instance.critical_rate + item.value, 0, 100);
                        }
                        else if (item.type.Equals("speed"))
                        {
                            Property.Instance.speed += item.value;
                        }
                        else if (item.type.Equals("amor_penetraction"))
                        {
                            Property.Instance.amor_penetraction = Mathf.Clamp(Property.Instance.amor_penetraction + item.value, 0, 50);
                        }
                        StartCoroutine(updatePropertyTable(Property.Instance.username, Property.Instance.blood, Property.Instance.attack_damage,
                            Property.Instance.amor, Property.Instance.critical_rate, Property.Instance.speed, Property.Instance.amor_penetraction));
                        break;
                    }
                }
                Item_Attached.Instance.num_item_attached++;
            }
        }

        if (item_Selected.quality <= 0)
        {
            Inventory_Item.Instance.items.Remove(item_Selected);
            deleteInventoryItem(item_Selected.inventoryID, item_Selected.itemID);
        }
        else
        {
            StartCoroutine(updateIventoryItemTable(item_Selected.inventoryID, item_Selected.itemID, item_Selected.quality));
        }
    }

    public void UnAttachedItem()
    {
        string name_slot_selected = gameObject.name;
        int slot_selected = Convert.ToInt16(name_slot_selected[name_slot_selected.Length - 1].ToString());
        
        int itemID_selected = Item_Attached.Instance.item_attacheds[slot_selected-1];
        if (itemID_selected == 0) return;
        Item_Attached.Instance.item_attacheds[slot_selected-1] = 0;
        Item_Attached.Instance.num_item_attached--;
        Inventory_Item item = Inventory_Item.Instance.items.FirstOrDefault(i => i.itemID == itemID_selected);
        Item itemSelected = Item.Instance.items.FirstOrDefault(i => i.itemID == itemID_selected);
        if(item != null)
        {
            item.quality++;
            StartCoroutine(updateIventoryItemTable(item.inventoryID, item.itemID, item.quality));
            StartCoroutine(updateItemAttachedTable(Item_Attached.Instance.username, Item_Attached.Instance.item_attacheds[0],
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
            StartCoroutine(updateIventoryItemTable(item.inventoryID, item.itemID, item.quality));
            StartCoroutine(updateItemAttachedTable(Item_Attached.Instance.username, Item_Attached.Instance.item_attacheds[0],
                Item_Attached.Instance.item_attacheds[1], Item_Attached.Instance.item_attacheds[2],
                Item_Attached.Instance.item_attacheds[3], Item_Attached.Instance.item_attacheds[4], Item_Attached.Instance.item_attacheds[5]));
        }

        if (itemSelected.type.Equals("amor"))
        {
            Property.Instance.amor -= itemSelected.value;
        }
        else if (itemSelected.type.Equals("attack"))
        {
            Property.Instance.attack_damage -= itemSelected.value;
        }
        else if (itemSelected.type.Equals("blood"))
        {
            Property.Instance.blood -= itemSelected.value;
        }
        else if (itemSelected.type.Equals("critical_rate"))
        {
            
            Property.Instance.critical_rate = Mathf.Clamp(Property.Instance.critical_rate + itemSelected.value, 0, 100);
        }
        else if (itemSelected.type.Equals("speed"))
        {
            Property.Instance.speed -= itemSelected.value;
        }
        else if (itemSelected.type.Equals("amor_penetraction"))
        {
            Property.Instance.amor_penetraction = Mathf.Clamp(Property.Instance.amor_penetraction + itemSelected.value, 0, 50);
        }
        StartCoroutine(updatePropertyTable(Property.Instance.username, Property.Instance.blood, Property.Instance.attack_damage,
                                    Property.Instance.amor, Property.Instance.critical_rate, Property.Instance.speed, Property.Instance.amor_penetraction));
    }

    
    IEnumerator updateItemAttachedTable(string username, int itemID1, int itemID2, int itemID3, int itemID4, int itemID5, int itemID6)
    {
        var data = new Dictionary<string, object>
        {
            {"itemID1", itemID1 },
            {"itemID2", itemID2 },
            {"itemID3", itemID3 },
            {"itemID4", itemID4 },
            {"itemID5", itemID5 },
            {"itemID6", itemID6 }
        };
        var task = databaseReference.Child("Item_Attached").Child(username).SetValueAsync(data);

        yield return new WaitUntil(predicate: () => task.IsCompleted);
        
    }

    IEnumerator updateIventoryItemTable(int inventoryID, int itemID, int quality)
    {
        var data = new Dictionary<string, object>
        {
            {"quality", quality}
        };

        var task = databaseReference.Child("Inventory_Item").Child(inventoryID.ToString()).Child(itemID.ToString()).SetValueAsync(data);

        yield return new WaitUntil(predicate:()=> task.IsCompleted);
    }


    IEnumerator updatePropertyTable(string username, int blood, int attack, int amor, int speed, int critical_rate, int amor_penetraction)
    {
        var data = new Dictionary<string, object>
        {
            { "blood", blood },
            { "amor", amor },
            { "attack_damage", attack },
            { "speed", speed },
            { "critical_rate", critical_rate },
            { "amor_penetraction", amor_penetraction }
        };

        var task = databaseReference.Child("Property").Child(username).SetValueAsync(data);

        yield return new WaitUntil(predicate:(()=> task.IsCompleted));
    }

    void deleteInventoryItem(int InventoryID, int itemID)
    {
        DatabaseReference nodeRef = databaseReference.Child("Inventory_Item").Child(InventoryID.ToString()).Child(itemID.ToString());
        nodeRef.RemoveValueAsync().ContinueWith(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log("delete sussecfull");
            }
        });
    }
}
