using UnityEngine;

public class ValveLight : TriggerableObjects
{
    public AkRoomAwareObject roomAware;
    bool IsActivated=false;

    private GameObject lit;
    [HideInInspector]
    public int id;
    private void Start()
    {
        lit = GetComponent<Light>().gameObject;
        lit.SetActive(false);
        CreateId();
        CheckTempSave();
    }

    public override void OnActivate()
    {
        if(!IsActivated)
        {
            lit.SetActive(true);
            AkSoundEngine.PostEvent("Play_Gas_Light", roomAware.gameObject);
            IsActivated = true;
            AddToSave();
        }
    }

    public override void OnDeactivate()
    {
        if (IsActivated)
        {
            lit.SetActive(false);
            AkSoundEngine.PostEvent("Stop_Gas_Light", roomAware.gameObject);
            IsActivated = false;
        }
    }

    private void CreateId() {
        id = Mathf.RoundToInt((transform.position.x * 100) + (transform.position.y * 10) + transform.position.z);
    }

    public void AddToSave() {
        Man_Save.Instance.savedIngredientsTemp.Add(id);
    }

    public void Used() {
        lit.SetActive(true);
        IsActivated = true;
    }

    private void CheckTempSave() {
        for (int i = 0; i < Man_Save.Instance.savedIngredientsTemp.Count; i++) {
            if (Man_Save.Instance.savedIngredientsTemp[i] == id) {
                Used();
            }
        }
    }
}
