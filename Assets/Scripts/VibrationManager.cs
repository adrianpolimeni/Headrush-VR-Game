using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager master;

    // Start is called before the first frame update
    void Start()
    {
        if (master && master != this) 
            Destroy(this);
        else
            master = this;
    }

    public void TriggerVibration(AudioClip audio, OVRInput.Controller controller)
    {
        try
        {
            OVRHapticsClip clip = new OVRHapticsClip(audio);
            if (controller == OVRInput.Controller.RTouch)
            {
                OVRHaptics.RightChannel.Preempt(clip);
            }
            if (controller == OVRInput.Controller.LTouch)
            {
                OVRHaptics.LeftChannel.Preempt(clip);
            }
        }
        catch (Exception e) {

        }
    }

    public void Stop()
    {
        try
        {
            OVRHaptics.LeftChannel.Clear();
            OVRHaptics.RightChannel.Clear();
        } catch (Exception e) {}
    }
}
