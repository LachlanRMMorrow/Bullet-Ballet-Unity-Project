using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates {
    Action,
    Planning,
    Paused
}

/// <summary>
/// A Unity Event that takes adds a paramater of GameStates when called
/// </summary>
public class EventGameState : UnityEngine.Events.UnityEvent<GameStates> { }

public class GameStateManager : MonoBehaviour {



    /// <summary>
    /// singleton for the gamestateManager
    /// </summary>
    private static GameStateManager m_Singleton;
    /// <summary>
    /// getter for the singleton to not allow it to be changed
    /// </summary>
    public static GameStateManager singleton { get { return m_Singleton; } }

    /// <summary>
    /// current state of the game
    /// </summary>
    [SerializeField]
    private GameStates m_CurrentState;

    /// <summary>
    /// getter for the current State of the game
    /// </summary>
    public static GameStates currentState { get { return m_Singleton.m_CurrentState; } }

    /// <summary>
    /// reference to a controller object
    /// </summary>
    private JInput.Controller m_Controller;

    /// <summary>
    /// Unity Event for adding callbacks
    /// called when there is a state change using setCurrentState and on game start
    /// </summary>
    [HideInInspector]
    public EventGameState m_StateChanged = new EventGameState();

    public UnityEngine.UI.Image m_StateModeDebugText;
    public Sprite m_StateModeDebugText1;
    public Sprite m_StateModeDebugText2;

    /// <summary>
    /// changes the game state to a_State
    /// and invokes the unity Event m_StateChanged with the new state
    /// </summary>
    /// <param name="a_State">new game state</param>
    public static void setCurrentState(GameStates a_State) {
        if (a_State != m_Singleton.m_CurrentState) {
            m_Singleton.m_StateChanged.Invoke(a_State);
        }
        m_Singleton.m_CurrentState = a_State;
    }
    /// <summary>
    /// sets up singleton on game start
    /// </summary>
    void Awake() {
        m_Singleton = this;
    }
    /// <summary>
    /// if code is changed and unity is playing
    /// it gets rid of the singleton reference, so this links it back up
    /// can be removed for final code
    /// </summary>
    void OnEnable() {
        if (m_Singleton == null) {
            Debug.LogError("Game was reset by code change? Most logic wont work");
        }
        m_Singleton = this;
    }

    /// <summary>
    /// set up event listeners for controlling time and gamestates
    /// also invoke the unityEvent to allow classes to do their setup
    /// </summary>
    void Start() {
        m_Singleton.m_StateChanged.AddListener(updateTimeScale);
        m_Singleton.m_StateChanged.AddListener(updateScreenText);
        m_Singleton.m_StateChanged.Invoke(currentState);
    }

    void Update() {
        m_Controller = JInput.CurrentController.currentController;
        if (m_Controller == null) {
            return;
        }

        ////swap planning mode from button press
        //if (m_Controller.WasButtonPressed(Keys.singleton.m_PlanningModeSwap)) {
        //    switch (currentState) {
        //        case GameStates.Action:
        //            setCurrentState(GameStates.Planning);
        //            break;
        //        case GameStates.Planning:
        //            setCurrentState(GameStates.Action);
        //            break;
        //    }
        //
        //}

        if (currentState == GameStates.Action) {
            if (m_Controller.WasButtonPressed(Keys.singleton.m_PlanningModeSwap)) {
                setCurrentState(GameStates.Planning);
            }
        } else {
            if (m_Controller.WasButtonReleased(Keys.singleton.m_PlanningModeSwap)) {
                setCurrentState(GameStates.Action);
            }
        }

    }

    /// <summary>
    /// function to set up the timescale between modes
    /// </summary>
    /// <param name="a_State">the next state</param>
    private void updateTimeScale(GameStates a_State) {
        if (a_State == GameStates.Action) {
            Time.timeScale = 1.0f;
        } else {
            Time.timeScale = 0.1f;
            //Time.timeScale = 1.0f;
        }
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    private void updateScreenText(GameStates a_State) {
        if (m_StateModeDebugText == null) {
            return;
        }

        switch (a_State) {
            case GameStates.Action:
                m_StateModeDebugText.sprite = m_StateModeDebugText1;
                break;
            case GameStates.Planning:
                m_StateModeDebugText.sprite = m_StateModeDebugText2;
                break;
        }
    }
}
