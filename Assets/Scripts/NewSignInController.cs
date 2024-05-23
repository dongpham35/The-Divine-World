using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewSignInController : MonoBehaviour
{
    private string url = "http://localhost/TheDiVWorld/api/";

    private string[] nameCharacter = { "Aang", "Katara", "Toph", "Zuko" };
    private string[] introCharacter = 
        { "xuất thân từ Nam Phong Tự, một trong những ngôi đền của Tộc Không Khí.",
        "nữ thủy thuật sư từ Bộ Tộc Thủy Phương Nam.",
        "nữ thổ thuật sư mù nhưng có khả năng thổ thuật phi thường.",
        "hoàng tử của Hỏa Quốc nhưng anh bị cha mình Hỏa Vương Ozai, trục xuất."
        };
    private int currentIndex;

    [SerializeField] private Sprite[] character;
    [SerializeField] private TMP_Text txtInproCharacter;
    [SerializeField] private Image imgCharacter;
    [SerializeField] private TMP_Text txtNameCharacter;

    private void Start()
    {
        currentIndex = 0;
        imgCharacter.sprite = character[currentIndex];
        txtInproCharacter.text = introCharacter[currentIndex];
        txtNameCharacter.text = nameCharacter[currentIndex];
    }

    public void NextCharacter()
    {
        currentIndex++;
        if (currentIndex >= 4) currentIndex = 0;
        imgCharacter.sprite = character[currentIndex];
        txtInproCharacter.text = introCharacter[currentIndex];
        txtNameCharacter.text = nameCharacter[currentIndex];
    }
    public void PreviousCharacter() 
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = 3;
        imgCharacter.sprite = character[currentIndex];
        txtInproCharacter.text = introCharacter[currentIndex];
        txtNameCharacter.text = nameCharacter[currentIndex];
    }

    public void SelectCharacter()
    {
        if(currentIndex == 0)
        {
            Account.Instance.avatar = "Aang";
            Account.Instance.gold = 0;
            Account.Instance.levelID = 0;
            Account.Instance.@class = "air";
            Account.Instance.experience_points = 0;
        }else if(currentIndex == 1)
        {
            Account.Instance.avatar = "Katara";
            Account.Instance.gold = 0;
            Account.Instance.levelID = 0;
            Account.Instance.@class = "water";
            Account.Instance.experience_points = 0;
        }
        else if(currentIndex == 2)
        {
            Account.Instance.avatar = "Toph";
            Account.Instance.gold = 0;
            Account.Instance.levelID = 0;
            Account.Instance.@class = "earth";
            Account.Instance.experience_points = 0;
        }
        else if( currentIndex == 3)
        {
            Account.Instance.avatar = "Zuko";
            Account.Instance.gold = 0;
            Account.Instance.levelID = 0;
            Account.Instance.@class = "fire";
            Account.Instance.experience_points = 0;
        }
        StartCoroutine(postAccountTable(Account.Instance.username, Account.Instance.gold, Account.Instance.levelID, Account.Instance.@class, Account.Instance.experience_points));
    }

    IEnumerator postAccountTable(string username, int gold, int levelID, string @class, int exp)
    {
        string URL = url + $"Account?username={username}&gold={gold}&levelID={levelID}&class={@class}&exp={exp}";
        using (UnityWebRequest request = UnityWebRequest.Post(URL,"Post"))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Loi Accoutn");
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
                    EditorUtility.DisplayDialog("Thông báo", "Tên tài khoản không tồn tại", "Ok");
#endif
                }
                else
                {
                    Debug.Log("Cap nhat thanh cong bang Account");
                    Up_level ul = Up_level.Instance.up_levels.FirstOrDefault(u => u.@class.Equals(Account.Instance.@class) && u.levelID == Account.Instance.levelID);
                    StartCoroutine(putPropertyTable(Account.Instance.username, ul.blood, ul.attack_damage, ul.amor, 0, ul.speed, 0));
                    SceneManager.LoadScene("MenuGame");
                }

            }
            request.Dispose();
        }
    }

    IEnumerator putPropertyTable(string username, int blood, int attack_damage, int amor, int critical_rate, int speed, int amor_penetraction)
    {
        string URL = url + $"Property?username={username}&blood={blood}&attack_damage={attack_damage}&amor={amor}&critical_rate={critical_rate}&speed={speed}&amor_penetraction={amor_penetraction}";
        using (UnityWebRequest request = UnityWebRequest.Put(URL, "Put"))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Loi property");
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
                    EditorUtility.DisplayDialog("Thông báo", "Tên tài khoản không tồn tại", "Ok");
#endif
                }
                else
                {
                    Debug.Log("Cap nhat thanh cong bang Property");
                    StartCoroutine(putInventoryTable(Account.Instance.username));
                    StartCoroutine(getProperty(Account.Instance.username));
                }

            }
            request.Dispose();
        }
    }

    IEnumerator putInventoryTable(string username)
    {
        string URL = url + $"Inventory?username={username}";
        using (UnityWebRequest request = UnityWebRequest.Put(URL, "Put"))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Loi Inventory");
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
                    EditorUtility.DisplayDialog("Thông báo", "Tên tài khoản không tồn tại", "Ok");
#endif
                }
                else
                {
                    Debug.Log("Cap nhat thanh cong bang Inventory");
                    StartCoroutine(getInventory(Account.Instance.username));
                }

            }
            request.Dispose();
        }
    }
    IEnumerator getInventory(string username)
    {
        string URL = url + "Inventory?username=" + username;
        using (UnityWebRequest request = UnityWebRequest.Get(URL))
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
                if (stats != null)
                {
                    Inventory.Instance.inventoryID = int.Parse(stats["inventoryID"]);
                    Inventory.Instance.username = stats["username"].ToString().Replace('"', ' ').Replace(" ", "");
                }

            }

            request.Dispose();
        }

    }
    IEnumerator getProperty(string username)
    {
        string URL =url + "Property?username=" + username;
        using (UnityWebRequest request = UnityWebRequest.Get(URL))
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
                if (stats != null)
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
}
