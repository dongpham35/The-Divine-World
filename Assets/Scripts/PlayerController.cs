using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string InputHorizontal;
    public string InputVertical;
    public KeyCode keycodePlayer1_Attack1;
    public KeyCode keycodePlayer1_Attack2;
    public KeyCode keycodePlayer1_Attack3;

    private Rigidbody2D rb;
    private SpriteRenderer spriRender_Player;
    private Animator animator_Player;
    private BoxCollider2D collider_Player;
    [SerializeField]private LayerMask layerMask;

    private enum anim { idle, run, jump, fall}

    private anim state = anim.idle;


    //property attack combo

    private float noOfCombo = 0;
    private float lastAttack = 0;
    private float delayCombo = 0.6f;

    //property player
    private int speed = 10;
    private int blood = 100;
    private int attack_damage;
    private static int power = 100;


    private float startTime;


    private Vector3 direction = new Vector3 (0, 0, 0);

    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        PlayerController.instance = this;

    }

    private float dirX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator_Player = GetComponent<Animator>();
        spriRender_Player = GetComponent<SpriteRenderer>();
        collider_Player = GetComponent<BoxCollider2D>();

    }

    private void Update()
    {
        dirX = Input.GetAxisRaw(InputHorizontal);
        if (isGround())
        {
            rb.velocity = new Vector2((float)dirX * speed * 2 / 3, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2((float)dirX * speed, rb.velocity.y);
        }

        if (Input.GetButtonDown(InputVertical) && isGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, speed * 1.5f);
        }

        if (animator_Player.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && Time.time- lastAttack > 0.3)
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

        if (Input.GetKeyDown(keycodePlayer1_Attack1))
        {
            anim_Attack_Nor();
            hitPlayer();
        }

        if(Input.GetKeyDown(keycodePlayer1_Attack2))
        {
            anim_fire();
        }

        if (Input.GetKeyDown(keycodePlayer1_Attack3))
        {
            anim_push();
        }
        if (blood == 0) anim_dead();
        UpdateAnimation();
    }

    
    private void UpdateAnimation()
    {
        if(rb.velocity.x > .0f)
        {
            spriRender_Player.flipX = false;
            state = anim.run;
            direction.Set(1, 0, 0);
        }else if(rb.velocity.x < .0f)
        {
            spriRender_Player.flipX=true;
            state = anim.run;
            direction.Set(-1, 0, 0);
        }
        else
        {
            state = anim.idle;
        }

        if(rb.velocity.y > 0.1f)
        {
            state = anim.jump;
        }

        if(rb.velocity.y < -0.1f)
        {
            state = anim.fall;
        }

        animator_Player.SetInteger("StateNor", (int)state);
    }

    private bool isGround()
    {
        return Physics2D.BoxCast(collider_Player.bounds.center, collider_Player.bounds.size, 0f, Vector2.down, .1f, layerMask);
    }

    private void anim_Attack_Nor()
    {
        lastAttack = Time.time;
        noOfCombo++;
        if(noOfCombo > 2)
        {
            noOfCombo = 1;
            animator_Player.SetBool("Attack2", false);

        }
        if (noOfCombo == 1)
        {
            animator_Player.SetBool("Attack2", false);
            animator_Player.SetBool("Attack1", true);
        }
        if(noOfCombo == 2)
        {
            animator_Player.SetBool("Attack1", true);
            animator_Player.SetBool("Attack2", true);


        }
    }


    private void anim_fire()
    {
        noOfCombo = 0;
        lastAttack = Time.time;
        animator_Player.SetBool("Attack2", false);
        animator_Player.SetBool("Attack1", false);
        animator_Player.SetBool("Attack3", true);
    }


    private void anim_push()
    {
        noOfCombo = 0;
        lastAttack = Time.time;
        animator_Player.SetBool("Attack2", false);
        animator_Player.SetBool("Attack1", false);
        animator_Player.SetBool("Attack4", true);
    }

    private void anim_dead()
    {
        animator_Player.SetBool("BeKnockOut", true);
    }

    private void hitPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + direction, direction, 0.5f, LayerMask.GetMask("PLayer"));
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag(gameObject.tag))
            {
                PlayerController enemy = gameObject.GetComponent<PlayerController>();
                enemy.hitPlayer();
            }
        }
    }


    public void beAttacked()
    {
        animator_Player.SetBool("BeAttacked", true);
    }
}
