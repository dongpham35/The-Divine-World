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

public class PlayerController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public HealthBar healthBar;
    public HealthBar powerBar;


    [SerializeField] float cooldownHit1;
    [SerializeField] float cooldownHit2;
    [SerializeField] float cooldownHit3;

    [SerializeField] AudioSource hit1;
    [SerializeField] AudioSource hit2;
    [SerializeField] AudioSource hit3;
    [SerializeField] AudioSource jump;

    PhotonView view;

    private Rigidbody2D rb;
    private SpriteRenderer spriRender_Player;
    private Animator animator_Player;
    private BoxCollider2D collider_Player;
    private Camera cam;

    private enum anim { idle, run, jump, fall}

    private anim state = anim.idle;
    private bool isBeAttacked;


    //property attack combo

    private float noOfCombo = 0;
    private float lastAttack = 0;
    private float delayCombo = 0.6f;

    private float lastbedamage;

    //property player
    private int speed ;
    private int blood ;
    private int attack_damage;
    private int amor;
    private int amor_penetraction;
    private int critical_rate;
    private int power;
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
    private int powerToUseHit2 = 15;

    public int currentHealth;
    public int gold;
    public int exp;

    private Vector3 direction;

    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }


    private float dirX;


    private void Start()
    {
        view = GetComponent<PhotonView>();
        if (!view.IsMine)
        {
            healthBar.gameObject.SetActive(false);
            powerBar.gameObject.SetActive(false);
        }
        if (view.IsMine)
        {
            cooldownHit1 = 0.3f;
            cooldownHit2 = 1f;
            cooldownHit3 = 1.5f;

            speed = Property.Instance.speed;
            blood = Property.Instance.blood;
            attack_damage = Property.Instance.attack_damage;
            amor = Property.Instance.amor;
            amor_penetraction = Property.Instance.amor_penetraction;
            critical_rate = Property.Instance.critical_rate;
            power = 100;


            rb = GetComponent<Rigidbody2D>();
            animator_Player = GetComponent<Animator>();
            spriRender_Player = GetComponent<SpriteRenderer>();
            collider_Player = GetComponent<BoxCollider2D>();

            direction = new Vector3(1, 0, 0);
            currentHealth = blood;
            currentPower = power;
            gold = Account.Instance.gold;
            exp = Account.Instance.experience_points;

            canAction = true;
            isBeAttacked = false;

            canHit1 = true;
            canHit2 = true;
            canHit3 = true;
            healthBar.SetMaxHealth(blood);
            powerBar.SetMaxHealth(power);
            Debug.Log("Blood: " + blood);
            Debug.Log("Username:  " + Account.Instance.username);
        }
    }

    private void Update()
    {

        if (view.IsMine)
        {
            if (Time.time - lastTimeUseHit1 >= cooldownHit1) canHit1 = true;
            if (Time.time - lastTimeUseHit2 >= cooldownHit2) canHit2 = true;
            if (Time.time - lastTimeUseHit3 >= cooldownHit3) canHit3 = true;
            if (currentHealth == 0)
            {
                animator_Player.SetBool("BeKnockOut", true);
                canAction = false;
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
                rb.velocity = new Vector2((float)dirX * speed * 2 / 3, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2((float)dirX * speed, rb.velocity.y);
            }

            if (Input.GetButtonDown("Vertical") && isGround() && canAction)
            {
                jump.Play();
                rb.velocity = new Vector2(rb.velocity.x, speed * 1.5f);
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
                spriRender_Player.flipX = false;
                state = anim.run;
                direction.Set(1, 0, 0);
            }
            else if (rb.velocity.x < .0f)
            {
                spriRender_Player.flipX = true;
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
            RaycastHit2D hit1 = Physics2D.Raycast(transform.position + direction * 0.3f, direction, 0.8f, LayerMask.GetMask("Player"));
            if (hit1.collider != null)
            {
                currentPower += 1;
                powerBar.SetHealth(currentPower);
                PhotonView photonview = hit1.collider.gameObject.GetComponent<PhotonView>();
                if(photonview != null)
                {
                    Debug.Log("photon view ID: " + photonview.ViewID);
                    if (!photonview.IsMine)
                    {
                        PlayerController player = hit1.collider.GetComponent<PlayerController>();
                        if (player != null)
                        {
                            player.beAttacked(attack_damage, amor_penetraction);
                            Debug.Log("Da danh" + hit1.collider.name);
                        }
                        else
                        {
                            Debug.Log("Khong tim thay player");
#if UNITY_EDITOR
                            EditorUtility.DisplayDialog("Thông báo", "Không thấy Player controller", "Ok");
#endif
                        }
                    }
                    else
                    {
                        Debug.Log("khong phai thang bj danh");
                    }
                }
                else
                {
                    Debug.Log("Khong tim thay photon view");
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "Không thấy Photon view", "Ok");
#endif 
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
                    angrypig.beAttacked(attack_damage, amor_penetraction);
                    if (angrypig.currenthealth == 0)
                    {
                        Account.Instance.experience_points += angrypig.experience_point;
                        Account.Instance.gold += angrypig.gold;
                    }

                }
                else if (enemy_name.Contains(name_Chicken))
                {

                    Chicken chicken = hit2.collider.GetComponent<Chicken>();
                    chicken.beAttacked(attack_damage, amor_penetraction);
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


    public void beAttacked(int damage, int am_penetraction)
    {
        if(view.IsMine)
        {
            isBeAttacked = true;
            int AmorAfterPenetraction = amor * (1 - am_penetraction / 100);
            damagebetaken = (float)(1 - 0.99 * AmorAfterPenetraction / (AmorAfterPenetraction + 60));
            canAction = false;
            lastbedamage = Time.time;
            changeHealth(-damage);
            healthBar.SetHealth(currentHealth);
            Debug.Log(currentHealth + "/" + blood);
        }
    }



    private void Fire()
    {
        if (view.IsMine)
        {
            GameObject projectileGameObject = PhotonNetwork.Instantiate(projectilePrefab.name, transform.position + direction * 0.3f, Quaternion.identity);
            Projectile projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.Launch(direction, speed * 15);
        }
    }

    private void Push()
    {
        if (view.IsMine)
        {
            if (!PhotonNetwork.CurrentRoom.Name.Contains("Map"))
            {
                RaycastHit2D hit4 = Physics2D.Raycast(transform.position + direction * 0.3f, direction, 1f, LayerMask.GetMask("Player"));
                if (hit4.collider != null)
                {
                    hit4.transform.position += direction * 0.7f;
                }
            }
            else
            {
                RaycastHit2D[] hit4_enemy = Physics2D.RaycastAll(transform.position + direction * 0.3f, direction, 1f, LayerMask.GetMask("Enemy"));
                foreach (var i in hit4_enemy)
                {
                    if (i.collider != null)
                    {
                        i.transform.position -= direction * 0.7f;
                    }
                }

            }
        } 
    }

    private void changeHealth(int bloodChange)
    {
        if (view.IsMine)
        {
            currentHealth = Mathf.Clamp(currentHealth + (int)(bloodChange * damagebetaken), 0, blood);
        }
    }

    IEnumerator postAccountTable_gold(string username, int gold)
    {

            string url = $"http://localhost/TheDiVWorld/api/Account?username={username}&gold={gold}";
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
                    Debug.Log("Cap nhat thanh cong bang Account");
                }
                request.Dispose();
            }

    }

    IEnumerator postAccountTable_exp(string username, int exp)
    {

            string url = $"http://localhost/TheDiVWorld/api/Account?username={username}&exp={exp}";
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
                    Debug.Log("Cap nhat thanh cong bang Account");
                }
                request.Dispose();
            }
        }

}
