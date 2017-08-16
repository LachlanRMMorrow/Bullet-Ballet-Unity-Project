using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour {

    public JInput.ControllerButtons m_PlanningModeSwap = JInput.ControllerButtons.Y;

    public JInput.ControllerAxes m_PlanningWayPointMovementX = JInput.ControllerAxes.LStickX;
    public JInput.ControllerAxes m_PlanningWayPointMovementY = JInput.ControllerAxes.LStickY;
    public JInput.ControllerButtons m_PlanningModeApply = JInput.ControllerButtons.A;

    public JInput.ControllerAxes m_LeftArmMovementX = JInput.ControllerAxes.LStickX;
    public JInput.ControllerAxes m_LeftArmMovementY = JInput.ControllerAxes.LStickY;
    public JInput.ControllerAxes m_RightArmMovementX = JInput.ControllerAxes.RStickX;
    public JInput.ControllerAxes m_RightArmMovementY = JInput.ControllerAxes.RStickY;

	public JInput.ControllerButtons m_SlowMoButton = JInput.ControllerButtons.B;

    public JInput.ControllerButtons m_LeftWeaponReload = JInput.ControllerButtons.LB;
    public JInput.ControllerButtons m_RightWeaponReload = JInput.ControllerButtons.RB;


    private static Keys m_Singleton;
    public static Keys singleton { get { return m_Singleton; } }

    void Awake() {
        m_Singleton = this;
    }
    void OnEnable() {
        m_Singleton = this;
    }

}
