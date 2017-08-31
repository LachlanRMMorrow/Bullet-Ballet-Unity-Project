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
        internal bool m_IsRight = true;
        internal PlayerShoot m_ArmShootScript;
    }

    public Arms m_LeftArm;
    public Arms m_RightArm;

    public float m_RotateSpeed = 200.0f;

    [Header("Player/Arm Rotation")]
    public bool m_RotateToFaceArms = true;

    [Range(0, 180)]
    public float m_BehindAngle = 45.0f;
    [Range(0, 180)]
    public float m_SideAngle = 45.0f;

    // Use this for initialization
    void Awake() {

        m_LeftArm.m_StartingRot = m_LeftArm.m_Model.localRotation;
        m_RightArm.m_StartingRot = m_RightArm.m_Model.localRotation;

        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);

        m_LeftArm.m_IsRight = false;

        m_LeftArm.m_ArmShootScript = m_LeftArm.m_ShootingArm.GetComponent<PlayerShoot>();
        m_RightArm.m_ArmShootScript = m_RightArm.m_ShootingArm.GetComponent<PlayerShoot>();
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
        //m_LeftArm.m_ShootingArm.gameObject.SetActive(m_LeftArm.m_HasDir);
        m_LeftArm.m_ArmShootScript.m_CanShoot = m_LeftArm.m_HasDir;
        m_RightArm.m_MovingTo.gameObject.SetActive(m_RightArm.m_HasDir);
        //m_RightArm.m_ShootingArm.gameObject.SetActive(m_RightArm.m_HasDir);
        m_RightArm.m_ArmShootScript.m_CanShoot = m_RightArm.m_HasDir;

        moveArms(m_LeftArm);
        moveArms(m_RightArm);

        if (m_RotateToFaceArms) {
            calcPlayerRotation();
        }

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

    private void rotateArmLocal(Transform a_Mover, Quaternion a_MoveTo) {

        //get the difference between our angle and the arm
        float quatAngle = Quaternion.Angle(a_Mover.localRotation, a_MoveTo);
        //how long it should take to complete by using rotate speed
        float timeToComplete = quatAngle / m_RotateSpeed;
        //how far do we have to move towards the final angle at the same speed
        float percentage = Mathf.Min(1.0f, Time.unscaledDeltaTime / timeToComplete);
        //apply angle via slerp between current angle and our final angle
        a_Mover.localRotation = Quaternion.Slerp(a_Mover.localRotation, a_MoveTo, percentage);
    }

    private void moveArms(Arms a_Arm) {
        if (a_Arm.m_HasDir) {
            //limit the m_MovingTo transform
            limitArmMovements(a_Arm);

            Vector3 rot2 = a_Arm.m_MovingTo.rotation.eulerAngles;
            //arm rotation is off by 90 degrees
                rot2.y += 90;
            Quaternion quat = Quaternion.Euler(rot2);

            //move model to moveTo
            rotateArm(a_Arm.m_Model, quat);

            //get models rotation without the x or z
            Vector3 rot = a_Arm.m_Model.rotation.eulerAngles;
            rot.x = rot.z = 0;
            //shooting is also 90 degrees off
            rot.y -= 90;
            Quaternion flatRot = Quaternion.Euler(rot);


            //apply rotation
            a_Arm.m_ShootingArm.rotation = flatRot;

        } else {
            //move model arm to default
            rotateArmLocal(a_Arm.m_Model, a_Arm.m_StartingRot);
        }
    }

    private void calcPlayerRotation() {
        float desiredRotY = 0;
        Quaternion quatRot = Quaternion.identity;
        int added = 0;

        if (m_RightArm.m_HasDir) {
            quatRot = m_RightArm.m_MovingTo.rotation;
            desiredRotY += (m_RightArm.m_MovingTo.rotation.eulerAngles.y + 180) % 360;
            added++;
        }
        if (m_LeftArm.m_HasDir) {
            if (added == 0) {
                quatRot = m_LeftArm.m_MovingTo.rotation;
            } else {
                quatRot = Quaternion.Slerp(quatRot, m_LeftArm.m_MovingTo.rotation, 0.5f);

            }
            desiredRotY += (m_LeftArm.m_MovingTo.rotation.eulerAngles.y + 180) % 360;
            added++;
        }

        if (added == 0) {
            return;
        } else {
            desiredRotY /= added;
            desiredRotY -= 180;
        }

        Vector3 rot = transform.rotation.eulerAngles;
        rot.y = desiredRotY;

        //transform.rotation = Quaternion.Euler(rot);
        transform.rotation = quatRot;
    }

    private void limitArmMovements(Arms a_Arm) {
        //backwards rot limit


        Vector3 armRot = a_Arm.m_MovingTo.localRotation.eulerAngles;

        float behindAngle = -m_BehindAngle;
        float sideAngle = m_SideAngle;

        bool isAngleAboveHalf = armRot.y < 180;


        if (a_Arm.m_IsRight) {
            if (armRot.y > 180 - m_BehindAngle && isAngleAboveHalf) {
                armRot.y = 180 - m_BehindAngle;
            }
            if (armRot.y < 180 + m_SideAngle && !isAngleAboveHalf) {
                armRot.y = 180 + m_SideAngle;
            }
        } else {
            float side = (m_SideAngle) % 360;
            if (armRot.y < 180 + m_BehindAngle && !isAngleAboveHalf) {
                armRot.y = 180 + m_BehindAngle;
            }
            if (armRot.y > 180 - side && isAngleAboveHalf) {
                armRot.y = 180 - side;
            }
        }

        a_Arm.m_MovingTo.localRotation = Quaternion.Euler(armRot);

        /*
        //todo: make a better version of this, that will limit the arms to not go back and not go to far across
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
        */
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
        //m_LeftArm.m_ShootingArm.gameObject.SetActive(false);
        //m_RightArm.m_ShootingArm.gameObject.SetActive(false);
        m_LeftArm.m_ArmShootScript.m_CanShoot = false;
        m_RightArm.m_ArmShootScript.m_CanShoot = false;
        //m_LeftArm.gameObject.SetActive(false);
        //m_RightArm.gameObject.SetActive(false);
    }
}