using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using Pathfinding.Nodes;
using Pathfinding;

[RequireComponent (typeof (RigidBodyController))]

public class Agent : Entity {
	
	public  int   lifeTotal = 50;
	private float _nodeSize = 2; 		// size of the node in the world measure
										// Hardcodeado. Traté de hacerlo andar dinámicamente, pero no pude.
										// Es el mismo número que el node size definido en el objeto A*, por IDE
	private float _delta = 0.1f;   		// time between ticks of "simulation"
	public  int   life;
	public  int   skill = 100;
	
	private RigidBodyController       _controller;	
	private List<PerceivableNode>     nodeList;							// TODO: Borrar. Es para test nomás
	public  List<EObject>             backpack = new List<EObject>();	
	public  List<EObject>             dropped  = new List<EObject>();
	public  Dictionary<string, float> actionDurations;
	public  AgentState                agentState;
	
	public  int   _depthOfSight;		// radius of vision, in nodes
	public  float _reach   = 1;			// radius of reach (of objects), in world magnitude
	private bool   toogle = false;
	//public  int   velocity = 5;			// TODO: revisar si esto va
	
	
	
	public  delegate       void ActionFinished(ActionResult result);
	private ActionFinished actionFinished = delegate(ActionResult x) { };
	
	public static Agent Create(	GameObject prefab, 
								Vector3 position, 
								IEngine se,
								string description, 
								string name, 
								int lifeTotal) 
	{
		GameObject gameObj    = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Agent      agent      = gameObj.GetComponent<Agent>();
		
		agent.lifeTotal 	  = lifeTotal;
		agent.life            = lifeTotal;
		agent._description    = description;
		agent._name           = name;
		agent._engine         = se;
		agent._delta	      = se._delta;
		agent._type		      = "agent";
		agent.actionDurations = new Dictionary<string, float>(se.actionDurations); // copio las duraciones de las acciones
		return agent;
	}
	
	public GUIStyle infoStyle;
	public GUIStyle nameStyle;	
	public GUIStyle lifeStyle;
	public GUIStyle lifeLevelStyle;
	
	void OnGUI () {
		Vector3 screenPos = Camera.mainCamera.WorldToScreenPoint(transform.position);
		//GUI.Label(new Rect(screenPos.x, screenPos.y, 10,5), _name);
		int infoHeight = 25;
		int infoWidth = 100;
		Rect infoRect = new Rect(screenPos.x - infoWidth/2, Screen.height - screenPos.y - infoHeight - 15, infoWidth, infoHeight);
		GUI.BeginGroup(infoRect, infoStyle);
		GUILayout.BeginVertical(infoStyle, GUILayout.Width(infoWidth));			
			GUILayout.Label(_name, nameStyle);
			/*Energy level*/
			GUILayout.BeginVertical(lifeStyle);
			int totalLifeWidth = 50;
			int lifeWidth = (int)(totalLifeWidth*((float)life/lifeTotal));
			GUILayout.Box("",GUILayout.Width(totalLifeWidth), GUILayout.Height(10));			
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRect.Set(lastRect.x, lastRect.y, lifeWidth, lastRect.width);			
			lifeLevelStyle = new GUIStyle(GUI.skin.box);
			lifeLevelStyle.normal.background = MakeTex(2, 2, Color.green);			
			GUI.Box(lastRect,"", lifeLevelStyle);			
			GUILayout.EndVertical();
		GUILayout.EndVertical();
		GUI.EndGroup();
	}

	public override void Start(){
		base.Start();		
		this._controller = this.GetComponent<RigidBodyController>();
		InvokeRepeating("execute", 0, _delta);
		rigidbody.sleepVelocity = 0f;
	}

