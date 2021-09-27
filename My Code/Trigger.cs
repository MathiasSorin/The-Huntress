using UnityEngine;
/*
 * Mathias Sorin
 * 2021-04-22
 */
public class Trigger : MonoBehaviour
{
    [Tooltip("Who triggers it (player by default)")]
    [SerializeField]
    private GameObject activator;
    /*[Tooltip("Main Camera")]//Vincent Charland 2021-05-19 TESTS VINCENT C. DO NOT DELETE YET
    [SerializeField]//Vincent Charland 2021-05-19
    private GameObject mainCam;//Vincent Charland 2021-05-19*/
    [Tooltip("What does it trigger")]
    [SerializeField]
    private TriggerableObjects triggerableObject;
    public GameObject objToActivate;
    //public GameObject objectToActivate;
    [Tooltip("Will Deactivate the triggerable object when exiting trigger")]
    [SerializeField]
    private bool deactivateOnExit = false;
    [Tooltip("Can only be activated once")]
    [SerializeField]
    private bool singleUse = false;
    [Tooltip("Detects from wich side trigger was activated (front or back)")]
    [SerializeField]
    private bool sideDetection = false;
    [Tooltip("Check to require player input when in trigger (interact key)")]
    [SerializeField]
    private bool playerInput = false;
    [Tooltip("Check to require player looking at triggerable object when in trigger")]//Vincent Charland 2021-05-19 TESTS VINCENT C. DO NOT DELETE YET
    [SerializeField]//Vincent Charland 2021-05-19 
    private bool onLook = false;//Vincent Charland 2021-05-19 */
    public bool toggle = false; // Daisy
    //private AvatarMaster avatar;
    private bool canTriggerLook = true;

    private bool activated = false;
    private bool inTrigger = false;
    private bool triggerCooldown = false;
    private Man_Input manInput;

    public SO_EventManager em;

    [HideInInspector]
    public int id;


    private void Start()
    {
        //avatar = Man_Game.Instance.character;
        manInput = FindObjectOfType<Man_Input>();
        if (!activator)
        {
            activator = FindObjectOfType<AvatarMaster>()?.gameObject;
            if (!activator)
            {
                Debug.LogWarning("Avatar is missing from the scene");
            }
        }
        CreateId();
        CheckTempSave();
    }
    private void Update()
    {
        if (!inTrigger)
        {
            return;
        }
        //PlayerInput added to fix a bug related to valves if bugs are created by this just remove playerInput from the condition
        else if (inTrigger && manInput.InteractPressed && playerInput)
        {
            if (toggle && activated)
            {
                OnDeactivate();
            }
            OnActivate();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (activator == null && other.CompareTag("Player") || activator == other.gameObject)
        {
            inTrigger = true;
        }

        if (singleUse && activated)
        {
            return;
        }

        if (playerInput)
        {
            if (inTrigger) em.notifHUD.Invoke(0, true);
            return;
        }

        if (other.gameObject != activator)
        {
            return;
        }
        if (onLook) //Vincent C. 2021-07-13
        {
            return;
        }
        
        if (!activated)
        {
            OnActivate();
        }
        else
        {
            OnDeactivate();
        }
    }
    //TESTS VINCENT C. DO NOT DELETE YET
    private void OnTriggerStay(Collider other) //Vincent Charland 2021-05-19 
    {
        if (onLook)//Vincent C. 2021-07-13
        {
            if (canTriggerLook)
            {
                if (objToActivate.GetComponent<Renderer>().isVisible)
                {
                    canTriggerLook = false;
                    Debug.Log("je suis visible");
                    OnActivate();
                }
            }
        }
        else return;
    }

    private void OnTriggerExit(Collider other)
    {
        if (activator == null && other.CompareTag("Player") || activator == other.gameObject)
        {
            inTrigger = false;
            em.notifHUD.Invoke(0, false);
        }

        if (deactivateOnExit)
        {
            OnDeactivate();
        }
      /*  //TESTS VINCENT C. DO NOT DELETE YET
        if (onLook)//Vincent Charland 2021-05-19 
        {
            RaycastHit hit;
            Debug.DrawRay(avatar.camM.transform.position, avatar.camM.transform.forward, Color.red);

            if (Physics.Raycast(avatar.camM.transform.position, avatar.camM.transform.forward, out hit, 1000f))
            {
                if (hit.collider.gameObject != triggerableObject)
                {
                    Debug.Log(hit.collider.gameObject);
                    Debug.Log("Je hit pu le triggerable obj");
                    OnDeactivate();
                }
            }
        }*/

    }
    public void OnActivate()
    {
        if(triggerCooldown)
        {
            return;
        }
        triggerCooldown = true;
        Invoke("TriggerCooldownReset", 0.5f);
        activated = true;
        AddToSave();
        if (sideDetection)
        {
            SideDetection();
        }
        triggerableObject.OnActivate();
        //em.notifHUD.Invoke(0, false);
    }
    private void OnDeactivate()
    {
        activated = false;
        triggerableObject.OnDeactivate();
    }
    private void SideDetection()
    {
        //To update with the new dot product in Trigger door
        if (Vector3.Dot(transform.forward, activator.transform.forward) < 0)
        {
            triggerableObject.side = -1;
        }
        else
        {
            triggerableObject.side = 1;
        }
    }
    private void TriggerCooldownReset()
    {
        triggerCooldown = false;
    }

    private void CreateId() {
        id = Mathf.RoundToInt((transform.position.x * 100) + (transform.position.y * 10) + transform.position.z);
    }

    public void AddToSave() {
        Man_Save.Instance.savedIngredientsTemp.Add(id);
    }

    public void Used() {
        if (!singleUse) {
            activated = true;
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

