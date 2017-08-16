using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArms : MonoBehaviour {

    /// <summary>
    /// m_Model: moves from facing down to m_Movingto, if joystick has no direction for that arm it moves to m_StartingRot
    /// m_MovingTo: moves to where the joystick is pointing, always rotates on the Y axis, this is also used as the projection for where the joystick is pointing
    /// m_ShootingArm: Follows m_MovingTo always rotates on the Y axis
    /// </summary>
    [System.Serializable]
    public class Arms {
        public Transform m_Model;
        public Transform m_MovingTo;
        public Transform m_ShootingArm;
        internal Quaternion m_StartingRot;
        internal bool m_HasDir;
    }

    public Arms m_LeftArm;
    public Arms m_RightArm;

    public float m_RotateSpeed = 200.0f;

    /// <summary>
    /// to stop arms from crossing through the body
    /// </summary>
    public bool m_LimitArms = true;
    [Range(0, 90)]
    public float m_LimitAngle = 45.0f;

    // Use this for initialization
    void Awake() {

        m_LeftArm.m_StartingRot = m_LeftArm.m_Model.rotation;
        m_RightArm.m_StartingRot = m_RightArm.m_Model.rotation;

        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);

    }

    // Update is called once per frame
    void Update() {
        JInput.Controller controller = JInput.CurrentController.currentController;
        if (controller == null) {
            return;
        }

        //get stick values
        Vector2 leftStick = new Vector2();
        leftStick.x = controller.getAxisValue(Keys.singleton.m_LeftArmMovementX);
        leftStick.y = controller.getAxisValue(Keys.singleton.m_LeftArmMovementY);
        Vector2 rightStick = new Vector2();
        rightStick.x = controller.getAxisValue(Keys.singleton.m_RightArmMovementX);
        rightStick.y = controller.getAxisValue(Keys.singleton.m_RightArmMovementY);

        if (leftStick.magnitude >= 0.1f) {//if there is then 0.1 moved on the stick 
            m_LeftArm.m_HasDir = true;

            //set rotation of the left arm to where the left stick is facing
            setRotation(m_LeftArm.m_MovingTo, leftStick);

        } else {
            m_LeftArm.m_HasDir = false;
        }

        if (rightStick.magnitude >= 0.1f) {//if there is then 0.1 moved on the stick                    
            m_RightArm.m_HasDir = true;

            //set rotation of the left arm to where the left stick is facing
            setRotation(m_RightArm.m_MovingTo, rightStick);

        } else {
            m_RightArm.m_HasDir = false;
        }
        //set the moveTo arms to be active or not depending if the sicks have high enough values
        m_LeftArm.m_MovingTo.gameObject.SetActive(m_LeftArm.m_HasDir);
        m_LeftArm.m_ShootingArm.gameObject.SetActive(m_LeftArm.m_HasDir);
        m_RightArm.m_MovingTo.gameObject.SetActive(m_RightArm.m_HasDir);
        m_RightArm.m_ShootingArm.gameObject.SetActive(m_RightArm.m_HasDir);


        moveArms(m_LeftArm);
        moveArms(m_RightArm);
    }


    private void setRotation(Transform a_Mover, Vector2 a_Dir) {

        //calc resulting angle
        float angle = Mathf.Atan2(a_Dir.x, -a_Dir.y) * Mathf.Rad2Deg;

        //get a quaternion version of angle
        Quaternion endRotation = Quaternion.Euler(new Vector3(0, angle, 0));

        a_Mover.rotation = endRotation;
    }


    private void rotateArm(Transform a_Mover, Quaternion a_MoveTo) {

        //get the difference between our angle and the arm
        float quatAngle = Quaternion.Angle(a_Mover.rotation, a_MoveTo);
        //how long it should take to complete by using rotate speed
        float timeToComplete = quatAngle / m_RotateSpeed;
        //how far do we have to move towards the final angle at the same speed
        float percentage = Mathf.Min(1.0f, Time.unscaledDeltaTime / timeToComplete);
        //apply angle via slerp between current angle and our final angle
        a_Mover.rotation = Quaternion.Slerp(a_Mover.rotation, a_MoveTo, percentage);
    }

    private void moveArms(Arms a_Arm) {
        if (a_Arm.m_HasDir) {
            //limit the m_MovingTo transform
            limitArmMovements();

            //move model to moveTo
            rotateArm(a_Arm.m_Model, a_Arm.m_MovingTo.rotation);

            //get models rotation without the x or z
            Vector3 rot = a_Arm.m_Model.rotation.eulerAngles;
            rot.x = rot.z = 0;
            Quaternion flatRot = Quaternion.Euler(rot);

            //apply rotation
            a_Arm.m_ShootingArm.rotation = flatRot;

        } else {
            //move model arm to default
            rotateArm(a_Arm.m_Model, a_Arm.m_StartingRot);
        }
    }

    private void limitArmMovements() {
        if (m_LimitArms) {
            float movementLimit = 90 - m_LimitAngle;
            //left arm
            Vector3 leftArmRot = m_LeftArm.m_MovingTo.localRotation.eulerAngles;
            //does top check
            //if the arm is directly right and movementLimit up
            if (leftArmRot.y >= movementLimit && leftArmRot.y <= 90) {
                leftArmRot.y = movementLimit;
            }
            //does bottom check
            //if the arm is directly right and movementLimit down
            if (leftArmRot.y <= 180 - movementLimit && leftArmRot.y >= 90) {
                leftArmRot.y = 180 - movementLimit;
            }
            m_LeftArm.m_MovingTo.localRotation = Quaternion.Euler(leftArmRot);
            //right arm
            Vector3 rightArmRot = m_RightArm.m_MovingTo.localRotation.eulerAngles;
            //does bottom check
            //directly left and movement limit down
            if (rightArmRot.y < 360 - movementLimit && rightArmRot.y > 270) {
                rightArmRot.y = 360 - movementLimit;
            }
            //does top check
            if (rightArmRot.y > 180 + movementLimit && rightArmRot.y < 270) {
                rightArmRot.y = 180 + movementLimit;
            }
            m_RightArm.m_MovingTo.localRotation = Quaternion.Euler(rightArmRot);
        }
    }

    private void stateChanged(GameStates a_NewState) {
        switch (a_NewState) {
            case GameStates.Planning:
                hideArms();
                enabled = false;
                break;
            case GameStates.Action:
                enabled = true;
                break;
            default:
                enabled = false;
                break;
        }
    }

    /// <summary>
    /// hides both arms
    /// </summary>
    private void hideArms() {
        m_LeftArm.m_ShootingArm.gameObject.SetActive(false);
        m_RightArm.m_ShootingArm.gameObject.SetActive(false);
        //m_LeftArm.gameObject.SetActive(false);
        //m_RightArm.gameObject.SetActive(false);
    }
}
