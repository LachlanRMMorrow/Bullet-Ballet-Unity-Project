using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventSystemOverride : MonoBehaviour {

    private JInput.Controller m_Controller;
    private Selectable m_CurrentSelected;

    private int m_LastPovDir = 0;

    private enum NavigationDirection {
        Left,
        Right,
        Up,
        Down
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        GameObject currentSelectedObject = EventSystem.current.currentSelectedGameObject;
        if (currentSelectedObject == null) {
            return;
        }
        m_CurrentSelected = currentSelectedObject.GetComponent<Selectable>();

        m_Controller = JInput.CurrentController.currentController;

        navigation();

        select();

        //if (Input.GetKeyDown(KeyCode.P)) {
        //    Selectable sdown = s.FindSelectableOnDown();
        //    print(sdown.gameObject.name);
        //}
    }

    private void select() {
        bool shouldSelect = Input.GetKeyDown(KeyCode.Return);
        if(m_Controller != null) {
            shouldSelect |= m_Controller.WasButtonPressed(JInput.ControllerButtons.Cross);
        }

        if (shouldSelect) {
            if (dropdownSelect()) {
                return;
            }
            if (toggleSelect()) {
                return;
            }
            buttonSelect();
            return;
        }

        bool shouldGoBack = Input.GetKeyDown(KeyCode.Escape);
        if (m_Controller != null) {
            shouldGoBack |= m_Controller.WasButtonPressed(JInput.ControllerButtons.Circle);
        }

        if (shouldGoBack) {
            if (toggleGoBack()) {
                return;
            }
            everythingGoBack();
            return;
        }
    }

    private bool dropdownSelect() {
        Dropdown item = m_CurrentSelected.GetComponent<Dropdown>();
        if (item == null) {
            return false;
        }
        print("Drop down Select");
        item.Show();
        return true;

    }


    private bool toggleSelect() {
        Toggle item = m_CurrentSelected.GetComponent<Toggle>();
        if (item == null) {
            return false;
        }
        print("toggle Select");
        item.isOn = !item.isOn;



        return true;
    }

    private bool buttonSelect() {
        Button item = m_CurrentSelected.GetComponent<Button>();
        if (item == null) {
            return false;
        }
        print("button Select");
        item.onClick.Invoke();
        return true;

    }

    private bool toggleGoBack() {
        Toggle item = m_CurrentSelected.GetComponent<Toggle>();
        if (item == null) {
            return false;
        }

       if(m_CurrentSelected.transform.parent.name == "Content") {
           Dropdown t = m_CurrentSelected.transform.parent.parent.parent.parent.GetComponent<Dropdown>();
           t.Hide();
       }

        return true;
    }

    private bool everythingGoBack() {
        UICloseMenu item = m_CurrentSelected.GetComponent<UICloseMenu>();
        if (item == null) {
            item = m_CurrentSelected.transform.parent.GetComponent<UICloseMenu>();
            if (item == null) {
                return false;
            }
        }

        item.m_CloseButton.onClick.Invoke();

        return true;
    }

    private void navigation() {
        JInput.ControllerHatDir hatDir = JInput.ControllerHatDir.None;
        if (m_Controller != null) {
            int currentPovDir = m_Controller.getPovDirection();

            if (currentPovDir != m_LastPovDir) {
                m_LastPovDir = currentPovDir;
                hatDir = m_Controller.getHatDirection();
            }
        }

        Slider slider = m_CurrentSelected.GetComponent<Slider>();
        //todo, change this to be calculated based on min/max values
        float sliderValueIncrement = 0.1f;

        if (hatDir == JInput.ControllerHatDir.Up || Input.GetKeyDown(KeyCode.UpArrow)) {
            setCurrentSelected(NavigationDirection.Up);
            return;
        }

        if (hatDir == JInput.ControllerHatDir.Down || Input.GetKeyDown(KeyCode.DownArrow)) {

            setCurrentSelected(NavigationDirection.Down);
            return;
        }
        if (hatDir == JInput.ControllerHatDir.Right || Input.GetKeyDown(KeyCode.RightArrow)) {
            if (slider != null) {
                if (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft) {
                    if(slider.value == slider.maxValue) {
                        setCurrentSelected(NavigationDirection.Right);
                    }else {
                        slider.value += sliderValueIncrement;
                    }
                    return;
                }
            }
            setCurrentSelected(NavigationDirection.Right);
            
        }

        if (hatDir == JInput.ControllerHatDir.Left || Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (slider != null) {
                if (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft) {
                    if (slider.value == slider.minValue) {
                        setCurrentSelected(NavigationDirection.Left);
                    } else {
                        slider.value -= sliderValueIncrement;
                    }
                    return;
                }
            }
            setCurrentSelected(NavigationDirection.Left);
            return;
        }
    }

    /// <summary>
    /// maybe switch this to use a direction enum?
    /// </summary>
    /// <param name="a_NewObject"></param>
    private void setCurrentSelected(NavigationDirection a_Dir) {
        Selectable newObject = null;
        switch (a_Dir) {
            case NavigationDirection.Left:
                newObject = m_CurrentSelected.FindSelectableOnLeft();
                break;
            case NavigationDirection.Right:
                newObject = m_CurrentSelected.FindSelectableOnRight();
                break;
            case NavigationDirection.Up:
                newObject = m_CurrentSelected.FindSelectableOnUp();
                break;
            case NavigationDirection.Down:
                newObject = m_CurrentSelected.FindSelectableOnDown();
                break;
        }
        if (newObject != null) {
            newObject.Select();
        }
    }

}
