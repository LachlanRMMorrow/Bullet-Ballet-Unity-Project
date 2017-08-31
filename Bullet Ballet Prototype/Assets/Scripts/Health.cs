using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public AudioClip m_DamageSound;
    public GameObject m_deathScreen;

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

    void Awake() {
        m_CurrentHealth = m_MaxHealth;
        updateSlider();
    }

    public void dealDamage(float a_Damage) {
        m_CurrentHealth -= a_Damage;
        SoundManager.PlaySFXRandomized(m_DamageSound);
        if (m_CurrentHealth <= 0) {
            noHealth();
        }
        updateSlider();
    }

    public float getHealthLeft() {
        return m_CurrentHealth;
    }

    public bool isDead() {
        return m_CurrentHealth <= 0;
    }

    private void noHealth() {
        bool isPlayer = CompareTag("Player");
        //death if is player tag
        if (isPlayer)
        {
            death();
        }
        //destroy if is Player or is Enemies tag
        if (isPlayer || CompareTag("Enemy")) {
            Destroy(gameObject);//TEMP DESTROY
        }
    }

    private void updateSlider() {
        //if there is a slider attached
        if (m_SliderReference != null) {
            m_SliderReference.value = m_CurrentHealth / m_MaxHealth;
        }
    }

    private void death()
    {
        m_deathScreen.SetActive(true);
        Time.timeScale = 0;

    }

}
