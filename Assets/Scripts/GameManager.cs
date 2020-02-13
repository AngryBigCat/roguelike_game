using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager instance;
    public BoardManager boardManager;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playerTurn = true;

    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    private int level = 1;
    private GameObject levelImage;
    private Text levelText;


    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();
        
        boardManager = GetComponent<BoardManager>();
        
        InitGame();
    }

    void Update()
    {
        if (playerTurn || enemiesMoving || doingSetup) return;

        StartCoroutine(MoveEnemies());
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;

        levelImage.SetActive(true);

        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();

        boardManager.SetupScene(level);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void CallbackInitialization()
    {
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    private static void onSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0) {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].MoveEnemy();

            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playerTurn = true;

        enemiesMoving = false;
    }

    public void GameOver()
    {
        enabled = false;
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);

        doingSetup = false;
    }
}
