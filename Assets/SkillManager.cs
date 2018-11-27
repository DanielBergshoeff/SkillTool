using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {

    [SerializeField]
    private Transform startPosition;

    [SerializeField]
    private List<Vector3> targetPositions;

    [SerializeField]
    private GameObject prefabSkill;
    
    

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

    private List<GameObject> skills;

    [HideInInspector]
    public int currentTab;

    [HideInInspector]
    public int triggerChoice, positionChoice, positionChoice1, positionChoice1Direction, targetChoice1, targetChoice1Direction;

    // Use this for initialization
    void Start() {
        skills = new List<GameObject>();
        targetPositions = new List<Vector3>();

        switch (triggerChoice) {
            case 1:
                currentTimeWaited = Random.Range(minTime, maxTime);
                break;
            case 2:
                currentTimeWaited = timeBetween;
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
        //If trigger by player input
        if (triggerChoice == 0) {
            if (Input.GetKeyDown(input)) {
                InstantiateSkill();
            }
        }
        //If trigger by time
        else if(triggerChoice == 1 || triggerChoice == 2) {
            currentTimeWaited -= Time.deltaTime;
            if(currentTimeWaited <= 0) {
                InstantiateSkill();
                //If trigger at random intervals
                if (triggerChoice == 1)
                    currentTimeWaited = Random.Range(minTime, maxTime);
                //If trigger at specific intervals
                else if (triggerChoice == 2)
                    currentTimeWaited = timeBetween;
            }
        }

        if (positionChoice == 1) {
            for (int i = 0; i < skills.Count; i++) {
                skills[i].transform.position = Vector3.MoveTowards(skills[i].transform.position, targetPositions[i], Time.deltaTime * skillSpeed);
            }
        }
	}

    private Vector3 GetPositionFromMenu(int choice1, int choice1dir, Vector3 vec, GameObject go, float dist) {
        Vector3 positionToSpawn = Vector3.zero;

        switch (choice1) {
            //If skill position is at a global position
            case 0:
                positionToSpawn = vec;
                break;
            //If skill position is relative to this local position
            case 1:
                positionToSpawn = this.transform.position + vec;
                break;
            //If skill position is at a different game object's position
            case 2:
                positionToSpawn = go.transform.position;
                break;
            //If skill position is in a certain direction
            case 3:
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
            case 4:
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
        Vector3 startPoint = GetPositionFromMenu(positionChoice1, positionChoice1Direction, skillPositionVector, skillPositionObject, skillPositionDistance);
        Vector3 targetPoint = GetPositionFromMenu(targetChoice1, targetChoice1Direction, skillTargetVector, skillTargetObject, skillTargetDistance);
        GameObject skill = Instantiate(prefabSkill, startPoint, Quaternion.identity);
        skills.Add(skill);
        targetPositions.Add(targetPoint);
    }
}
