using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(SkillManager))]
public class SkillManagerEditor : Editor {

    private SkillManager myTarget;
    private SerializedObject soTarget;

    //Prefab
    private SerializedProperty prefabSkill;

    //Trigger
    private SerializedProperty triggerOption;

    private SerializedProperty input;
    private SerializedProperty minTime;
    private SerializedProperty maxTime;
    private SerializedProperty timeBetween;
    private SerializedProperty cooldownTime;

    //Position
    private SerializedProperty startPositionOption;
    private SerializedProperty endPositionOption;

    private SerializedProperty skillPositionVector;
    private SerializedProperty skillPositionObject;
    private SerializedProperty skillPositionDirection;
    private SerializedProperty skillPositionDistance;

    //Target
    private SerializedProperty skillTargetVector;
    private SerializedProperty skillTargetObject;
    private SerializedProperty skillTargetDirection;
    private SerializedProperty skillTargetDistance;

    //Effect
    private SerializedProperty destroyOnEndPosition;

    private SerializedProperty effectTargetGameObject;
    private SerializedProperty effectTargetName;
    private SerializedProperty effectTargetTag;
    private SerializedProperty effectTargetScriptName;

    private SerializedProperty skillSpeed;

    //Script
    private SerializedProperty selectedScript;

    private SerializedProperty triggerEvent;

    private void OnEnable() {
        myTarget = (SkillManager)target;
        soTarget = new SerializedObject(target);

        //Prefab
        prefabSkill = soTarget.FindProperty("prefabSkill");

        //Trigger
        triggerOption = soTarget.FindProperty("triggerOption");

        input = soTarget.FindProperty("input");
        cooldownTime = soTarget.FindProperty("cooldownTime");
        minTime = soTarget.FindProperty("minTime");
        maxTime = soTarget.FindProperty("maxTime");
        timeBetween = soTarget.FindProperty("timeBetween");

        //Position
        skillPositionVector = soTarget.FindProperty("skillPositionVector");
        skillPositionObject = soTarget.FindProperty("skillPositionObject");
        skillPositionDirection = soTarget.FindProperty("skillPositionDirection");
        skillPositionDistance = soTarget.FindProperty("skillPositionDistance");

        //Target
        skillTargetVector = soTarget.FindProperty("skillTargetVector");
        skillTargetObject = soTarget.FindProperty("skillTargetObject");
        skillTargetDirection = soTarget.FindProperty("skillTargetDirection");
        skillTargetDistance = soTarget.FindProperty("skillTargetDistance");

        //Effect
        destroyOnEndPosition = soTarget.FindProperty("destroyOnEndPosition");

        effectTargetGameObject = soTarget.FindProperty("effectTargetGameObject");
        effectTargetName = soTarget.FindProperty("effectTargetName");
        effectTargetTag = soTarget.FindProperty("effectTargetTag");
        effectTargetScriptName = soTarget.FindProperty("effectTargetScriptName");

        //Script
        selectedScript = soTarget.FindProperty("selectedScript");

        //Speed
        skillSpeed = soTarget.FindProperty("skillSpeed");

        triggerEvent = soTarget.FindProperty("triggerEvent");
    }

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        soTarget.Update();
        EditorGUI.BeginChangeCheck();

        myTarget.currentTab = GUILayout.Toolbar(myTarget.currentTab, new string[] { "Trigger", "Position", "Prefab", "Effect" });

        switch(myTarget.currentTab) {
            case 0:
                EditorGUILayout.PropertyField(triggerOption);
                switch(myTarget.triggerOption) {
                    case TriggerOptions.PlayerInput:
                        EditorGUILayout.LabelField("Trigger by player input");
                        EditorGUILayout.PropertyField(input);
                        EditorGUILayout.PropertyField(cooldownTime);
                        break;
                    case TriggerOptions.Random:
                        EditorGUILayout.LabelField("Trigger at random intervals");
                        EditorGUILayout.PropertyField(minTime);
                        EditorGUILayout.PropertyField(maxTime);
                        break;
                    case TriggerOptions.Continuous:
                        EditorGUILayout.LabelField("Trigger at specific intervals");
                        EditorGUILayout.PropertyField(timeBetween);
                        break;
                }
                break;
            case 1:
                myTarget.positionChoice = GUILayout.Toolbar(myTarget.positionChoice, new string[] { "Constant position", "Move position" });

                EditorGUILayout.LabelField("Start position");
                ShowTargetOptions(ref myTarget.startPositionOption, ref myTarget.positionChoice1Direction, ref skillPositionVector, ref skillPositionObject, ref skillPositionDistance);

                switch (myTarget.positionChoice) {
                    case 0:
                        
                        break;
                    case 1:
                        EditorGUILayout.LabelField("End position");
                        ShowTargetOptions(ref myTarget.endPositionOption, ref myTarget.targetChoice1Direction, ref skillTargetVector, ref skillTargetObject, ref skillTargetDistance);
                        EditorGUILayout.LabelField("Speed");
                        EditorGUILayout.PropertyField(skillSpeed);
                        break;
                }
                break;
            case 2:
                EditorGUILayout.PropertyField(prefabSkill);
                break;
            case 3:
                EditorGUILayout.PropertyField(destroyOnEndPosition);
                myTarget.targetEffect = (TargetType)EditorGUILayout.EnumPopup("Target type: ", myTarget.targetEffect);
                switch(myTarget.targetEffect) {
                    case TargetType.GameObject:
                        EditorGUILayout.PropertyField(effectTargetGameObject);
                        break;
                    case TargetType.Name:
                        EditorGUILayout.PropertyField(effectTargetName);
                        break;
                    case TargetType.Tag:
                        EditorGUILayout.PropertyField(effectTargetTag);
                        break;
                    case TargetType.Script:
                        EditorGUILayout.PropertyField(effectTargetScriptName);
                        break;
                }

                myTarget.effectOptionsChoice = GUILayout.Toolbar(myTarget.effectOptionsChoice, new string[] { "Change variable(s) in script", "Call method", "Destroy object" });
                switch (myTarget.effectOptionsChoice) {
                    case 0:
                        EditorGUILayout.PropertyField(triggerEvent);
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(selectedScript);
                        if (myTarget.selectedScript != null) {
                            System.Type type = myTarget.selectedScript.GetType();
                            Debug.Log(type.FullName);
                            FieldInfo[] fields = type.GetFields();
                            EditorGUILayout.LabelField(fields[0].Name);
                        }
                        break;
                }


                break;
        }

        if(EditorGUI.EndChangeCheck()) {
            soTarget.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }
    }

    private void ShowTargetOptions(ref PositionOptions positionChoice, ref int choice1dir, ref SerializedProperty vec, ref SerializedProperty obj, ref SerializedProperty dist) {
        positionChoice = (PositionOptions)EditorGUILayout.EnumPopup("Position type: ", positionChoice);
        switch (positionChoice) {
            case PositionOptions.Global:
            case PositionOptions.Local:
                EditorGUILayout.PropertyField(vec);
                break;
            case PositionOptions.GameObject:
                EditorGUILayout.PropertyField(obj);
                break;
            case PositionOptions.Direction:
                choice1dir = GUILayout.Toolbar(choice1dir, new string[] { "Forward", "Backward", "Left", "Right", "Up", "Down" });
                EditorGUILayout.PropertyField(dist);
                break;
        }
    }
}
