using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    WaveConfig waveConfig;
    [SerializeField] List<Transform> wayPoints;
    bool reflect = false;
    int waypointIndex = 0;
    [SerializeField] float distanceFromCamera = -7.4f;

    Level level;

    // Start is called before the first frame update
    void Start()
    {
        level = FindObjectOfType<Level>();
        wayPoints = waveConfig.GetWayPoints();
       
       // transform.position = wayPoints[waypointIndex].transform.position;

       // transform.rotation = Quaternion.Euler(90, -180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void SetWaveConfig(WaveConfig waveConfig, bool reflect)
    {
        this.waveConfig = waveConfig;
        this.reflect = reflect;
        /*if (reflect)
        {
            Camera gameCamera = Camera.main;
            
            var offset = gameCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromCamera)).x;

            for (int idx = 0; idx < wayPoints.Count; idx++)
            {
                var pos = wayPoints[idx].transform.position;
                wayPoints[idx].transform.position = new Vector3(
                    offset - pos.x,
                     pos.y,
                      pos.z);
            }

        }*/

    }

    

    private void Move()
    {
        if (waypointIndex <= wayPoints.Count - 1)
        {
            if (!reflect)
            {
                Vector3 dir;


                var targetPosition = wayPoints[waypointIndex].transform.position;

                var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
                var rotationThisFrame = waveConfig.GetRotateSpeed() * Time.deltaTime;
                dir = targetPosition - transform.position;
                Quaternion rotation = Quaternion.LookRotation(dir, -Vector3.forward);
                //Debug.Log(rotation);

                transform.position = Vector3.MoveTowards
                          (transform.position, targetPosition, movementThisFrame);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationThisFrame);


                if (transform.position == targetPosition)
                    waypointIndex++;
            }

            else
            {
                Camera gameCamera = Camera.main;

                var offset = gameCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromCamera)).x;
                Vector3 dir;


                var targetPosition = wayPoints[waypointIndex].transform.position;
                targetPosition.x = offset - targetPosition.x;

                var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
                var rotationThisFrame = waveConfig.GetRotateSpeed() * Time.deltaTime;
                dir = targetPosition - transform.position;
                Quaternion rotation = Quaternion.LookRotation(dir, -Vector3.forward);         
                //Debug.Log(rotation);

                transform.position = Vector3.MoveTowards
                          (transform.position, targetPosition, movementThisFrame);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationThisFrame);


                if (transform.position == targetPosition)
                    waypointIndex++;
            }
        }

        else
        {
            level.EnemyDestroyed(transform);
            Destroy(gameObject);
        }
            
    }
}
