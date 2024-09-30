using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushingEnemy : EnemyMovement
{
    public float chargeSpeed = 5f;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, player.position) < 5f) // Adjust range as needed
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            rb.velocity = directionToPlayer * chargeSpeed;
        }
        else
        {
            rb.velocity = direction * speed; // Normal movement
        }
    }
}