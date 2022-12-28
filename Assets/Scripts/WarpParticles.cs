using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpParticles : MonoBehaviour
{
    [SerializeField] int simulationSpeed = 7;
    [SerializeField] float changeSimulationSpeedTime = 1f;

    public void EndWarpParticles()
    {
       /*var particleSystem = GetComponent<ParticleSystem>();
        var psMain = particleSystem.main;
        psMain.simulationSpeed = 1;*/
         StartCoroutine(EndWarp());
    }
   

    public void StartWarpParticles()
    {
        //Debug.Log("thunder");
       
        StartCoroutine(StartWarp());
       
    }

    private IEnumerator StartWarp()
    {
        var particleSystem = GetComponent<ParticleSystem>();
        var psMain = particleSystem.main;
        for (int i = 1; i <= simulationSpeed; i++)
        {
            psMain.simulationSpeed = i;
            yield return new WaitForSeconds(changeSimulationSpeedTime);
        }
    }

    private IEnumerator EndWarp()
    {
        var particleSystem = GetComponent<ParticleSystem>();
        var psMain = particleSystem.main;
        for (int i = simulationSpeed; i >= 1; i--)
        {
            psMain.simulationSpeed = i;
            yield return new WaitForSeconds(changeSimulationSpeedTime);
        }
    }
}
