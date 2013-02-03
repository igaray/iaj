using System;
using System.Collections.Generic;
using Pathfinding.Nodes;
using Pathfinding;
using System.Linq;


// to be deprecated
public class PerceptElement{
	
	public int   		  elementType; // 0: entity, 1: list of entities, 2: list of nodes
	public Entity 		  entity;
//	public List<Entity>   entities;
//	public List<GridNode> nodes;
	public List<IPerceivableEntity> list;
	
	public PerceptElement(Entity entity){
		
		elementType = 0;
		this.entity = entity;
	}
	
	public PerceptElement (){
		
	}
	
	public PerceptElement addEntities(List<IPerceivableEntity> l){
		elementType = 1;
		list        = l;
		return this;
	}
	
//	public PerceptElement addNodes(List<PerceivableNode> l){
//		elementType = 2;
//		nodes = l;
//		return this;
//	}
	
//	public PerceptElement(List<Entity> entities){
//		
//		elementType   = 1;
//		this.entities = entities;
//	}
//	
//	public PerceptElement(List<GridNode> nodes){
//		
//		elementType = 2;
//		this.nodes  = nodes;
//	}
	
}
