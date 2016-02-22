using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    public int damage;
    public string ownedTag;
	void OnTriggerEnter(Collider hit)
    {
        //Check the impact isn't on the object firing or an ally
        if(!hit.CompareTag(ownedTag))
        {
            //Collect all the monobehaviours on the object incase it has more than one
            MonoBehaviour[] scripts = hit.transform.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour mb in scripts)
            {
                //if one of the scripts implements the interface for being hit, then run that method
                if (mb is iHurtable)
                {
                    //Look into interfaces again, apply 'hit'
                    iHurtable h = (iHurtable)mb;
                    h.Hit(damage, transform.position);
                }
            }
            Destroy(gameObject);
        }
    }
}
