using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;

[CustomEditor(typeof(SkillCreator))]
public class SkillManagerEditor : Editor {
    private SerializedProperty skillName;

    private SkillCreator myTarget;
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
    private SerializedProperty effectRange;

    private SerializedProperty effectTargetGameObject;
    private SerializedProperty effectTargetName;
    private SerializedProperty effectTargetTag;
    private SerializedProperty effectTargetScriptName;

    private SerializedProperty skillSpeed;

    //Script
    private SerializedProperty selectedScript;

    private SerializedProperty triggerEvent;

    private SerializedProperty fieldMaxValueBool;
    private SerializedProperty fieldMinValueBool;
    private SerializedProperty fieldMaxValue;
    private SerializedProperty fieldMinValue;

    static string[] ignoreMethods = new string[] { "Start", "Update" };

    private void OnEnable() {   
        myTarget = (SkillCreator)target;
        soTarget = new SerializedObject(target);

        skillName = soTarget.FindProperty("skillName");

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
        effectRange = soTarget.FindProperty("effectRange");

        effectTargetGameObject = soTarget.FindProperty("effectTargetGameObject");
        effectTargetName = soTarget.FindProperty("effectTargetName");
        effectTargetTag = soTarget.FindProperty("effectTargetTag");
        effectTargetScriptName = soTarget.FindProperty("effectTargetScriptName");

        //Script
        selectedScript = soTarget.FindProperty("selectedScript");

        //Speed
        skillSpeed = soTarget.FindProperty("skillSpeed");

        triggerEvent = soTarget.FindProperty("triggerEvent");

        fieldMaxValueBool = soTarget.FindProperty("fieldMaxValueBool");
        fieldMinValueBool = soTarget.FindProperty("fieldMinValueBool");
        fieldMaxValue = soTarget.FindProperty("fieldMaxValue");
        fieldMinValue = soTarget.FindProperty("fieldMinValue");
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
                EditorGUILayout.LabelField("Skill name");
                EditorGUILayout.PropertyField(skillName);
                break;
            case 3:
                EditorGUILayout.PropertyField(destroyOnEndPosition);
                EditorGUILayout.PropertyField(effectRange);
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

                myTarget.effectOptionsChoice = GUILayout.Toolbar(myTarget.effectOptionsChoice, new string[] { "Change variable", "Call method", "Destroy object" });
                switch (myTarget.effectOptionsChoice) {
                    case 0:
                        EditorGUILayout.PropertyField(selectedScript);
                        if (myTarget.selectedScript != null && !myTarget.selectedScript.Equals(null)) {
                            var type = SkillManager.GetType(myTarget.selectedScript.name);
                            //System.Type type = typeof(Unit);
                            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                            var typeInstance = System.Activator.CreateInstance(type);

                            string[] fieldNames = new string[fields.Length];
                            for (int i = 0; i < fields.Length; i++) {
                                fieldNames[i] = fields[i].Name;
                            }

                            myTarget.fieldChosenChoice = GUILayout.Toolbar(myTarget.fieldChosenChoice, fieldNames);
                            object temp = fields[myTarget.fieldChosenChoice].GetValue(typeInstance);
                            if (temp is float) {
                                if (myTarget.fieldValue == null)
                                    myTarget.fieldValue = 0.0f;
                                myTarget.fieldValueFloat = EditorGUILayout.FloatField(fields[myTarget.fieldChosenChoice].Name + ": ", myTarget.fieldValueFloat);
                            }
                            else if (temp is int) {
                                myTarget.fieldValueInt = EditorGUILayout.IntField(fields[myTarget.fieldChosenChoice].Name + ": ", myTarget.fieldValueInt);
                            }
                            else if (temp is double) {
                                if (myTarget.fieldValue == null)
                                    myTarget.fieldValue = (double)0;
                                myTarget.fieldValueDouble = EditorGUILayout.DoubleField(fields[myTarget.fieldChosenChoice].Name + ": ", myTarget.fieldValueDouble);
                            }

                            myTarget.variableChangeChoice = GUILayout.Toolbar(myTarget.variableChangeChoice, new string[] { "Set variable", "Add to", "Subtract from" });
                            if (myTarget.variableChangeChoice >= 0 && myTarget.variableChangeChoice < 3) {
                                EditorGUILayout.LabelField("Set max value");
                                EditorGUILayout.PropertyField(fieldMaxValueBool);
                                EditorGUILayout.LabelField("Set min value");
                                EditorGUILayout.PropertyField(fieldMinValueBool);
                                
                                if (myTarget.fieldMaxValueBool) {
                                    EditorGUILayout.LabelField("Max value");
                                    EditorGUILayout.PropertyField(fieldMaxValue);
                                }
                                if (myTarget.fieldMinValueBool) {
                                    EditorGUILayout.LabelField("Min value");
                                    EditorGUILayout.PropertyField(fieldMinValue);
                                }
                            }
                            }
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(selectedScript);
                        if(myTarget.selectedScript != null && !myTarget.selectedScript.Equals(null)) {
                            Type t = SkillManager.GetType(myTarget.selectedScript.name);
                            var typeInstance = System.Activator.CreateInstance(t);
                            string[] names = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(x => x.DeclaringType == t) // Only list methods defined in our own class
                            .Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
                            .Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
                            .Select(x => x.Name)
                            .ToArray();
                            myTarget.selectedMethod = EditorGUILayout.Popup(myTarget.selectedMethod, names);

                            myTarget.selectedMethodString = names[myTarget.selectedMethod];
                        }
                        break;

                }
                break;
        }

        if(GUILayout.Button("Create skill")) {
            myTarget.SaveSkill();
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
