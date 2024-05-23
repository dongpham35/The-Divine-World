using Assets.Scripts.Models;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private float lastFireTime;
    private int damageSkill;
    private float damageSkill_rate;

    public int attack, am_penetraction;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastFireTime = Time.time;
    }

    private void Start()
    {
        if (gameObject.name.Contains("Aang"))//gio 27
        {
            damageSkill = 4 + (int)(0.27 * attack);
        }
        else if (gameObject.name.Contains("Katara"))//nuoc 30
        {
            damageSkill = 5 + (int)(0.3 * attack);
        }
        else if (gameObject.name.Contains("Toph"))//dat 32
        {
            damageSkill = 5 + (int)(0.32 * attack);
        }
        else if (gameObject.name.Contains("Zuko"))//lua 35
        {
            damageSkill = 7 + (int)(0.35 * attack);
        }
    }

    private void Update()
    {
        if(Time.time - lastFireTime > 2f)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        rb.AddForce(direction *  force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player") && !PhotonNetwork.CurrentRoom.Name.Contains("Map"))
        {
            PlayerController enemy = collision.collider.GetComponent<PlayerController>();
            enemy.beAttack(damageSkill, am_penetraction);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.name.Contains("AngryPig"))
            {
                AngryPig angrypig = collision.gameObject.GetComponent<AngryPig>();
                angrypig.beAttacked(damageSkill, Property.Instance.amor_penetraction);
            }else if (collision.gameObject.name.Contains("Chicken"))
            {
                Chicken chicken = collision.gameObject.GetComponent<Chicken>();
                chicken.beAttacked(damageSkill, Property.Instance.amor_penetraction);
            }else if (collision.gameObject.name.Contains("Duck"))
            {

            }else if (collision.gameObject.name.Contains("Truck"))
            {

            }
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

}
