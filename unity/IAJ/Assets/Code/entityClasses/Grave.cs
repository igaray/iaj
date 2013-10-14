using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Grave : Building {

	public SimulationState ss;
	
	public List<EObject> content = new List<EObject>();	
	
	private bool open = false;
		
	public override void Start() {
		SimulationState.getInstance().stdout.Send("entro Start");
		base.Start();
		this._type = "grave";
		this.ss    = (GameObject.FindGameObjectWithTag("GameController").
			GetComponent(typeof(SimulationEngineComponentScript))
			as SimulationEngineComponentScript).engine as SimulationState;
				
		this.ss.addGrave(this);
		initializeContent();		
	}
	
	public void put(EObject obj){		
		SimulationState.getInstance().stdout.Send("entro put");
		obj.gameObject.SetActive(false);
		obj.transform.position = this.transform.position;
		content.Add(obj);				
		SimulationState.getInstance().stdout.Send("salio put");
	}
	
	public void setOpen(bool open) {
		this.open = open;
		if (open) {
			foreach (EObject obj in content) {				
				drop(obj);				
			}						
			content.Clear();			
		}
	}
	
	public bool isOpen() {
		return open;
	}
	
	public void drop(EObject obj){				
		obj.gameObject.SetActive(true);
		Vector3 newPosition = this.transform.position;
		newPosition.y += 2.5f;
		obj.transform.position = newPosition;
		obj.rigidbody.AddForce(new Vector3(20,20,20));		
	}

	public void initializeContent() {
		SimulationState.getInstance().stdout.Send("entro initialize");
		if (_name.Equals("grave0")) {						
			Gold gold_0_1 = Gold.Create(new Vector3(0,0,0));			
			this.put(gold_0_1);
		
			Gold gold_0_2 = Gold.Create(new Vector3(0,0,0));			
			this.put(gold_0_2);		
		} else if (_name.Equals("grave1")) {						
			Gold gold_1_1 = Gold.Create(new Vector3(0,0,0));			
			this.put(gold_1_1);
		
			Gold gold_1_2 = Gold.Create(new Vector3(0,0,0));			
			this.put(gold_1_2);		
		} else if (_name.Equals("grave2")) {						
			Gold gold_2_1 = Gold.Create(new Vector3(0,0,0));			
			this.put(gold_2_1);			
		} else if (_name.Equals("grave3")) {						
			Gold gold_3_1 = Gold.Create(new Vector3(0,0,0));			
			this.put(gold_3_1);
		
			Gold gold_3_2 = Gold.Create(new Vector3(0,0,0));			
			this.put(gold_3_2);		
			
			Gold gold_3_3 = Gold.Create(new Vector3(0,0,0));			
			this.put(gold_3_3);		
		}
		
	}
	
	public override string toProlog(){						
		List<string> content = new List<string>();
		foreach(EObject eo in this.content) {
			content.Add(eo.toProlog());			
		}
		return base.toProlog() + String.Format("[[has, {0}]])", PrologList.AtomList<string>(content));		
	}	
					
}
