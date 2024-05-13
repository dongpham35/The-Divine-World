using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;

public class MenuGameController : MonoBehaviourPunCallbacks
{

    private static MenuGameController instance;

    public static MenuGameController Instance { get { return instance; } }

    [SerializeField] private Sprite[] avatars;
    [SerializeField] private Sprite[] imgItem;
    [SerializeField] private Sprite[] imgCharacter;
    [SerializeField] private GameObject panel_inventory;
    [SerializeField] private GameObject panel_Shop;
    [SerializeField] private GameObject panel_Information;
    [SerializeField] private GameObject panel_Setting;
    [SerializeField] private GameObject panel_Map;
    [SerializeField] private GameObject panel_PvP;
    [SerializeField] private Transform Content; // content of scoll view in inventory table
    [SerializeField] private Transform ContentShop; //content of scoll view in shop
    [SerializeField] private GameObject prefabsInventorySlot;
    [SerializeField] private GameObject prefabsShopSlot;

    [Header("item being attached player")]
    [SerializeField] private GameObject panel_slot_attached1;
    [SerializeField] private GameObject panel_slot_attached2;
    [SerializeField] private GameObject panel_slot_attached3;
    [SerializeField] private GameObject panel_slot_attached4;
    [SerializeField] private GameObject panel_slot_attached5;
    [SerializeField] private GameObject panel_slot_attached6;
    //Property of player in main menu game
    [Header("Property of player in main menu game")]
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text username;
    [SerializeField] private TMP_Text gold;

    //Property of player in inventory
    [Header("property of player in inventory")]
    [SerializeField] private Image img_character;
    [SerializeField] private TMP_Text blood;
    [SerializeField] private TMP_Text damage;
    [SerializeField] private TMP_Text amor;
    [SerializeField] private TMP_Text speed;
    [SerializeField] private TMP_Text critical_rate;
    [SerializeField] private TMP_Text amor_penetraction;

    [Header("Property of Map")]
    [SerializeField] private TMP_InputField ipfNameRoom;
    [SerializeField] private TMP_InputField ipfNameRoomPvP;

    private int sum_itemID_attached;

    private AudioSource soundTrack;

    private void Awake()
    {
        if (Account.Instance.@class.Equals("air"))
        {
            avatar.sprite = avatars[0];
            img_character.sprite = imgCharacter[0];
        }
        else if (Account.Instance.@class.Equals("water"))
        {
            avatar.sprite = avatars[1];
            img_character.sprite = imgCharacter[1];
        }
        else if (Account.Instance.@class.Equals("earth"))
        {
            avatar.sprite = avatars[2];
            img_character.sprite = imgCharacter[2];
        }
        else if (Account.Instance.@class.Equals("fire"))
        {
            avatar.sprite = avatars[3];
            img_character.sprite = imgCharacter[3];
        }
        panel_inventory.SetActive(false);
        panel_Shop.SetActive(false);
        panel_Information.SetActive(false);
        panel_Setting.SetActive(false);
        panel_Map.SetActive(false);
        panel_PvP.SetActive(false);

        
        sum_itemID_attached = Item_Attached.Instance.item_attacheds.Sum();
        StartCoroutine(reLoadInventory());
        StartCoroutine(reLoadMenuGame());
    }

    private void Start()
    {
        soundTrack = GetComponent<AudioSource>();
        soundTrack.Play();
    }

