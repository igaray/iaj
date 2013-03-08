using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// éstas son las implementaciones de las clases que tengan el estado del motor
public interface IEngine{
	// tiempo entre iteraciones del motor
	float _delta
	{
		get;
		set;
	}
	
	// monedas en el mundo
	IDictionary<string, Gold> coins
	{
		get;
		set;
	}
	
	// duración de las acciones en segundos
	IDictionary <string, float> actionDurations
	{
		get;
	}
	
	bool test
	{
		get;
	}
	
	void addGold(Gold gold);
}

// Esto sirve para mantener el puntero al IEngine en el objeto de Unity (el Component). 
// El objeto que herede de esto deberá estar asociado a un GameObject, con el tag GameController
public interface IEngineComponent {
	IEngine engine
	{
		get;
	}
}