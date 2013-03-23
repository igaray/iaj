using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Nodes;
using UnityEngine;

// The perceivable node class is a shell for grid nodes
public class PerceivableNode : IPerceivableEntity
{
	public GridNode _node;
	
	public PerceivableNode (GridNode node)
	{
		this._node = node;
	}
	
	public string toProlog(){		
		List<int> neighbors = connections();
		return String.Format("node({0}, {1}, {2})", 
			this._node.GetIndex(), 
			Entity.Vector3ToProlog((Vector3)this._node.position),
			PrologList.AtomList<int>(neighbors));
	}
	
	public List<int> connections(){
		Node[] nodes = this._node.connections;
		List<int> indexes = new List<int>();
		foreach (Node node in this){
			indexes.Add(node.GetNodeIndex());
		}
		return indexes;
	}
	
	// this does the same as the previous method, but without the need of creating a perceivable node object
	public static List<int> connections(GridNode node){
		Node[] nodes = node.connections;
		List<int> indexes = new List<int>();
		foreach (Node n in new PerceivableNode(node)){
			indexes.Add(n.GetNodeIndex());
		}
		return indexes;
	}
	
	// devuelve un iterador para los vecinos del nodo, para ser usado en un foreach
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

