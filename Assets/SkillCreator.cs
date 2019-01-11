using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class SkillCreator : MonoBehaviour {

    [SerializeField]
    public TriggerEvent triggerEvent = new TriggerEvent();
    public TriggerEvent onTriggerEvent { get { return triggerEvent; } set { triggerEvent = value; } }

    [SerializeField]
    public Object selectedScript;
    public string selectedMethod;

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

    //Range of effect
    public float effectRange;

    public TargetType targetEffect;

    //Possible effect targets
    public GameObject effectTargetGameObject;
    public string effectTargetName;
    public string effectTargetTag;
    public string effectTargetScriptName;

    //Effect options
    public int effectOptionsChoice;
    public int fieldChosenChoice;

    public object fieldValue;

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
        skill.triggerEvent = triggerEvent;
        skill.triggerOption = triggerOption;

        string path = "Assets/Skills";

        if (!Directory.Exists(Path.GetFullPath(path))) {
            AssetDatabase.CreateFolder("Assets", "Skills");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + skill.GetType().ToString() + ".asset");

        AssetDatabase.CreateAsset(skill, assetPathAndName);
        Undo.RegisterCreatedObjectUndo(this, "Create " + skill.name);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
