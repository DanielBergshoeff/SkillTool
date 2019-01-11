using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public float health;
    private float damage;
    public int mana;

    public float Health {
        get {
            return health;
        }
        set {
            health = value;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TransformUp() {
        this.transform.position = this.transform.position + this.transform.up * 5;
    }
}
