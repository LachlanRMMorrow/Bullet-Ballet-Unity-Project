using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoManager : MonoBehaviour {

    /** Pause Variables */

    private GameObject pauseMenu;
    private GameObject optionsMenu;
    private UnityEngine.UI.Button resume;

    public static bool m_isPaused;

    /** Energy */

    public bool m_UnlimitedSlowMo = false;
    public float m_LengthOfAISlowMoTime = 2.0f;

    private GameStates m_NextGameState = GameStates.Action;

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

    float BGMStoredPlayTime;
    SoundManager soundMan;

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
    private bool m_PlayerStartedSlowMo = true;


    [Header("UI Elements")]
    public UnityEngine.UI.Slider m_SlowmoSlider;

    private bool m_HasGameStarted = false;


    public static bool m_ManualBulletTime;

	private GameSettings m_SettingsManager;

    // Use this for initialization
    void Awake() {
        //update energy left
        m_EnergyLeft = m_MaxEnergy;

        //add state changed listerner
        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);
        soundMan = SoundManager.GetInstance();

        //get options and pause menus
        pauseMenu = GameObject.Find("Canvas").transform.Find("Pause Menu").gameObject;
        optionsMenu = GameObject.Find("Canvas").transform.Find("Options Menu").gameObject;
        resume = pauseMenu.transform.Find("Resume").GetComponent<UnityEngine.UI.Button>();


        //forcing(well giving a stern error) this object to have a slider
        if (m_SlowmoSlider == null) {
            Debug.LogError("Slow mo manager is missing it's reference to the Slow mo slider");
        }

		m_SettingsManager = GetComponent<SettingsManager>().gameSettings;

        //start off paused with a timescale of 0
        //update the fixedTimescale aswell
        //m_isPaused = true;
        //Time.timeScale = 0;
        //updateTimeScale(false);

        //update the ui
        updateUi();

        Time.fixedDeltaTime = 1;
    }

    // Update is called once per frame
    void Update() {
        //if we are not in action made return
        if(GameStateManager.currentState != GameStates.Action) {
            return;
        }

        //do controller input
        controllerInput();

        //if game is paused, return
        if (m_isPaused) {
            return;
        }

       


        if (m_IsSlowmoOn) {
            //if we used the trigger, then run this instead
            if (!m_PlayerStartedSlowMo) {

                if (Time.unscaledTime - m_TriggerStartTime > m_TriggerSlowMoLength) {
                    //time is up, lets finish it
                    m_IsSlowmoOn = false;
                    m_PlayerStartedSlowMo = true;
                    updateTimeScale(true);
                }

                return;// no need to do the rest or update the ui since the energy left wont change
            }

            //if we have unlimited then return
			if (m_UnlimitedSlowMo && !m_SettingsManager.manualBulletTime) {
                return;
            }

            //update energy normaly
            m_EnergyLeft -= m_EnergyDecrement * Time.unscaledDeltaTime;
            if (m_EnergyLeft <= 0) {
                m_EnergyLeft = 0;
                m_IsSlowmoOn = false;
                playAudio(m_SlowMoEnd);
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

        //pause menu if
        if (controller.WasButtonPressed(JInput.ControllerButtons.Start)) {
            GameObject eS = GameObject.Find("EventSystem");
            PauseMenu pm = eS.GetComponent<PauseMenu>();
            GUIManager gM = eS.GetComponent<GUIManager>();
            //TODO change check for multiple menus to be cleaner after testing
            if (optionsMenu == null || optionsMenu.activeInHierarchy == false) {

                //open screen if game is paused, close it if it's not paused
                if (pauseMenu != null) {

                    pauseMenu.SetActive(!m_isPaused);
                    gM.ScreenBlur(!m_isPaused);



                } else {
                    Debug.LogError("SlowMo Manager is missing reference in pauseMenu");
                }


                //if not paused then:
                if (!m_isPaused) {

                    //update timescale
                    Time.timeScale = 0;

                    //get pauseMenu from the EventSystem and call the PauseActive function

                    if (eS != null) {

                        if (pm != null) {
                            pm.PauseActive();
                        } else {
                            Debug.LogError("EventSystem is missing Component, PauseMenu");
                        }
                    } else {
                        Debug.LogError("There Is no Object called EventSystem in the scene");
                    }
                } else//if paused then:
                  {
                    updateTimeScale(true);
                    GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
                }



                //flip the pause
                m_isPaused = !m_isPaused;
                //update timescale, (false to tell it not to change the time scale)
                updateTimeScale(false);
            }
        }

		if (!m_ManualBulletTime  && !m_SettingsManager.manualBulletTime) {
            return;
        }

        //slow mo start if, also checks if the game is paused or not
        if (controller.WasButtonPressed(Keys.singleton.m_SlowMoButton) && !m_isPaused) {

            if (m_EnergyLeft <= 0)
            {
                GameObject.Find("MANAGER").GetComponent<ErrorSound>().PlayClip();
            }

            m_PlayerStartedSlowMo = true;//remove the trigger did use flag, if it's on then this will stop it, otherwise this wont do anything
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


    public void updateTimeScale(bool a_UpdateTimeScale) {

        if (a_UpdateTimeScale) {
            //if the next state is planning (should not be able to do anything when it's in planning mode so this should work fine)
            if (m_NextGameState == GameStates.Planning) {
                Time.timeScale = m_PlanningModeTimeScale;
            } else {
                if (m_IsSlowmoOn) {
                    Time.timeScale = m_SlowMoTimeScale;
                    m_PlayerSpeedScale = m_PlayerWeaponSpeedScale;

                } else {
                    //reset time.timeScale and m_PlayerSpeedScale back to m_NormalSpeed
                    Time.timeScale = m_PlayerSpeedScale = m_NormalSpeed;

                }
                soundMan = SoundManager.GetInstance();
                BGMStoredPlayTime = soundMan.bgmSource.time;
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
        m_LastUsedAudio = soundMan.PlayAndStoreSFX(a_Clip);
    }

    private void updateUi() {
        if (m_SlowmoSlider != null) {
            m_SlowmoSlider.value = m_EnergyLeft / m_MaxEnergy;
        }
    }

    private void stateChanged(GameStates a_NewState) {
        //prevent the timescale from being updated on the game start
        //because we want the timescale to be 0 on start
        if (!m_HasGameStarted) {
            m_HasGameStarted = true;
            return;
        }
        m_NextGameState = a_NewState;
        switch (a_NewState) {
            case GameStates.Action:
                playAudio(m_SlowMoEnd);
                break;
            case GameStates.Planning:
                playAudio(m_SlowMoStart);
                break;
        }
        updateTimeScale(true);
    }

    /// <summary>
    /// starts the slowMo for the game
    /// </summary>
    /// <param name="a_SlowMoTime">how long the slowMo will last</param>
    public void startSlowmo(float a_SlowMoTime) {
		if (m_SettingsManager.manualBulletTime) {
			return;
		}
        m_TriggerSlowMoLength = a_SlowMoTime;
        m_TriggerStartTime = Time.unscaledTime;
        m_PlayerStartedSlowMo = false;
        //start the slowmo
        m_IsSlowmoOn = true;
        updateTimeScale(true);
    }

}
