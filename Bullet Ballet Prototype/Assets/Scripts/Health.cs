using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    /// <summary>
    /// max health of this object
    /// </summary>
    public float m_MaxHealth;
    /// <summary>
    /// current health of this object
    /// set to max health on start
    /// </summary>
    private float m_CurrentHealth;

    /// <summary>
    /// slider for this health object to display health at
    /// </summary>
    public UnityEngine.UI.Slider m_SliderReference;

    [HideInInspector]
    /// <summary>
    /// event to invoke when this object runs out of health
    /// </summary>
    public UnityEngine.Events.UnityEvent m_ObjectDiedEvent;

    public AudioClip m_DamageSound;

    void Awake() {
        //set up default health values
        m_CurrentHealth = m_MaxHealth;


        //forcing(well giving a stern error) the player to have a slider
        if (m_SliderReference == null) {
            if (GetComponent<PlayerMovement>() != null) {//if this object has the PlayerMovement script (this means it's the players)
                Debug.LogError("Player is missing it's reference to the health slider");
            }
            //else, this is fine.. as this object would be something that should be updating it's health to a slider
        }

        //update the slider
        updateSlider();
    }

    /// <summary>
    /// deals damage to this object, will not deal damage
    /// </summary>
    /// <param name="a_Damage"></param>
    public void dealDamage(float a_Damage) {
        //if we have already lost all our health, then
        if (isDead()) {
            return;
        }
        //remove health
        m_CurrentHealth -= a_Damage;
        //check if this object is dead or not after the damage
        if (isDead()) {
            m_CurrentHealth = 0;
            m_ObjectDiedEvent.Invoke();
        }
        //update the ui slider
        updateSlider();
        //play sound
        SoundManager.PlaySFXRandomized(m_DamageSound);
    }

    /// <summary>
    /// the current health
    /// </summary>
    /// <returns>current health</returns>
    public float getHealthLeft() {
        return m_CurrentHealth;
    }

    /// <summary>
    /// quick way to check if this object has lost all it's health
    /// </summary>
    /// <returns>true if health is below 0</returns>
    public bool isDead() {
        return m_CurrentHealth <= 0;
    }

    private void updateSlider() {
        //if there is a slider attached
        if (m_SliderReference != null) {
            m_SliderReference.value = m_CurrentHealth / m_MaxHealth;
        }
    }

}
