using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour {

    public Animator animator;
    public List<Rigidbody> rigidbodies = new List<Rigidbody>();

    public bool RagdollOn
    {
        set
        {
            animator.enabled = !value;
            foreach (Rigidbody r in rigidbodies)
                r.isKinematic = !value;
        }
    }

	void Start ()
    {
        animator = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        Transform[] allChildren = GetComponentsInChildren<Transform>();

        foreach(Transform child in allChildren)
        {
            if(child.gameObject.GetComponent<Rigidbody>() != null)
            rigidbodies.Add(child.gameObject.GetComponent<Rigidbody>());
        }

        RagdollOn = false;
        
    }
	
}
