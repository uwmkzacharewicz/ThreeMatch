using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;


    public void PlayRandomDestroyNoise()
    {
        int i = Random.Range(0, destroyNoise.Length);
        destroyNoise[i].Play();
    }
}
