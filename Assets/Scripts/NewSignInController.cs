using Assets.Scripts.Models;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewSignInController : MonoBehaviour
{

    DatabaseReference databaseReference;

    private string[] nameCharacter = { "Aang", "Katara", "Toph", "Zuko" };
    private string[] introCharacter = 
        { "xuất thân từ Nam Phong Tự, một trong những ngôi đền của Tộc Không Khí.",
        "nữ thủy thuật sư từ Bộ Tộc Thủy Phương Nam.",
        "nữ thổ thuật sư mù nhưng có khả năng thổ thuật phi thường.",
        "hoàng tử của Hỏa Quốc nhưng anh bị cha mình Hỏa Vương Ozai, trục xuất."
        };

    private string[] inforCharacter =
    {
        "Blood: 48" + "\n" + "Attack: 7" + "\n" + "Amor: 20" + "\n" + "Speed: 18",
        "Blood: 53" + "\n" + "Attack: 8" + "\n" + "Amor: 21" + "\n" + "Speed: 13",
        "Blood: 50" + "\n" + "Attack: 9" + "\n" + "Amor: 25" + "\n" + "Speed: 10",
        "Blood: 45" + "\n" + "Attack: 12" + "\n" + "Amor: 16" + "\n" + "Speed: 15"
    };

    private int currentIndex;

    [SerializeField] private Sprite[] character;
    [SerializeField] private TMP_Text txtInproCharacter;
    [SerializeField] private Image imgCharacter;
    [SerializeField] private TMP_Text txtNameCharacter;
    [SerializeField] private TMP_Text txtInforCharacter;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        currentIndex = 0;
        imgCharacter.sprite = character[currentIndex];
        txtInproCharacter.text = introCharacter[currentIndex];
        txtNameCharacter.text = nameCharacter[currentIndex];
        txtInforCharacter.text = inforCharacter[currentIndex];
    }

    public void NextCharacter()
    {
        currentIndex++;
        if (currentIndex >= 4) currentIndex = 0;
        imgCharacter.sprite = character[currentIndex];
        txtInproCharacter.text = introCharacter[currentIndex];
        txtNameCharacter.text = nameCharacter[currentIndex];
        txtInforCharacter.text = inforCharacter[currentIndex];
    }
    public void PreviousCharacter() 
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = 3;
        imgCharacter.sprite = character[currentIndex];
        txtInproCharacter.text = introCharacter[currentIndex];
        txtNameCharacter.text = nameCharacter[currentIndex];
        txtInforCharacter.text = inforCharacter[currentIndex];
    }

    public void SelectCharacter()
    {
        if(currentIndex == 0)
        {
            Account.Instance.avatar = "Aang";
            Account.Instance.gold = 0;
            Account.Instance.levelID = 0;
            Account.Instance.classname = "air";
            Account.Instance.experience_points = 0;
        }else if(currentIndex == 1)
        {
            Account.Instance.avatar = "Katara";
            Account.Instance.gold = 0;
            Account.Instance.levelID = 0;
            Account.Instance.classname = "water";
            Account.Instance.experience_points = 0;
        }
        else if(currentIndex == 2)
        {
            Account.Instance.avatar = "Toph";
            Account.Instance.gold = 0;
            Account.Instance.levelID = 0;
            Account.Instance.classname = "earth";
            Account.Instance.experience_points = 0;
        }
        else if( currentIndex == 3)
        {
            Account.Instance.avatar = "Zuko";
            Account.Instance.gold = 0;
            Account.Instance.levelID = 0;
            Account.Instance.classname = "fire";
            Account.Instance.experience_points = 0;
            Account.Instance.level = 0;
        }
        StartCoroutine(UpdateAccount(Account.Instance.username, Account.Instance.gold, Account.Instance.levelID, Account.Instance.classname, Account.Instance.experience_points, Account.Instance.level, Account.Instance.avatar));
    }

    IEnumerator UpdateAccount(string username, int gold, int levelID, string classname, int exp, int level, string avatar)
    {
        var data = new Dictionary<string, object>
        {
            {"level",  level},
            {"avatar",  avatar},
            {"password", Account.Instance.password },
            {"email", Account.Instance.email },
            {"gold", gold},
            {"levelID", levelID},
            {"class", classname },
            {"exp", exp}
        };

        var task = databaseReference.Child("Account").Child(username).SetValueAsync(data);

        yield return new WaitUntil(predicate:() => task.IsCompleted);

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
        Up_level levelDefault = Up_level.Instance.up_levels.Find(u => u.levelID == 0 && u.classname.Equals(Account.Instance.classname));
        StartCoroutine(SetProperty(Account.Instance.username, levelDefault.blood, levelDefault.attack_damage, levelDefault.amor, levelDefault.speed, 0, 0));
    }

    IEnumerator SetProperty(string username, int blood, int attack, int amor, int speed, int critical_rate, int amor_penetraction)
    {
        Property.Instance.username = username;
        Property.Instance.blood = blood;
        Property.Instance.attack_damage = attack;
        Property.Instance.amor = amor;
        Property.Instance.speed = speed;
        Property.Instance.critical_rate = critical_rate;
        Property.Instance.amor_penetraction = amor_penetraction;
        var data = new Dictionary<string, object>
        {
            {"blood", blood },
            {"attack_damage", attack },
            {"amor",amor },
            {"speed", speed },
            {"amor_penentraction", amor_penetraction },
            {"critical_rate", critical_rate }
        };

        var task = databaseReference.Child("Property").Child(username).SetValueAsync(data);

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        var getInventoryCount = databaseReference.Child("Inventory").GetValueAsync();

        yield return new WaitUntil(predicate: ()=> getInventoryCount.IsCompleted);

        Inventory.Instance.username = username;
        Inventory.Instance.inventoryID = (int)getInventoryCount.Result.ChildrenCount + 1;
        StartCoroutine(SetInventory(Inventory.Instance.username, Inventory.Instance.inventoryID));
    }

    IEnumerator SetInventory(string username, int inventoryID)
    {
        var data = new Dictionary<string, object>
        {
            { "inventoryID", inventoryID}
        };

        var task = databaseReference.Child("Inventory").Child(username).SetValueAsync(data);

        yield return new WaitUntil(predicate:()=> task.IsCompleted);
        StartCoroutine(setInventory_Item(username, inventoryID, 0));
    }

    IEnumerator setInventory_Item(string username, int inventoryID, int quality)
    {
        var data = new Dictionary<string, object>
        {
            {"quality", quality }
        };

        var task = databaseReference.Child("Inventory_Item").Child(inventoryID.ToString()).Child("default").SetValueAsync(data);

        yield return new WaitUntil(predicate:()=> task.IsCompleted);

        StartCoroutine(setItem_Attached(username, Item_Attached.Instance.item_attacheds[0], Item_Attached.Instance.item_attacheds[1], Item_Attached.Instance.item_attacheds[2], Item_Attached.Instance.item_attacheds[3],
            Item_Attached.Instance.item_attacheds[4], Item_Attached.Instance.item_attacheds[5]));
    }

    IEnumerator setItem_Attached(string username, int itemID1, int itemID2, int itemID3, int itemID4, int itemID5, int itemID6)
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

        yield return new WaitUntil(predicate:() => task.IsCompleted);
        StartCoroutine(setSession(username));
    }

    IEnumerator setSession(string username)
    {
        var data = new Dictionary<string, object>
        {
            {"winnerID", "none" }
        };

        var task = databaseReference.Child("Session").Child(username).Child("default").SetValueAsync(data);

        yield return new WaitUntil(predicate:(() => task.IsCompleted));

        SceneManager.LoadScene("MenuGame");
    }
}
