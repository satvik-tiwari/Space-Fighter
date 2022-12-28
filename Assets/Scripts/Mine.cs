using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class Mine : MonoBehaviour
{
    [SerializeField] Animator animator;
    //[SerializeField] AudioClip mineBasicSfx;
    //[SerializeField] [Range(0, 1)] float mineBasicSoundVolume = 0.7f;
    [SerializeField] AudioClip mineArmedSfx;

    [SerializeField] AudioClip mineClickSfx;
    [SerializeField] [Range(0, 1)] float mineClickSoundVolume = 0.7f;
    [SerializeField] float mineArmedSoundDelay = 0.4f;
    // [SerializeField] [Range(0, 1)] float mineArmedSoundVolume = 0.7f;

    int mineDamage;
    float mineArmedPaddingZ;
    float waitTimeBeforeMineExplosion;
    float timeIntervalBetweenConsecutiveChains;
    float scanSurroundingEnemiesRadius;
    LayerMask mineLayerMask;
    GameObject mineExplosionWaveVfxPrefab;
    float durationOfMineExplosionWave;
   

    int numEnemiesInMineChain = 0;
    Rigidbody2D rb;
    Collider2D collider2D;
    List<GameObject> enemyList = new List<GameObject>();
    //DamageDealer damageDealer;
    MeshRenderer meshRenderer;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        //damageDealer = GetComponent<DamageDealer>();
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.95f, 1f);
        audioSource.Play();
        //CheckPhysics();
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetParameters(int numEnemiesInMineChain,
        int mineDamage, float mineArmedPaddingZ,
        float waitTimeBeforeMineExplosion,
        float timeIntervalBetweenConsecutiveChains,
        float scanSurroundingEnemiesRadius,
        LayerMask mineLayerMask,
        GameObject mineExplosionWaveVfxPrefab,
        float durationOfMineExplosionWave)
    {
        this.numEnemiesInMineChain = numEnemiesInMineChain;
        this.mineDamage = mineDamage;
        this.mineArmedPaddingZ = mineArmedPaddingZ;
        this.waitTimeBeforeMineExplosion = waitTimeBeforeMineExplosion;
        this.timeIntervalBetweenConsecutiveChains = timeIntervalBetweenConsecutiveChains;
        this.scanSurroundingEnemiesRadius = scanSurroundingEnemiesRadius;
        this.mineLayerMask = mineLayerMask;
        this.mineExplosionWaveVfxPrefab = mineExplosionWaveVfxPrefab;
        this.durationOfMineExplosionWave = durationOfMineExplosionWave;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        collider2D.isTrigger = false;
        rb.velocity = Vector2.zero;
        
        transform.position = new Vector3(other.transform.position.x,
            other.transform.position.y,
            other.transform.position.z + mineArmedPaddingZ);
        transform.parent = other.transform;
        animator.SetTrigger("Arm");

        audioSource.Stop();

        

        if (!(other.gameObject.tag == "Shredder"))
        {
            AudioSource.PlayClipAtPoint(mineClickSfx, Camera.main.transform.position, mineClickSoundVolume);

            audioSource.clip = mineArmedSfx;
            audioSource.PlayDelayed(mineArmedSoundDelay);
            //AudioSource.PlayClipAtPoint(mineArmedSfx, Camera.main.transform.position, mineArmedSoundVolume);

            enemyList.Add(other.gameObject);

            StartCoroutine(ExplodeMine(enemyList, other.gameObject));
        }

        
    }

    private IEnumerator ExplodeMine(List<GameObject> enemyList, GameObject enemy)
    {
        yield return new WaitForSeconds(waitTimeBeforeMineExplosion);

        audioSource.Stop();
        meshRenderer.enabled = false;
        transform.parent = null;
        ExplosionWave(enemy);
        yield return StartCoroutine(RecurrsiveChainMine(enemyList));

        Destroy(gameObject);
    }

    private IEnumerator RecurrsiveChainMine(List<GameObject> enemyList)
    {
        if(numEnemiesInMineChain <= 0 || enemyList.Count == 0)
        {
            yield return null;
        }

        else
        {
            List<Vector3> enemyPosList = new List<Vector3>();
            foreach(GameObject enemy in enemyList)
            {
                enemyPosList.Add(enemy.transform.position);
                enemy.GetComponent<Enemy>().ProcessHit(mineDamage);
                numEnemiesInMineChain--;
            }

            yield return new WaitForSeconds(timeIntervalBetweenConsecutiveChains);

            List<GameObject> newEnemyList = new List<GameObject>();     //check for duplicaytion

            foreach(Vector3 enemyPos in enemyPosList)
            {
                newEnemyList.AddRange((GetSurroundingEnemies(enemyPos)));
            }

            newEnemyList = newEnemyList.Distinct().ToList();

            yield return StartCoroutine(RecurrsiveChainMine(newEnemyList));
        }
    }

    private List<GameObject> GetSurroundingEnemies(Vector3 pos)
    {
        List<GameObject> surroundingObjects = new List<GameObject>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, scanSurroundingEnemiesRadius, mineLayerMask);

        foreach(Collider2D collider in colliders)
        {
            surroundingObjects.Add(collider.gameObject);
        }

        return surroundingObjects;
    }

    private void Destroy()
    {
        Destroy(gameObject);

    }

    private void ExplosionWave(GameObject enemy)
    {
        Color color = enemy.transform.GetChild(0).GetComponent<Renderer>().
            material.GetColor("_EmissionColor");
        GameObject explosionWaveVfx = Instantiate(mineExplosionWaveVfxPrefab,
            transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = explosionWaveVfx.GetComponent<ParticleSystem>().main;
        main.startColor = color;
        Destroy(explosionWaveVfx, durationOfMineExplosionWave);
    }

    /*private void OnDrawGizmos()
   {
       Gizmos.color = Color.red;
       Gizmos.DrawWireSphere(transform.position, 1f);
   }*/

    /*void CheckPhysics()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f, layerMask);
        foreach(Collider2D collider in colliders)
        {
            Debug.Log(collider.name + "\n");
        }
    }*/
}