	private Texture2D MakeTex( int width, int height, Color col ) {
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	public void setCallback(ActionFinished s){
		actionFinished = s;
	}
	
	// esto se ejecuta en cada ciclo de "simulación"
	void execute(){
		position = transform.position;
		nodeList = this.perceptNodes();
		checkInsideBuilding();
		// TEST
		if (_engine.test){
			if (!_controller.moving && nodeList.Count > 1){
				GridNode node = nodeList[UnityEngine.Random.Range(0, nodeList.Count)]._node;
				_controller.move((Vector3)node.position);	
				
			}
			List<Gold>  goldList = this.perceptObjects<Gold> ("gold");
			
			foreach(Gold gold in goldList){
				if (!dropped.Contains(gold))
					pickup(gold);
			}
			
			if (backpack.Count > 1){
				drop(backpack[0]);
				dropped.Add(backpack[0]);
			}
		}
		// TEST
		
	}		
	private void checkInsideBuilding(){
		SimulationState ss;		
		ss = (GameObject.FindGameObjectWithTag("GameController").
			GetComponent(typeof(SimulationEngineComponentScript))
			as SimulationEngineComponentScript).engine as SimulationState;				
		//ss.stdout.Send("Inns: "+ss.inns.Values.ToString());
		int currentNode = (AstarPath.active.GetNearest(this.transform.position).node as GridNode).GetIndex();				
		if (SimulationState.getInstance().nodeToInn.ContainsKey(currentNode)) {
			Inn inn = SimulationState.getInstance().nodeToInn[currentNode];
			inn.heal(this);
		}					
	}
	
	public void noopPosCon() {
		//Invoke("stoppedAction", actionDurations["noop"]);
	}
	
	public bool movePreConf(int node){
		Node actualNode = AstarPath.active.GetNearest(transform.position).node as GridNode;		
		return PerceivableNode.connections(actualNode as GridNode).Contains(node);
	}
	
	public void movePosCond(int node){
		float cost = _controller.move((Vector3)(AstarPath.active.graphs[0].nodes[node].position));
		subLife((int)(cost+0.5)); //0.5 de redondeo, dado que el cast a int trunca.	
	}
	
	// posiblemente innecesario
	public void moveToPosition(Vector3 target){
		_controller.move(target);	
	}
	
	public void stopActionAfter(float time) {
		Invoke("stoppedAction", time);
	}
	
	public void stoppedAction() {		
		if (!isConscious()) {
			//TODO: DIEEEEEEE
			dropEverything();			
			Invoke("wakeUp", 20);
		} else		
			actionFinished(ActionResult.success);
	}

	private void wakeUp() {
		life = (int)(lifeTotal * .2f);
		actionFinished(ActionResult.success);
	}

	public Boolean isConscious() {
		return life > 0;
	}

	public void setUnconscious() {
		life = 0;
		dropEverything();
	}

	public void actionFailed(){
		actionFinished(ActionResult.failure);
	}
	
	public void subLife(int dif) {
		if(this.life - dif <= 0)
			this.life = 0;				
		else
			this.life = life - dif;	
				
		updateEnergyLevel();		
	}
	
	private void updateEnergyLevel() {
		//SimulationEngineComponentScript.ss.stdout.Send("energybar "+transform.Find("energybar").Find("energyLevel").ToString());		
		Transform energyLevel = transform.Find("energybar").Find("energyLevel");
		Vector3 origScale = energyLevel.localScale;
		Vector3 origPos = energyLevel.localPosition;
		energyLevel.localScale = new Vector3((float)life/lifeTotal, origScale.y, origScale.z);
		energyLevel.localPosition = new Vector3(-((1f - energyLevel.localScale.x)/2f), origPos.y, origPos.z);
		//SimulationEngineComponentScript.ss.stdout.Send("energyLevel: "+energyLevel.localScale.x);
		//SimulationEngineComponentScript.ss.stdout.Send("energyPos: "+energyLevel.localPosition);		
		//transform.Find("energyLevel").
		
		
	}
	
	public void addLife(int dif) {
		if(this.life + dif >= this.lifeTotal){
			this.life = this.lifeTotal;
		} else {
			this.life += dif;	
		}
		updateEnergyLevel();
	}

	public void addSkill(int diff) {
		skill += diff;
	}
	
	public bool pickupPreCon(EObject obj){				
		return obj.isAtGround() && isReachableObject(obj);     //TODO: revisar si isReachableObject es necesario
	}
	
	public void pickupPosCon(EObject obj){
		obj.gameObject.SetActive(false);
		this.backpack.Add(obj);
	}
	
	//DEPRECATED
	public void pickup(EObject obj) {
		
		if (isReachableObject(obj)){
			obj.gameObject.SetActive(false);
			this.backpack.Add(obj);
		}
		else{
			// TODO: excepcion? devolver falso? guardarlo en algun lado?
		}
	}
	
	public bool dropPreCon(EObject obj){
		return backpack.Contains(obj);     
	}
	
	public void dropPosCon(EObject obj){
		Vector3 newPosition = this.transform.position;
		this.backpack.Remove(obj);
		obj.gameObject.SetActive(true);
		newPosition.y += 2.5f;
		obj.setPosition(newPosition);
		obj.rigidbody.AddForce(new Vector3(20,20,20)); //TODO: cambiar esta fruta
	}	
	
	public void dropEverything() {
		foreach (EObject obj in new List<EObject>(backpack))
			dropPosCon(obj);
	}
	
	public bool attackPreCon(Agent target) {
		if (target.Equals(this) || life <= 0 || target.life <= 0)
			return false;
		
		Vector3 distance = this.transform.position - this.position;
		return distance.magnitude < 11f;
	}
	
	public void attackPosCon(Agent target) {				
		int diceSides = 100; //TODO add as setting
				
		System.Random dice = new System.Random();		
		int plusAg = dice.Next(diceSides);
		int plusTargetAg = dice.Next(diceSides);		
		int attackPowerAg = skill + plusAg;
		int resistanceTargetAg = target.skill + plusTargetAg;
		//SimulationEngineComponentScript.ss.stdout.Send("attack : power = "+attackPowerAg+" resist = "+resistanceTargetAg +". ");
		if (attackPowerAg > resistanceTargetAg) {
			int harm = attackPowerAg - resistanceTargetAg;
			target.subLife(harm);
			this.skill = this.skill + 1;
			//SimulationEngineComponentScript.ss.stdout.Send("success. skill = "+ skill+". ");
		}
		Transform bubblegun = transform.Find("bubbleGun");
		bubblegun.LookAt(target.position);		
		bubblegun.particleSystem.Play();	
	}
	
	private void attackStop() {
		
	}
	
	public bool castSpellOpenPreCon(Grave grave, EObject potion){
		return grave != null && !grave.isOpen()
			&& potion != null && backpack.Contains(potion);
	}
	
	public void castSpellOpenPosCon(Grave grave, EObject potion){
		grave.setOpen(true);		
		backpack.Remove(potion);
		castSpellEffect();
	}

	public bool castSpellSleepPreCon(Agent target, EObject potion){
		SimulationState.getInstance().stdout.Send(target._name);
		SimulationState.getInstance().stdout.Send(potion.ToString());
		Vector3 distance = this.transform.position - this.position;
		return target != null && target.isConscious()
			&& potion != null && backpack.Contains(potion) && distance.magnitude < 11f;
	}
	
	public void castSpellSleepPosCon(Agent target, EObject potion){
		target.setUnconscious();		
		backpack.Remove(potion);
		castSpellEffect();
		target.spelledEffect();
	}

	public void castSpellEffect() {
		transform.light.enabled = true;
		transform.light.color = Color.white;
		Invoke("disableLight", 3);
	}

	public void spelledEffect() {
		transform.light.enabled = true;
		transform.light.color = Color.red;
		Invoke("disableLight", 3);
	}

	private void disableLight() {
		transform.light.enabled = false;
	}

	//DEPRECATED
	public void drop(EObject obj) {
		
		if (backpack.Contains(obj)){
			Vector3 newPosition = this.transform.position;
			this.backpack.Remove(obj);
			obj.gameObject.SetActive(true);
			newPosition.y += 2.5f;
			obj.transform.position = newPosition;
			if (!obj._type.Equals("potion"))
				obj.rigidbody.AddForce(new Vector3(20,20,20)); //TODO: cambiar esta fruta
		}
		else{
			//TODO: excepcion? devolver falso? guardarlo en algun lado?
		}
	}
	
	public void perceive(Percept p){						
		p.addEntitiesRange(perceptObjects<Agent>("agent").Cast<IPerceivableEntity>().ToList());
		p.addEntitiesRange(perceptObjects<Gold> ("gold") .Cast<IPerceivableEntity>().ToList());
		p.addEntitiesRange(perceptObjects<Inn>  ("inn")  .Cast<IPerceivableEntity>().ToList());
		p.addEntitiesRange(perceptObjects<Grave>  ("grave")  .Cast<IPerceivableEntity>().ToList());
		p.addEntitiesRange(perceptObjects<Potion>  ("potion")  .Cast<IPerceivableEntity>().ToList());
		p.addEntitiesRange(perceptNodes()				 .Cast<IPerceivableEntity>().ToList());
	}
	
	public override string toProlog(){
		string aux = base.toProlog();		
		List<string> auxList = new List<string>();
		foreach(EObject eo in this.backpack) {
			auxList.Add(eo.toProlog());			
		}
		aux = aux + String.Format("[[life, {0}], [lifeTotal, {1}], [skill, {2}], [lastAction, [{3}, {4}]], [has, {5}]])", 
			this.life, this.lifeTotal, this.skill, this.agentState.lastAction.toProlog(), this.agentState.lastActionTime,
			PrologList.AtomList<string>(auxList));		
		return aux;
	}
	
	public string selfProperties(bool inProlog = true){
		Building building = inBuilding();
		string   inside   = building != null ? building._name : "no";
		string   aux;
		if (inProlog){ //future implementations will have XML generations instead of prolog
			aux = string.Format("selfProperties({0}, {1}, {2}, {3}, {4}, {5})",
				this._name,
				this.agentState.lastActionResult,
//				"todo",
//				"todo",
				this.life,
				this.lifeTotal,
				new PrologList(this.backpack).toProlog(),
				inside);
		}
		else{
			throw new NotImplementedException();
		}
		return aux;
	}
	
	public Building inBuilding(){
		return null; //TODO: implementar
	}
		
	private List<ObjectType> perceptObjects<ObjectType>(string type, string layer = "perception") 
		where ObjectType : Component // cuánta magia
									 // acá restrinjo el tipo pasado por parámetro
		{
		Collider[] colliders = 
			Physics.OverlapSphere(this.transform.position, _depthOfSight * _nodeSize,
				1 << LayerMask.NameToLayer(layer)); // usa mascaras, con el << agregas con BITWISE-OR
		List<ObjectType> aux = new List<ObjectType>();
		
    	foreach (Collider hit in colliders) {    		
			if (hit.tag == type) {
				ObjectType hitObj = hit.gameObject.GetComponent<ObjectType>();
				GridNode hitNode = AstarPath.active.GetNearest(hitObj.transform.position).node as GridNode;
				if (isVisibleNode(hitNode) && hitNode.walkable)  //to avoid perceiving an object and not perceiving its associated node.
					aux.Add(hit.gameObject.GetComponent<ObjectType>());
			} 
    	}
		return aux;
	}
	
	private List<PerceivableNode> perceptNodes(){
		GridNode gridNode = AstarPath.active.GetNearest(transform.position).node as GridNode;
		List   <PerceivableNode> connections = new List   <PerceivableNode>();
		Queue  <BFNode>          q           = new Queue  <BFNode>         ();
		HashSet<Node>            visited     = new HashSet<Node>           ();
		//Breadth First Search
		BFNode t = new BFNode(0, gridNode);
		q.Enqueue(t);
		while (q.Count > 0){
			t = q.Dequeue();
			if (t.node._node.walkable) //Si el nodo seleccionado es transitable, entonces lo agrego al conjunto resultado.
				connections.Add(t.node);
			if (t.depth < _depthOfSight){ //si no está en el límite, agr
				foreach(Node node in t){ //Itero por vecinos conectados y no conectados del nodo en la grilla, y los agrego a la frontera
										 //siempre que esten en dentro de la distancia de vision.
					if (!(visited.Contains(node)) && //famoso if de reglón de ancho, warpeado
						isVisibleNode(node))
					{	
						visited.Add(node);
						q.Enqueue(new BFNode(t.depth + 1, node as GridNode));
					}
				}
			}
		}
		return connections;	
	}
	
	//check if the node is in a visible distance
	private bool isVisibleNode(Node node){
		
		return (new Int3(transform.position) -
				node.position).worldMagnitude < _depthOfSight * _nodeSize;
	}

	private bool isReachableObject(EObject obj){		
		return (obj.transform.position - transform.position).magnitude < _depthOfSight * _nodeSize;
	}		
}

//Breadth first nodes
class BFNode{
	public PerceivableNode node;
	public int             depth;
	
