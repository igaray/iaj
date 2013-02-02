using UnityEngine;
using System.Collections;

public class Inn : Building {
	
	public float healCoefficient = 0.01f;
	
	public void heal(Agent agent){
		if (agent.life < agent.lifeTotal)
			agent.addLife(Mathf.CeilToInt(agent.lifeTotal * healCoefficient));
	}
}
