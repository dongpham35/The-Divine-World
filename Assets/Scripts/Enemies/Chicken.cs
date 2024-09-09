using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Chicken : MonoBehaviour, IEnemy
{
    public List<GameObject> characters;
    private GameObject character;
    private bool isSeeplayer = false;

    [Header("property of the Chicken")]
    [SerializeField] private int blood = 15;
    [SerializeField] private int attack_damage = 1;
    [SerializeField] private int amor = 3;
    [SerializeField] private int speed = 4;
    [SerializeField] private int amor_penetraction = 0;
    public int gold = 1;
    public int experience_point = 1;

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

    private float cooldownAction = 1f;// default cooldown = 1s

    private void Start()
    {
        spriteChicken = GetComponent<SpriteRenderer>();
        animChicken = GetComponent<Animator>();
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
            if (currenthealth == 0)
            {
                animChicken.SetBool("hit1", true);
                Destroy(gameObject);
            }
            if (!canAction)
            {
                lastbedamage -= Time.deltaTime;
                if (lastbedamage <= 0)
                {
                    canAction = true;
                }
                return;
            }
            if (character == null)
            {
                characters = GameObject.FindGameObjectsWithTag("Player").ToList();
            }
            CheckDistance();
            

            if (lastbedamage <= 0f && canAction)
            {
                animChicken.SetBool("hit1", false);
            }

            
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (character == null) return;
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


    public void BeAttack(int attack, int am_penetraction)
    {
        int AmorAfterPenetraction = amor * (1 - am_penetraction / 100);
        damagebetaken = (float)(1 - 0.99 * AmorAfterPenetraction / (AmorAfterPenetraction + 60));
        canAction = false;
        lastbedamage = cooldownAction;
        animChicken.SetBool("hit1", true);
        currenthealth = Mathf.Clamp(currenthealth - (int)(attack * damagebetaken), 0, blood);
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
