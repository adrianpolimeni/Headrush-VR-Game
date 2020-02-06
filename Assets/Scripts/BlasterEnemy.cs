using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlasterEnemy : Blaster
{   
    void Start()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }

    public override void Shoot()
    {
        audio.PlayOneShot(ShotSound);
        MuzzleFlash.Play();
    }
}
