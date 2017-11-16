using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [System.Serializable]
    public struct SliderData {
        public Slider m_Slider;
        internal RectTransform m_SliderTransform;
        public RectTransform m_SliderMask;
        internal Vector2 m_StartSize;
    }

    public SliderData[] m_SliderMasks;

    void Start() {
        for(int i = 0; i < m_SliderMasks.Length; i++) {
            m_SliderMasks[i].m_SliderTransform = m_SliderMasks[i].m_Slider.transform as RectTransform;
            m_SliderMasks[i].m_StartSize = m_SliderMasks[i].m_SliderTransform.sizeDelta;
        }
    }

    void Update() {
        for (int i = 0; i < m_SliderMasks.Length; i++) {
            Vector3 size = m_SliderMasks[i].m_SliderMask.sizeDelta;
            size.y = m_SliderMasks[i].m_Slider.value * m_SliderMasks[i].m_StartSize.y;
            m_SliderMasks[i].m_SliderMask.sizeDelta = size;
        }
    }
}
