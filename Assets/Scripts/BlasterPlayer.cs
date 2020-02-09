using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class BlasterPlayer : Blaster
{
    public ParticleSystem CylinderSteam;
    public LayerMask EnemyLayer, HeadLayer, UILayer;
    public GameObject BulletHit;
    Text clipText;
    Canvas HUD;
    OVRGrabbable grab;
    Player player;
    LineRenderer laser;
    RectTransform[] transformUI;
    Rigidbody rigidbody;
    void Start()
    {
        animator = GetComponent<Animator>();
        grab = GetComponent<OVRGrabbable>();
        audio = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        laser = GetComponentInChildren<LineRenderer>();
        clipText = GetComponentsInChildren<Text>()[0];
        HUD = GetComponentInChildren<Canvas>();
        transformUI = HUD.GetComponentsInChildren<RectTransform>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void Update()
    {
        UpdateLaser();

        if (CheckGrabbed())
            CheckTrigger();
        Vector2 drawHealth = new Vector2((float)player.Health, 15f); 
        transformUI[2].sizeDelta = drawHealth;
        transformUI[3].sizeDelta = drawHealth;
        if (MainManager.GameFinished && OVRInput.GetDown(OVRInput.Button.One))
        {
            player.TriggerRestart();
        }

        // Secret HighScore Reset
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) &&
            OVRInput.GetDown(OVRInput.Button.Three) &&
            OVRInput.GetDown(OVRInput.Button.Four)) {
            PlayerPrefs.SetFloat("Time", 0f);
            player.TriggerRestart();
        }
    }

    void UpdateLaser() {
        RaycastHit hit;
        if (Physics.Raycast(barrelLocation.position, Vector3.forward, out hit, Mathf.Infinity, EnemyLayer)
            || Physics.Raycast(barrelLocation.position, Vector3.forward, out hit, Mathf.Infinity, HeadLayer))
        {
            laser.SetPosition(1, hit.distance * Vector3.forward);
        }
        else
        {
            laser.SetPosition(1, Vector3.forward * 5f);
        }
    }


    bool CheckGrabbed()
    {
        if (grab != null && grab.isGrabbed)
        {
            HUD.enabled = true;
            rigidbody.constraints = RigidbodyConstraints.None;
            if (!MainManager.GameStarted)
                MainManager.UI.PlayStart();
            return true;
        }
        else
        {
            HUD.enabled = false;
            if (isReloading)
            {
                StopCoroutine(ReloadCoroutine(0f));
                isReloading = false;
                audio.Stop();
                animator.SetBool(RELOAD_ANIM, isReloading);
                clipText.text = clip + "";
                CylinderSteam.Stop();
                try
                {
                    VibrationManager.master.Stop();
                }
                catch (NullReferenceException) { }
            }
        }
        return false;
    }

    void CheckTrigger()
    {
        if ((OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && !isReloading && MainManager.GameStarted && player.isAlive))
        {
            if (clip != 0)
                Shoot();
            else
                StartCoroutine(ReloadCoroutine(2f));
        }
        else
        {
            animator.ResetTrigger(SHOOT_ANIM);
        }
    }

    public override void Shoot()
    {
        animator.SetTrigger(SHOOT_ANIM);
        try
        {
            VibrationManager.master.TriggerVibration(ShotSound, grab.grabbedBy.GetController());
        }
        catch (NullReferenceException) { }
        audio.PlayOneShot(ShotSound);

        clip--;
        clipText.text = clip + "";
        MuzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(barrelLocation.position, barrelLocation.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, HeadLayer))
        {
            player.PlaySound(0);
            hit.collider.gameObject.layer = EnemyLayer;
            hit.transform.SendMessageUpwards("OnHeadHit");
        }
        else if (Physics.Raycast(barrelLocation.position, barrelLocation.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, EnemyLayer))
        {
            hit.transform.SendMessageUpwards("OnHit", hit);
            player.PlaySound(1);
        }
        else if (Physics.Raycast(barrelLocation.position, barrelLocation.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, UILayer))
        {
            Image img = hit.transform.gameObject.GetComponentInChildren<Image>();
            player.PlaySound(2);
            if (img != null)
                img.color = new Color(1f, 1f, 1f, 1f);
        }
        Instantiate(BulletHit, hit.point, hit.transform.rotation);
    }

    private IEnumerator ReloadCoroutine(float reloadTime)
    {
        isReloading = true;
        CylinderSteam.Play();
        animator.SetBool(RELOAD_ANIM, true);
        clipText.text = "Reloading";
        audio.Stop();
        audio.PlayOneShot(ReloadSound);
        try
        {
            VibrationManager.master.TriggerVibration(ReloadSound, grab.grabbedBy.GetController());
        }
        catch (NullReferenceException) { }

        yield return new WaitForSeconds(reloadTime);

        clip = CLIP_SIZE;
        clipText.text = clip + "";
        isReloading = false;
        animator.SetBool(RELOAD_ANIM, false);
    }
}
