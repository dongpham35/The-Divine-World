using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AngryPig : MonoBehaviour
{
    public Transform character;
    private bool isSeeplayer = false;

    [Header("property of the angry pig")]
    [SerializeField] private int blood = 20;
    [SerializeField] private int attack_damage = 10;
    [SerializeField] private int amor = 10;
    [SerializeField] private int speed = 5;
    [SerializeField] private int critical_rate = 0;
    [SerializeField] private int amor_penetraction = 0;
    public int gold = 3;
    public int experience_point = 10;

    private Animator animAngryPig;
    private SpriteRenderer spriteAngryPig;
    private PhotonView view;
    private Rigidbody2D rb;

    private bool isColliderPLayer;
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

        if(character == null)
        {
            character = GameObject.FindGameObjectWithTag("Player").transform;
        }
        currenthealth = blood;
        
        isColliderPLayer = false;
        canAction = true;

        direction = new Vector3(1, 0, 0);
    }

    private void Update()
    {
        if (view.IsMine)
        {
            if (currenthealth == 0)
            {
                animAngryPig.SetBool("hit2", true);
                Destroy(gameObject);
            }

            if(Time.time - lastbedamage > 1f)
            {
                animAngryPig.SetBool("hit1", false);
                canAction = true;
            }

            if (isColliderPLayer && canAction)
            {
                if (Time.time - lastDamageTime > 2f)
                {
                    PlayerController player = character.GetComponent<PlayerController>();
                    player.beAttacked(attack_damage, amor_penetraction);
                    lastDamageTime = Time.time;
                }
            }
            
            if(Vector2.Distance(transform.position, character.position) < 4f)
            {
                isSeeplayer = true;
            }
            if (!isSeeplayer)
            {
                MoveNormalState();
            }
            else
            {
                
                direction = (character.position - transform.position).normalized;
                MoveNormalState();
            }

        }
    }

    private void MoveNormalState()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + direction * 0.1f, direction, 0.7f, LayerMask.GetMask("Ground"));
        if(hit.collider != null && isSeeplayer)
        {
            rb.velocity = new Vector2(rb.velocity.x * 1.2f, speed * 2.3f);
        }else if (hit.collider != null && !isSeeplayer)
        {
            direction = -direction;
        }

        rb.velocity = new Vector2(speed * direction.x, rb.velocity.y);
        if(rb.velocity.x > 0)
        {
            animAngryPig.SetInteger("StateNor", 1);
            spriteAngryPig.flipX = true;
        }else if(rb.velocity.x < 0)
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


    public void beAttacked(int attack, int am_penetraction)
    {
        int AmorAfterPenetraction = amor * ( 1 - am_penetraction / 100);
        damagebetaken = (float)(1 - 0.99 * AmorAfterPenetraction / (AmorAfterPenetraction + 60));
        canAction = false;
        lastbedamage = Time.time;
        animAngryPig.SetBool("hit1", true);
        currenthealth = Mathf.Clamp(currenthealth - (int)(attack * damagebetaken), 0, blood);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isColliderPLayer)
        {
            isColliderPLayer = true;
            PlayerController player = character.GetComponent<PlayerController>();
            player.beAttacked(attack_damage, amor_penetraction);
            lastDamageTime = Time.time;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isColliderPLayer = false;
        }
    }
}
