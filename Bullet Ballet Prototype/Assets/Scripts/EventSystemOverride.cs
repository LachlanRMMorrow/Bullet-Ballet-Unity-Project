using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventSystemOverride : MonoBehaviour {

    /// <summary>
    /// reference to the current controller, this is set up at the start of update
    /// Can be null
    /// </summary>
    private JInput.Controller m_Controller;
    /// <summary>
    /// reference to the currently selected selectable object, this is set up at the start of update
    /// Can be null
    /// </summary>
    private Selectable m_CurrentSelected;

    private int m_LastPovDir = 0;
    /// <summary>
    /// can we use the analog stick to move to the next element
    /// </summary>
    private bool m_CanSickMove = true;

    /// <summary>
    /// when moving around using the left stick, this is how much it needs to move by before we take input from it
    /// </summary>
    [Range(0, 1)]
    public float m_AmountNeededForStickMove = 0.9f;

    /// <summary>
    /// directions for navigation around the UI
    /// </summary>
    private enum NavigationDirection {
        Left,
        Right,
        Up,
        Down
    }


    // Update is called once per frame
    void Update() {
        //check to see if there is something selected
        GameObject currentSelectedObject = EventSystem.current.currentSelectedGameObject;
        if (currentSelectedObject == null) {
            return;
        }
        //get it's selectable object
        m_CurrentSelected = currentSelectedObject.GetComponent<Selectable>();

        //get the current controller
        m_Controller = JInput.CurrentController.currentController;

        //run navigation between ui
        navigation();

        //run selecting elements and going back 
        interact();
    }

    /// <summary>
    /// will select and go back to the previous canvas(if there is a UICloseMenu on the object or its parent)
    /// </summary>
    private void interact() {
        //check keyboard input and controller input if it's plugged in for select buttons
        bool shouldSelect = Input.GetKeyDown(KeyCode.Return);
        if (m_Controller != null) {
            shouldSelect |= m_Controller.WasButtonPressed(JInput.ControllerButtons.Cross);
        }

        //was a select button pressed
        if (shouldSelect) {
            //run drop down,toggle and button functions
            if (dropdownSelect()) {
                return;
            }
            if (toggleSelect()) {
                return;
            }
            buttonSelect();
            return;
        }

        //check keyboard input and controller input if it's plugged in for back/close buttons
        bool shouldGoBack = Input.GetKeyDown(KeyCode.Escape);
        if (m_Controller != null) {
            shouldGoBack |= m_Controller.WasButtonPressed(JInput.ControllerButtons.Circle);
        }

        if (shouldGoBack) {
            //run the toggle-dropdown go back and the close ui go back
            if (toggleGoBack()) {
                return;
            }
            everythingGoBack();
            return;
        }
    }

    /// <summary>
    /// selecting the drop down, will show it
    /// </summary>
    /// <returns>false if object is not a drop down</returns>
    private bool dropdownSelect() {
        Dropdown item = m_CurrentSelected.GetComponent<Dropdown>();
        if (item == null) {
            return false;
        }
        item.Show();
        return true;

    }


    /// <summary>
    /// selecting the toggle, will flip the check box
    /// </summary>
    /// <returns>false if object is not a toggle</returns>
    private bool toggleSelect() {
        Toggle item = m_CurrentSelected.GetComponent<Toggle>();
        if (item == null) {
            return false;
        }
        item.isOn = !item.isOn;

        return true;
    }

    /// <summary>
    /// selecting the button, will run it's onclick event system
    /// </summary>
    /// <returns>false if object is not a button</returns>
    private bool buttonSelect() {
        Button item = m_CurrentSelected.GetComponent<Button>();
        if (item == null) {
            return false;
        }
        item.onClick.Invoke();
        return true;

    }

    /// <summary>
    /// going back on a toggle will check to see if it's part of a drop down
    /// if it is part of a dropdown then it will close the drop down
    /// </summary>
    /// <returns>false if object is not a toggle</returns>
    private bool toggleGoBack() {
        Toggle item = m_CurrentSelected.GetComponent<Toggle>();
        if (item == null) {
            return false;
        }

        //need a better way to check if it's part of a drop down
        if (m_CurrentSelected.transform.parent.name == "Content") {
            //need a better way to get the drop down script
            Dropdown t = m_CurrentSelected.transform.parent.parent.parent.parent.GetComponent<Dropdown>();
            t.Hide();
            return true;
        }

        return false;
    }

    /// <summary>
    /// looks for the UICloseMenu script on this object or it's parent
    /// will run the UICloseMenu scripts referenced button
    /// does not check if the button is a null reference
    /// </summary>
    /// <returns>false if we cant find a UICloseMenu script</returns>
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

    /// <summary>
    /// check for arrow keys, controller hat switch and controller left analog stick input
    /// </summary>
    private void navigation() {
        //defaults for hat and stick values
        //if these stay as the default then nothing should happen
        JInput.ControllerHatDir hatDir = JInput.ControllerHatDir.None;
        Vector2 stickMovement = Vector2.zero;

        //if we have a controller then get values from it
        if (m_Controller != null) {
            //get hat value
            int currentPovDir = m_Controller.getPovDirection();
            //remove hat value if it's the same as last frame
            //to prevent double input
            if (currentPovDir != m_LastPovDir) {
                m_LastPovDir = currentPovDir;
                hatDir = m_Controller.getHatDirection();
            }

            //get stick values
            stickMovement = m_Controller.getAxisValue(JInput.ControllerVec2Axes.LStick);

            //remove switch value if it's not all the way or if we have just used it
            if (!m_CanSickMove) {
                //if the stick cant move and has been returned back to the starting position then allow the stick to be moved again
                if (stickMovement.magnitude <= 0.3f) {
                    m_CanSickMove = true;
                }
                //remove the stick movement to prevent double inputs
                stickMovement = Vector2.zero;
            } else {
                //we can move the stick to navigate the ui
                //but check if it's been moved past a amount we want to get input from
                if (stickMovement.magnitude >= m_AmountNeededForStickMove) {
                    //double it just to be sure it's over m_AmountNeededForStickMove
                    //it could be under due to one of the other axis
                    stickMovement *= 2;
                    m_CanSickMove = false;
                } else {
                    //else if we dont want to get input from the stick, just remove the value
                    stickMovement = Vector2.zero;
                }
            }
        }

        //get slider for slider value movement
        Slider slider = m_CurrentSelected.GetComponent<Slider>();
        //todo, change this to be calculated based on min/max values
        float sliderValueIncrement = 0.1f;

        //up
        if (hatDir == JInput.ControllerHatDir.Up || Input.GetKeyDown(KeyCode.UpArrow) || stickMovement.y < -m_AmountNeededForStickMove) {
            setCurrentSelected(NavigationDirection.Up);
            return;
        }

        //down
        if (hatDir == JInput.ControllerHatDir.Down || Input.GetKeyDown(KeyCode.DownArrow) || stickMovement.y > m_AmountNeededForStickMove) {

            setCurrentSelected(NavigationDirection.Down);
            return;
        }

        //right
        if (hatDir == JInput.ControllerHatDir.Right || Input.GetKeyDown(KeyCode.RightArrow) || stickMovement.x > m_AmountNeededForStickMove) {
            //if we have a slider
            if (slider != null) {
                //it's in the left-right direction
                if (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft) {
                    //if the value is already at max
                    if (slider.value == slider.maxValue) {
                        //then move to the next element
                        setCurrentSelected(NavigationDirection.Right);
                    } else {
                        //else add to the value
                        slider.value += sliderValueIncrement;
                    }
                    return;
                }
            }
            //if were not on a slider or in the wrong direction
            setCurrentSelected(NavigationDirection.Right);

        }

        //left
        if (hatDir == JInput.ControllerHatDir.Left || Input.GetKeyDown(KeyCode.LeftArrow) || stickMovement.x < -m_AmountNeededForStickMove) {
            //if we have a slider
            if (slider != null) {
                //it's in the left-right direction
                if (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft) {
                    //if the value is already at max
                    if (slider.value == slider.minValue) {
                        //then move to the next element
                        setCurrentSelected(NavigationDirection.Left);
                    } else {
                        //else add to the value
                        slider.value -= sliderValueIncrement;
                    }
                    return;
                }
            }
            //if were not on a slider or the slider is in the wrong direction
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
        //get the next object based on a_Dir
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
        //and select it if it's not null
        if (newObject != null) {
            newObject.Select();
        }
    }

}
