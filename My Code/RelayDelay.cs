using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Mathias Sorin
 * 2021-04-22
 */
public class RelayDelay : TriggerableObjects
{
    public float beforeActivationDelay;
    public float afterActivationDelay;
    public TriggerableObjects triggerableObject;

    public override void OnActivate()
    {
        Invoke("On", beforeActivationDelay);
    }
    public override void OnDeactivate()
    {
        Invoke("Off", afterActivationDelay);
    }

    private void On()
    {
        triggerableObject.OnActivate();
    }
    private void Off()
    {
        triggerableObject.OnDeactivate();
    }
}