using System.Collections;
using UnityEngine;

public class BombPlantingEnemy : MonoBehaviour
{
    [Header("Bomb")]
    public GameObject bombPrefab;
    public float bombFuseTime = 3f; // Fuse time before explosion
    public int bombAmount = 1; // Number of bombs the enemy can plant
    private int bombsRemaining;

    private Vector2[] directions = new Vector2[] {
        new Vector2(1, 0), // Right
        new Vector2(0, 1), // Up
        new Vector2(-1, 0), // Left
        new Vector2(0, -1) // Down
    };

    [Header("Explosion")]
    public Explosion explosionPrefab; // Explosion prefab
    public LayerMask explosionLayerMask; // Layer mask to detect bombs
    public float explosionDuration = 1f; // Duration of the explosion effect
    public int explosionRadius = 1; // Radius of the explosion

    [Header("Audio")]
    public AudioClip explosionSound; // Sound to play on explosion
    private AudioSource audioSource; // Reference to the audio source

    private void OnEnable()
    {
        bombsRemaining = bombAmount;
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        StartCoroutine(PlaceBombRoutine());
    }

    private IEnumerator PlaceBombRoutine()
    {
        while (true)
        {
            if (bombsRemaining > 0)
            {
                yield return new WaitForSeconds(Random.Range(2f, 5f)); // Random delay between bomb placements
                StartCoroutine(PlaceBomb());
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator PlaceBomb()
    {
        Vector2 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        GameObject bomb = Instantiate(bombPrefab, position, Quaternion.identity);
        bombsRemaining--;

        yield return new WaitForSeconds(bombFuseTime);

        Explode(position); // Explode at bomb position
        Destroy(bomb);
        bombsRemaining++;
    }

    private void Explode(Vector2 position)
    {
        // Play explosion sound
        PlayExplosionSound();

        // Instantiate explosion effect
        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);

        // Check in all directions for nearby bombs
        foreach (var direction in directions)
        {
            ExplodeInDirection(position, direction, explosionRadius);
        }
    }

    private void PlayExplosionSound()
    {
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
    }

    private void ExplodeInDirection(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0) return;

        position += direction;

        // Use OverlapCircleAll for circle collider detection
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, 2f, explosionLayerMask); // Adjust the radius as needed

        // Debugging: Log the number of colliders hit
        Debug.Log($"Enemy exploding at {position}, found {hitColliders.Length} colliders.");
        foreach (var collider in hitColliders)
        {
            Debug.Log($"Detected collider: {collider.name}, tag: {collider.tag}, layer: {LayerMask.LayerToName(collider.gameObject.layer)}");
        }
        // Iterate over the hit colliders to trigger nearby bombs
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Bomb"))
            {
                Debug.Log("Enemy found a bomb to detonate.");
                var bombController = hitCollider.GetComponent<BombController>();
                if (bombController != null)
                {
                    bombController.InstantDetonate(); // Detonate the bomb immediately
                }
            }
        }

        // Instantiate explosion effect
        Explosion explosionEffect = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosionEffect.SetActiveRenderer(length > 1 ? explosionEffect.middle : explosionEffect.end);
        explosionEffect.SetDirection(direction);
        explosionEffect.DestroyAfter(explosionDuration);

        // Continue the explosion chain
        ExplodeInDirection(position, direction, length - 1);
    }
}
