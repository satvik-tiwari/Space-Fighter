using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class Player : MonoBehaviour
{
    //configuration parameters
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float paddingX = 1f;
    [SerializeField] float paddingY = 1f;
    [SerializeField] float health = 200f;
    [SerializeField] AudioClip deathSfx;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.7f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;
    [SerializeField] GameObject playerDestroyVfxPrefab;
    [SerializeField] GameObject playerDestroyVFxPrefab2;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] float warpEngineLifetime = 5f;
    [SerializeField] float initialWarpEngineLifetime = 1.45f;
    [SerializeField] float warpEnginespeed = 1.35f;
    [SerializeField] float initialWarpEnginespeed = 0.77f;
    //[]

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.1f;
    [SerializeField] float waitTime = 0.8f;
    [SerializeField] Vector2 laserPadding, minePadding =  new Vector2(0, 1.235522f);
    [SerializeField] Vector2 laserPadding1 = new Vector2(0.4f, 0.15f);
    [SerializeField] Vector2 laserPadding2 = new Vector2(0.8f, 0.15f);

    [Header("WarpShields")]
    [SerializeField] GameObject shield;
   // [SerializeField] GameObject shield2;
    [SerializeField] float shieldActiveTime = 5f;

    float missileFiringPeriod, missilePaddingX,
          missilePaddingY, missileSpeed;

    float homingMissileFiringPeriod, homingMissilePaddingX,
          homingMissilePaddingY, homingMissileSpeed,
            homingMissileRotateSpeed;

    float mineFireSpeed, mineArmedPaddingZ,
        waitTimeBeforeMineExplosion,
        timeIntervalBetweenConsecutiveChains,
        scanSurroundingEnemiesRadius,
        durationOfMineExplosionWave, durationOfShield;
    

    float xMin, xMax, yMin, yMax;
    float timer = 0f;
    //float homingMissileDamage;
    
    Coroutine firingCoroutine;
    Coroutine missileCoroutine;
    Vector3 laserPos, minePos;
    Vector3 laserPos1;
    Vector3 laserPos2;
    Vector3 missilePos1;
    Vector3 missilePos2;
    Level level;


    bool firing = false, fireSafety = false, isMissileActive = false,
    missileFiring = false, isHomingMissilesReady = false,
    activateHomingMissile = false, isMineReady = false,
        isShieldReady = false;

    int numPrimaryWeaponBullets = 1;
    int numHomingMissiles;
    int numHomingMissilePowerups = 0,
        numMinePowerUps = 0, numEnemiesInMineChain,
        mineDamage;
    int numOfShieldPowerUp;

    GameObject missileLPrefab, missileRPrefab;
    GameObject homingMissilePrefabL, homingMissilePrefabR,
        minePrefab, mineExplosionWaveVfxPrefab,
        shieldPrefab;

    LayerMask mineLayerMask;

   

    // Start is called before the first frame update
    void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        SetUpBoundaries();
        level = FindObjectOfType<Level>();
        StartCoroutine(DeactivateShields());
        
       
        //recoil = GetComponent<RecoilScriptDone>();
    }

   /* public void SetEndPoint(GameObject enemy)
    {
        currenemy = enemy;
    }*/
    // Update is called once per frame
    void Update()
    {
        /*electricBolt.transform.GetChild(0).transform.position = transform.position;
        if(currenemy != null)
        {
            electricBolt.transform.GetChild(1).transform.position = currenemy.transform.position;
        }*/
        Move();
        Fire();
        NormalMissiles();
        HomingMissiles();
        //Electro();
        Mines();
        Shield();
        //Test();
        
    }



    /*private void Test()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            Mines();
        }
    }*/



    /*private void Electro()
    {
       if(Input.GetButtonDown("Fire3") &&
            isElectroReady)
        {
            isElectroReady = false;
            //LockPrimaryWeapons(true);
            Transform[] activeEnemies = level.GetEnemiesAlive().ToArray();
          //  StartCoroutine(FireElectro(activeEnemies));
        }
    }*/

    //  private IEnumerator FireElectro(Transform[] activeEnemies)
    // {

    //  }
    private void Shield()
    {
        if(Input.GetKeyDown(KeyCode.T) && 
            isShieldReady && numOfShieldPowerUp > 0)
        {
            isShieldReady = false;
            numOfShieldPowerUp--;
            StartCoroutine(ActivateShield());
        }
    }

    private IEnumerator ActivateShield()
    {
        GameObject shield = Instantiate(shieldPrefab,
            transform.position, Quaternion.identity);
        shield.transform.parent = transform;

        yield return new WaitForSeconds(durationOfShield);
        Destroy(shield);
        isShieldReady = true;
    }

    private void Mines()
    {
        if (Input.GetButtonDown("Fire3") &&
            isMineReady &&
            numMinePowerUps > 0)
        {
            isMineReady = false;
            LockPrimaryWeapons(true);
            numMinePowerUps--;
            
            FireMines();
        }
    }

    private void FireMines()
    {

        minePos = new Vector3(transform.position.x,
                       transform.position.y + minePadding.y,
                       transform.position.z);

        GameObject mine = Instantiate(minePrefab,
                       //transform.position,
                       minePos,
                       Quaternion.identity) as GameObject;

        mine.GetComponent<Mine>().SetParameters(numEnemiesInMineChain,
            mineDamage, mineArmedPaddingZ,
            waitTimeBeforeMineExplosion,
            timeIntervalBetweenConsecutiveChains,
            scanSurroundingEnemiesRadius,
            mineLayerMask,
            mineExplosionWaveVfxPrefab,
            durationOfMineExplosionWave);

        mine.GetComponent<Rigidbody2D>().velocity = new Vector2(0, mineFireSpeed);


        isMineReady = true;
        LockPrimaryWeapons(false);
    }

    private void HomingMissiles()
    {
        if (Input.GetButtonDown("Fire2") && 
            isHomingMissilesReady && 
            numHomingMissilePowerups > 0)
        {
            isHomingMissilesReady = false;
            LockPrimaryWeapons(true);
            numHomingMissilePowerups--;
            Transform[] activeEnemies = level.GetEnemiesAlive().ToArray();
            StartCoroutine(FireHomingMissiles(activeEnemies));
        }
    }

    private void NormalMissiles()
    {
        if (isMissileActive && !missileFiring)
            missileCoroutine = StartCoroutine(ContinouslyFireMissile());
    }

    private IEnumerator FireHomingMissiles(Transform[] activeEnemies)
    {
        int activeEnemiesCount = activeEnemies.Length;
        int itr = 0;
        int homingMissilesCount = 0;
        bool changeActiveEnemy = false;

        if (activeEnemiesCount == 0)
            goto EndOfCoroutine;

        Transform activeEnemy = activeEnemies[itr];
        float currentEnemyHelath = activeEnemy.GetComponent<Enemy>().GetEnemyHealth();

        float homingMissileDamage = homingMissilePrefabL.GetComponent<DamageDealer>().GetDamage();

        //int length = activeEnemies.Count;
        while(itr < activeEnemiesCount && homingMissilesCount <= numHomingMissiles-1)
        {
            //Debug.Log(numHomingMissiles + "total");
            var homingMissilePos1 = new Vector3(transform.position.x + homingMissilePaddingX,
                transform.position.y + homingMissilePaddingY,
                transform.position.z);

            var homingMissilePos2 = new Vector3(transform.position.x - homingMissilePaddingX,
                transform.position.y + homingMissilePaddingY,
                transform.position.z);

            //Debug.Log(itr + ", " + activeEnemiesCount + ", " + activeEnemies.Length);
            if (changeActiveEnemy)
            {
                do
                {
                    itr++;
                    if (itr >= activeEnemiesCount)
                    {
                        Debug.Log("Itr : " + itr);
                        goto EndOfCoroutine;
                    }
                       
                    activeEnemy = activeEnemies[itr];
                    
                } while (activeEnemy == null) ;

                    currentEnemyHelath = activeEnemy.GetComponent<Enemy>().GetEnemyHealth();
            }

          //  if (activeEnemy != null)
         //   {
                GameObject homingMissile1 = Instantiate(homingMissilePrefabL,
                homingMissilePos1,
                Quaternion.identity) as GameObject;
                homingMissile1.GetComponent<HomingMissile>().SetTarget(activeEnemy,
                    homingMissileSpeed, homingMissileRotateSpeed);

                currentEnemyHelath -= homingMissileDamage;
                homingMissilesCount++;

                if (currentEnemyHelath <= 0)
                {
                    
                   
                    changeActiveEnemy = true;
                }

                else
                {
                    
                    changeActiveEnemy = false;
                }
               // Debug.Log("Missile count : " + homingMissilesCount);
          //  }

          //  else
          //  {
           //     itr++;
          //  }

            Debug.Log(itr + ", " + activeEnemiesCount + ", " + activeEnemies.Length);

            if (itr < activeEnemiesCount && homingMissilesCount <= numHomingMissiles-1)
            {
                if (changeActiveEnemy)
                {
                    do
                    {
                        itr++;
                        if (itr >= activeEnemiesCount)
                        {
                            Debug.Log("Itr : " + itr);
                            goto EndOfCoroutine;
                        }
                        activeEnemy = activeEnemies[itr];
                    } while (activeEnemy == null) ;
                        currentEnemyHelath = activeEnemy.GetComponent<Enemy>().GetEnemyHealth();
                }

              //  if(activeEnemy != null)
             //   {
                    GameObject homingMissile2 = Instantiate(homingMissilePrefabR,
                               homingMissilePos2,
                               Quaternion.identity) as GameObject;
                    homingMissile2.GetComponent<HomingMissile>().SetTarget(activeEnemy,
                        homingMissileSpeed, homingMissileRotateSpeed);

                    currentEnemyHelath -= homingMissileDamage;
                    homingMissilesCount++;

                     if (currentEnemyHelath <= 0)
                    {
                       
                        
                        changeActiveEnemy = true;
                    }

                    else
                    {
                      
                        changeActiveEnemy = false;
                    }
                    Debug.Log("Missile count : " + homingMissilesCount);
            //    }

           //     else
            //    {
            //        itr++;
            //    }
            }

            
            yield return new WaitForSeconds(homingMissileFiringPeriod);
        }

        EndOfCoroutine:
        isHomingMissilesReady = true;
        LockPrimaryWeapons(false);
        //activate homing missile system by setting it to true
    }

    private IEnumerator ContinouslyFireMissile()
    {
        missileFiring = true;
        while (true)
        {
            
            missilePos1 = new Vector3(transform.position.x + missilePaddingX,
                transform.position.y + missilePaddingY,
                transform.position.z);

            missilePos2 = new Vector3(transform.position.x - missilePaddingX,
                transform.position.y + missilePaddingY,
                transform.position.z);

            GameObject missile1 = Instantiate(missileRPrefab,
                                 missilePos1,
                                 Quaternion.identity) as GameObject;

            GameObject missile2 = Instantiate(missileLPrefab,
                     missilePos2,
                     Quaternion.identity) as GameObject;

            missile1.GetComponent<Rigidbody2D>().velocity = new Vector2(0, missileSpeed);
            missile2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, missileSpeed);

            yield return new WaitForSeconds(missileFiringPeriod);
        }
    }

    private IEnumerator DeactivateShields()
    {
        yield return new WaitForSeconds(shieldActiveTime);
        shield.SetActive(false);
        level.SetPlayerReady(true);

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();
        if(other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy)
            {
                enemy.ProcessHit(damageDealer);
            }
             
        }
        if(!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        //Debug.Log(health);
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject playerDestroyVfx2 = Instantiate(playerDestroyVFxPrefab2, transform.position, transform.rotation);
        GameObject playerDestroyVfx = Instantiate(playerDestroyVfxPrefab, transform.position, transform.rotation);
        Destroy(playerDestroyVfx, durationOfExplosion);
        Destroy(playerDestroyVfx2, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSfx, Camera.main.transform.position, deathSoundVolume);
    }

    private void Fire()
    {
        
        if(Input.GetButtonDown("Fire1") && !fireSafety)
        {

            //recoil.AddRecoil();
            if(!firing)
                firingCoroutine = StartCoroutine(FireContinously());
        }

        if(Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
            firing = false;
            //timer = 0f;
        }
       
    }

    IEnumerator FireContinously()
    {
        firing = true;
        while (true)
        {
            
            switch (numPrimaryWeaponBullets)
            {
                case 1:
                    laserPos = new Vector3(transform.position.x,
                        transform.position.y + laserPadding.y,
                        transform.position.z);

                   GameObject laser = Instantiate(laserPrefab,
                                  //transform.position,
                                  laserPos,
                                  Quaternion.identity) as GameObject;

                    laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);

                    break;

                case 2:
                    laserPos1 = new Vector3(transform.position.x - laserPadding1.x,
                        transform.position.y + laserPadding1.y,
                        transform.position.z);

                    laserPos2 = new Vector3(transform.position.x + laserPadding1.x,
                        transform.position.y + laserPadding2.y,
                        transform.position.z);

                   GameObject laser1 = Instantiate(laserPrefab,
                                  //transform.position,
                                  laserPos1,
                                  Quaternion.identity) as GameObject;

                    GameObject laser2 = Instantiate(laserPrefab,
                                  //transform.position,
                                  laserPos2,
                                  Quaternion.identity) as GameObject;

                    laser1.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
                    laser2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);

                    break;

                case 3:
                    laserPos = new Vector3(transform.position.x,
                        transform.position.y + laserPadding.y,
                        transform.position.z);

                    laserPos1 = new Vector3(transform.position.x - laserPadding1.x,
                        transform.position.y + laserPadding1.y,
                        transform.position.z);

                    laserPos2 = new Vector3(transform.position.x + laserPadding1.x,
                        transform.position.y + laserPadding2.y,
                        transform.position.z);

                    GameObject laser_ = Instantiate(laserPrefab,
                                   //transform.position,
                                   laserPos,
                                   Quaternion.identity) as GameObject;

                    GameObject laser1_ = Instantiate(laserPrefab,
                                  //transform.position,
                                  laserPos1,
                                  Quaternion.identity) as GameObject;

                    GameObject laser2_ = Instantiate(laserPrefab,
                                  //transform.position,
                                  laserPos2,
                                  Quaternion.identity) as GameObject;

                    laser_.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
                    laser1_.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
                    laser2_.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);

                    break;
            }
            

          

           
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);

        }
    }

    

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);


        transform.position = new Vector3(newXPos, newYPos, transform.position.z);

       // Debug.Log("move");

    }

    private void SetUpBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + paddingX;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - paddingX;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + paddingY;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - paddingY;
    }

    public void StartWarpEngines()
    {
        //shield2.SetActive(true);
        //level.SetPlayerReady(false);
        var engines = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem engine in engines)
        {
            var mainEngine = engine.main;
            mainEngine.startLifetime = warpEngineLifetime;
            mainEngine.startSpeed = warpEnginespeed;
        }
    }

    public void StopWarpEngines()
    {
        var engines = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem engine in engines)
        {
            var mainEngine = engine.main;
            mainEngine.startLifetime = initialWarpEngineLifetime;
            mainEngine.startSpeed = initialWarpEnginespeed;
        }
    }

    public void SetMissileStats(bool isMissileActive, GameObject missileLPrefab,
        GameObject missileRPrefab, float missileFiringPeriod,
        float missilePaddingX, float missilePaddingY,
        float missileSpeed)
    {
        this.isMissileActive = isMissileActive;
        this.missileLPrefab = missileLPrefab;
        this.missileRPrefab = missileRPrefab;
        this.missileFiringPeriod = missileFiringPeriod;
        this.missilePaddingX = missilePaddingX;
        this.missilePaddingY = missilePaddingY;
        this.missileSpeed = missileSpeed;
    }

    public void IncrementNumberOfPrimaryWeaponBullets()
    {
        numPrimaryWeaponBullets++;
        if (numPrimaryWeaponBullets > 3)
            numPrimaryWeaponBullets = 3;
    }

    public void ReadyHomingMissiles(bool isHomingMissilesReady,
        int numHomingMissiles,
        GameObject homingMissilePrefabL, GameObject homingMissilePrefabR,
        float homingMissileFiringPeriod,
        float homingMissilePaddingX, float homingMissilePaddingY,
        float homingMissileSpeed, float homingMissileRotateSpeed)
    {
        this.isHomingMissilesReady = isHomingMissilesReady;
        this.numHomingMissiles = numHomingMissiles;
        this.homingMissilePrefabL = homingMissilePrefabL;
        this.homingMissilePrefabR = homingMissilePrefabR;
        this.homingMissileFiringPeriod = homingMissileFiringPeriod;
        this.homingMissilePaddingX = homingMissilePaddingX;
        this.homingMissilePaddingY = homingMissilePaddingY;
        this.homingMissileSpeed = homingMissileSpeed;
        this.homingMissileRotateSpeed = homingMissileRotateSpeed;
        this.numHomingMissilePowerups+=2;
    }

    private void LockPrimaryWeapons(bool val)
    {
        if (val)
        {
            if(firingCoroutine != null)
                StopCoroutine(firingCoroutine);

            if(missileCoroutine != null)
                StopCoroutine(missileCoroutine);
        }
        fireSafety = val;
        missileFiring = !val;
        
    }

    public void ReadyMine(bool isMineReady,
        GameObject minePrefab, int numEnemiesInMineChain,
        int numOfMinesPerPowerUp,
        float mineFireSpeed, int mineDamage, float mineArmedPaddingZ,
        float waitTimeBeforeMineExplosion,
        float timeIntervalBetweenConsecutiveChains,
        float scanSurroundingEnemiesRadius,
        LayerMask mineLayerMask,
        GameObject mineExplosionWaveVfxPrefab,
        float durationOfMineExplosionWave)
    {
        this.isMineReady = isMineReady;
        this.minePrefab = minePrefab;
        this.numEnemiesInMineChain = numEnemiesInMineChain;
        this.mineFireSpeed = mineFireSpeed;
        this.numMinePowerUps += numOfMinesPerPowerUp;

        this.mineDamage = mineDamage;
        this.mineArmedPaddingZ = mineArmedPaddingZ;
        this.waitTimeBeforeMineExplosion = waitTimeBeforeMineExplosion;
        this.timeIntervalBetweenConsecutiveChains = timeIntervalBetweenConsecutiveChains;
        this.scanSurroundingEnemiesRadius = scanSurroundingEnemiesRadius;
        this.mineLayerMask = mineLayerMask;
        this.mineExplosionWaveVfxPrefab = mineExplosionWaveVfxPrefab;
        this.durationOfMineExplosionWave = durationOfMineExplosionWave;
    }

    public void ReadyShield(bool isShieldReady,
        GameObject shieldPrefab,
        float durationOfShield)
    {
        this.isShieldReady = isShieldReady;
        this.shieldPrefab = shieldPrefab;
        this.numOfShieldPowerUp++;
        this.durationOfShield = durationOfShield;
    }

    /*public void ReadyElectro(bool isElectroReady,
        GameObject electroPrefab, int numElectroCharges)
    {
        this.isElectroReady = isElectroReady;
        this.electroPrefab = electroPrefab;
        this.numElectroCharges = numElectroCharges;
    }*/
}
