using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] int powerUpId = 11;

    PlayerWeaponSystem playerWeaponSystem;
    // Start is called before the first frame update
    void Start()
    {
        playerWeaponSystem = FindObjectOfType<PlayerWeaponSystem>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Trigger");
        playerWeaponSystem.AddPowerUp(powerUpId);
        Destroy(gameObject);
    }
    
    public int GetPowerUpId()
    {
        return powerUpId;
    }
}
