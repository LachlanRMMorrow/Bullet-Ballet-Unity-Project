using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArms : MonoBehaviour {
    //todo: we can remove the m_MovingTo and the m_ShootingArm, since those objects are now empty/not being used

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
        //the transform that the bullets shoot from, they use the rotation and position of this transform
        //it's taken from a variable in PlayerShoot on the m_ShootingArm
        internal Transform m_ShootPoint;
        //starting local rotation of m_ShootPoint
        internal Quaternion m_ShootPointStartingLocalRot;
        internal Quaternion m_StartingRot;
        internal bool m_HasDir;
        internal bool m_IsRight = true;
        internal PlayerShoot m_ArmShootScript;
        internal Animator m_ArmAnimator;
    }

    public Arms m_LeftArm;
    public Arms m_RightArm;

    public float m_RotateSpeed = 200.0f;

    public bool m_CanMoveArms = true;


    [Header("Player/Arm Rotation")]
    public bool m_RotateToFaceArms = true;
    [Range(1, 25)]
    public float m_PlayerBodyRotateSpeed = 10.0f;

    [Range(80, 180)]
    public float m_BehindAngle = 45.0f;
    [Range(80, 180)]
    public float m_SideAngle = 45.0f;

    [Header("Aim assist")]
    /// <summary>
    /// distance at which the aim assist will be at max power
    /// </summary>
    public float m_DistanceForMaxAssist = 20.0f;

    // Use this for initialization
    void Awake() {

        m_LeftArm.m_StartingRot = m_LeftArm.m_Model.localRotation;
        m_RightArm.m_StartingRot = m_RightArm.m_Model.localRotation;

        m_LeftArm.m_ShootPoint = m_LeftArm.m_ShootingArm.GetComponent<PlayerShoot>().m_ShootPoint;
        m_RightArm.m_ShootPoint = m_RightArm.m_ShootingArm.GetComponent<PlayerShoot>().m_ShootPoint;
        m_LeftArm.m_ShootPointStartingLocalRot = m_LeftArm.m_ShootPoint.localRotation;
        m_RightArm.m_ShootPointStartingLocalRot = m_RightArm.m_ShootPoint.localRotation;

        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);

        m_LeftArm.m_ArmAnimator = m_LeftArm.m_Model.parent.GetComponent<Animator>();
        m_RightArm.m_ArmAnimator = m_RightArm.m_Model.parent.GetComponent<Animator>();
        if (m_LeftArm.m_ArmAnimator == null || m_RightArm.m_ArmAnimator == null) {
            Debug.LogError("Player Arms cant find the model animator");
        }

        m_LeftArm.m_IsRight = false;

        m_LeftArm.m_ArmShootScript = m_LeftArm.m_ShootingArm.GetComponent<PlayerShoot>();
        m_RightArm.m_ArmShootScript = m_RightArm.m_ShootingArm.GetComponent<PlayerShoot>();
    }

    // Update is called once per frame
    void Update() {
        if (SlowMoManager.m_isPaused) {
            return;
        }

        //get controller
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


        //apply stick positions to animator
        if (m_LeftArm.m_ArmAnimator != null) {
            m_LeftArm.m_ArmAnimator.SetFloat("Blend_X", leftStick.x);
            m_LeftArm.m_ArmAnimator.SetFloat("Blend_Y", -leftStick.y);
        }
        if (m_RightArm.m_ArmAnimator != null) {
            m_RightArm.m_ArmAnimator.SetFloat("Blend_X", rightStick.x);
            m_RightArm.m_ArmAnimator.SetFloat("Blend_Y", -rightStick.y);
        }

        //if there is then 0.1 moved on the sticks
        bool isLeftStickBeingUsed = leftStick.magnitude >= 0.1f;
        bool isRightStickBeingUsed = rightStick.magnitude >= 0.1f;

        //apply hasDir to arms based on stick movement
        m_LeftArm.m_HasDir = isLeftStickBeingUsed;
        m_RightArm.m_HasDir = isRightStickBeingUsed;



        //set the rotation of the sticks if it is being used
        if (isLeftStickBeingUsed) {
            //set rotation of the left arm to where the left stick is facing
            setRotation(m_LeftArm.m_MovingTo, leftStick);
        }

        if (isRightStickBeingUsed) {
            //set rotation of the left arm to where the left stick is facing
            setRotation(m_RightArm.m_MovingTo, rightStick);
        }


        //stop the arms from moving if m_CanMoveArms is false
        if (!m_CanMoveArms) {
            m_LeftArm.m_HasDir = m_RightArm.m_HasDir = false;
        }


        //set the moveTo arms to be active or not depending if the sicks have high enough values
        m_LeftArm.m_MovingTo.gameObject.SetActive(m_LeftArm.m_HasDir);
        m_LeftArm.m_ArmShootScript.m_CanShoot = m_LeftArm.m_HasDir;

        m_RightArm.m_MovingTo.gameObject.SetActive(m_RightArm.m_HasDir);
        m_RightArm.m_ArmShootScript.m_CanShoot = m_RightArm.m_HasDir;


        //move both arms
        moveArms(m_LeftArm);
        moveArms(m_RightArm);


        //rotate players body based on arm movement
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

            runAutoAim(a_Arm);

        } else {
            //move model arm to default
            rotateArmLocal(a_Arm.m_Model, a_Arm.m_StartingRot);
        }
    }

    private void runAutoAim(Arms a_Arm) {
        //add a small amount of aim assistance to the arms rotation

        //things to add to aim assist
        // - stop helping when aiming through walls (just needs a layer mask change)
        // - start effecting the aim when the object is close enough (don't assist when the enemy is far away)
        // - (DONE) better way to allow for easy modification of variables 
        // - (DONE) only apply offset to the shooting arm and not change the models rotation (so it doesn't effect the gun's laser)

        RaycastHit hit;
        //only hit objects which are in the AimAssistance layer
        int layerMask = (1 << LayerMask.NameToLayer("AimAssistance"));
        //do the ray cast, with the direction, remove the y axis so it doesn't point up or down and miss the collider
        if (Physics.Raycast(a_Arm.m_Model.position, Vector3.Scale(new Vector3(1, 0, 1), a_Arm.m_ShootingArm.forward), out hit, 100, layerMask)) {
            //store the current rotation
            Quaternion normalRotation = a_Arm.m_ShootingArm.rotation;
            //have it look at the hit transform
            a_Arm.m_ShootPoint.LookAt(hit.transform);

            //ratio of distance between the max distance and the current distance
            //this is used to calculate the power of the aim assist
            //the closer the object is, the less powerful
            float distance = Mathf.Clamp01(hit.distance/ m_DistanceForMaxAssist);

            //set the rotation to be between the current rotation and the hit transform
            a_Arm.m_ShootPoint.rotation = Quaternion.Lerp(normalRotation, a_Arm.m_ShootPoint.rotation,distance);
        }else {
            //if were not hitting a aim assist, then reset the shoot point
            a_Arm.m_ShootPoint.localRotation = a_Arm.m_ShootPointStartingLocalRot;
        }
        //debug draw
        Debug.DrawRay(a_Arm.m_Model.position, Vector3.Scale(new Vector3(1, 0, 1), a_Arm.m_ShootPoint.forward) * 100, Color.red);
    }

    private void calcPlayerRotation() {
        Quaternion quatRot = Quaternion.identity;
        int added = 0;

        float currentY = transform.rotation.eulerAngles.y;

        if (m_RightArm.m_HasDir) {
            quatRot = m_RightArm.m_MovingTo.rotation;
            added++;
        }
        if (m_LeftArm.m_HasDir) {
            if (added == 0) {
                quatRot = m_LeftArm.m_MovingTo.rotation;
            } else {
                quatRot = Quaternion.Slerp(quatRot, m_LeftArm.m_MovingTo.rotation, 0.5f);

            }
            //print(Quaternion.Dot(m_RightArm.m_MovingTo.rotation, m_LeftArm.m_MovingTo.rotation));
            added++;
        }

        if (added == 0) {
            return;
        }

        float percentage = Time.unscaledDeltaTime * m_PlayerBodyRotateSpeed;
        transform.rotation = Quaternion.Slerp(transform.rotation, quatRot, percentage);
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