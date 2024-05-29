using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class txtDamageMove : MonoBehaviour
{
    PhotonView view;
    Rigidbody2D rb;
    Vector3 direction;
    float timeSpawn;
    private void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            timeSpawn = Time.time;
            rb = GetComponent<Rigidbody2D>();
            int ran = Random.Range(-1, 1);
            direction = new Vector3(ran, 1, 0);
            rb.velocity = direction;
        }
    }

    private void Update()
    {
            if (Time.time - timeSpawn >= 0.5f)
            {
                Destroy(gameObject);
            }
    }
}
