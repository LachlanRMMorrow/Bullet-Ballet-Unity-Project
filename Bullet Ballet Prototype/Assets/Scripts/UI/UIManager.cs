using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    //todo: make this into a struct and use an array
    public Slider m_HealthSlider;
    public Slider m_SlowmoSlider;
    public Slider m_DashSlider;
    private RectTransform m_HealthSliderTransform;
    private RectTransform m_SlowmoSliderTransform;
    private RectTransform m_DashSliderTransform;
    public RectTransform m_HealthMask;
    public RectTransform m_SlowmoMask;
    public RectTransform m_DashMask;

    private Vector2 m_HealthStartSize;
    private Vector2 m_SlowmoStartSize;
    private Vector2 m_DashStartSize;

    void Start () {
        m_HealthSliderTransform = m_HealthSlider.transform as RectTransform;
        m_HealthStartSize = m_HealthSliderTransform.sizeDelta;

        m_SlowmoSliderTransform = m_SlowmoSlider.transform as RectTransform;
        m_SlowmoStartSize = m_SlowmoSliderTransform.sizeDelta;

        m_DashSliderTransform = m_DashSlider.transform as RectTransform;
        m_DashStartSize = m_DashSliderTransform.sizeDelta;

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
        {
            Vector3 size = m_DashMask.sizeDelta;
            size.y = m_DashSlider.value * m_DashStartSize.y;
            m_DashMask.sizeDelta = size;
        }
    }
}
