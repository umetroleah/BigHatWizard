﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static AudioClip dashingSound, explosionSound, walkingSound, landingSound, shootingSound, shotExplosionSound, hurtSound;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        dashingSound = Resources.Load<AudioClip>("dash");
        explosionSound = Resources.Load<AudioClip>("explosion");
        walkingSound = Resources.Load<AudioClip>("step");
        landingSound = Resources.Load<AudioClip>("land");
        shootingSound = Resources.Load<AudioClip>("laser_shoot");
        shotExplosionSound = Resources.Load<AudioClip>("laser_explode");
        hurtSound = Resources.Load<AudioClip>("hurt");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "dashing":
                audioSrc.PlayOneShot(dashingSound);
                break;
            case "explosion":
                audioSrc.PlayOneShot(explosionSound);
                break;
            case "walking":
                audioSrc.PlayOneShot(walkingSound);
                break;
            case "landing":
                audioSrc.PlayOneShot(landingSound);
                break;
            case "shooting":
                audioSrc.PlayOneShot(shootingSound);
                break;
            case "shot explode":
                audioSrc.PlayOneShot(shotExplosionSound);
                break;
            case "hurt":
                audioSrc.PlayOneShot(hurtSound);
                break;
            default:
                break;

        }


    }
}
