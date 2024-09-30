using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombController : MonoBehaviour
{
    [Header("Bomb")]
    public KeyCode inputKey = KeyCode.Space; // Key to place bomb
    public GameObject bombPrefab; // Bomb prefab
    public float bombFuseTime = 3f; // Fuse time for bomb
    public int bombAmount = 1; // Number of bombs the player can place
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

    [Header("Destructible")]
    public Tilemap destructibleTiles; // Tilemap for destructible tiles
    public Destructible destructiblePrefab; // Prefab for destructible objects

    [Header("Audio")]
    public AudioClip explosionSound; // Sound to play on explosion
    private AudioSource audioSource; // Reference to the audio source

    private void OnEnable()
    {
        bombsRemaining = bombAmount; // Reset bombs remaining when enabled
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    private void Update()
    {
        if (bombsRemaining > 0 && Input.GetKeyDown(inputKey))
        {
            StartCoroutine(PlaceBomb());
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

        Explode(position); // Explode after fuse time
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

        // Check for any destructible tiles
        ClearDestructible(position);

        // Use OverlapCircleAll for circle collider detection
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, 0.5f, explosionLayerMask); // Reduced radius for precision

        // Debugging: Log the number of colliders hit
        Debug.Log($"Exploding at {position}, found {hitColliders.Length} colliders.");
        foreach (var collider in hitColliders)
        {
            Debug.Log($"Detected collider: {collider.name}, tag: {collider.tag}, layer: {LayerMask.LayerToName(collider.gameObject.layer)}");
        }

        // Iterate over the hit colliders to trigger nearby bombs
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Bomb"))
            {
                Debug.Log("Found a bomb to detonate.");
                var bombController = hitCollider.GetComponent<BombController>();
                if (bombController != null)
                {
                    bombController.InstantDetonate(); // Immediately detonate the bomb
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

    private void ClearDestructible(Vector2 position)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        if (tile != null)
        {
            Instantiate(destructiblePrefab, position, Quaternion.identity); // Instantiate destructible effect
            destructibleTiles.SetTile(cell, null); // Remove the tile from the Tilemap
        }
    }

    public void InstantDetonate()
    {
        StopAllCoroutines(); // Stop any ongoing bomb placements
        Explode(transform.position); // Explode immediately
    }

    // Method to add bombs to the player's inventory
    public void AddBomb(int amount)
    {
        bombAmount += amount; // Increase the total number of bombs the player can place
        bombsRemaining += amount; // Update the remaining bombs accordingly
    }
}
