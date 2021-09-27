using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Mathias Sorin
 * 2021-04-22
 */
public abstract class TriggerableObjects : MonoBehaviour, IActivable
{
    [HideInInspector]
    public int side;

    [HideInInspector]
    public bool door_isOpening = false;
    public abstract void OnActivate();
    public abstract void OnDeactivate();
}