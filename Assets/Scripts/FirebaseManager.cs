using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Assets.Scripts.Models;

public class FirebaseManager : MonoBehaviour
{
    DatabaseReference databaseReference;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        GetAccount("admin");
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

                    Debug.Log($"username:{Account.Instance.username}-email:{Account.Instance.email}-password:{Account.Instance.password}");
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
}
