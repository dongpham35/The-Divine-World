using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Assets.Scripts.Models;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class FirebaseManager : MonoBehaviour
{
    DatabaseReference databaseReference;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public IEnumerator GetAccount(string username)
    {
        var data = databaseReference.Child("Account").Child(username.ToString()).GetValueAsync();

        yield return new WaitUntil(predicate: () => data.IsCompleted);
        DataSnapshot snapshot = data.Result;
        if (snapshot.Exists)
        {
            Account.Instance.username = username.ToString();
            foreach (var i in snapshot.Children)
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
                    Account.Instance.classname = i.Value.ToString();
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
            StartCoroutine(GetFriend(Account.Instance.username));
        }
        else
        {
            Debug.Log("User not found.");
        }
    }
    public IEnumerator GetFriend(string username)
    {
        var task = databaseReference.Child("Friend").Child(username).GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        DataSnapshot snapshot = task.Result;
        if (snapshot.Exists)
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
            Debug.Log("null data");
        }
        StartCoroutine(GetInventory(Account.Instance.username));
    }
    public IEnumerator GetInventory(string username)
    {
        var task = databaseReference.Child("Inventory").Child(username).GetValueAsync();

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        DataSnapshot snapshot = task.Result;
        if (snapshot.Exists)
        {
            if (snapshot.HasChildren)
            {
                Inventory.Instance.username = username;
                Inventory.Instance.inventoryID = int.Parse(snapshot.Child("inventoryID").GetRawJsonValue().ToString());
                StartCoroutine(GetInventory_Item(Inventory.Instance.inventoryID));
                
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

    public IEnumerator GetInventory_Item(int inventoryID)
    {
        string KeyInventoryID = inventoryID.ToString();
        var task = databaseReference.Child("Inventory_Item").Child(inventoryID.ToString()).GetValueAsync();

        
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        DataSnapshot snapshot = task.Result;
        if (snapshot.Exists)
        {
            foreach (var child in snapshot.Children)
            {
                if (child.Key.Equals("default")) continue;
                Inventory_Item in_item = new Inventory_Item();
                in_item.inventoryID = inventoryID;
                in_item.itemID = int.Parse(child.Key.ToString());
                in_item.quality = int.Parse(child.Child("quality").GetRawJsonValue());
                Inventory_Item.Instance.items.Add(in_item);
            }
            StartCoroutine(GetItem());
        }
        else
        {
            Debug.Log("data null");
        }
    }
    public IEnumerator GetItem()
    {
        var task = databaseReference.Child("Item").GetValueAsync();
        
        yield return new WaitUntil(predicate:() => task.IsCompleted);


        DataSnapshot datasnapshot = task.Result;
        if (datasnapshot.Exists)
        {
            if (datasnapshot.HasChildren)
            {
                foreach (var child in datasnapshot.Children)
                {
                    if (child.Key.Equals("item")) continue;
                    Item item = new Item();
                    item.itemID = int.Parse(child.Key);
                    foreach (var childItem in child.Children)
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

                StartCoroutine(GetItemAttached(Account.Instance.username));
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

    public IEnumerator GetItemAttached(string username)
    {
        var task = databaseReference.Child("Item_Attached").Child(username).GetValueAsync();

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        DataSnapshot dataSnapshot = task.Result;
        if (dataSnapshot.Exists)
        {
            if (dataSnapshot.HasChildren)
            {
                Item_Attached.Instance.username = username;
                DataSnapshot[] datas = dataSnapshot.Children.ToArray();
                for (int i = 0; i < dataSnapshot.ChildrenCount; i++)
                {
                    Item_Attached.Instance.item_attacheds[i] = int.Parse(datas[i].Value.ToString());
                }
                StartCoroutine(GetProperty(Account.Instance.username));
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

    public IEnumerator GetProperty(string username)
    {
        var task = databaseReference.Child("Property").Child(username).GetValueAsync();
        
        yield return new WaitUntil(predicate:() => task.IsCompleted);
        DataSnapshot datasnapshot = task.Result;
        if (!datasnapshot.Exists)
        {
            Debug.Log("Property of player are not initial");
            yield return 0;
        }
        Property.Instance.username = username;
        foreach (var child in datasnapshot.Children)
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
        StartCoroutine(GetSession(Account.Instance.username));
    }

    public IEnumerator GetSession(string username)
    {
        var task = databaseReference.Child("Session").Child(username).GetValueAsync();

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        DataSnapshot datasnapshot = task.Result;
        if (!datasnapshot.Exists)
        {
            Debug.Log("Session table has no data");
            yield return 0;
        }
        foreach (var child in datasnapshot.Children)
        {
            if (child.Key.Equals("default")) continue;
            Sessions s = new Sessions();
            s.username = username;
            s.sessionID = int.Parse(child.Key);
            foreach (var childValue in child.Children)
            {
                if (childValue.Key.Equals("start_time"))
                {
                    s.start_time = DateTime.ParseExact(childValue.Value.ToString(), "yyyy-MM-dd hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }
                if (childValue.Key.Equals("end_time"))
                {
                    s.end_time = DateTime.ParseExact(childValue.Value.ToString(), "yyyy-MM-dd hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }
                if (childValue.Key.Equals("username2ID"))
                {
                    s.username2ID = childValue.Value.ToString();
                    continue;
                }
                if (childValue.Key.Equals("winnerID"))
                {
                    s.winnerID = childValue.Value.ToString();
                    continue;
                }
            }
            Sessions.Instance.sessions.Add(s);
        }

        StartCoroutine(GetUp_level());
    }

    public IEnumerator GetUp_level()
    {
        var task = databaseReference.Child("Up_level").GetValueAsync();

        yield return new WaitUntil(predicate: () => task.IsCompleted);


        DataSnapshot datasnapshot = task.Result;
        if (!datasnapshot.Exists)
        {
            Debug.Log("Up_level has no data");
            yield return 0;
        }
        foreach (var level in datasnapshot.Children)
        {
            if (level.Key.Equals("defaultLevel"))
            {
                foreach (var classname in level.Children)
                {
                    Up_level ul = new Up_level();

                    ul.levelID = 0;

                    ul.classname = classname.Key;

                    foreach (var property in classname.Children)
                    {
                        if (property.Key.Equals("amor"))
                        {
                            ul.amor = int.Parse(property.Value.ToString());
                            continue;
                        }
                        if (property.Key.Equals("attack_damage"))
                        {
                            ul.attack_damage = int.Parse(property.Value.ToString());
                            continue;
                        }
                        if (property.Key.Equals("blood"))
                        {
                            ul.blood = int.Parse(property.Value.ToString());
                            continue;
                        }
                        if (property.Key.Equals("speed"))
                        {
                            ul.speed = int.Parse(property.Value.ToString());
                            continue;
                        }
                    }

                    Up_level.Instance.up_levels.Add(ul);
                }
                continue;
            }
            foreach (var classname in level.Children)
            {
                Up_level ul = new Up_level();

                ul.levelID = int.Parse(level.Key);

                ul.classname = classname.Key;

                foreach (var property in classname.Children)
                {
                    if (property.Key.Equals("amor"))
                    {
                        ul.amor = int.Parse(property.Value.ToString());
                        continue;
                    }
                    if (property.Key.Equals("attack_damage"))
                    {
                        ul.attack_damage = int.Parse(property.Value.ToString());
                        continue;
                    }
                    if (property.Key.Equals("blood"))
                    {
                        ul.blood = int.Parse(property.Value.ToString());
                        continue;
                    }
                    if (property.Key.Equals("speed"))
                    {
                        ul.speed = int.Parse(property.Value.ToString());
                        continue;
                    }
                }

                Up_level.Instance.up_levels.Add(ul);
            }
        }

        SceneManager.LoadScene("MenuGame"); // complete load data
    }

}
