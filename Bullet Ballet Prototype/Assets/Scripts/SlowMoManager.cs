using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoManager : MonoBehaviour {

	float m_EnergyLeft;
	public float m_MaxEnergy = 100;
	
	public float m_EnergyDecrement;
	public float m_EnergyIncrement;

	public float m_DeltaTimeScale = 0.25f;

    [SerializeField]
    private float m_PlayerWeaponSpeedScale = 0.5f;
    /// <summary>
    /// static version of the player weapon speed
    /// will be 1 when slow mo is off
    /// and = to m_PlayerWeaponSpeed when slow mo is on
    /// </summary>
    public static float m_PlayerSpeedScale = 1.0f;

    /// <summary>
    /// bool to check if the slowmo is on or not
    /// </summary>
	private bool m_IsSlowmoOn = false;


    //debug ui elements to render what this class is doing
    public UnityEngine.UI.Text m_SlowmoText;
    public UnityEngine.UI.Slider m_SlowmoSlider;
    private string m_TextPrefix;

	// Use this for initialization
	void Awake () {
		GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);
		m_EnergyLeft = m_MaxEnergy;
		if(m_SlowmoText != null) {
			m_TextPrefix = m_SlowmoText.text;
		}
		updateUi();
	}
	
	// Update is called once per frame
	void Update () {
		controllerInput();

		if (m_IsSlowmoOn) {
			m_EnergyLeft -= m_EnergyDecrement * Time.unscaledDeltaTime;
			if(m_EnergyLeft <= 0) {
				m_IsSlowmoOn = false;
				updateTimeScale();
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

        if (controller.WasButtonPressed(Keys.singleton.m_SlowMoButton)) {
			m_IsSlowmoOn = !m_IsSlowmoOn;
			updateTimeScale();
		}
	}


	private void updateTimeScale() {
		if (m_IsSlowmoOn) {
			Time.timeScale = m_DeltaTimeScale;
            m_PlayerSpeedScale = m_PlayerWeaponSpeedScale;
		} else {
            //reset time.timeScale and m_PlayerSpeedScale back to 1
			Time.timeScale = m_PlayerSpeedScale = 1.0f;
        }
	}

	private void updateUi() {
		if (m_SlowmoText != null) {
			m_SlowmoText.text = m_TextPrefix + " " + Mathf.FloorToInt(m_EnergyLeft);
		}
        if(m_SlowmoSlider != null) {
            m_SlowmoSlider.value = m_EnergyLeft / m_MaxEnergy;
        }
	}

	private void stateChanged(GameStates a_NewState) {
		switch (a_NewState) {
			case GameStates.Action:
				enabled = true;

				break;
			case GameStates.Planning:
				enabled = false;
				m_IsSlowmoOn = false;
				break;
		}
        updateTimeScale();
	}

}
