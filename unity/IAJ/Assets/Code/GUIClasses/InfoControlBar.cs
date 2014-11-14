using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InfoControlBar : MonoBehaviour
{
	Vector2 scrollPosition;
	
	void OnGUI () {
		GUI.Window(0, new Rect(Screen.width - 190, 10, 180, 530), WindowFunction, "Game Control / Info");		
	}
	
	public GUIStyle timeLabelStyle;
	
	void WindowFunction(int windowId) {				
		GUILayout.BeginVertical();
			GUILayout.Label(SimulationState.getInstance().gameTime.ToString(), timeLabelStyle);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			foreach (AgentState agentState in SimulationState.getInstance().agents.Values) {
				AgentPanel(agentState.agentController);
			}
			GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}
		
	void AgentPanel(Agent agent) {		
		GUILayout.BeginVertical();	
			//GUILayout.Box(agent._name, GUILayout.Height(100f));			
		    GUILayout.Box(agent._name);			
			GUILayout.Label("HP: "+agent.life+"/"+agent.lifeTotal+"  "+"XP: "+agent.skill);
			GUILayout.BeginHorizontal();
			 GUILayout.Label("BP:");
			 Dictionary<Type, int> typeToCount = new Dictionary<Type, int>();
			 Dictionary<Type, Texture> typeToIcon = new Dictionary<Type, Texture>();			 			
			 foreach (EObject obj in agent.backpack) {
				if (typeToCount.ContainsKey(obj.GetType()))
					typeToCount[obj.GetType()] = typeToCount[obj.GetType()] + 1;
				else {
					typeToCount.Add(obj.GetType(), 1);
					typeToIcon.Add(obj.GetType(), obj.getIcon());
				}				
			 }
			foreach (Type type in typeToCount.Keys) {
			GUILayout.BeginHorizontal();
				GUIStyle iconStyle = new GUIStyle();
				iconStyle.margin = new RectOffset(0, 0, 5, 0);				
				GUILayout.Label(typeToIcon[type], iconStyle, GUILayout.Width (20), GUILayout.Height (15));
			    GUIStyle textStyle = new GUIStyle();				
				textStyle.margin = new RectOffset(0, 10, 5, 0);				
				textStyle.normal.textColor = Color.white;
			    GUILayout.Label(": " + typeToCount[type], textStyle);
			GUILayout.EndHorizontal();
			}
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
}

