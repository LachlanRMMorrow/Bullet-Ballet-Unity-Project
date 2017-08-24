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

        m_LeftArm.m_StartingRot = m_LeftArm.m_Model.rotation;
        m_RightArm.m_StartingRot = m_RightArm.m_Model.rotation;

        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);

        m_LeftArm.m_IsRight = false;
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

    private void moveArms(Arms a_Arm) {
        if (a_Arm.m_HasDir) {
            //limit the m_MovingTo transform
            limitArmMovements(a_Arm);

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
        print(armRot.y);

        float behindAngle = -m_BehindAngle;
        float sideAngle = m_SideAngle;

        bool isAngleAboveHalf = armRot.y < 180;


        if (a_Arm.m_IsRight) {
            if (armRot.y > 180 - m_BehindAngle && isAngleAboveHalf) {
                armRot.y = 180 - m_BehindAngle;
                print("behind");
            }
            if (armRot.y < 180 + m_SideAngle && !isAngleAboveHalf) {
                armRot.y = 180 + m_SideAngle;
                print("side");
            }
        } else {
            float side = (m_SideAngle) % 360;
            if (armRot.y < 180 + m_BehindAngle && !isAngleAboveHalf) {
                armRot.y = 180 + m_BehindAngle;
                print("behind");
            }
            if (armRot.y > 180 - side && isAngleAboveHalf) {
                armRot.y = 180 - side;
                print("side");
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
        m_LeftArm.m_ShootingArm.gameObject.SetActive(false);
        m_RightArm.m_ShootingArm.gameObject.SetActive(false);
        //m_LeftArm.gameObject.SetActive(false);
        //m_RightArm.gameObject.SetActive(false);
    }
}

/// <summary>
/// Averaging Quaternions using code from here http://wiki.unity3d.com/index.php/Averaging_Quaternions_and_Vectors
/// </summary>
class QuatMath {
    //Get an average (mean) from more then two quaternions (with two, slerp would be used).
    //Note: this only works if all the quaternions are relatively close together.
    //Usage: 
    //-Cumulative is an external Vector4 which holds all the added x y z and w components.
    //-newRotation is the next rotation to be added to the average pool
    //-firstRotation is the first quaternion of the array to be averaged
    //-addAmount holds the total amount of quaternions which are currently added
    //This function returns the current average quaternion
    public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount) {

        float w = 0.0f;
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        //Before we add the new rotation to the average (mean), we have to check whether the quaternion has to be inverted. Because
        //q and -q are the same rotation, but cannot be averaged, we have to make sure they are all the same.
        if (!QuatMath.AreQuaternionsClose(newRotation, firstRotation)) {

            newRotation = QuatMath.InverseSignQuaternion(newRotation);
        }

        //Average the values
        float addDet = 1f / (float)addAmount;
        cumulative.w += newRotation.w;
        w = cumulative.w * addDet;
        cumulative.x += newRotation.x;
        x = cumulative.x * addDet;
        cumulative.y += newRotation.y;
        y = cumulative.y * addDet;
        cumulative.z += newRotation.z;
        z = cumulative.z * addDet;

        //note: if speed is an issue, you can skip the normalization step
        return NormalizeQuaternion(x, y, z, w);
    }

    public static Quaternion NormalizeQuaternion(float x, float y, float z, float w) {

        float lengthD = 1.0f / (w * w + x * x + y * y + z * z);
        w *= lengthD;
        x *= lengthD;
        y *= lengthD;
        z *= lengthD;

        return new Quaternion(x, y, z, w);
    }

    //Changes the sign of the quaternion components. This is not the same as the inverse.
    public static Quaternion InverseSignQuaternion(Quaternion q) {

        return new Quaternion(-q.x, -q.y, -q.z, -q.w);
    }

    //Returns true if the two input quaternions are close to each other. This can
    //be used to check whether or not one of two quaternions which are supposed to
    //be very similar but has its component signs reversed (q has the same rotation as
    //-q)
    public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2) {

        float dot = Quaternion.Dot(q1, q2);

        if (dot < 0.0f) {

            return false;
        } else {

            return true;
        }
    }
}
