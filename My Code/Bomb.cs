using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Mathias Sorin
 * 2021-04-22
 */
public class Bomb : Throwable_Parent
{
    public float explosionRadius = 5f;
    public float explosionDelay = 3f;
    public float explosionForce = 700f;

    private bool isExploded = false;

    private bool checkCollision = false;
    private bool checkFloor = false;

    public GameObject explosionAudioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (!checkCollision) {
            checkCollision = true;
            //#SOUND for collision

            //Debug.Log("Bomb_Collision_01");

            AkSoundEngine.PostEvent("Play_Bomb_Hit", gameObject.GetComponentInChildren<AkRoomAwareObject>().gameObject);

        }
        AkSoundEngine.PostEvent("Play_Bomb_Hit", gameObject.GetComponentInChildren<AkRoomAwareObject>().gameObject);

        if (isExploded)
        {
            return;
        }
        isExploded = true;
        GetComponent<Rigidbody>().mass = 100;
        Invoke("Explode", explosionDelay);
        /*if (collision.gameObject.layer == 10) {

            
            if (!checkFloor) {
                //#SOUND for Floor Collision
                checkFloor = true;

                //Debug.Log("Bomb_Collision_02");

                AkSoundEngine.PostEvent("Play_Bomb_Hit", gameObject.GetComponentInChildren<AkRoomAwareObject>().gameObject);

                if (isExploded) {
                    return;
                }
                isExploded = true;
                GetComponent<Rigidbody>().mass = 100;
                Invoke("Explode", explosionDelay);
            }
        }*/
        // ------ Post Bomb Collison SFX Here

    }

    private void Explode()
    {
        em.spawnFX.Invoke("Explosion", transform.position, Quaternion.identity); //Add :Patrick Vasile 06-02-2021
        Play_Sound_Explode();
        Collider[] colliderList;
        colliderList = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliderList)
        {
            IBombable obj = collider.gameObject.GetComponent<IBombable>();
            IDamageable id = collider.gameObject.GetComponentInParent<IDamageable>();
            if (obj != null)
            {
                obj.OnExplosion();
            }
            if(id != null) {
                id.TakeDamage(5, false, false, true, false, 0f);
            }

        }
        Collider[] colliderList2;
        colliderList2 = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliderList2)
        {
            Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
            IBombable obj = collider.gameObject.GetComponent<IBombable>();
            if (rb != null && obj != null) //permet deviter de eject les pushables objects ou anything with a rigidbody in the air.
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }



        
        em.returnToPoolThrow.Invoke(this);
        GetComponent<Rigidbody>().mass = 1;
        isExploded = false;
        checkCollision = false;
        checkFloor = false;



        void Play_Sound_Explode()
        {
            AkSoundEngine.PostEvent("Play_Bomb_Explosion", explosionAudioSource);
        }

    }
}