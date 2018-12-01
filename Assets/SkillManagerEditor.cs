using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    public SerializedProperty skillPositionVector;
    public SerializedProperty skillPositionObject;
    public SerializedProperty skillPositionDirection;
    public SerializedProperty skillPositionDistance;

    //Target
    public SerializedProperty skillTargetVector;
    public SerializedProperty skillTargetObject;
    public SerializedProperty skillTargetDirection;
    public SerializedProperty skillTargetDistance;

    //Effect
    public SerializedProperty destroyOnEndPosition;

    public SerializedProperty skillSpeed;

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

        //Speed
        skillSpeed = soTarget.FindProperty("skillSpeed");
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
