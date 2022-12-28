using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 100f;
    [SerializeField] int scoreValue = 10;
    [SerializeField] HealthBar healthBar;
    //[SerializeField] bool healthBarActive = true;

    [Header("Shooting")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileVelocity = 10f;
    [SerializeField] bool addRandomVelocityAtProjectile = false;

    [Header("Enemy Destroy VFX")]
    [SerializeField] GameObject enemyDestroyVfxPrefab;
  
    [SerializeField] GameObject enemyDamageVfxPrefab;
    [SerializeField] float durationOfExplosion = 0.7f;

    [Header("Sound VFX")]
    [SerializeField] AudioClip deathSfx;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.7f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;
   // [SerializeField] AudioClip damageSound;
   // [SerializeField] [Range(0, 1)] float damageSoundVolume = 0.25f;

    [Header("Player Powe Up Drop")]
    [SerializeField] GameObject[] powerUps;
    [SerializeField] float[] powerUpDropProbabilites;

    Level level;
    // private List<GameObject> projectilesList;

    // Start is called before the first frame update
    void Start()
    {
        // projectilesList = new List<GameObject>();
        level = FindObjectOfType<Level>();
        level.CountEnemies();
        level.AddEnemies(transform);

        if (healthBar != null)
          healthBar.SetMaxHealth(health);

        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);

    }

    // Update is called once per frame
    void Update()
    {
        if (projectilePrefab != null)
            CountDownAndShoot();

       /* if(addRandomVelocityAtProjectile && projectilesList != null)
        {
            for(int i = 0; i < projectilesList.Count; i++)
            {
                projectilesList[i].GetComponent<Rigidbody2D>().velocity =
                    projectilesList[i].GetComponent<Rigidbody2D>().velocity.magnitude *
                    new Vector2(UnityEngine.Random.Range(-0.02f, 0.02f),
                    UnityEngine.Random.Range(-1f, 0)).normalized;
            }
        }*/
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if(shotCounter <= 0)
        {
            Fire();
            shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject projectile = Instantiate(
            projectilePrefab, 
            transform.position,
            Quaternion.identity) as GameObject; 
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileVelocity);
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
        // projectilesList.Add(projectile);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        //if (damageDealer.name.StartsWith("Mine")) { return;  }
        if (!damageDealer) { return; }

        ProcessHit(damageDealer);
        
    }

    public void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        if(healthBar != null)
          healthBar.SetHealth(health);
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }

        else
        {
            TakeDamage(damageDealer.transform);
        }
    }

    public void ProcessHit(int damage) //overloading function processhit
    {
        health -= damage;
        if (healthBar != null)
            healthBar.SetHealth(health);

        if (health <= 0)
        {
            Die();
        }

        else
        {
            TakeDamage(transform);
        }
    }

    private void TakeDamage(Transform transform)
    {
        GameObject enemyDamageVfx = Instantiate(enemyDamageVfxPrefab,
            transform.position,
            this.transform.rotation);

        Destroy(enemyDamageVfx, durationOfExplosion);

      /*  AudioSource.PlayClipAtPoint(damageSound, Camera.main.transform.position,
            damageSoundVolume);*/
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        level.EnemyDestroyed(transform);
        //level.DeleteEnemies(transform);
       
        Destroy(gameObject);
        DropPowerUp();
        

       // GameObject enemyDestroyVfx2 = Instantiate(enemyDestroyVFxPrefab2, transform.position, transform.rotation);
        GameObject enemyDestroyVfx = Instantiate(enemyDestroyVfxPrefab, transform.position, transform.rotation);

        Destroy(enemyDestroyVfx, durationOfExplosion);
        //Destroy(enemyDestroyVfx2, durationOfExplosion);

        AudioSource.PlayClipAtPoint(deathSfx, Camera.main.transform.position, deathSoundVolume);
    }

    void DropPowerUp()
    {
        
        for(int itr = 0; itr < powerUps.Length; itr++)
        {
            float randomNum = UnityEngine.Random.Range(0.0f, 1.0f);
           // Debug.Log(randomNum);
            if(randomNum >= powerUpDropProbabilites[itr])
            {
                Instantiate(powerUps[itr], transform.position, Quaternion.identity);
                break;
            }
        }
        
       // float randomNum = UnityEngine.Random.Range(0, 1);
    }

    public float GetEnemyHealth()
    {
        return health;
    }
}
