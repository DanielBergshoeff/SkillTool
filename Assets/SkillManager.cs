using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[SerializeField]
public enum TriggerOptions {
    PlayerInput,
    Random,
    Continuous
}

[SerializeField]
public enum PositionOptions {
    Global,
    Local,
    GameObject,
    Direction,
    Mouse
}

[SerializeField]
public enum TargetType {
    GameObject,
    Name,
    Tag,
    Script
}

[SerializeField]
public enum EffectOptions {
    
}




public class SkillManager : MonoBehaviour {

    [System.Serializable]
    public class TriggerEvent : UnityEvent { }

    [SerializeField]
    public TriggerEvent triggerEvent = new TriggerEvent();
    public TriggerEvent onTriggerEvent {  get { return triggerEvent; } set { triggerEvent = value; } }

    [SerializeField]
    public Object selectedScript;
    public string selectedMethod;

    [SerializeField]
    private Transform startPosition;

    [SerializeField]
    private List<Vector3> targetPositions;

    [SerializeField]
    private GameObject prefabSkill;

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
    private float currentTimeWaited;

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

    //From script
    public string onEffectScriptName;

    private List<GameObject> skills;

    [HideInInspector]
    public int currentTab;

    [HideInInspector]
    public int positionChoice, positionChoice1Direction, targetChoice1Direction;

    // Use this for initialization
    void Start() {
        skills = new List<GameObject>();
        targetPositions = new List<Vector3>();

        switch (triggerOption) {
            case TriggerOptions.Random:
                currentTimeWaited = Random.Range(minTime, maxTime);
                break;
            case TriggerOptions.Continuous:
                currentTimeWaited = timeBetween;
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
        //If trigger by player input
        if (triggerOption == TriggerOptions.PlayerInput) {
            if (Input.GetKeyDown(input)) {
                InstantiateSkill();
            }
        }
        //If trigger by time
        else if(triggerOption == TriggerOptions.Random || triggerOption == TriggerOptions.Continuous) {
            currentTimeWaited -= Time.deltaTime;
            if(currentTimeWaited <= 0) {
                InstantiateSkill();
                //If trigger at random intervals
                if (triggerOption == TriggerOptions.Random)
                    currentTimeWaited = Random.Range(minTime, maxTime);
                //If trigger at specific intervals
                else if (triggerOption == TriggerOptions.Continuous)
                    currentTimeWaited = timeBetween;
            }
        }

        if (positionChoice == 1) {
            for (int i = 0; i < skills.Count; i++) {
                skills[i].transform.position = Vector3.MoveTowards(skills[i].transform.position, targetPositions[i], Time.deltaTime * skillSpeed);
                if (Vector3.Distance(skills[i].transform.position, targetPositions[i]) < 0.01f) {
                    if (destroyOnEndPosition) {
                        {
                            Destroy(skills[i]);
                            skills.Remove(skills[i]);
                            targetPositions.Remove(targetPositions[i]);
                        }
                    }

                    //Effect
                    switch(targetEffect) {
                        //If the target of effect is a script
                        case TargetType.Script:
                            Collider[] hitColliders = Physics.OverlapSphere(skills[i].transform.position, effectRange);
                            foreach (Collider collider in hitColliders) {
                                var script = collider.gameObject.GetComponent(effectTargetScriptName);
                                if(script != null) {
                                    DoEffect(collider.gameObject);
                                }
                            }
                            break;
                    }
                }
            }
        }
	}

    private void DoEffect(GameObject go) {
        switch(effectOptionsChoice) {
            //If a variable needs to be changed in the script
            case 0:

                break;
            //If a method needs to be called in the script
            case 1:

                break;
            //If the gameobject needs to be destroyed
            case 2:
                Destroy(go);
                break;
        }
    }

    private Vector3 GetPositionFromMenu(PositionOptions positionChoice, int choice1dir, Vector3 vec, GameObject go, float dist) {
        Vector3 positionToSpawn = Vector3.zero;

        switch (positionChoice) {
            //If skill position is at a global position
            case PositionOptions.Global:
                positionToSpawn = vec;
                break;
            //If skill position is relative to this local position
            case PositionOptions.Local:
                positionToSpawn = this.transform.position + vec;
                break;
            //If skill position is at a different game object's position
            case PositionOptions.GameObject:
                positionToSpawn = go.transform.position;
                break;
            //If skill position is in a certain direction
            case PositionOptions.Direction:
                Vector3 directionVector = Vector3.zero;
                if (dist > 0) {
                    switch (choice1dir) {
                        //Forward
                        case 0:
                            directionVector = transform.forward;
                            break;
                        //Backward
                        case 1:
                            directionVector = -transform.forward;
                            break;
                        //Left
                        case 2:
                            directionVector = -transform.right;
                            break;
                        //Right
                        case 3:
                            directionVector = transform.right;
                            break;
                        //Up
                        case 4:
                            directionVector = transform.up;
                            break;
                        //Down
                        case 5:
                            directionVector = -transform.up;
                            break;
                    }
                }

                positionToSpawn = this.transform.position + directionVector * dist;
                break;
            //If skill position is at mouse
            case PositionOptions.Mouse:
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f)) {
                    positionToSpawn = new Vector3(hit.point.x, 1, hit.point.z);
                }
                break;
        }

        return positionToSpawn;
    }

    private void InstantiateSkill() {
        Vector3 startPoint = GetPositionFromMenu(startPositionOption, positionChoice1Direction, skillPositionVector, skillPositionObject, skillPositionDistance);
        Vector3 targetPoint = GetPositionFromMenu(endPositionOption, targetChoice1Direction, skillTargetVector, skillTargetObject, skillTargetDistance);
        GameObject skill = Instantiate(prefabSkill, startPoint, Quaternion.identity);
        skills.Add(skill);
        targetPositions.Add(targetPoint);
    }
}
