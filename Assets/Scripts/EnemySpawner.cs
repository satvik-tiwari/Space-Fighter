using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<WaveConfig> waveConfigs;
    [SerializeField] int startingWave = 0;
   // [SerializeField] bool looping = false;
    [SerializeField] float distanceFromCamera = -7.4f;

    [SerializeField]bool allWavesInstantiated = false;
    bool startSpawning = false;
    Level level;

    
    // Start is called before the first frame update
    void Start()
    {
        level = FindObjectOfType<Level>();
        
        //while (looping);
    }

    private void Update()
    {
        if(level.IsPlayerReady() && !startSpawning)
        {
            startSpawning = true;
            StartCoroutine(SpawnAllWaves());
        }
    }

    private IEnumerator SpawnAllWaves()
    {
        
        for(int waveCount = startingWave; waveCount < waveConfigs.Count; waveCount++)
        {
            var currentWave = waveConfigs[waveCount];
            yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            
        }
        allWavesInstantiated = true;
    }

    

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        for(int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++)
        {
           


            if (enemyCount == 0)
                yield return new WaitForSeconds(waveConfig.GetInitialTimeDelay());

            if (waveConfig.GetIsReflect())
            {
                Camera gameCamera = Camera.main;

                var offset = gameCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromCamera)).x;
                var targetPosition = waveConfig.GetWayPoints()[1].transform.position;
                var currentPosition = waveConfig.GetWayPoints()[0].transform.position;
                var dir = targetPosition - currentPosition;

                Quaternion rotation = Quaternion.LookRotation(dir, -Vector3.forward);

                var targetPosition2 = new Vector3(offset - targetPosition.x, targetPosition.y, targetPosition.z);
                var currentPosition2 = new Vector3(offset - currentPosition.x, currentPosition.y, currentPosition.z);
                var dir2 = targetPosition2 - currentPosition2;

                Quaternion rotation2 = Quaternion.LookRotation(dir2, -Vector3.forward);

                var newEnemy = Instantiate(
                waveConfig.GetEnemyPrefab(),
                currentPosition,
                rotation);

                newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig, false);

                var newEnemy2 = Instantiate(
            waveConfig.GetEnemyPrefab(),
            currentPosition2,
            rotation2);

                newEnemy2.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig, true);

                /*if(enemyCount == 0)
                {
                    FindObjectOfType<Player>().SetEndPoint(newEnemy);
                }*/
            }

            

            else
            {
                var targetPosition = waveConfig.GetWayPoints()[1].transform.position;
                var currentPosition = waveConfig.GetWayPoints()[0].transform.position;
                var dir = targetPosition - currentPosition;

                Quaternion rotation = Quaternion.LookRotation(dir, -Vector3.forward);

                var newEnemy = Instantiate(
                waveConfig.GetEnemyPrefab(),
                currentPosition,
                rotation);

                newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig, false);
            }
         

            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }
    }

    public bool HaveAllWavesInstantiated()
    {
        return allWavesInstantiated;
    }

    
}
