using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AngryPig : MonoBehaviour, IEnemy
{
    public List<GameObject> characters;
    private GameObject character;
    private bool isSeeplayer = false;

    [Header("property of the angry pig")]
    [SerializeField] private int blood = 20;
    [SerializeField] private int attack_damage = 10;
    [SerializeField] private int amor = 10;
    [SerializeField] private int speed = 5;
    [SerializeField] private int critical_rate = 0;
    [SerializeField] private int amor_penetraction = 0;
    public int gold = 2;
    public int experience_point = 3;

    private Animator animAngryPig;
    private SpriteRenderer spriteAngryPig;
    private PhotonView view;
    private Rigidbody2D rb;

    private float lastDamageTime;

    private float damagebetaken; //%
    internal int currenthealth;
    private float lastbedamage;
    private bool canAction;

    private Vector3 direction;

    private void Start()
    {
        spriteAngryPig = GetComponent<SpriteRenderer>();
        animAngryPig = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();

        if (characters.Count == 0)
        {
            characters = GameObject.FindGameObjectsWithTag("Player").ToList();
        }
        currenthealth = blood;
        canAction = true;

        direction = new Vector3(1, 0, 0);
    }

    

    private void Update()
    {
        if (view.IsMine)
        {
            if (character == null)
            {
                characters = GameObject.FindGameObjectsWithTag("Player").ToList();
            }
            CheckDistance();
            if (currenthealth == 0)
            {
                animAngryPig.SetBool("hit1", true);
                Destroy(gameObject);
            }

            if (Time.time - lastbedamage > 1f)
            {
                animAngryPig.SetBool("hit1", false);
                canAction = true;
            }


            if (Vector2.Distance(transform.position, character.transform.position) < 2f)
            {
                isSeeplayer = true;
            }
            if (!isSeeplayer)
            {
                MoveNorState();
            }
            else
            {

                direction = (character.transform.position - transform.position).normalized;
                MoveNorState();
            }
            HitPLayer();
        }
    }


    public void CheckDistance()
    {
        float distance = 0f;
        foreach (var ch in characters)
        {
            if (view.IsMine)
            {
                if (distance == 0)
                {
                    distance = Vector2.Distance(transform.position, ch.transform.position);
                    character = ch;
                    continue;
                }
                if (distance > Vector2.Distance(transform.position, ch.transform.position))
                {
                    distance = Vector2.Distance(transform.position, ch.transform.position);
                    character = ch;
                }
            }
        }
    }
    public void MoveNorState()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + direction * 0.1f, direction, 0.7f, LayerMask.GetMask("Ground"));
        if (hit.collider != null && isSeeplayer)
        {
            rb.velocity = new Vector2(rb.velocity.x * 1.2f, speed * 2.3f);
        }
        else if (hit.collider != null && !isSeeplayer)
        {
            direction = -direction;
        }

        rb.velocity = new Vector2(speed * direction.x, rb.velocity.y);
        if (Vector2.Distance(transform.position, character.transform.position) < 0.7f)
        {
            rb.velocity = Vector2.zero;
        }
        if (rb.velocity.x > 0)
        {
            animAngryPig.SetInteger("StateNor", 1);
            spriteAngryPig.flipX = true;
        }
        else if (rb.velocity.x < 0)
        {
            animAngryPig.SetInteger("StateNor", 1);
            spriteAngryPig.flipX = false;
        }
        else
        {
            animAngryPig.SetInteger("StateNor", 0);
        }
        if (isSeeplayer)
        {
            animAngryPig.SetInteger("StateNor", 2);
        }

    }



    public void BeAttack(int attack_damage, int amor_penetraction)
    {
        int AmorAfterPenetraction = amor * (1 - amor_penetraction / 100);
        damagebetaken = (float)(1 - 0.99 * AmorAfterPenetraction / (AmorAfterPenetraction + 60));
        canAction = false;
        lastbedamage = Time.time;
        animAngryPig.SetBool("hit1", true);
        currenthealth = Mathf.Clamp(currenthealth - (int)(attack_damage * damagebetaken), 0, blood);
    }

    public void HitPLayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + direction * 0.3f, direction, 0.3f, LayerMask.GetMask("Player"));
        if (hit.collider != null && Time.time - lastDamageTime > 1.5f)
        {
            PlayerController player = hit.collider.GetComponent<PlayerController>();
            player.beAttack(attack_damage, amor_penetraction);
            lastDamageTime = Time.time;
        }
    }
}
