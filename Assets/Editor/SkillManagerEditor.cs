﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;

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
    private SerializedProperty effectRange;

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

                myTarget.effectOptionsChoice = GUILayout.Toolbar(myTarget.effectOptionsChoice, new string[] { "Change variable(s) in script", "Call method", "Destroy object" });
                switch (myTarget.effectOptionsChoice) {
                    case 0:
                        EditorGUILayout.PropertyField(selectedScript);
                        var type = GetType(myTarget.selectedScript.name);
                        //System.Type type = typeof(Unit);
                        var fields = type.GetFields();
                        var typeInstance = System.Activator.CreateInstance(type);

                        string[] fieldNames = new string[fields.Length];
                        for (int i = 0; i < fields.Length; i++) {
                            fieldNames[i] = fields[i].Name;
                        }

                        myTarget.fieldChosenChoice = GUILayout.Toolbar(myTarget.fieldChosenChoice, fieldNames);

                        object temp = fields[myTarget.fieldChosenChoice].GetValue(typeInstance);
                        if (temp is float) {
                            fields[myTarget.fieldChosenChoice].SetValue(typeInstance, EditorGUILayout.FloatField(fields[myTarget.fieldChosenChoice].Name + ": ", (float)temp));
                        }
                        else if (temp is int) {
                            fields[myTarget.fieldChosenChoice].SetValue(typeInstance, EditorGUILayout.IntField(fields[myTarget.fieldChosenChoice].Name + ": ", (int)temp));
                        }

                        /*for (int i = 0; i < fields.Length; i++) {
                            object temp = fields[i].GetValue(typeInstance);
                            
                            EditorGUILayout.LabelField(fields[i].Name);
                            
                            if (temp is float) {
                                fields[i].SetValue(typeInstance, EditorGUILayout.FloatField(fields[i].Name + ": ", (float)temp));
                            }
                            else if (temp is int) {
                                fields[i].SetValue(typeInstance, EditorGUILayout.IntField(fields[i].Name + ": ", (int)temp));
                            }
                        }*/

                        break;
                    case 1:
                        EditorGUILayout.PropertyField(triggerEvent);
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

    public static Type GetType(string TypeName) {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // If the TypeName is a full name, then we can try loading the defining assembly directly
        if (TypeName.Contains(".")) {

            // Get the name of the assembly (Assumption is that we are using 
            // fully-qualified type names)
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;

            // Ask that assembly to return the proper Type
            type = assembly.GetType(TypeName);
            if (type != null)
                return type;

        }

        // If we still haven't found the proper type, we can enumerate all of the 
        // loaded assemblies and see if any of them define the type
        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies) {

            // Load the referenced assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null) {
                // See if that assembly defines the named type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }

        // The type just couldn't be found...
        return null;

    }
}
