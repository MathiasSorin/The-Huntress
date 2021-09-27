using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Mathias Sorin
 * 2021-04-22
 */
/*
 * Daisy Provencher
 * 2021-07-14
 */
public class TriggerDoor : MonoBehaviour
{
    [Tooltip("Will close the door when exiting the trigger")]
    public bool deactivateOnExit = false;
    [Tooltip("Can only be activated once")]
    public bool singleUse = false;
    [Tooltip("Check to require player input when in trigger (interact key)")]
    public bool playerInput = false;
    [Tooltip("Assigned in Prefab")]
    public TriggerableObjects[] doors;
    [HideInInspector]
    public int id;
    public bool locked;
    public SO_QuestItem key;

    public string ID;

    public bool isPLLevel = false;
    
    private List<IDoorable> activator = new List<IDoorable>();
    private bool activated = false;
    //private bool inTrigger = false;

    public IDoorable opener;

    bool isBlocked = false;

    public TriggerableObjects timeline; // simon lapointe 7/13/2021 starts timeline for door to basement

    private SO_EventManager em;

    [Header("Wwise")]
    public AK.Wwise.Switch DoorType;
    public AkRoomAwareObject audioSourceObject;
    private bool Door_isPlaying = false;

    bool isUsed = false;

    private void Start()
    {
        em = Man_Game.Instance.em;
        CreateId();
        CheckTempSave();
    }

    private void FixedUpdate()
    {
        //if (!inTrigger) return;
        if (activated) return;
        for (int i = 0; i < activator.Count; i++)
        {

            if (activator[i] == null)
            {
                continue; //Comme un break mais on continue la boucle au lieu de l'arrêter
            }

            if (activator[i].IsOpeningDoor())
            {
                opener = activator[i];
                OnActivate();

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        // Bookshelf bloque la porte
        IPushable bookshelf = other.GetComponent<IPushable>();
        if (bookshelf != null)
        {
            isBlocked = true;
        }
        IDoorable isDoorable = other.GetComponentInParent<IDoorable>();
        if (isDoorable != null && !activator.Contains(isDoorable) && other.GetComponent<VisionCone>() == null) activator.Add(isDoorable);
        else return;

        if (singleUse && activated) return;
    }

    private void OnTriggerExit(Collider other)
    {
        if (isBlocked)
        {
            IPushable bookshelf = other.GetComponent<IPushable>();
            if (bookshelf != null)
            {
                isBlocked = false;
            }
        }

        IDoorable temp = other.GetComponentInParent<IDoorable>();

        //inTrigger = false;

        if (temp != null && other.GetComponent<VisionCone>() == null)
        {
            activator.Remove(temp);
            //CheckOtherSide();
            if (deactivateOnExit && activator.Count <= 0) OnDeactivate();
        }



        
    }
    private void OnActivate()
    {
        if (isBlocked || locked)
        {
            if (!Door_isPlaying)
            {
                //if (ID != "") em.dialogUI.Invoke(ID, 4, 0);
                DoorType.SetValue(audioSourceObject.gameObject);
                Door_isPlaying = true;
                AkSoundEngine.SetSwitch("Interact_Type", "Locked", audioSourceObject.gameObject);
                AkSoundEngine.PostEvent("Play_InteractObject", audioSourceObject.gameObject);
                Invoke("DoorIsPlaying", 2f);
                if (locked)
                {
                    if (!CheckKey())
                    {
                        if (ID != "") em.dialogUI.Invoke(ID, 4, 0);
                        em.notifHUD.Invoke(4, true);
                        return;
                    }

                }
            }
            return;
        }

        activated = true;
        SideDetection();

        foreach (TriggerableObjects door in doors)
        {
            if (!door.door_isOpening)
            {
                door.OnActivate();
            }
            
        }

        if (timeline != null)timeline.OnActivate();

        for (int i = 0; i < activator.Count; i++)
        {
            if (activator[i] != opener)
            {
                activator[i].OnDoorOpen(activator.Count > 1, true);
            }
        }

        opener.OnDoorOpen(activator.Count > 1, false);
    }

    void DoorIsPlaying()
    {
        Door_isPlaying = false;
    }

    private void OnDeactivate()
    {
        activated = false;
        foreach (TriggerableObjects door in doors)
        {
            door.OnDeactivate();
        }
    }
    private void SideDetection()
    {
        Vector3 dir = (((MonoBehaviour)opener).transform.position - transform.position).normalized;
        if (Vector3.Dot(transform.forward, dir) < 0)
        {
            foreach (TriggerableObjects door in doors)
            {
                door.side = 1;
            }
        }
        else
        {
            foreach (TriggerableObjects door in doors)
            {
                door.side = -1;
            }
        }
    }

    private void CheckOtherSide()
    {
        for (int i = 0; i < activator.Count; i++)
        {
            activator[i].OnDoorOpen(activator.Count > 1, false);
        }
    }

    private bool CheckKey()
    {
        if (isUsed) return true;
        if (key == null) return false;
        if (key.qtyInventory >= 1)
        {
            locked = false;
            AddToSave();
            if (!isPLLevel)
            {
                key.Remove();
            }
            return true;
        }
        return false;
    }

    private void CreateId() {
        id = Mathf.RoundToInt((transform.position.x * 100) + (transform.position.y * 10) + transform.position.z);
    }

    public void AddToSave() {
        Man_Save.Instance.savedIngredientsTemp.Add(id);
    }

    public void Used() {
        isUsed = true;
        locked = false;
        //Debug.Log("This door is unlocked due to save or temp save");
    }
    private void CheckTempSave()
    {
        for (int i = 0; i < Man_Save.Instance.savedIngredientsTemp.Count; i++)
        {
            if (Man_Save.Instance.savedIngredientsTemp[i] == id)
            {
                Used();
            }
        }
    }
    public void UnlockDoorAndSave()
    {
        locked = false;
        AddToSave();
    }
}
