using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour
{
  //  [SerializeField] float speed = 5f;
   // [SerializeField] float rotateSpeed = 200f;
    [SerializeField] GameObject missileDestroyVfxPrefab;
    
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip missileDestroySfx;
    [SerializeField] AudioClip missileFireSfx;
    [SerializeField] [Range(0, 1)] float missileDestroySoundVolume = 0.7f;
    [SerializeField] [Range(0, 1)] float missileFireSoundVolume = 0.7f;
    
    Transform target;
    Rigidbody2D rb;

    bool fire = false;
    Level level;

    float speed, rotateSpeed;

    int colorSwitch = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        level = FindObjectOfType<Level>();
        AudioSource.PlayClipAtPoint(missileFireSfx, Camera.main.transform.position, missileFireSoundVolume);
    }

    // Update is called once per frame

    private void LateUpdate()
    {
        if(fire)
        {
            
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if(fire)
        {
            if(target == null)
            {
                target = level.GetATarget();
               
            }
            if (target == null)
            {
                fire = false;
                Destroy();

            }

            if (target != null)
            {
                Vector2 direction = (Vector2)target.position - rb.position;
                direction.Normalize();

                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                // Debug.Log(rotateAmount);
                rb.angularVelocity = -rotateAmount * rotateSpeed;
                //Debug.Log(rb.angularVelocity);
                rb.velocity = transform.up * speed;

               
                // Debug.Log(target.position);
            }


        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
        
       

        GameObject missileDestroyVfx = Instantiate(missileDestroyVfxPrefab, transform.position, transform.rotation);
        
        Destroy(missileDestroyVfx, durationOfExplosion);
       
        AudioSource.PlayClipAtPoint(missileDestroySfx, Camera.main.transform.position, missileDestroySoundVolume);
    }

    public void SetTarget(Transform target, float speed, float rotateSpeed)
    {
        this.speed = speed;
        this.rotateSpeed = rotateSpeed;
        this.target = target;
        fire = true;
        
    }

    

}
