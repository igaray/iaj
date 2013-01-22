using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : Entity {

	private int lifeTotal = 100;
	// private int vida_min = 0;
	public int life;
	
	public List<Objeto> backpack = new List<Objeto>();
	
	public Agent(string description, string name, int life) 
		: base(description, "Agente", name, false){
		
		this.life = life;
	}
	
	public void subLife(int dif) {
		if(this.life - dif <= 0){
			this.life = 0;	
			//TODO: DIEEEEEEE
		} else {
			this.life = life - dif;	
		}
	}
	
	public void addLife(int dif) {
		if(this.life + dif >= this.lifeTotal){
			this.life = this.lifeTotal;
		} else {
			this.life += dif;	
		}
	}
	
	public void pickup(Objeto obj) {
		//TODO: 
		// - check the distance between agent and object
		// - remove object from the game
		this.backpack.Add(obj);
	}
	
	public void drop(Objeto obj) {
		//TODO: 
		// - add object to the game
		this.backpack.Remove(obj);
	}
	
}
