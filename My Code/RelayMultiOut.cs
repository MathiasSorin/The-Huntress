using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Mathias Sorin
 * 2021-04-22
 */
public class RelayMultiOut : TriggerableObjects
{
    public TriggerableObjects[] triggerableObjects;

    public override void OnActivate()
    {
        foreach (TriggerableObjects x in triggerableObjects)
        {
            try
            {
                TriggerDoor temp = x.gameObject.transform.parent.gameObject.GetComponentInChildren<TriggerDoor>();
                if (temp)
                {
                    temp.UnlockDoorAndSave();
                    continue;
                }
            }
            catch
            {
                x.OnActivate();
                continue;
            }
            x.OnActivate();
        }
    }
    public override void OnDeactivate()
    {
        foreach (TriggerableObjects x in triggerableObjects)
        {
            try
            {
                TriggerDoor temp = x.gameObject.transform.parent.gameObject.GetComponentInChildren<TriggerDoor>();
                if (temp)
                {
                    //temp.locked = true;
                    continue;
                }
            }
            catch
            {
                x.OnDeactivate();
                continue;
            }
            x.OnDeactivate();
        }
    }
}