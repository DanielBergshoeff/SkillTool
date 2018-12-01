using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class SkillManager : MonoBehaviour {

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
                if(destroyOnEndPosition) {
                    if (Vector3.Distance(skills[i].transform.position, targetPositions[i]) < 0.01f) {
                        Destroy(skills[i]);
                        skills.Remove(skills[i]);
                        targetPositions.Remove(targetPositions[i]);
                    }
                }
            }
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
