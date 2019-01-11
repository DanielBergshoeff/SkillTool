using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class Skill : ScriptableObject {

    [SerializeField]
    public TriggerEvent triggerEvent;
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

}
