using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainFx : MonoBehaviour
{
    private ParticleSystem chainParticle;
    private int playCount;
    private void Awake()
    {
        chainParticle = GetComponent<ParticleSystem>();
        chainParticle.Stop();
        chainParticle.Clear();
    }
    public void SetPlayCount(int count)
    {
        playCount = count;
    }

    public void PlayParticle()
    {
        if (playCount < 1)
            return;

        if (chainParticle.isPlaying)
        {
            chainParticle.Stop();
            chainParticle.Clear();
        }

        chainParticle.Play();
    }

}
