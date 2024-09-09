using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using JetBrains.Annotations;
using TMPro;
using Firebase.Database;
using Photon.Realtime;
using System.Linq;
using ExitGames.Client.Photon;

public class PlayerController : MonoBehaviourPunCallbacks
{

    public GameObject projectilePrefab;
    public HealthBar healthBar;
    public HealthBar powerBar;
    public GameObject panel_finish;
    public GameObject cam;
    public GameObject txtDamageTaken;


    [SerializeField] float cooldownHit1;
    [SerializeField] float cooldownHit2;
    [SerializeField] float cooldownHit3;

    [SerializeField] AudioSource hit1;
    [SerializeField] AudioSource hit2;
    [SerializeField] AudioSource hit3;
    [SerializeField] AudioSource jump;

    PhotonView view;
    public int IdLoser;
    public const byte EventCode_Loser = 1;
    private bool endGame;

    private Rigidbody2D rb;
    private SpriteRenderer spriRender_Player;
    private Animator animator_Player;
    private BoxCollider2D collider_Player;
    private DatabaseReference databaseReference;

    private enum anim { idle, run, jump, fall}

    private anim state = anim.idle;
    private bool isBeAttacked;
    private bool isKnockOut;
    private bool isSpriterender;

    //property attack combo

    private float noOfCombo = 0;
    private float lastAttack = 0;
    private float delayCombo = 0.6f;

    private float lastbedamage;

    //property player
    private float speed ;
    private int blood ;
    private int attack_damage;
    private int amor;
    private int amor_penetraction;
    private int critical_rate;
    private int power = 100;
    private int currentPower;


    private float startTime;
    private float damagebetaken;
    private bool canAction;

    private float lastTimeUseHit1;
    private float lastTimeUseHit2;
    private float lastTimeUseHit3;
    private bool canHit1;
    private bool canHit2;
    private bool canHit3;

    private GameObject maincamera;

    public int currentHealth;
    
    public int gold;
    public int exp;
    public int numOfElement;

    private Vector3 direction;

    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    private Property mineProperty;
    private float dirX;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            if (Account.Instance.classname.Equals("air"))
            {
                numOfElement = 1;
            }else if (Account.Instance.classname.Equals("water"))
            {
                numOfElement = 1;
            }else if (Account.Instance.classname.Equals("earth"))
            {
                numOfElement = -1;
            }else if (Account.Instance.classname.Equals("fire"))
            {
                numOfElement = -1;
            }
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        }
    }
