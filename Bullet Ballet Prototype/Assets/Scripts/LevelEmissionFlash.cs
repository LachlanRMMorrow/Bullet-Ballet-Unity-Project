using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEmissionFlash : MonoBehaviour {

    public Material[] m_Materials;

    public Color m_NormalColor;
    /// <summary>
    /// color to lerp between, this will be either EnemyHitFlashColor and PlayerHitFlashColor
    /// </summary>
    private Color m_FlashColor;


    public Color m_EnemyHitFlashColor;
    public Color m_PlayerHitFlashColor;

    public AnimationCurve m_ColorCurve;
    public float m_FlashLength = 1;

    private float m_FlashStartTime = 0;
    private bool m_IsFlashing = false;

    public static LevelEmissionFlash m_Singleton;

    public void Start() {
        applyColorToAllMaterials(m_NormalColor);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.RightShift)) {
            startFlash(true);
        }
        if (m_IsFlashing) {

            float percentage = (Time.time - m_FlashStartTime) / m_FlashLength;

            if(percentage >= 1) {
                m_IsFlashing = false;
                percentage = 1;
            }

            float curveEvaluation = m_ColorCurve.Evaluate(percentage);

            Color currentColor = Color.Lerp(m_NormalColor, m_FlashColor, curveEvaluation);

            applyColorToAllMaterials(currentColor);

        }
    }

    public void startFlash(bool a_IsPlayer) {
        m_FlashStartTime = Time.time;
        m_IsFlashing = true;
        //set flash color
        m_FlashColor = a_IsPlayer ? m_PlayerHitFlashColor : m_EnemyHitFlashColor;
    }

    private void applyColorToAllMaterials(Color a_Color) {
        for(int i = 0; i < m_Materials.Length; i++) {
            if(m_Materials[i] == null) {
                continue;
            }
            m_Materials[i].SetColor("_EmissionColor", a_Color);
        }
    }

    public void OnEnable() {
        m_Singleton = this;
    }
}
