using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blaster : MonoBehaviour
{

    public ParticleSystem MuzzleFlash, CylinderSteam;
    public Transform barrelLocation;
    public AudioClip ShotSound, ReloadSound;
    public LayerMask EnemyLayer, HeadLayer;
    public GameObject blood;
    public Text RoundsDisplay;
    OVRGrabbable grab;
    bool isReloading = false;
    private int clip = 8;
    private string clipText = "8";
    private AudioSource audio;
    private Animator animator;
    private const string SHOOT_ANIM = "Shoot";
    private const string RELOAD_ANIM = "isReloading";

    void Start()
    {
        animator = GetComponent<Animator>();
        grab = GetComponent<OVRGrabbable>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        checkGrabbed();
        
        if ((OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && grab.isGrabbed && !isReloading))
        {
            if (clip != 0)
                Shoot();
            else
                StartCoroutine(ReloadCoroutine());
        }
        else
        {
            animator.ResetTrigger(SHOOT_ANIM);
        }
    }

    void checkGrabbed() {
        if (grab.isGrabbed)
        {
            RoundsDisplay.text = clipText;
        }
        else
        {
            RoundsDisplay.text = "";
            if (isReloading)
            {
                StopCoroutine(ReloadCoroutine());
                isReloading = false;
                audio.Stop();
                animator.SetBool(RELOAD_ANIM, isReloading);
                clipText = clip+"";
                CylinderSteam.Stop();
                try
                {
                   VibrationManager.master.Stop();
                }
                catch (NullReferenceException) { }
            }
        }
    }


    void Shoot()
    {
        animator.SetTrigger(SHOOT_ANIM);
        try
        {
            VibrationManager.master.TriggerVibration(ShotSound, grab.grabbedBy.GetController());
        }
        catch (NullReferenceException) { }
        audio.PlayOneShot(ShotSound);
        RaycastHit hit;
        if (Physics.Raycast(barrelLocation.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, HeadLayer))
        {
            EnemyHit(hit);
        }
        else if (Physics.Raycast(barrelLocation.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, EnemyLayer))
        {
            EnemyHit(hit);
        }
        clip--;
        clipText = clip + "";
        MuzzleFlash.Play(); 
    }

    void EnemyHit(RaycastHit hit) {
        hit.transform.SendMessageUpwards("Ragdoll", true);
        hit.rigidbody.AddForce(transform.TransformDirection(Vector3.forward) * 30f, ForceMode.Impulse);
        GameObject t = Instantiate(blood, hit.point, hit.transform.rotation);
        t.transform.parent = hit.transform;
    }


    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        CylinderSteam.Play();
        animator.SetBool(RELOAD_ANIM, true);
        clipText = "Reloading";
        audio.Stop();
        audio.PlayOneShot(ReloadSound);
        try
        {
            VibrationManager.master.TriggerVibration(ReloadSound, grab.grabbedBy.GetController());
        }
        catch (NullReferenceException) { }

        yield return new WaitForSeconds(2);

        clip = 8;
        clipText = clip + "";
        isReloading = false;
        animator.SetBool(RELOAD_ANIM, false);
    }


}
