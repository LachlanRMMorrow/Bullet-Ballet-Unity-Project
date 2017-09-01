using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

    public enum TriggerAxes {
        LeftTrigger = 4,//4 as a reference to the Controller Axes enum
        RightTrigger//this will be 5
    }

    public TriggerAxes m_ShootTrigger = TriggerAxes.LeftTrigger;
    public Transform m_ShootPoint;

    //weapon stuff
    //move to a weapon class
    private bool m_HasShot = false;
    public WeaponShooter m_CurrentWeapon;

    public bool m_CanShoot = true;

    // Use this for initialization
    void Start() {

        if (m_ShootPoint == null) {
            m_ShootPoint = transform;
        }


    }

    // Update is called once per frame
    void Update() {
        if (SlowMoManager.m_isPaused) {
            return;
        }
        JInput.Controller controller = JInput.CurrentController.currentController;
        if (controller == null) {
            return;
        }

        //does this script use the left trigger?
        bool triggerLeft = m_ShootTrigger == TriggerAxes.LeftTrigger;

        //can this arm shoot or not?
        if (m_CanShoot) {


            ///shoot

            //is the trigger down more then 80% of the way
            bool isTriggerDown = controller.getTriggerValue(triggerLeft) >= 0.8f;


            if (isTriggerDown) {
                //check if the player has not shot already
                //this is so the player has to release the trigger to fire again
                if (!m_HasShot) {
                    m_HasShot = true;

                    m_CurrentWeapon.shoot(m_ShootPoint);
                }
            } else {//trigger is released, allow player to shoot again
                m_HasShot = false;
            }
        }

        ///reload
        JInput.ControllerButtons reloadButton = triggerLeft ? Keys.singleton.m_LeftWeaponReload : Keys.singleton.m_RightWeaponReload;
        if (controller.WasButtonPressed(reloadButton)) {
            m_CurrentWeapon.reload();
        }
    }

    void OnValidate() {
        //no referenced weapon
        if (m_CurrentWeapon == null) {
            m_CurrentWeapon = GetComponent<WeaponShooter>();
            //no weapon on same object
            if (m_CurrentWeapon == null) {
                //do error
                Debug.LogError("Player cant find a weapon!!, it can be a reference or on the same object");
            }
        }
    }
}