	public BFNode(int depth, PerceivableNode node){
		this.node  = node;
		this.depth = depth;
	}
	
	public BFNode(int depth, GridNode node){
		this.node  = new PerceivableNode(node);
		this.depth = depth;
	}

	// Devuelve un iterador para los vecinos del nodo. Incluye los vecinos del nodo en la grilla no conectados por arcos,
	// dado que nos va a interesar expandir la busqueda desde ellos para llegar a otros nodos efectivamente 
	// conectados al grafo principal.
	public IEnumerator<Node> GetEnumerator(){
		GridGraph graph            = AstarPath.active.astarData.gridGraph;
		Node[]    nodes            = graph.nodes;
		int []    neighbourOffsets = graph.neighbourOffsets;
		Node      aux;
		int       index;
		
		for (int i = 0; i < 8; i++){ //las 8 conexiones posibles de cada nodo
			index = node._node.GetIndex();
			
			if(node._node.GetConnection(i)){ //connected nodes
				aux = nodes[index + neighbourOffsets[i]];
				if (aux.walkable){	
					yield return aux;
				}
			} else { //disconnected nodes
				int neighIndex = index + neighbourOffsets[i];
				if (neighIndex >= 0 && neighIndex < nodes.Count()) {
					aux = nodes[neighIndex];
					yield return aux ;
				}
			}
		}
	}
}
