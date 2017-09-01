﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoManager : MonoBehaviour {

    /** Pause Variables */

    public GameObject pauseMenu;
    public GameObject resume;
    public GameObject player;

    public static bool m_isPaused;

    /** Energy */

    [Header("Energy data")]
    public float m_MaxEnergy = 100;
    float m_EnergyLeft;

    public float m_EnergyDecrement;
    public float m_EnergyIncrement;

    /** Time Scales */

    [Header("Time Speed Scales")]
    [Range(0, 2)]
    public float m_NormalSpeed = 1.0f;
    [Range(0, 2)]
    public float m_FixedUpdateScale = 0.02f;
    [Range(0, 2)]
    public float m_PlanningModeTimeScale = 0.1f;
    [Range(0, 2)]
    public float m_SlowMoTimeScale = 0.25f;

    [SerializeField]
    [Range(0, 2)]
    private float m_PlayerWeaponSpeedScale = 0.5f;
    /// <summary>
    /// static version of the player weapon speed
    /// will be 1 when slow mo is off
    /// and = to m_PlayerWeaponSpeed when slow mo is on
    /// </summary>
    public static float m_PlayerSpeedScale = 1.0f;

    /** Audio */
    [Header("Audio")]
    public AudioClip m_SlowMoStart;
    public AudioClip m_SlowMoEnd;

    private AudioSource m_LastUsedAudio;


    /** Slow-mo flags */

    /// <summary>
    /// bool to check if the slowmo is on or not
    /// </summary>
    private bool m_IsSlowmoOn = false;

    /** Trigger slowMo */

    //private variables which are set when the slowmo trigger is started
    /// <summary>
    /// how long the slowmo should last if we used a trigger to start it
    /// </summary>
    private float m_TriggerSlowMoLength;
    private float m_TriggerStartTime;
    /// <summary>
    /// did we use the trigger to start the current slowmo?
    /// </summary>
    private bool m_TriggerDidUse = false;

    [Header("UI Elements")]
    public UnityEngine.UI.Slider m_SlowmoSlider;

    // Use this for initialization
    void Awake() {
        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);
        m_EnergyLeft = m_MaxEnergy;

        updateUi();
    }

    // Update is called once per frame
    void Update() {
        controllerInput();


        if (m_IsSlowmoOn) {

            //if we used the trigger, then run this instead
            if (m_TriggerDidUse) {

                if (Time.unscaledTime - m_TriggerStartTime > m_TriggerSlowMoLength) {
                    //time is up, lets finish it
                    m_IsSlowmoOn = false;
                    m_TriggerDidUse = false;
                    updateTimeScale(true);
                }

                return;// no need to do the rest or update the ui since the energy left wont change
            }

            m_EnergyLeft -= m_EnergyDecrement * Time.unscaledDeltaTime;
            if (m_EnergyLeft <= 0) {
                m_IsSlowmoOn = false;
                updateTimeScale(true);
            }
        } else {
            m_EnergyLeft += m_EnergyIncrement * Time.unscaledDeltaTime;
            if (m_EnergyLeft >= m_MaxEnergy) {
                m_EnergyLeft = m_MaxEnergy;
            }
        }


        updateUi();
    }

    /// <summary>
    /// check controller input to see if the slow mo button was pressed
    /// </summary>
	private void controllerInput() {
        //controller, turn on/off
        JInput.Controller controller = JInput.CurrentController.currentController;
        if (controller == null) {
            return;
        }

        if (controller.WasButtonPressed(JInput.ControllerButtons.Start))
        {
            if (pauseMenu.activeInHierarchy == false)
            {
                //GameObject player = GameObject.Find("Player");
                //player.GetComponent<PlayerMovement>().enabled = false;
                //player.GetComponent<PlayerArms>().enabled = false;


                updateTimeScale(false);
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                GameObject eS = GameObject.Find("EventSystem");
                eS.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(resume);
                eS.GetComponent<PauseMenu>().PauseActive();
            }
            else
            {
                //GameObject player = GameObject.Find("Player");
                //player.GetComponent<PlayerMovement>().enabled = true;
                //player.GetComponent<PlayerArms>().enabled = true;

                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
            m_isPaused = pauseMenu.activeInHierarchy;

        }



        if (controller.WasButtonPressed(Keys.singleton.m_SlowMoButton) && pauseMenu.activeInHierarchy == false) {
            m_TriggerDidUse = false;//remove the trigger did use flag, if it's on then this will stop it, otherwise this wont do anything
            m_IsSlowmoOn = !m_IsSlowmoOn;
            updateTimeScale(true);

            //play the audio
            if (m_IsSlowmoOn) {
                playAudio(m_SlowMoStart);
            } else {
                playAudio(m_SlowMoEnd);
            }
        }
    }


    private void updateTimeScale(bool a_UpdateTimeScale) {

        if (a_UpdateTimeScale) {
            if (m_IsSlowmoOn) {

                Time.timeScale = m_SlowMoTimeScale;
                m_PlayerSpeedScale = m_PlayerWeaponSpeedScale;

            } else {
                //reset time.timeScale and m_PlayerSpeedScale back to m_NormalSpeed
                Time.timeScale = m_PlayerSpeedScale = m_NormalSpeed;

            }
        }
        //update fixed delta time
        Time.fixedDeltaTime = m_FixedUpdateScale * Time.timeScale;
    }

    private void playAudio(AudioClip a_Clip) {
        //if the last audio that was played is still being played, then remove it... (well just make it's volume 0)
        if (m_LastUsedAudio != null) {
            m_LastUsedAudio.volume = 0;
            m_LastUsedAudio.Pause();
        }
        //then start the new sound
        SoundManager soundMan = SoundManager.GetInstance();
        m_LastUsedAudio = soundMan.PlayAndStoreSFX(a_Clip);
    }

    private void updateUi() {
        if (m_SlowmoSlider != null) {
            m_SlowmoSlider.value = m_EnergyLeft / m_MaxEnergy;
        }
    }

    private void stateChanged(GameStates a_NewState) {
        switch (a_NewState) {
            case GameStates.Action:
                enabled = true;

                Time.timeScale = m_NormalSpeed;
                playAudio(m_SlowMoEnd);
                break;
            case GameStates.Planning:
                enabled = false;
                m_IsSlowmoOn = false;

                Time.timeScale = m_PlanningModeTimeScale;
                playAudio(m_SlowMoStart);
                break;
        }
        updateTimeScale(false);
    }

    public void startTriggerSlowmo(float a_SlowMoTime) {
        m_TriggerSlowMoLength = a_SlowMoTime;
        m_TriggerStartTime = Time.unscaledTime;
        m_TriggerDidUse = true;
        //start the slowmo
        m_IsSlowmoOn = true;
        updateTimeScale(true);
    }

}
