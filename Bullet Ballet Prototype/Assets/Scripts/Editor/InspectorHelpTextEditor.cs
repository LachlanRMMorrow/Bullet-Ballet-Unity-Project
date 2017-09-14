using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(InspectorHelpText))]
public class InspectorHelpTextEditor : Editor {

    public override void OnInspectorGUI() {
        InspectorHelpText myTarget = (InspectorHelpText)target;

        EditorGUILayout.HelpBox(myTarget.m_HelpText,MessageType.Info);
    }

}
