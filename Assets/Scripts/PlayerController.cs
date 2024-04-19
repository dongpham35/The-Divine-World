using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    private Rigidbody2D rb;
    private SpriteRenderer spriRender_Player;
    private Animator animator_Player;
    private BoxCollider2D collider_Player;
    [SerializeField]private LayerMask layerMask;

    private enum anim { idle, run, jump, fall}

    private anim state = anim.idle;

    //property attack combo
    private int comboIndex = 0;
    private bool isAttacking = false;

    //property player
    private int speed = 10;
    private int blood = 1;
    private int attack_damage;

    private Vector2 direction = new Vector2 (0, 0);

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
        //animator_Player.SetInteger("Attack", -1);

    }

    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        if (isGround())
        {
            rb.velocity = new Vector2((float)dirX * speed * 2 / 3, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2((float)dirX * speed, rb.velocity.y);
        }

        if (Input.GetButtonDown("Vertical") && isGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, speed * 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(Attack_nor());
        }
        else
        {
            animator_Player.SetInteger("Attack", -1);
        }
        Debug.Log(state);
        UpdateAnimation();
    }


    IEnumerator Attack_nor()
    {
        while(true)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                comboIndex = 0;
                rb.velocity = new Vector2(direction.x, rb.velocity.y);
                animator_Player.SetInteger("Attack", comboIndex);
            }
            else
            {
                comboIndex++;
                if (comboIndex > 1) comboIndex = 0;
                rb.velocity = new Vector2(direction.x, rb.velocity.y);
                animator_Player.SetInteger("Attack", comboIndex);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    
    private void UpdateAnimation()
    {
        if(rb.velocity.x > .0f)
        {
            spriRender_Player.flipX = false;
            state = anim.run;
            direction.Set(1, 0);
        }else if(rb.velocity.x < .0f)
        {
            spriRender_Player.flipX=true;
            state = anim.run;
            direction.Set(-1, 0);
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
}
