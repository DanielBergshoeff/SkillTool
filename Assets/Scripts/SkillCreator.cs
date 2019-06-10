using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class SkillCreator : MonoBehaviour {

    public string skillName;
    public Object objMethod;


    [SerializeField]
    public Object selectedScript = null;
    public int selectedMethod;
    public string selectedMethodString;

    [SerializeField]
    public Transform startPosition;

    [SerializeField]
    public List<Vector3> targetPositions;

    [SerializeField]
    public GameObject prefabSkill;

    public TriggerOptions triggerOption;
    public PositionOptions startPositionOption;
    public PositionOptions endPositionOption;

    //TRIGGER
    //Trigger to start skill
    public KeyCode input;
    public float cooldownTime;

    //Min and max time between skill start at random
    public float minTime;
    public float maxTime;

    //Time between skill start -> continuously
    public float timeBetween;
    public float currentTimeWaited;

    //POSITION
    public Vector3 skillPositionVector;

    //Position based on GameObject
    public GameObject skillPositionObject;

    //Position based on direction and distance
    public Vector3 skillPositionDirection;
    public float skillPositionDistance;

    //TARGET
    public Vector3 skillTargetVector;

    //Target based on GameObject
    public GameObject skillTargetObject;

    //Target based on direction and distance
    public Vector3 skillTargetDirection;
    public float skillTargetDistance;

    public float skillSpeed;

    //EFFECT
    public bool destroyOnEndPosition;
    public bool destroyOnEffect;

    //Range of effect
    public float effectRange;

    public TargetType targetEffect;

    //Possible effect targets
    public GameObject effectTargetGameObject;
    public string effectTargetName;
    public string effectTargetTag;
    public string effectTargetScriptName;

    //Effect options
    public EffectOptions whenToTriggerEffect;
    public float skillSize;

    public float timeTillEffect;
    public float minTimeTillEffect;
    public float maxTimeTillEffect;

    public int effectOptionsChoice;
    public int fieldChosenChoice;
    public int variableChangeChoice;

    public object fieldValue;
    public int fieldValueInt;
    public float fieldValueFloat;
    public double fieldValueDouble;

    public bool fieldMaxValueBool;
    public bool fieldMinValueBool;
    public int fieldMaxValue;
    public int fieldMinValue;

    //From script
    public string onEffectScriptName;

    [HideInInspector]
    public int currentTab;

    [HideInInspector]
    public int positionChoice, positionChoice1Direction, targetChoice1Direction;

    public GameObject skillGameObject;

    public void SaveSkill() {
        /*using (StreamWriter streamWriter = new StreamWriter("jsonSkill.txt")) {
            string json = JsonUtility.ToJson(this);
            streamWriter.Write(json);
        }*/

        Skill skill = (Skill)ScriptableObject.CreateInstance("Skill");
        skill.cooldownTime = cooldownTime;
        skill.currentTab = currentTab;
        skill.currentTimeWaited = currentTimeWaited;
        skill.destroyOnEndPosition = destroyOnEndPosition;
        skill.destroyOnEffect = destroyOnEffect;
        skill.effectOptionsChoice = effectOptionsChoice;
        skill.effectRange = effectRange;
        skill.effectTargetGameObject = effectTargetGameObject;
        skill.effectTargetName = effectTargetName;
        skill.effectTargetScriptName = effectTargetScriptName;
        skill.effectTargetTag = effectTargetTag;
        skill.endPositionOption = endPositionOption;
        skill.fieldChosenChoice = fieldChosenChoice;
        skill.fieldValue = fieldValue;
        skill.input = input;
        skill.maxTime = maxTime;
        skill.minTime = minTime;
        skill.onEffectScriptName = onEffectScriptName;
        skill.positionChoice = positionChoice;
        skill.positionChoice1Direction = positionChoice1Direction;
        skill.prefabSkill = prefabSkill;
        skill.selectedMethod = selectedMethod;
        skill.selectedScript = selectedScript;
        skill.selectedMethodString = selectedMethodString;
        skill.skillGameObject = skillGameObject;
        skill.skillPositionDirection = skillPositionDirection;
        skill.skillPositionDistance = skillPositionDistance;
        skill.skillPositionObject = skillPositionObject;
        skill.skillPositionVector = skillPositionVector;
        skill.skillSpeed = skillSpeed;
        skill.skillTargetDirection = skillTargetDirection;
        skill.skillTargetDistance = skillTargetDistance;
        skill.skillTargetObject = skillTargetObject;
        skill.skillTargetVector = skillTargetVector;
        skill.startPosition = startPosition;
        skill.startPositionOption = startPositionOption;
        skill.targetChoice1Direction = targetChoice1Direction;
        skill.targetEffect = targetEffect;
        skill.targetPositions = targetPositions;
        skill.timeBetween = timeBetween;
        skill.triggerOption = triggerOption;
        skill.variableChangeChoice = variableChangeChoice;
        skill.skillName = skillName;
        skill.whenToTriggerEffect = whenToTriggerEffect;
        skill.skillSize = skillSize;

        skill.fieldValueInt = fieldValueInt;
        skill.fieldValueFloat = fieldValueFloat;
        skill.fieldValueDouble = fieldValueDouble;

        skill.fieldMaxValueBool = fieldMaxValueBool;
        skill.fieldMinValueBool = fieldMinValueBool;
        skill.fieldMaxValue = fieldMaxValue;
        skill.fieldMinValue = fieldMinValue;

        skill.timeTillEffect = timeTillEffect;
        skill.minTimeTillEffect = minTimeTillEffect;
        skill.maxTimeTillEffect = maxTimeTillEffect;

        string path = "Assets/Skills";

        if (!Directory.Exists(Path.GetFullPath(path))) {
            AssetDatabase.CreateFolder("Assets", "Skills");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + skill.skillName + ".asset");

        AssetDatabase.CreateAsset(skill, assetPathAndName);
        Undo.RegisterCreatedObjectUndo(skill, "Create " + skill.skillName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
