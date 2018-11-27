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
    private SerializedProperty input;
    private SerializedProperty minTime;
    private SerializedProperty maxTime;
    private SerializedProperty timeBetween;
    private SerializedProperty cooldownTime;

    //Position
    public SerializedProperty skillPositionVector;
    public SerializedProperty skillPositionObject;
    public SerializedProperty skillPositionDirection;
    public SerializedProperty skillPositionDistance;

    //Target
    public SerializedProperty skillTargetVector;
    public SerializedProperty skillTargetObject;
    public SerializedProperty skillTargetDirection;
    public SerializedProperty skillTargetDistance;

    public SerializedProperty skillSpeed;

    private void OnEnable() {
        myTarget = (SkillManager)target;
        soTarget = new SerializedObject(target);

        //Prefab
        prefabSkill = soTarget.FindProperty("prefabSkill");

        //Trigger
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
                myTarget.triggerChoice = GUILayout.Toolbar(myTarget.triggerChoice, new string[] { "Player input", "Random", "Continuously" });
                switch(myTarget.triggerChoice) {
                    case 0:
                        EditorGUILayout.LabelField("Trigger by player input");
                        EditorGUILayout.PropertyField(input);
                        EditorGUILayout.PropertyField(cooldownTime);
                        break;
                    case 1:
                        EditorGUILayout.LabelField("Trigger at random intervals");
                        EditorGUILayout.PropertyField(minTime);
                        EditorGUILayout.PropertyField(maxTime);
                        break;
                    case 2:
                        EditorGUILayout.LabelField("Trigger at specific intervals");
                        EditorGUILayout.PropertyField(timeBetween);
                        break;
                }
                break;
            case 1:
                myTarget.positionChoice = GUILayout.Toolbar(myTarget.positionChoice, new string[] { "Constant position", "Move position" });
                switch(myTarget.positionChoice) {
                    case 0:
                        EditorGUILayout.LabelField("Constant position");
                        ShowTargetOptions(ref myTarget.positionChoice1, ref myTarget.positionChoice1Direction, ref skillPositionVector, ref skillPositionObject, ref skillPositionDistance);
                        break;
                    case 1:
                        EditorGUILayout.LabelField("Start position");
                        ShowTargetOptions(ref myTarget.positionChoice1, ref myTarget.positionChoice1Direction, ref skillPositionVector, ref skillPositionObject, ref skillPositionDistance);
                        EditorGUILayout.LabelField("End position");
                        ShowTargetOptions(ref myTarget.targetChoice1, ref myTarget.targetChoice1Direction, ref skillTargetVector, ref skillTargetObject, ref skillTargetDistance);
                        EditorGUILayout.LabelField("Speed");
                        EditorGUILayout.PropertyField(skillSpeed);
                        break;
                }
                break;
            case 2:
                EditorGUILayout.PropertyField(prefabSkill);
                break;
            case 3:

                break;
        }

        if(EditorGUI.EndChangeCheck()) {
            soTarget.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }
    }

    private void ShowTargetOptions(ref int choice1, ref int choice1dir, ref SerializedProperty vec, ref SerializedProperty obj, ref SerializedProperty dist) {
        choice1 = GUILayout.Toolbar(choice1, new string[] { "Global", "Local", "GameObject", "Direction", "Mouse" });
        switch (choice1) {
            case 0:
            case 1:
                EditorGUILayout.PropertyField(vec);
                break;
            case 2:
                EditorGUILayout.PropertyField(obj);
                break;
            case 3:
                choice1dir = GUILayout.Toolbar(choice1dir, new string[] { "Forward", "Backward", "Left", "Right", "Up", "Down" });
                EditorGUILayout.PropertyField(dist);
                break;
        }
    }
}
