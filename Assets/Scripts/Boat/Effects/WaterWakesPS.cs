using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move the particles to a position above the waves
public class WaterWakesPS : MonoBehaviour 
{
    ParticleSystem foamPS;

    ParticleSystem.Particle[] foamParticles;


    void Start() 
	{
        foamPS = GetComponent<ParticleSystem>();	

        foamParticles = new ParticleSystem.Particle[foamPS.main.maxParticles];
    }



    //Should be in lateupdate accoridng to 
    //https://docs.unity3d.com/ScriptReference/ParticleSystem.GetParticles.html
    void LateUpdate() 
	{
        //GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = foamPS.GetParticles(foamParticles);

        float timer = Time.time;

        //Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 particlePos = foamParticles[i].position;

            //Add an extra height so the particles are above the water
            particlePos.y = WaterController.current.GetWaveYPos(particlePos, timer) + 0.2f;

            foamParticles[i].position = particlePos;
        }

        //Apply the particle changes to the particle system
        foamPS.SetParticles(foamParticles, numParticlesAlive);
    }
}
