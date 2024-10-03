using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRadius = 40.0f;
    public float[] timeThresholds;
    public float[] spawnRates;
    public int threshIndex;

    bool timersGo;
    float spawnTimer;
    float sceneTimer;
    float currentSpawnRate;

    // Start is called before the first frame update

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if(SceneManager.GetActiveScene().name == "Init")
        {
            SceneManager.LoadScene("GameOver");
        }
    }
    void Start()
    {
        timersGo = false;
        spawnTimer = 0.0f;
        sceneTimer= 0.0f;
        threshIndex = 0;
        currentSpawnRate = spawnRates[0];
        spawnTimer = currentSpawnRate * 0.5f; //first enemy shows up twice as fast
    }

    // Update is called once per frame
    void Update()
    {
        if (timersGo)
        {
            sceneTimer += Time.deltaTime;
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= currentSpawnRate)
            {
                spawnEnemies(1);
                spawnTimer = 0;
            }

            if (threshIndex < timeThresholds.Length && sceneTimer >= timeThresholds[threshIndex])
            {
                threshIndex += 1;
                currentSpawnRate = spawnRates[threshIndex];
                Debug.Log(currentSpawnRate);
                Debug.Log(sceneTimer);
            }
        }    
    }

    void spawnEnemies(int amount)
    {
        for (int i = 0; i < amount; i++) {
            float randomAngle = UnityEngine.Random.Range(0.0f, 180.0f);
            Vector3 startPos = new Vector3((float)Math.Cos(Mathf.Deg2Rad * randomAngle) * spawnRadius,
                                        -30.0f,
                                        (float)Math.Sin(Mathf.Deg2Rad * randomAngle) * spawnRadius);
           
            Instantiate(enemyPrefab, startPos, Quaternion.identity);
        }
    }
    
    public void gameStart()
    {
        StartCoroutine(loadGameStart());
    }

    public void gameOver(float fadeOffest)
    {
        timersGo = false;
        sceneTimer -= fadeOffest;
        StartCoroutine(loadGameOver());
    }
    IEnumerator loadGameStart()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("Tower");
        while (!load.isDone)
        {
            yield return null;
        }
        timersGo = true;
        spawnTimer = 0.0f;
        sceneTimer = 0.0f;
        threshIndex = 0;
        currentSpawnRate = spawnRates[0];
        spawnTimer = currentSpawnRate * 0.5f;
    }

    IEnumerator loadGameOver()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("GameOver");
        while (!load.isDone)
        {
            yield return null;
        }
        TextMeshPro scoreText = GameObject.FindGameObjectWithTag("score").GetComponent<TextMeshPro>();
        String formattedTime;
        if (sceneTimer < 60)
        {
            formattedTime = ((int)sceneTimer).ToString();
        }
        else
        {
            int minutes = (int)sceneTimer / 60;
            int seconds = (int)(sceneTimer - minutes*60);
            formattedTime = minutes.ToString() + " minutes and " + seconds.ToString();
        }
        scoreText.text = "You Survived for " + formattedTime + " seconds";
    }
}
