using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

    public float health;
    private float damage;
    public int mana;

    public float maxHealth;
    public int maxMana;

    public Slider sliderHealth;
    public Slider sliderMana;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        sliderHealth.value = health / maxHealth;
        sliderMana.value = (float)mana / maxMana;
    }

    public void TransformUp() {
        this.transform.position = this.transform.position + this.transform.up * 5;
    }
}
