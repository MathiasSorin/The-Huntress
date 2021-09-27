using UnityEngine;

public class SoclePorte : TriggerableObjects
{
    public GameObject obj;
    public Transform pos;
    private bool activated = false;
    [HideInInspector]
    public int id;

    private void Start() {
        CreateId();
        CheckTempSave();
    }
    public override void OnActivate()
    {
        if (activated) return;
        AddToSave();
        activated = true;
        GameObject temp = Instantiate(obj, pos.position, pos.rotation);
        Destroy(temp.GetComponent<QuestPickups>());
    }

    public override void OnDeactivate()
    {
        
    }

    private void CreateId() {
        id = Mathf.RoundToInt((transform.position.x * 100) + (transform.position.y * 10) + transform.position.z);
    }

    public void AddToSave() {
        Man_Save.Instance.savedIngredientsTemp.Add(id);
    }

    public void Used() {
        activated = true;
        GameObject temp = Instantiate(obj, pos.position, pos.rotation);
        Destroy(temp.GetComponent<QuestPickups>());
    }

    private void CheckTempSave() {
        for (int i = 0; i < Man_Save.Instance.savedIngredientsTemp.Count; i++) {
            if (Man_Save.Instance.savedIngredientsTemp[i] == id) {
                Used();
            }
        }
    }
}