    IEnumerator reLoadMenuGame()
    {
        while (true)
        {
            username.text = Account.Instance.username;
            gold.text = Account.Instance.gold.ToString();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void TurnOnInventory()
    {
        panel_inventory.SetActive(true);
        
        blood.text = "Máu: " + Property.Instance.blood.ToString();
        damage.text = "Stvl: " + Property.Instance.attack_damage.ToString();
        amor.text = "Giáp: " + Property.Instance.amor.ToString();
        speed.text = "Tốc độ: " + Property.Instance.speed.ToString();
        critical_rate.text = "Chí mạng: " + Property.Instance.critical_rate.ToString();
        amor_penetraction.text = "Xuyên giáp: " + Property.Instance.amor_penetraction.ToString();
        loadInventory();
        loadItemAttached();
    }

    public void TurnOffInventory()
    {
        panel_inventory.SetActive(false);
        for(int i = 0; i< Content.childCount; i++)
        {
            Destroy(Content.GetChild(i).gameObject);
        }
    }


    private void loadInventory()
    {
        for (int i = 0; i < Inventory_Item.Instance.items.Count; i++)
        {
            if (Inventory_Item.Instance.items[i].quality == 0) continue;
            int itemid = Inventory_Item.Instance.items[i].itemID;
            int quality = Inventory_Item.Instance.items[i].quality;
            string itemname = Item.Instance.items.FirstOrDefault(item => item.itemID == itemid).name;

            GameObject item = Instantiate(prefabsInventorySlot);
            item.GetComponentInChildren<TMP_Text>().text = itemname;
            Transform img = item.transform.Find("itemImg");
            img.GetComponentInChildren<Image>().sprite = imgItem[itemid];
            Transform itemImg = item.transform.Find("itemImg");
            itemImg.GetComponentInChildren<TMP_Text>().text = quality.ToString();

            item.transform.SetParent(Content);
            item.transform.localScale = Vector2.one;
        }
    }

    private void loadItemAttached()
    {

            panel_slot_attached1.GetComponent<Image>().sprite = imgItem[Item_Attached.Instance.item_attacheds[0]];
            panel_slot_attached2.GetComponent<Image>().sprite = imgItem[Item_Attached.Instance.item_attacheds[1]];
            panel_slot_attached3.GetComponent<Image>().sprite = imgItem[Item_Attached.Instance.item_attacheds[2]];
            panel_slot_attached4.GetComponent<Image>().sprite = imgItem[Item_Attached.Instance.item_attacheds[3]];
            panel_slot_attached5.GetComponent<Image>().sprite = imgItem[Item_Attached.Instance.item_attacheds[4]];
            panel_slot_attached6.GetComponent<Image>().sprite = imgItem[Item_Attached.Instance.item_attacheds[5]];
    }

    public void TurnOnShop()
    {
        panel_Shop.SetActive(true);
        loadShop();
    }

    public void TurnOffShop()
    {
        panel_Shop.SetActive(false);
        for(int i = 0; i<ContentShop.childCount; i++)
        {
            Destroy(ContentShop.GetChild(i).gameObject);
        }
    }
    private void loadShop()
    {
        for(int i = 0;i < Item.Instance.items.Count;i++)
        {
            var item = Instantiate(prefabsShopSlot);
            Item item_current = Item.Instance.items[i];
            item.GetComponent<Image>().sprite = imgItem[item_current.itemID];
            item.GetComponentInChildren<TMP_Text>().text = item_current.name;

            var item_image = item.GetComponentsInChildren<Image>();
            foreach(var img in item_image)
            {
                if (img.gameObject.name.Equals("imgCost"))
                {
                    img.GetComponentInChildren<TMP_Text>().text = item_current.cost.ToString();
                }
            }

            item.transform.SetParent(ContentShop);
            item.transform.localScale = Vector2.one;
        }
    }

    

    IEnumerator reLoadInventory()
    {
        while (true)
        {
            blood.text = "Máu: " + Property.Instance.blood.ToString();
            damage.text = "Stvl: " + Property.Instance.attack_damage.ToString();
            amor.text = "Giáp: " + Property.Instance.amor.ToString();
            speed.text = "Tốc độ: " + Property.Instance.speed.ToString();
            critical_rate.text = "Chí mạng: " + Property.Instance.critical_rate.ToString();
            amor_penetraction.text = "Xuyên giáp: " + Property.Instance.amor_penetraction.ToString();
            if (sum_itemID_attached != Item_Attached.Instance.item_attacheds.Sum())
            {
                for (int i = 0; i < Content.childCount; i++)
                {
                    Destroy(Content.GetChild(i).gameObject);
                }
                loadInventory();
                loadItemAttached();
                sum_itemID_attached = Item_Attached.Instance.item_attacheds.Sum();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void TurnOnMap()
    {
        panel_Map.SetActive(true);
    }

    public void TurnOffMap()
    {
        panel_Map.SetActive(false);
    }

    public void TurnOnPvP()
    {
        panel_PvP.SetActive(true);
    }

    public void TurnOffPvP()
    {
        panel_PvP.SetActive(false);
    }

    public void CreateMapPrimevalForestRoom()
    {
        string nameMap = "Map 1 " + Account.Instance.username;
        RoomOptions option = new RoomOptions { MaxPlayers = 2};
        PhotonNetwork.CreateRoom(nameMap, option);
    }

    public void CreatePvPRoom()
    {
        string nameRoom = Account.Instance.username;
        RoomOptions option = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(nameRoom, option);
    }

    public void JoinRoom()
    {
        if (panel_Map.activeSelf)
        {
            PhotonNetwork.JoinRoom(ipfNameRoom.text);
        }else if (panel_PvP.activeSelf)
        {
            PhotonNetwork.JoinRoom(ipfNameRoomPvP.text);
        }
        
    }

    public void LogOut()
    {

    }


    public override void OnJoinedRoom()
    {
        if (panel_Map.activeSelf)
        {
            PhotonNetwork.LoadLevel("Map");
        }
        else if (panel_PvP.activeSelf)
        {
            PhotonNetwork.LoadLevel("PvP");
        }
    }

    public void TurnOnInformation()
    {
        panel_Information.SetActive(true);
    }

    public void TurnOnSetting()
    {
        panel_Setting.SetActive(true);
    }
    public void TurnOffInformation()
    {
        panel_Information.SetActive(false);
    }

    public void TurnOffSetting()
    {
        panel_Setting.SetActive(false);
    }

    

}
