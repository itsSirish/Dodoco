using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedEnemy : EnemyMovement
{
    private bool shieldActive = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            if (shieldActive)
            {
                shieldActive = false;
                // Change appearance to indicate shield is broken
            }
            else
            {
                DeathSequence();
            }
        }
    }
}