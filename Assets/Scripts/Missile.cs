using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] AudioClip missileFireSound;
    [SerializeField] [Range(0, 1)] float missileFireSoundVoulume = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource.PlayClipAtPoint(missileFireSound, Camera.main.transform.position, missileFireSoundVoulume);
    }

   
}
