using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameObject[] enemies;
    private GameObject player;
    private BombController playerBomb;
    private MovementController playerMovement;
    public float totalTime = 201;
    private float startTime;

    [Header("UI Element")]
    public TextMeshProUGUI speed;
    public TextMeshProUGUI bombRadius;
    public TextMeshProUGUI bombCount;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameMessage;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        player = GameObject.FindGameObjectWithTag("Player");
        playerBomb = player.GetComponent<BombController>();
        playerMovement = player.GetComponent<MovementController>();
        gameMessage.gameObject.SetActive(false);
        startTime = Time.time;
    }

    private void Update()
    {
        speed.text = playerMovement.speed.ToString();
        bombRadius.text = playerBomb.explosionRadius.ToString();
        bombCount.text = playerBomb.bombAmount.ToString();
        float timer = Mathf.Floor(totalTime - (Time.time - startTime));
        timerText.text = "Time Left: " + (timer >= 0 ? timer.ToString() : "0");
        if (timer < 0)
        {
            GameFailed();
        }
    }

    public void CheckWinState()
    {
        int aliveCount = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].activeSelf) {
                aliveCount++;
            }
        }

        if (aliveCount <= 0) {
            // next level
            gameMessage.text = "Congratulations";
            gameMessage.gameObject.SetActive(true);
            //Invoke(nameof(NewRound), 4f);
        }
    }

    public void GameFailed()
    {
        gameMessage.text = "You Failed";
        gameMessage.gameObject.SetActive(true);
        Invoke(nameof(NewRound), 4f);
    }

    //restart this level
    private void NewRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
