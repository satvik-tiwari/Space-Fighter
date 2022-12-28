using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSystem : MonoBehaviour
{
    [Header("Missiles")]
    [SerializeField] GameObject missileLPrefab;
    [SerializeField] GameObject missileRPrefab;
    [SerializeField] float missileFiringPeriod = 1.5f;
    [SerializeField] float missilePaddingX = 1f;
    [SerializeField] float missilePaddingY = 1f;
    [SerializeField] float missileSpeed = 10f;

    [Header("Homing Missiles")]
    [SerializeField] int numHomingMissiles;
    [SerializeField] GameObject homingMissilePrefabL;
    [SerializeField] GameObject homingMissilePrefabR;
    [SerializeField] float homingMissileFiringPeriod;
    [SerializeField] float homingMissilePaddingX;
    [SerializeField] float homingMissilePaddingY;
    [SerializeField] float homingMissileSpeed = 10;
    [SerializeField] float homingMissileRotateSpeed = 20;

    /*[Header("Electic Attack")]
    [SerializeField] int numOfCharges = 10;
    [SerializeField] GameObject elctricAttackPrefab;*/

    [Header("Mines")]
    [SerializeField] GameObject minePrefab;
    [SerializeField] int numEnemiesInMineChain;
    [SerializeField] int numOfMinesPerPowerUp = 5;
    [SerializeField] float mineFireSpeed = 7.5f;
    [SerializeField] int mineDamage = 10000;
    [SerializeField] float mineArmedPaddingZ = -0.433f;
    [SerializeField] float waitTimeBeforeMineExplosion = 1.5f;
    [SerializeField] float timeIntervalBetweenConsecutiveChains = 0.2f;
    [SerializeField] float scanSurroundingEnemiesRadius = 3.6f;
    [SerializeField] LayerMask mineLayerMask;
    [SerializeField] GameObject mineExplosionWaveVfxPrefab;
    [SerializeField] float durationOfMineExplosionWave = 0.8f;

    [Header("Shields")]
    [SerializeField] GameObject shieldPrefab;
    [SerializeField] float durationOfShield;

    //[Header("Primary Weapon")]
    //[SerializeField] 
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPowerUp(int powerUpId)
    {
        switch(powerUpId)
        {
            case 1: player.IncrementNumberOfPrimaryWeaponBullets();
                break;

            case 11: player.SetMissileStats(true, missileLPrefab,
                missileRPrefab, missileFiringPeriod,
                missilePaddingX, missilePaddingY,
                missileSpeed);
                break;

            case 12:
                player.ReadyHomingMissiles(true,
                    numHomingMissiles,
        homingMissilePrefabL, homingMissilePrefabR,
        homingMissileFiringPeriod, homingMissilePaddingX, 
        homingMissilePaddingY, homingMissileSpeed,
        homingMissileRotateSpeed);
                //Debug.Log("Power Received");
                break;

            case 13:
               player.ReadyMine(true,
        minePrefab, numEnemiesInMineChain,
        numOfMinesPerPowerUp,
        mineFireSpeed, mineDamage, mineArmedPaddingZ,
        waitTimeBeforeMineExplosion,
        timeIntervalBetweenConsecutiveChains,
        scanSurroundingEnemiesRadius,
        mineLayerMask,
        mineExplosionWaveVfxPrefab,
        durationOfMineExplosionWave);

                break;

            case 14:
                player.ReadyShield(true,
        shieldPrefab, durationOfShield);

                break;


                // player.ReadyElectro(true, elctricAttackPrefab, numOfCharges);
        }
    }
}
