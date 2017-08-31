using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRunAnimation : BulletHitHandlerBase {

    public string m_TriggerName = "";


    public Behaviour[] m_ListOfBehavioursToDisable;
    public Collider[] m_ListOfCollidersToDisable;

    protected override void hit() {
        base.hit();
        //get animator
        Animator animator = GetComponent<Animator>();
        if (animator != null) {
            //if it exists then set the trigger
            animator.SetTrigger(m_TriggerName);
        }
        for (int i = 0; i < m_ListOfBehavioursToDisable.Length; i++) {
            m_ListOfBehavioursToDisable[i].enabled = false;
        }
        for (int i = 0; i < m_ListOfCollidersToDisable.Length; i++) {
            m_ListOfCollidersToDisable[i].enabled = false;
        }
    }

}
