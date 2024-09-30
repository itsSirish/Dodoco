// using System.Collections;
// using UnityEngine;

// public class EnemyBombController : MonoBehaviour
// {
//     [Header("Bomb Settings")]
//     public GameObject bombPrefab; // Prefab for the bomb
//     public float bombFuseTime = 3f; // Time until the bomb explodes
//     public int bombAmount = 1; // Max bombs an enemy can place
//     private int bombsRemaining;

//     private Vector2[] directions = new Vector2[]
//     {
//         new Vector2(1, 0),   // Right
//         new Vector2(0, 1),   // Up
//         new Vector2(-1, 0),  // Left
//         new Vector2(0, -1)   // Down
//     };

//     [Header("Explosion Settings")]
//     public Explosion explosionPrefab;
//     public LayerMask explosionLayerMask;
//     public float explosionDuration = 1f;
//     public int explosionRadius = 1;

//     [Header("Bomb Timer")]
//     public float bombPlacementInterval = 5f; // Time between bomb placements
//     private bool placingBombs = true;

//     private void Start()
//     {
//         bombsRemaining = bombAmount;
//         StartCoroutine(PlaceBombAutomatically());
//     }

//     private IEnumerator PlaceBombAutomatically()
//     {
//         while (placingBombs)
//         {
//             if (bombsRemaining > 0)
//             {
//                 StartCoroutine(PlaceBomb());
//                 yield return new WaitForSeconds(bombPlacementInterval);
//             }
//             else
//             {
//                 yield return null;
//             }
//         }
//     }

//     protected IEnumerator PlaceBomb()
//     {
//         Vector2 position = transform.position;
//         position.x = Mathf.Round(position.x);
//         position.y = Mathf.Round(position.y);

//         GameObject bomb = Instantiate(bombPrefab, position, Quaternion.identity);
//         bombsRemaining--;

//         yield return new WaitForSeconds(bombFuseTime);

//         position = bomb.transform.position;
//         position.x = Mathf.Round(position.x);
//         position.y = Mathf.Round(position.y);

//         Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
//         explosion.SetActiveRenderer(explosion.start);
//         explosion.DestroyAfter(explosionDuration);

//         Explode(position, Vector2.up, explosionRadius);
//         Explode(position, Vector2.down, explosionRadius);
//         Explode(position, Vector2.left, explosionRadius);
//         Explode(position, Vector2.right, explosionRadius);

//         Destroy(bomb);
//         bombsRemaining++;
//     }

//     private void Explode(Vector2 position, Vector2 direction, int length)
//     {
//         if (length <= 0)
//         {
//             return;
//         }

//         position += direction;

//         // Check for player bomb in the explosion range
//         Collider2D hit = Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask);
//         if (hit && hit.gameObject.CompareTag("PlayerBomb"))
//         {
//             // Detonate the player bomb if within range
//             BombController playerBombController = hit.gameObject.GetComponent<BombController>();
//             if (playerBombController != null)
//             {
//                 playerBombController.StartCoroutine(playerBombController.PlaceBomb());
//             }
//             return;
//         }

//         // Explosion visual
//         Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
//         explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
//         explosion.SetDirection(direction);
//         explosion.DestroyAfter(explosionDuration);

//         Explode(position, direction, length - 1);
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
//         {
//             other.isTrigger = false;
//         }
//     }
// }
