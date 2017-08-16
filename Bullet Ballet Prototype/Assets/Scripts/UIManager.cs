using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Slider m_HealthSlider;
    public Slider m_SlowmoSlider;
    private RectTransform m_HealthSliderTransform;
    private RectTransform m_SlowmoSliderTransform;
    public RectTransform m_HealthMask;
    public RectTransform m_SlowmoMask;

    private Vector2 m_HealthStartSize;
    private Vector2 m_SlowmoStartSize;
    
	void Start () {
        m_HealthSliderTransform = m_HealthSlider.transform as RectTransform;
        m_HealthStartSize = m_HealthSliderTransform.sizeDelta;

        m_SlowmoSliderTransform = m_SlowmoSlider.transform as RectTransform;
        m_SlowmoStartSize = m_SlowmoSliderTransform.sizeDelta;

    }
	
	void Update () {
        {
            Vector3 size = m_HealthMask.sizeDelta;
            size.y = m_HealthSlider.value * m_HealthStartSize.y;
            m_HealthMask.sizeDelta = size;
        }
        {
            Vector3 size = m_SlowmoMask.sizeDelta;
            size.y = m_SlowmoSlider.value * m_SlowmoStartSize.y;
            m_SlowmoMask.sizeDelta = size;
        }
    }
}
