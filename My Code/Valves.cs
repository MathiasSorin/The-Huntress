using UnityEngine;
using System;
public class Valves : TriggerableObjects
{
    //public enum wantedRotation { Zero = 0, Ninenty = 90, centqsies = 180, deuxcentsx = 270 };
    [Header("Desired rotation 1 to activate the next valves")]
    public int wantedRotation = 0;
    public TriggerableObjects[] nextValves;

    [Header("Desired rotation 2 to activate the next valves")]
    public int wantedRotation1 = 0;
    public TriggerableObjects[] nextValves1;

    [Header("Helps to indicates if valve is on")]
    public GameObject valveLight;
    private GameObject lit;

    [Header("Check if this is the first valve")]
    public bool previousValveIsOn;

    [Header("if > 0, will deactivate after said time")]
    public float timedValve = 0;
    private bool timedValveLock = false;

    [Header("Don't change me!")]
    public bool isStraightValve;

    private Transform valveTransform;
    private float newRotation = 90;
    private int oppositeRotation;
    private bool isOn;
    private bool hasLoaded;

    public bool isBossFight;
    public TriggerableObjects VfxActivator;

    public AkRoomAwareObject roomAware;

    bool isPlayingSound;

    [HideInInspector]
    public int id; 


    private void Start()
    {
        //change
        //newRotation = transform.rotation.y + 90;
        valveTransform = GetComponent<Transform>();
        oppositeRotation = GetOppositeRotation();
        CreateId();
        CheckTempSave();
        
        if (valveLight != null || lit != null)
        {
            try
            {
                lit = valveLight.GetComponentInChildren<Light>().gameObject;
                lit.SetActive(false);
            }
            catch
            {
                //tampis
            }
        }


    }

    public override void OnActivate()
    {
        if (timedValveLock)
        {
            return;
        }
        if(timedValve<=0)
        {
            //AkSoundEngine.PostEvent("Play_Gas_Valve_Turn", roomAware.gameObject);
        }
        valveTransform.Rotate(valveTransform.rotation.x, valveTransform.rotation.y, 90f, Space.Self);
        if (newRotation == 360)
        {
            newRotation = 0;
        }

        if (newRotation == wantedRotation && previousValveIsOn)
        {
            IsOn(nextValves);
            AddToSave();
        }
        else if (newRotation == wantedRotation1 && previousValveIsOn)
        {
            IsOn(nextValves1);
            AddToSave();
        }
        else if (isStraightValve && newRotation == oppositeRotation && previousValveIsOn)
        {
            AddToSave();
            IsOn(nextValves);
        }
        else
        {
            IsOff(nextValves);
            IsOff(nextValves1);
        }

        newRotation += 90;

        
    }

    public override void OnDeactivate()
    {
        IsOff(nextValves);
        IsOff(nextValves1);


    }

