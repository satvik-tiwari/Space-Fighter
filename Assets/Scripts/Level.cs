using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] int numOfEnemies;
    [SerializeField] float waitTimeForScenChange = 5f;
    [SerializeField] GameObject starFieldClose;
    [SerializeField] GameObject starFieldFar;
    [SerializeField] float waitTimeBeforeWarp = 2f;

    SceneLoader sceneLoader;
    EnemySpawner enemySpawner;
    bool isPlayerReady = false;
    BackgroundScroller backgroundScroller;

    List<Transform> enemies;
   
    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<Transform>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        backgroundScroller = FindObjectOfType<BackgroundScroller>();
        StartCoroutine(EndWarp());
        
    }

   

    // Update is called once per frame
    public void CountEnemies()
    {
        numOfEnemies++;
    }

    public void AddEnemies(Transform enemy)
    {
        enemies.Add(enemy);
    }

    private void DeleteEnemies(Transform enemy)
    {
        enemies.Remove(enemy);
    }

    public void EnemyDestroyed(Transform enemy)
    {
        DeleteEnemies(enemy);
        numOfEnemies--;
        if(numOfEnemies <=0 && enemySpawner.HaveAllWavesInstantiated())
        {
            StartCoroutine(StartWarp());
            
        }
    }

    private IEnumerator StartWarp()
    {
        yield return new WaitForSeconds(waitTimeBeforeWarp);

        starFieldClose.GetComponent<WarpParticles>().StartWarpParticles();
        starFieldFar.GetComponent<WarpParticles>().StartWarpParticles();

        FindObjectOfType<Player>().StartWarpEngines();
        backgroundScroller.SetStartWarp(true);

        StartCoroutine(GoToNextLevel());
    }

    private IEnumerator EndWarp()
    {
        FindObjectOfType<Player>().StartWarpEngines();
        backgroundScroller.SetEndWarp(true);

       // starFieldClose.GetComponent<WarpParticles>().StartWarpParticles();
        //starFieldFar.GetComponent<WarpParticles>().StartWarpParticles();

        yield return new WaitForSeconds(waitTimeForScenChange);

        starFieldClose.GetComponent<WarpParticles>().EndWarpParticles();
        starFieldFar.GetComponent<WarpParticles>().EndWarpParticles();
        FindObjectOfType<Player>().StopWarpEngines();
        backgroundScroller.SetEndWarp(false);
    }

    private IEnumerator GoToNextLevel()
    {
        yield return new WaitForSeconds(waitTimeForScenChange);
        sceneLoader.LoadNextScene();
    }

    public void SetPlayerReady(bool isPlayerReady)
    {
        this.isPlayerReady = isPlayerReady;
    }

    public bool IsPlayerReady()
    {
        return isPlayerReady;
    }

    public List<Transform> GetEnemiesAlive()
    {
        return enemies;
    }

    public Transform GetATarget()
    {
        if (enemies.Count > 0)
            return enemies[enemies.Count - 1];

        else
            return null;
    }

    public int GetLength()
    {
        return enemies.Count;
    }
}
