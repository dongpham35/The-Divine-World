using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    public Transform character;
    private bool isSeeplayer = false;

    [Header("property of the Chicken")]
    [SerializeField] private int blood = 15;
    [SerializeField] private int attack_damage = 1;
    [SerializeField] private int amor = 3;
    [SerializeField] private int speed = 4;
    [SerializeField] private int critical_rate = 0;
    [SerializeField] private int amor_penetraction = 0;
    public int gold = 1;
    public int experience_point = 3;

    private Animator animChicken;
    private SpriteRenderer spriteChicken;
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
        spriteChicken = GetComponent<SpriteRenderer>();
        animChicken = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();

        if (character == null)
        {
            character = GameObject.FindGameObjectWithTag("Player").transform;
        }
        currenthealth = blood;
        canAction = true;

        direction = new Vector3(1, 0, 0);
    }

    private void Update()
    {
        if (view.IsMine)
        {
            if (currenthealth == 0)
            {
                animChicken.SetBool("hit1", true);
                Destroy(gameObject);
            }

            if (Time.time - lastbedamage > 1f)
            {
                animChicken.SetBool("hit1", false);
                canAction = true;
            }


            if (Vector2.Distance(transform.position, character.position) < 2f)
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
            hitPlayer();
        }
    }

    private void MoveNormalState()
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
        if(Vector2.Distance(transform.position, character.position) < 0.7f)
        {
            rb.velocity = Vector2.zero;
        }
        if (rb.velocity.x > 0)
        {
            animChicken.SetInteger("StateNor", 1);
            spriteChicken.flipX = true;
        }
        else if (rb.velocity.x < 0)
        {
            animChicken.SetInteger("StateNor", 1);
            spriteChicken.flipX = false;
        }   
        else
        {
            animChicken.SetInteger("StateNor", 0);
        }
        if (isSeeplayer)
        {
            animChicken.SetInteger("StateNor", 1);
        }
        
    }


    public void beAttacked(int attack, int am_penetraction)
    {
        int AmorAfterPenetraction = amor * (1 - am_penetraction / 100);
        damagebetaken = (float)(1 - 0.99 * AmorAfterPenetraction / (AmorAfterPenetraction + 60));
        canAction = false;
        lastbedamage = Time.time;
        animChicken.SetBool("hit1", true);
        currenthealth = Mathf.Clamp(currenthealth - (int)(attack * damagebetaken), 0, blood);
    }

    private void hitPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + direction * 0.3f, direction, 0.5f, LayerMask.GetMask("Player"));
        if(hit.collider != null && Time.time - lastDamageTime > 1.5f)
        {
            PlayerController player = character.GetComponent<PlayerController>();
            player.beAttack(attack_damage, amor_penetraction);
            lastDamageTime = Time.time;
        }
    }
}