    private void IsOn(TriggerableObjects[] triggerableObjects)
    {
        if (triggerableObjects == nextValves)
        {
            IsOff(nextValves1);
        }
        else if (triggerableObjects == nextValves1)
        {
            IsOff(nextValves);
        }
        if (valveLight != null || lit != null)
        {
            try
            {
                lit.GetComponent<ValveLight>().OnActivate();
                lit.SetActive(true);
                AddToSave();
            }
            catch
            {
                //Vive la suisse
            }
        }
        if (timedValve > 0)
        {
            if(!isPlayingSound)
            {
                AkSoundEngine.PostEvent("Play_Gas_Valve_Timer", roomAware.gameObject);
                isPlayingSound = true;
            }
            else
            {
                AkSoundEngine.PostEvent("Play_Gas_Valve_Turn", roomAware.gameObject);
            }
            timedValveLock = true;
            Invoke("ValveTimerOff", timedValve);
        }

        if (isBossFight == true) { VfxActivator.OnActivate(); }
        isOn = true;
        foreach (TriggerableObjects valve in triggerableObjects)
        {
            try
            {
                TriggerDoor temp2 = valve.gameObject.transform.parent.gameObject.GetComponentInChildren<TriggerDoor>();
                if (temp2)
                {
                    temp2.locked = false;
                    temp2.AddToSave();
                    continue;
                }
            }
            catch
            {
                //tampis
            }
            Valves temp = valve.GetComponent<Valves>();
            if (temp)
            {
                temp.Check();
                temp.previousValveIsOn = true;
            }
            else
            {
                valve.OnActivate();
            }
        }
    }
    private void IsOff(TriggerableObjects[] triggerableObjects)
    {

        if (valveLight != null || lit != null)
        {
            try
            {
                lit.GetComponent<ValveLight>().OnDeactivate();
                lit.SetActive(false);
            }
            catch
            {
                //rien a voir ici
            }
        }

        if (isBossFight == true) { VfxActivator.OnDeactivate(); }
        foreach (TriggerableObjects valve in triggerableObjects)
        {
            try
            {
                TriggerDoor temp2 = valve.gameObject.transform.parent.gameObject.GetComponentInChildren<TriggerDoor>();
                if (temp2)
                {
                    temp2.locked = true;

                }
            }
            catch
            {
                //tampis
            }
            Valves temp = valve.GetComponent<Valves>();
            if (temp)
            {
                temp.previousValveIsOn = false;
            }
            RelayMultiIn multiIn = valve.GetComponent<RelayMultiIn>();
            if (multiIn && !isOn)
            {
                continue;
            }
            valve.OnDeactivate();
            isOn = false;
        }
    }

    public void Check()
    {
        if (newRotation - 90 == wantedRotation)
        {
            IsOn(nextValves);
        }
        else if (newRotation - 90 == wantedRotation1)
        {
            IsOn(nextValves1);
        }
        else if (isStraightValve && newRotation - 90 == oppositeRotation)
        {
            IsOn(nextValves);
        }
        else
        {
            IsOff(nextValves);
            IsOff(nextValves1);
        }
    }

    private int GetOppositeRotation()
    {
        switch (wantedRotation)
        {
            case 0:
                return 180;
            case 90:
                return 270;
            case 180:
                return 0;
            case 270:
                return 90;
            default:
                return 0;
        }
    }

    private void ValveTimerOff()
    {
        timedValveLock = false;
        AkSoundEngine.PostEvent("Stop_Gas_Valve_Timer", roomAware.gameObject);
        isPlayingSound = false;
        IsOff(nextValves);
        IsOff(nextValves1);
        valveTransform.Rotate(valveTransform.rotation.x, valveTransform.rotation.y, 90f, Space.Self);
        if (newRotation == 360)
        {
            newRotation = 0;
        }
        newRotation += 90;
    }


    //Accesseurs
    public bool TurnedOn //peut pas mettre IsOn car ya deja de quoi de nommer IsOn, ca me laisse pas
    {
        get { return isOn; }
        set { isOn = value; }
    }

    private void CreateId() {
        id = Mathf.RoundToInt((transform.position.x * 100) + (transform.position.y * 10) + transform.position.z);
    }

    public void AddToSave() {
        Man_Save.Instance.savedIngredientsTemp.Add(id);
    }
    //testing something here
    public void UsedInvoke() {
        Invoke("Used", 1f);
    }
    //testing something here too
    private  void TurnUntilActive() {
        int counter = 0;
        if (wantedRotation != 0) {
            counter = wantedRotation / 90;
        }
        for (int i = 0; i<counter;i++) {
            OnActivate();
            //Debug.Log("called valve turn from save");
        }
        hasLoaded = true;
        
    }

    public void Used() {

        if (!hasLoaded) {
            //Debug.Log("Valve loaded");
            TurnUntilActive();
        }

        //valveTransform.Rotate(valveTransform.rotation.x, valveTransform.rotation.y, wantedRotation, Space.Self);
        //newRotation += wantedRotation;
        Check();
        if (valveLight != null || lit != null) {
            lit = valveLight.GetComponentInChildren<Light>().gameObject;
            lit.SetActive(true);
        }
    }

    private void CheckTempSave() {
        for (int i = 0; i < Man_Save.Instance.savedIngredientsTemp.Count; i++) {
            if (Man_Save.Instance.savedIngredientsTemp[i] == id) {
                Used();
            }
        }
    }
}
