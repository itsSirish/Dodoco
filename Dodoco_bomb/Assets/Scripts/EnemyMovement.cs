using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    protected Rigidbody2D rb;
    private Vector2 lastPos;
    protected Vector2 direction = Vector2.down;
    public float speed = 2.5f;

    public float gridSize = 1.0f; // Size of each grid cell


    public int damageToPlayer = 10;  // Damage to player when colliding

    [Header("Sprites")]
    public AnimatedSpriteRenderer spriteRendererUp;
    public AnimatedSpriteRenderer spriteRendererDown;
    public AnimatedSpriteRenderer spriteRendererLeft;
    public AnimatedSpriteRenderer spriteRendererRight;
    public AnimatedSpriteRenderer spriteRendererDeath;
    private AnimatedSpriteRenderer activeSpriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown;
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        if (Vector2.Distance(position, lastPos) < 0.0001f)  // if meeting a wall, change a random direction
        {
            Vector2[] directions = { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
            AnimatedSpriteRenderer[] sprites = { spriteRendererDown, spriteRendererLeft, spriteRendererUp, spriteRendererRight };
            int dire;
            do
            {
                dire = Random.Range(0, 4);
            } while (directions[dire] == direction);
            SetDirection(directions[dire], sprites[dire]);
        }

        Vector2 translation = speed * Time.fixedDeltaTime * direction;
        rb.MovePosition(position + translation);
        lastPos = rb.position;
    }

    private void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer)
    {
        direction = newDirection;

        spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
        spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
        spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
        spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;

        activeSpriteRenderer = spriteRenderer;
        activeSpriteRenderer.idle = direction == Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision with the player
        // if (collision.gameObject.CompareTag("Player"))
        // {
        //     PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        //     if (playerHealth != null)
        //     {
        //         playerHealth.TakeDamage(damageToPlayer);  // Player takes damage
        //     }
        // }

        // Collision with another enemy, reverse direction
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Calculate a new random direction that is different from the current one
            Vector2[] possibleDirections = { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
            List<Vector2> validDirections = new List<Vector2>(possibleDirections);

            // Remove current direction from possible directions
            validDirections.Remove(direction);

            // Choose a new random direction from valid options
            int index = Random.Range(0, validDirections.Count);
            SetDirection(validDirections[index], GetSpriteRendererForDirection(validDirections[index]));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            DeathSequence();
        }
    }

    protected void DeathSequence()
    {
        enabled = false;

        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = false;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;
        spriteRendererDeath.enabled = true;

        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }

    private void OnDeathSequenceEnded()
    {
        gameObject.SetActive(false);
        EnemyManager.Instance.RemoveEnemy(this.gameObject);  // Notify EnemyManager to remove this enemy
    }

    private AnimatedSpriteRenderer GetSpriteRendererForDirection(Vector2 dir)
    {
        if (dir == Vector2.up) return spriteRendererUp;
        if (dir == Vector2.down) return spriteRendererDown;
        if (dir == Vector2.left) return spriteRendererLeft;
        if (dir == Vector2.right) return spriteRendererRight;

        return spriteRendererDown; // Default case
    }

}
