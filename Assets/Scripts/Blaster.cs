using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Blaster : MonoBehaviour
{
    public ParticleSystem MuzzleFlash;
    public Transform barrelLocation;
    public AudioClip ShotSound, ReloadSound;
    protected bool isReloading = false;
    protected int clip = 12;
    protected AudioSource audio;
    protected Animator animator;
    public int CLIP_SIZE = 12;
    public const string SHOOT_ANIM = "Shoot";
    public const string RELOAD_ANIM = "isReloading";

    public abstract void Shoot();


}
