using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Assets.Scripts.Models;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
    DatabaseReference databaseReference;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void GetAccount(string username)
    {
        databaseReference.Child("Account").Child(username).GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    Account.Instance.username = username;
                    foreach(var i in snapshot.Children)
                    {
                        if (i.Key.Equals("avatar"))
                        {
                            Account.Instance.avatar = i.Value.ToString();
                            continue;
                        }
                        if (i.Key.Equals("email"))
                        {
                            Account.Instance.email = i.Value.ToString();
                            continue;
                        }
                        if (i.Key.Equals("password"))
                        {
                            Account.Instance.password = i.Value.ToString();
                            continue;
                        }
                        if (i.Key.Equals("gold"))
                        {
                            Account.Instance.gold = int.Parse(i.Value.ToString());
                            continue;
                        }
                        if (i.Key.Equals("levelID"))
                        {
                            Account.Instance.levelID = int.Parse(i.Value.ToString());
                            continue;
                        }
                        if (i.Key.Equals("class"))
                        {
                            Account.Instance.@class = i.Value.ToString();
                            continue;
                        }
                        if (i.Key.Equals("level"))
                        {
                            Account.Instance.level = int.Parse(i.Value.ToString());
                            continue;
                        }
                        if (i.Key.Equals("exp"))
                        {
                            Account.Instance.experience_points = int.Parse(i.Value.ToString());
                            continue;
                        }

                    }
                }
                else
                {
                    Debug.Log("User not found.");
                }
            }
            else
            {
                Debug.LogError("Failed to get data: " + task.Exception);
            }
        });
    }
    public void GetFriend(string username)
    {
        databaseReference.Child("Friend").Child(username).GetValueAsync().ContinueWith(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    if (snapshot.HasChildren)
                    {
                        foreach (var child in snapshot.Children)
                        {
                            Friend f = new Friend();
                            f.username = username;
                            f.username2ID = child.Value.ToString();
                            Friend.Instance.friends.Add(f);
                        }
                    }
                    else
                    {
                        Debug.Log("has no friend");
                    }
                }
                else
                {
                    Debug.Log("null data");
                }
            }
            else
            {
                Debug.LogError("task faild");
            }
        });
    }
    public void GetInventory(string username)
    {
        databaseReference.Child("Inventory").Child(username).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    if (snapshot.HasChildren)
                    {
                        Inventory.Instance.username = username;
                        Inventory.Instance.inventoryID = int.Parse(snapshot.Child("inventoryID").GetRawJsonValue().ToString());    
                    }
                    else
                    {
                        Debug.Log("Inventory of player are not initial");
                    }
                }
                else
                {
                    Debug.Log("null data");
                }
            }
            else
            {
                Debug.LogError("task faild");
            }
        });
    }

    public void GetInventory_Item(int inventoryID)
    {
        databaseReference.Child("Inventory_Item").Child(inventoryID.ToString()).GetValueAsync().ContinueWith(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    if (snapshot.HasChildren)
                    {
                        foreach(var child in snapshot.Children)
                        {
                            if (child.Key.Equals("default")) continue;
                            Inventory_Item in_item = new Inventory_Item();
                            in_item.inventoryID = inventoryID;
                            in_item.itemID = int.Parse(child.Key.ToString());
                            in_item.quality = int.Parse(child.Child("quality").GetRawJsonValue());
                            Inventory_Item.Instance.items.Add(in_item);
                        }
                    }
                    else
                    {
                        Debug.Log("Inventory null");
                    }
                }
                else
                {
                    Debug.Log("data null");
                }
            }
            else
            {
                Debug.LogError("task faild");
            }
        });
    }
    public void GetItem()
    {
        databaseReference.Child("Item").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot datasnapshot = task.Result;
                if (datasnapshot.Exists)
                {
                    if(datasnapshot.HasChildren)
                    {
                        foreach (var child in datasnapshot.Children)
                        {
                            Item item = new Item();
                            item.itemID = int.Parse(child.Key);
                            foreach(var childItem in child.Children)
                            {
                                if (childItem.Key.Equals("description"))
                                {
                                    item.description = childItem.Value.ToString();
                                    continue;
                                }
                                if (childItem.Key.Equals("image"))
                                {
                                    item.image = childItem.Value.ToString();
                                    continue;
                                }
                                if (childItem.Key.Equals("name"))
                                {
                                    item.name = childItem.Value.ToString();
                                    continue;
                                }
                                if (childItem.Key.Equals("type"))
                                {
                                    item.type = childItem.Value.ToString();
                                    continue;
                                }
                                if (childItem.Key.Equals("cost"))
                                {
                                    item.cost = int.Parse(childItem.Value.ToString());
                                    continue;
                                }
                                if (childItem.Key.Equals("value"))
                                {
                                    item.value = int.Parse(childItem.Value.ToString());
                                    continue;
                                }
                            }
                            Item.Instance.items.Add(item);
                        }
                    }
                    else
                    {
                        Debug.Log("Item table has no item");
                    }
                }
                else
                {
                    Debug.Log("null data");
                }
            }
            else
            {
                Debug.LogError("Task faild");
            }
        });
    }

    public void GetItemAttached(string username)
    {
        databaseReference.Child("Item_Attached").Child(username).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                if (dataSnapshot.Exists)
                {
                    if (dataSnapshot.HasChildren)
                    {
                        Item_Attached.Instance.username = username;
                        DataSnapshot[] datas = dataSnapshot.Children.ToArray();
                        for(int i = 0; i < dataSnapshot.ChildrenCount; i++)
                        {
                            Item_Attached.Instance.item_attacheds[i] = int.Parse(datas[i].Value.ToString());
                        }
                    }
                    else
                    {
                        Debug.Log("Item attached has no data");
                    }
                }
                else
                {
                    Debug.Log("Item attached table has no data");
                }
            }
            else
            {
                Debug.LogError("task faild");
            }
        });
    }

    public void GetProperty(string username)
    {
        databaseReference.Child("Property").Child(username).GetValueAsync().ContinueWith(task =>
        {
            if(!task.IsCompleted)
            {
                Debug.LogError("task faild");
                return;
            }
            DataSnapshot datasnapshot = task.Result;
            if (!datasnapshot.Exists)
            {
                Debug.Log("Property of player are not initial");
                return;
            }
            Property.Instance.username = username;
            foreach(var child in datasnapshot.Children)
            {
                if (child.Key.Equals("amor"))
                {
                    Property.Instance.amor = int.Parse(child.Value.ToString());
                    continue;
                }
                if (child.Key.Equals("amor_penetraction"))
                {
                    Property.Instance.amor_penetraction = int.Parse(child.Value.ToString());
                    continue;
                }
                if (child.Key.Equals("attack_damage"))
                {
                    Property.Instance.attack_damage = int.Parse(child.Value.ToString());
                    continue;
                }
                if (child.Key.Equals("blood"))
                {
                    Property.Instance.blood = int.Parse(child.Value.ToString());
                    continue;
                }
                if (child.Key.Equals("critical_rate"))
                {
                    Property.Instance.critical_rate = int.Parse(child.Value.ToString());
                    continue;
                }
                if (child.Key.Equals("speed"))
                {
                    Property.Instance.speed = int.Parse(child.Value.ToString());
                    continue;
                }

            }
            
        });
    }

    public void GetSession(string username)
    {
        databaseReference.Child("Session").Child(username).GetValueAsync().ContinueWith(task =>
        {
            if (!task.IsCompleted)
            {
                Debug.LogError("task faild");
                return;
            }
            DataSnapshot datasnapshot = task.Result;
            if (!datasnapshot.Exists)
            {
                Debug.Log("Session table has no data");
                return;
            }
            
        });
    }
}
