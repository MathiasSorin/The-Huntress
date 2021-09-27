using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Mathias Sorin
 * 2021-04-22
 */
public class RelayMultiIn : TriggerableObjects
{
    public int triggers = 1;
    public TriggerableObjects triggerableObject;

    private int activations = 0;

    public override void OnActivate()
    {
        activations++;
        if (activations == triggers)
        {
            try
            {
                TriggerDoor temp2 = triggerableObject.gameObject.transform.parent.gameObject.GetComponentInChildren<TriggerDoor>();
                if (temp2)
                {
                    temp2.locked = false;
                    return;
                }
            }
            catch
            {
                triggerableObject.OnActivate();
            }
            triggerableObject.OnActivate();
        }
    }
    public override void OnDeactivate()
    {
        if (activations > 0) activations--;
        if (activations < triggers)
        {
            try
            {
                TriggerDoor temp2 = triggerableObject.gameObject.transform.parent.gameObject.GetComponentInChildren<TriggerDoor>();
                if (temp2)
                {
                    //temp2.locked = true;
                }
            }
            catch
            {
                triggerableObject.OnDeactivate();
            }
        }
    }
}
