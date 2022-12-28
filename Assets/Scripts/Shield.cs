using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    DamageDealer damageDealer;
    // Start is called before the first frame update
    void Start()
    {
        damageDealer = GetComponent<DamageDealer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.name.StartsWith("Ship"))
        {
            other.GetComponent<Enemy>().ProcessHit(damageDealer);
        }
    }
}