/*
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
    }*/

    private void Start()
    {
        
        panel_finish.SetActive(false);
        if (view.IsMine)
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            maincamera = Instantiate(cam, new Vector3(0, 0, -10), Quaternion.identity);
            maincamera.transform.SetParent(transform);
            maincamera.transform.localPosition = new Vector3(0, 0, -10);
            maincamera.SetActive(true);
            maincamera.GetComponent<AudioListener>().enabled = true;

            rb = GetComponent<Rigidbody2D>();
            animator_Player = GetComponent<Animator>();
            spriRender_Player = GetComponent<SpriteRenderer>();
            collider_Player = GetComponent<BoxCollider2D>();
            speed = Property.Instance.speed * 0.2f;
            blood = Property.Instance.blood;
            attack_damage = Property.Instance.attack_damage;
            amor = Property.Instance.amor;
            amor_penetraction = Property.Instance.amor_penetraction;
            critical_rate = Property.Instance.critical_rate;

            gold = Account.Instance.gold;
            exp = Account.Instance.experience_points;
            view.RPC("DataSynchronization", RpcTarget.OthersBuffered, Property.Instance.speed, Property.Instance.blood, Property.Instance.attack_damage,
                Property.Instance.amor, Property.Instance.amor_penetraction, Property.Instance.critical_rate, Account.Instance.gold, Account.Instance.experience_points, power, numOfElement);
            healthBar.SetMaxHealth(blood);
            powerBar.SetMaxHealth(power);

            currentHealth = blood;
            currentPower = power;
            canAction = true;
            isBeAttacked = false;
            isKnockOut = false;
            isSpriterender = false;
            canHit1 = true;
            canHit2 = true;
            canHit3 = true;
            cooldownHit1 = 0.3f;
            cooldownHit2 = 1f;
            cooldownHit3 = 1.5f;
            direction = new Vector3(1, 0, 0);
            IdLoser = 0;
            endGame = false;
        }
        else
        {
            healthBar.gameObject.SetActive(false);
            powerBar.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void DataSynchronization(int syn_speed,int syn_blood,int syn_attack,int syn_amor,int syn_amorPenetraction,int syn_crit, int syn_gold,int syn_exp,int syn_power, int NoE)
    {
        speed = syn_speed;
        blood = syn_blood;
        attack_damage = syn_attack;
        amor = syn_amor;
        amor_penetraction = syn_amorPenetraction;
        critical_rate = syn_crit;

        gold = syn_gold;
        exp = syn_exp;

        currentHealth = blood;
        currentPower = power;
        healthBar.SetMaxHealth(syn_blood);
        powerBar.SetMaxHealth(syn_power);
        numOfElement = NoE;
    }

    private void Update()
    {

        if (view.IsMine)
        {
            if (endGame)
            {
                if (IdLoser == view.ViewID)
                {
                    Debug.Log("Loser : " + view.ViewID);
                    return;
                }
                else
                {
                    Debug.Log("Winner: " + view.ViewID);
                }
            }

            if (Time.time - lastTimeUseHit1 >= cooldownHit1) canHit1 = true;
            if (Time.time - lastTimeUseHit2 >= cooldownHit2) canHit2 = true;
            if (Time.time - lastTimeUseHit3 >= cooldownHit3) canHit3 = true;
            if (currentHealth == 0)
            {
                IdLoser = view.ViewID;
                isKnockOut = true;
                animator_Player.SetBool("BeKnockOut", isKnockOut);
                canAction = false;
                photonView.RPC("changeKnockOut", RpcTarget.Others);
                ExecuteEndGame(view.ViewID);
                
                endGame = true;
                PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
            }
            
            if (Time.time - lastbedamage > 0.3f && currentHealth != 0)
            {
                isBeAttacked = false;
                animator_Player.SetBool("BeAttacked", isBeAttacked);
                canAction = true;
            }
            dirX = canAction ? Input.GetAxisRaw("Horizontal") : 0;
            if (isGround())
            {
                rb.velocity = new Vector2((float)dirX * speed * 2 / 3 , rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2((float)dirX * speed, rb.velocity.y);
            }

            if (Input.GetButtonDown("Vertical") && isGround() && canAction )
            {
                jump.Play();
                rb.velocity = new Vector2(rb.velocity.x , speed);
            }

            if (animator_Player.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && Time.time - lastAttack > 0.3)
            {
                animator_Player.SetBool("Attack1", false);
            }
            if (animator_Player.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && Time.time - lastAttack > 0.3)
            {
                animator_Player.SetBool("Attack2", false);
            }
            if (animator_Player.GetCurrentAnimatorStateInfo(0).IsName("Attack3") && Time.time - lastAttack > 0.5)
            {
                animator_Player.SetBool("Attack3", false);
            }
            if (animator_Player.GetCurrentAnimatorStateInfo(0).IsName("Attack4") && Time.time - lastAttack > 0.4)
            {
                animator_Player.SetBool("Attack4", false);
            }

            if (Time.time - lastAttack > delayCombo)
            {
                noOfCombo = 0;

            }

            if (Input.GetKeyDown(KeyCode.J) && canAction && canHit1)
            {
                hit1.Play();
                canHit1 = false;
                lastTimeUseHit1 = Time.time;
                anim_Attack_Nor();
                hitPlayer();
            }

            if (Input.GetKeyDown(KeyCode.K) && canAction && canHit2 && currentPower >= 15)
            {
                currentPower -= 15;
                powerBar.SetHealth(currentPower);
                hit2.Play();
                canHit2 = false;
                lastTimeUseHit2 = Time.time;
                anim_fire();
                Fire();
            }

            if (Input.GetKeyDown(KeyCode.L) && canAction && canHit3)
            {
                hit3.Play();
                canHit3 = false;
                lastTimeUseHit3 = Time.time;
                anim_push();
                Push();
            }
            UpdateAnimation();
        }

    }

    
    private void UpdateAnimation()
    {
        if (view.IsMine)
        {
            if (rb.velocity.x > .0f)
            {
                isSpriterender = false;
                state = anim.run;
                direction.Set(1, 0, 0);
            }
            else if (rb.velocity.x < .0f)
            {
                isSpriterender = true;
                state = anim.run;
                direction.Set(-1, 0, 0);
            }
            else
            {
                state = anim.idle;
            }

            if (rb.velocity.y > 0.1f)
            {
                state = anim.jump;
            }

            if (rb.velocity.y < -0.1f)
            {
                state = anim.fall;
            }
            
            spriRender_Player.flipX = isSpriterender;
            if (PhotonNetwork.InRoom)
            {
                photonView.RPC("flipX", RpcTarget.OthersBuffered, isSpriterender);
            }
            animator_Player.SetInteger("StateNor", (int)state);
            animator_Player.SetBool("BeAttacked", isBeAttacked);
        }
    }

    private bool isGround()
    {
        if (view.IsMine)
        {
            return Physics2D.BoxCast(collider_Player.bounds.center, collider_Player.bounds.size, 0f, Vector2.down, .1f, LayerMask.GetMask("Ground")) ||
            Physics2D.BoxCast(collider_Player.bounds.center, collider_Player.bounds.size, 0f, Vector2.down, .1f, LayerMask.GetMask("Enemy"));
        } return false;
    }

    private void anim_Attack_Nor()
    {
        if (view.IsMine)
        {
            lastAttack = Time.time;
            noOfCombo++;
            if (noOfCombo > 2)
            {
                noOfCombo = 1;
                animator_Player.SetBool("Attack2", false);

            }
            if (noOfCombo == 1)
            {
                animator_Player.SetBool("Attack2", false);
                animator_Player.SetBool("Attack1", true);
            }
            if (noOfCombo == 2)
            {
                animator_Player.SetBool("Attack1", true);
                animator_Player.SetBool("Attack2", true);
            }
        }
    }


    private void anim_fire()
    {
        if (view.IsMine)
        {
            noOfCombo = 0;
            lastAttack = Time.time;
            animator_Player.SetBool("Attack2", false);
            animator_Player.SetBool("Attack1", false);
            animator_Player.SetBool("Attack3", true);
        }   
    }


    private void anim_push()
    {
        if (view.IsMine)
        {
            noOfCombo = 0;
            lastAttack = Time.time;
            animator_Player.SetBool("Attack2", false);
            animator_Player.SetBool("Attack1", false);
            animator_Player.SetBool("Attack4", true);
        }
    }



    private void hitPlayer()
    {

        if(view.IsMine)
        {
            int ran = Random.Range(1, 100);
            float crit = 0f;
            if(ran <= critical_rate)
            {
                crit = 0.6f; ;
            }
            RaycastHit2D hit1 = Physics2D.Raycast(transform.position + direction * 0.3f, direction, 0.8f, LayerMask.GetMask("Player"));
            if (hit1.collider != null && !PhotonNetwork.CurrentRoom.Name.Contains("Map"))
            {
                
                currentPower += 1;
                powerBar.SetHealth(currentPower);
                PhotonView photonview = hit1.collider.gameObject.GetComponent<PhotonView>();
                if(photonview != null)
                {
                    PlayerController enemy = hit1.collider.GetComponent<PlayerController>();
                    if(numOfElement * enemy.numOfElement < 0)
                    {
                        enemy.beAttack((int)(attack_damage * (1.4f + crit)), amor_penetraction);
                    }
                    else
                    {
                        enemy.beAttack((int)(attack_damage * (1 + crit)), amor_penetraction);
                    }

                }

            }

            RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(0, -0.1f, 0) + direction * 0.3f, direction, 0.8f, LayerMask.GetMask("Enemy"));
            if (hit2.collider != null)
            {
                string name_angrypig = "Enemy_AngryPig";
                string name_Chicken = "Enemy_Chicken";
                string name_Duck = "Enemy_Duck";
                string name_Truck = "Enemy_Truck";
                string enemy_name = hit2.collider.gameObject.name;
                if (enemy_name.Contains(name_angrypig))
                {

                    AngryPig angrypig = hit2.collider.GetComponent<AngryPig>();
                    angrypig.BeAttack((int)(attack_damage * (1 + crit)), amor_penetraction);
                    if (angrypig.currenthealth == 0)
                    {
                        Account.Instance.experience_points += angrypig.experience_point;
                        Account.Instance.gold += angrypig.gold;
                    }

                }
                else if (enemy_name.Contains(name_Chicken))
                {

                    Chicken chicken = hit2.collider.GetComponent<Chicken>();
                    chicken.BeAttack((int)(attack_damage * (1 + crit)), amor_penetraction);
                    if (chicken.currenthealth == 0)
                    {
                        Account.Instance.experience_points += chicken.experience_point;
                        Account.Instance.gold += chicken.gold;
                    }
                }
                else if (enemy_name.Contains(name_Duck))
                {

                }
                else if (enemy_name.Contains(name_Truck))
                {

                }
            }
        }

    }

    public void Gift()
    {
        if(view.IsMine)
        {
            Account.Instance.gold += 3;
            Account.Instance.experience_points += 5;
            StartCoroutine(updateAccountTable(Account.Instance.username, Account.Instance.gold, Account.Instance.experience_points));
        }
    }
    

    public void beAttack(int damage, int am_penetraction)
    {
        
        int AmorAfterPenetraction = amor * (1 - am_penetraction / 100);
        damagebetaken = (float)(1 - 0.99 * AmorAfterPenetraction / (AmorAfterPenetraction + 60));
        isBeAttacked = true;
        canAction = false;
        lastbedamage = Time.time;
        changeHealth(-damage);
        healthBar.SetHealth(currentHealth);
        view.RPC("changeHealthBar", RpcTarget.Others, currentHealth);
        GameObject txtdamagetacken = PhotonNetwork.Instantiate(txtDamageTaken.name, transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        txtdamagetacken.GetComponentInChildren<TMP_Text>().text = ((int)(damage * damagebetaken)).ToString();
    }

    public void changeHealth(int bloodChange)
    {
        currentHealth = Mathf.Clamp(currentHealth + (int)(bloodChange * damagebetaken), 0, blood);
        
    }

    [PunRPC]
    public void changeHealthBar(int current)
    {
        isBeAttacked = true;
        canAction = false;
        lastbedamage = Time.time;
        currentHealth = current;
        healthBar.SetHealth(current);
    }


    private void Fire()
    {
        if (view.IsMine)
        {
            GameObject projectileGameObject = PhotonNetwork.Instantiate(projectilePrefab.name, transform.position + direction * 0.8f, Quaternion.identity);
            Projectile projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.attack = Property.Instance.attack_damage;
            projectile.am_penetraction = Property.Instance.amor_penetraction;
            projectile.Launch(direction, speed * 30);

        }
    }

    private void Push()
    {
        if (view.IsMine)
        {
            if (!PhotonNetwork.CurrentRoom.Name.Contains("Map"))
            {
                RaycastHit2D hit4 = Physics2D.Raycast(transform.position + direction * 0.3f, direction, 0.8f, LayerMask.GetMask("Player"));
                if (hit4.collider != null)
                {
                    hit4.transform.position += (direction * 0.7f);
                }
            }
            else
            {
                RaycastHit2D[] hit4_enemy = Physics2D.RaycastAll(transform.position + direction * 0.3f, direction, 0.8f, LayerMask.GetMask("Enemy"));
                foreach (var i in hit4_enemy)
                {
                    if (i.collider != null)
                    {
                        i.transform.position += direction * 1.2f;
                    }
                }

            }
        } 
    }

    [PunRPC]
    public void changeKnockOut()
    {
        isKnockOut = true;
        canAction = false;
    }

    public void ExecuteEndGame(int idLoser)
    {
       IdLoser = idLoser;

        // Create a content array for the event data
        object[] content = new object[] { IdLoser };

        // Raise an event to all other players
        PhotonNetwork.RaiseEvent(EventCode_Loser, content, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    private void OnEventReceived(EventData photonEvent)
    {
        if (photonEvent.Code == EventCode_Loser)
        {
            object[] data = (object[])photonEvent.CustomData;
            int idLoser = (int)data[0];

            Debug.Log("Loser ID received via RaiseEvent: " + idLoser);

            // Handle the logic of endgame here
            if (PhotonView.Get(this).ViewID != idLoser)
            {
                Gift();// give Gift for winner
            }
            panel_finish.SetActive(true);
        }
    }

    private void DebugViewID()
    {
        Debug.Log(view.ViewID);
    }

    [PunRPC]
    public void flipX(bool state)
    {
        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        sp.flipX = state; 
    }


    public void ExitToMenuGame()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }


    public override void OnLeftRoom()
    {
        if (view.IsMine)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("MenuGame");
    }

    public override void OnJoinedLobby()
    {

        if (view.IsMine)
        {
            SceneManager.LoadScene("MenuGame");
        }
    }

    IEnumerator updateAccountTable(string username, int gold, int exp)
    {
        var data = new Dictionary<string, object>
        {
            {"avartar", Account.Instance.avatar },
            {"email", Account.Instance.email },
            {"password", Account.Instance.password },
            {"gold", gold },
            {"levelID", Account.Instance.levelID },
            {"class", Account.Instance.classname },
            {"level", Account.Instance.level },
            {"exp", exp }
        };

        var task = databaseReference.Child("Account").Child(username).SetValueAsync(data);

        yield return new WaitUntil(predicate: () => task.IsCompleted);
    }

}
