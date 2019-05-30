using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Reflection;
using System.Linq;

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

[System.Serializable]
public class TriggerEvent : UnityEvent { }

[System.Serializable]
public class SkillInstances {
    public List<Skill> skills;
}


public class SkillManager : MonoBehaviour {
    public Skill[] allSkills;
    
    [SerializeField]
    private SkillInstances[] skillInstances;

    // Use this for initialization
    void Start() {
        skillInstances = new SkillInstances[allSkills.Length];
        for (int i = 0; i < skillInstances.Length; i++) {
            skillInstances[i] = new SkillInstances();
            skillInstances[i].skills = new List<Skill>();
        }
        foreach (Skill skill in allSkills) {
            skill.targetPositions = new List<Vector3>();

            switch (skill.triggerOption) {
                case TriggerOptions.Random:
                    skill.currentTimeWaited = Random.Range(skill.minTime, skill.maxTime);
                    break;
                case TriggerOptions.Continuous:
                    skill.currentTimeWaited = skill.timeBetween;
                    break;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        foreach (Skill skill in allSkills) {
            //If trigger by player input
            if (skill.triggerOption == TriggerOptions.PlayerInput) {
                if (Input.GetKeyDown(skill.input)) {
                    InstantiateSkill(skill);
                }
            }
            //If trigger by time
            else if (skill.triggerOption == TriggerOptions.Random || skill.triggerOption == TriggerOptions.Continuous) {
                skill.currentTimeWaited -= Time.deltaTime;
                if (skill.currentTimeWaited <= 0) {
                    InstantiateSkill(skill);
                    //If trigger at random intervals
                    if (skill.triggerOption == TriggerOptions.Random)
                        skill.currentTimeWaited = Random.Range(skill.minTime, skill.maxTime);
                    //If trigger at specific intervals
                    else if (skill.triggerOption == TriggerOptions.Continuous)
                        skill.currentTimeWaited = skill.timeBetween;
                }
            }
        }

        for (int i = 0; i < allSkills.Length; i++) {
            if (allSkills[i].positionChoice == 1) {
                for (int j = 0; j < skillInstances[i].skills.Count; j++) {
                    skillInstances[i].skills[j].skillGameObject.transform.position = Vector3.MoveTowards(skillInstances[i].skills[j].skillGameObject.transform.position, skillInstances[i].skills[j].targetPositions[0], Time.deltaTime * skillInstances[i].skills[j].skillSpeed);
                    if (Vector3.Distance(skillInstances[i].skills[j].skillGameObject.transform.position, skillInstances[i].skills[j].targetPositions[0]) < 0.01f) {
                        //Effect
                        switch (allSkills[i].targetEffect) {
                            //If the target of effect is a script
                            case TargetType.Script: {
                                    Collider[] hitColliders = Physics.OverlapSphere(skillInstances[i].skills[j].skillGameObject.transform.position, allSkills[i].effectRange);
                                    foreach (Collider collider in hitColliders) {
                                        var script = collider.gameObject.GetComponent(allSkills[i].effectTargetScriptName);
                                        if (script != null) {
                                            DoEffect(collider.gameObject, skillInstances[i].skills[j]);
                                        }
                                    }
                                }
                                break;
                            case TargetType.Name: {
                                    Collider[] hitColliders = Physics.OverlapSphere(skillInstances[i].skills[j].skillGameObject.transform.position, allSkills[i].effectRange);
                                    foreach (Collider collider in hitColliders) {
                                        if (collider.gameObject.name == allSkills[i].effectTargetName) {
                                            DoEffect(collider.gameObject, skillInstances[i].skills[j]);
                                        }
                                    }
                                }
                                break;
                            case TargetType.Tag: {
                                    Collider[] hitColliders = Physics.OverlapSphere(skillInstances[i].skills[j].skillGameObject.transform.position, allSkills[i].effectRange);
                                    foreach (Collider collider in hitColliders) {
                                        if (collider.gameObject.tag == allSkills[i].effectTargetTag) {
                                            DoEffect(collider.gameObject, skillInstances[i].skills[j]);
                                        }
                                    }
                                }
                                break;
                        }

                        if (allSkills[i].destroyOnEndPosition) {
                            {
                                Destroy(skillInstances[i].skills[j].skillGameObject);
                                skillInstances[i].skills.Remove(skillInstances[i].skills[j]);
                            }
                        }
                    }
                }
            }
        }
	}

    private void DoEffect(GameObject go, Skill skill) {
        switch(skill.effectOptionsChoice) {
            //If a variable needs to be changed in the script
            case 0:
                var script = go.GetComponent(skill.selectedScript.name);
                var type = GetType(skill.selectedScript.name);
                var fields = type.GetFields();

                FieldInfo field = script.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[skill.fieldChosenChoice];

                switch (skill.variableChangeChoice) {
                    //If the variable is set
                    case 0:
                        if (field.GetValue(script) is float) {
                            field.SetValue(script, skill.fieldValueFloat);
                        }
                        else if (field.GetValue(script) is int) {
                            field.SetValue(script, skill.fieldValueInt);
                        }
                        if (field.GetValue(script) is double) {
                            field.SetValue(script, skill.fieldValueDouble);
                        }
                        field.SetValue(script, skill.fieldValue);
                        break;
                    //If the variable is added to
                    case 1:
                        if (field.GetValue(script) is float) {
                            if (skill.fieldValueFloat + (float)field.GetValue(script) <= skill.fieldMaxValue) 
                                field.SetValue(script, (float)field.GetValue(script) + skill.fieldValueFloat);
                            else 
                                field.SetValue(script, skill.fieldMaxValue);
                        }
                        else if (field.GetValue(script) is int) {
                            if (skill.fieldValueInt + (int)field.GetValue(script) <= skill.fieldMaxValue) 
                                field.SetValue(script, (int)field.GetValue(script) + skill.fieldValueInt);
                            else 
                                field.SetValue(script, skill.fieldMaxValue);
                        }
                        if(field.GetValue(script) is double) {
                            if (skill.fieldValueDouble + (double)field.GetValue(script) <= skill.fieldMaxValue)
                                field.SetValue(script, (double)field.GetValue(script) + skill.fieldValueDouble);
                            else
                                field.SetValue(script, skill.fieldMaxValue);
                        }
                        break;
                    //If the variable is subtracted from
                    case 2:
                        var newValue = field.GetValue(script);
                        if (field.GetValue(script) is float) {
                            newValue = (float)field.GetValue(script) - skill.fieldValueFloat;
                            if (skill.fieldMinValueBool && skill.fieldMinValue >= (float)newValue)
                                newValue = (float)skill.fieldMinValue;

                        }
                        else if (field.GetValue(script) is int) {
                            newValue = (int)field.GetValue(script) - skill.fieldValueInt;
                            if (skill.fieldMinValueBool && skill.fieldMinValue >= (int)newValue)
                                newValue = (int)skill.fieldMinValue;
                        }
                        else if (field.GetValue(script) is double) {
                            newValue = (double)field.GetValue(script) - skill.fieldValueDouble;
                            if (skill.fieldMinValueBool && skill.fieldMinValue >= (double)newValue)
                                newValue = (double)skill.fieldMinValue;
                        }

                        field.SetValue(script, newValue);
                        break;
                }

                break;
            //If a method needs to be called in the script
            case 1:
                MonoBehaviour methodscript = (MonoBehaviour)go.GetComponent(skill.selectedScript.name);
                methodscript.Invoke(skill.selectedMethodString, 0f);
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

    private void InstantiateSkill(Skill skillToInstantiate) {
        for (int i = 0; i < allSkills.Length; i++) {
            if(skillToInstantiate == allSkills[i]) {
                Skill skill = Instantiate(skillToInstantiate);
                Vector3 startPoint = GetPositionFromMenu(skill.startPositionOption, skill.positionChoice1Direction, skill.skillPositionVector, skill.skillPositionObject, skill.skillPositionDistance);
                Vector3 targetPoint = GetPositionFromMenu(skill.endPositionOption, skill.targetChoice1Direction, skill.skillTargetVector, skill.skillTargetObject, skill.skillTargetDistance);
                GameObject skillgo = Instantiate(skill.prefabSkill, startPoint, Quaternion.identity);
                skill.skillGameObject = skillgo;
                skill.targetPositions.Add(targetPoint);
                skillInstances[i].skills.Add(skill);
            }
        }
    }

    public static System.Type GetType(string TypeName) {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = System.Type.GetType(TypeName);

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
