using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
/*
 * Mathias Sorin
 * 2021-04-22
 */
public class Bait : Throwable_Parent {
    //public SO_EventManager em;
    public float explosionRadius = 5f;
    public float explosionDelay = 3f;
    public float explosionInterval = 1f;
    public int explosions = 5;
    public VisualEffect vfxBoom;
    private int explosion = 0;

    private bool checkCollision = false;
    private bool checkFloor = false;

    private void Start() {

        vfxBoom = Man_Game.Instance.GetComponentInChildren<Man_PoolFx>().poolFX[17].fxObject.GetComponent<VisualEffect>();


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!checkCollision) {
            //#SOUND Collision

            //Debug.Log("SFX_Collision_01");

            AkSoundEngine.PostEvent("Play_Leurre_Hit", gameObject.GetComponentInChildren<AkRoomAwareObject>().gameObject);

            checkCollision = true;
        }
        if (collision.gameObject.layer == 10) {
            if (!checkFloor) {
                //#SOUND for Floor Collision

                AkSoundEngine.PostEvent("Play_Leurre_Explosion", gameObject.GetComponentInChildren<AkRoomAwareObject>().gameObject);
                AkSoundEngine.PostEvent("Play_Leurre_Hit", gameObject.GetComponentInChildren<AkRoomAwareObject>().gameObject);


                //Debug.Log("Bomb_Collision_02");

                checkFloor = true;

                Invoke("Explode", explosionDelay);
                em.spawnFX.Invoke("Bait", transform.position, Quaternion.identity); //Add :Patrick Vasile 06-10-2021
                //#SOUND Bait/Leurre/Lure
                 //Debug.Log("Leurre/Bait/Lure_Collision");
            }

        }

        
    }

    private void Explode() {
        /*Collider[] colliderList;
        colliderList = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliderList)
        {
            IBombable temp = collider.gameObject.GetComponent<IBombable>(); //Change this for enemy hearing
            if (temp != null)
            {
                //temp.Activate(); //Change this for enemy hearing
            }
        }*/
        em.soundEvent.Invoke(transform.position, explosionRadius);
        if (explosion < explosions) {
            explosion++;
            Invoke("Explode", explosionInterval);

            
        }



        //else Destroy(this.gameObject);
        else {
            
            CancelInvoke("Explode");
            explosion = 0;
            em.returnToPoolThrow.Invoke(this);
            vfxBoom.Stop();
            checkCollision = false;
            checkFloor = false;
            //Debug.Log("Leurre/Bait/Lure_Destroy");
        }
    }
}