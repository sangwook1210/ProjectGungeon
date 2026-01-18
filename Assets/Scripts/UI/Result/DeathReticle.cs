using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathReticle : MonoBehaviour
{
    private GameObject Player;
    public GameObject ResultBook;

    public Animator reticleAnim;

    public AudioSource reticleAudioSource;
    public AudioClip reticleSpawnClip;
    public AudioClip reticleLockOnClip;
    public AudioClip reticleShotClip;

    public void reticleSpawn()
    {
        reticleAudioSource.PlayOneShot(reticleSpawnClip);
    }
    public void reticleSelectTrigger()
    {
        int randomTrigger = Random.Range(0, 4);

        if (randomTrigger == 1)
        {
            reticleAnim.SetTrigger("MoveTrigger1");
        }
        else if (randomTrigger == 2)
        {
            reticleAnim.SetTrigger("MoveTrigger2");
        }
        else if (randomTrigger == 3)
        {
            reticleAnim.SetTrigger("MoveTrigger3");
        }
        else
        {
            reticleAnim.SetTrigger("MoveTrigger4");
        }

        reticleSpawn();
    }
    public void reticleLockOn()
    {
        reticleAudioSource.PlayOneShot(reticleLockOnClip);
    }
    public void reticleShot()
    {
        reticleAudioSource.PlayOneShot(reticleShotClip);
    }
    public void reticleEnd()
    {
        Player.GetComponent<Player>().PlayerDeadAnim();
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
}
