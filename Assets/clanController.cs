﻿using UnityEngine;
using System.Collections;

public class clanController : MonoBehaviour {

	public string clanName = "A";
	public const int totalWarriors = 10;
	public int aliveWarriors;
	public int totalAtckPwr = 100;
	public int totalDefPwr = 100;
	public bool uniformPowerDistribution = true;
	public Vector3 desp;
	public Sprite warriorImg;
	public Vector3 scale;
	public Vector3 origin;
	public Vector3 deathPosition;
	public Vector2 boardSize;
	public Transform enemy;

	Warrior[] warriors;

	bool first = true;

	public class Warrior
	{
		public int id;
		public GameObject gO;
		public int atck;
		public int def;

	}

	// Use this for initialization
	void Start () {
		aliveWarriors = totalWarriors;
		warriors = new Warrior[totalWarriors];
		int ttAtck = totalAtckPwr;
		int ttDef = totalDefPwr;
		for (int i = 0; i < totalWarriors; ++i) {
			warriors [i] = new Warrior ();
			warriors [i].id = i;
			if (uniformPowerDistribution) {
				warriors [i].atck = totalAtckPwr / totalWarriors;
				warriors [i].def = totalDefPwr / totalWarriors;
			} else {
				if (i == totalWarriors - 1) {
					warriors [i].atck = ttAtck;
					warriors [i].def = ttDef;
				} else {
					int aux = Random.Range (1, ttAtck);
					warriors [i].atck = aux;
					ttAtck -= aux;
					aux = Random.Range (1, ttDef);
					warriors [i].def = aux;
					ttDef -= aux;
				}
			}
			warriors [i].gO = new GameObject ();
			warriors [i].gO.AddComponent<SpriteRenderer> ().sprite = warriorImg;
			warriors [i].gO.transform.position = origin + desp*i;
			warriors [i].gO.name = clanName + i.ToString ();
			warriors [i].gO.transform.localScale = scale;
		}
	}

	void test (){
		Vector2 mv = new Vector2 (1,1);
		move (0, mv);
		move (1, mv * -1);

		move (0, mv);
		move (1, mv * -1);

		move (0, mv);
		move (1, mv * -1);

		move (0, mv);
		move (1, mv * -1);

		//move (0, mv);
		move (1, mv * -1);

		attack (0);
	}

	void Update(){
		if (first)
			test ();
		first = false;
	}

	bool checkLimits(Vector3 posi)
	{
		if (posi.x < 0 || posi.y < 0)
			return false;
		if (posi.x >= boardSize.x || posi.y >= boardSize.y)
			return false;
		return true;
	}

	//return true if the movement is succesful
	public bool move (int warriorId, Vector2 direction)
	{
		Vector3 aux = Vector3.zero;
		if (warriors.Length > warriorId && warriors[warriorId].def > 0)
		{
			if (direction.x > 0)
				aux += new Vector3 (1, 0, 0);
			else if (direction.x < 0)
				aux += new Vector3 (-1, 0, 0);
			if (direction.y > 0)
				aux += new Vector3 (0, 1, 0);
			else if (direction.y < 0)
				aux += new Vector3 (0, -1, 0);
			Vector3 vaux = warriors [warriorId].gO.transform.position;
			warriors [warriorId].gO.transform.position += aux;
			if (!checkLimits (warriors [warriorId].gO.transform.position)) {
				warriors [warriorId].gO.transform.position = vaux;
				return false;
			}
			return true;
		}
		return false;
	}

	bool checkPosition (Vector2 pos, int id){
		if (warriors [id].def <= 0)
			return false;
		float auxX = warriors [id].gO.transform.position.x; 
		if (auxX < pos.x || auxX > pos.x + 1)
			return false;
		auxX = warriors [id].gO.transform.position.y; 
		if (auxX < pos.y || auxX > pos.y + 1)
			return false;
		return true;
	}

	public int getDamage (Vector2 pos, int dmg)
	{
		int selected = -1;
		for (int i = 0; i < totalWarriors; ++i) {
			if (checkPosition (pos, i)) {
				if (selected == -1)
					selected = i;
				else if (warriors [selected].def < warriors [i].def)
					selected = i;
			}
		}
		if (selected != -1) {
			if (warriors [selected].def > dmg) {
				warriors [selected].def -= dmg;
				totalDefPwr -= dmg;
				return dmg;
			} else {
				int auxDmg = warriors [selected].def;
				totalDefPwr -= auxDmg;
				totalAtckPwr -= warriors [selected].atck;
				warriors [selected].def = -1;
				warriors [selected].gO.transform.position = deathPosition;
				--aliveWarriors;
				return auxDmg;
			}
		}
		return 0;
	}

	//return damage recived by the enemy
	public int attack (int warriorId){
		if (warriorId < 0 || warriorId >= totalWarriors)
			return 0;
		if (warriors [warriorId].def <= 0)
			return 0;
		Vector2 aux = new Vector2 (warriors [warriorId].gO.transform.position.x, warriors [warriorId].gO.transform.position.y);
		int auxDmg = enemy.GetComponent<clanController>().getDamage (aux, warriors [warriorId].atck);
		Debug.Log (auxDmg + " damage infricted by clan "+ clanName);
		return auxDmg;
	}

}
