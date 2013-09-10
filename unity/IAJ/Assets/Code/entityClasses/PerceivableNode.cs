using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Nodes;
using UnityEngine;

public class PerceivableNode : IPerceivableEntity
{
	public GridNode _node;
	
	public static Hashtable nodeToAdyacencyList = new Hashtable();
	
	public PerceivableNode (GridNode node)
	{
		this._node = node;		
	}
	
	public string toProlog(){
					
		List<string> neighbors = (List<string>)nodeToAdyacencyList[this._node];
		if (neighbors == null) {
			neighbors = weightedConnections();
			nodeToAdyacencyList.Add(this._node, neighbors);
		}						
		return String.Format("node({0}, {1}, {2})", 
			this._node.GetIndex(), 
			Entity.Vector3ToProlog((Vector3)this._node.position),
			PrologList.AtomList<string>(neighbors));
	}
	
	public List<int> connections(){
		Node[] nodes = this._node.connections;
		List<int> indexes = new List<int>();
		foreach (Node node in this){
			indexes.Add(node.GetNodeIndex());
		}
		return indexes;
	}
	
	public List<string> weightedConnections(){
		Node[] nodes = this._node.connections;			
		List<string> result = new List<string>();
		foreach (Node node in this){
			result.Add("["+node.GetNodeIndex()+","+RigidBodyController.connectionCost((Vector3)this._node.position, (Vector3)node.position)+"]");
		}
		return result;
	}
	
	public static List<int> connections(GridNode node){
		Node[] nodes = node.connections;
		List<int> indexes = new List<int>();
		foreach (Node n in new PerceivableNode(node)){
			indexes.Add(n.GetNodeIndex());
		}
		return indexes;
	}
	
	// devuelve un iterador para los vecinos del nodo
	public IEnumerator<Node> GetEnumerator(){
		GridGraph graph            = AstarPath.active.astarData.gridGraph;
		Node[]    nodes            = graph.nodes;
		int []    neighbourOffsets = graph.neighbourOffsets;
		Node      aux;
		int       index;
		
		for (int i = 0; i < 8; i++){ //las 8 conexiones posibles de cada nodo
			index = _node.GetIndex();

			if(_node.GetConnection(i)){
				aux = nodes[index + neighbourOffsets[i]];
				if (aux.walkable){	
					yield return aux;
				}
			}
		}
	}
}

