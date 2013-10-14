using UnityEngine;
using System.Collections;

public class InfoControlBar : MonoBehaviour
{
	
	void OnGUI () {
		GUI.Window(0, new Rect(Screen.width - 170, 20, 160, 300), WindowFunction, "Game Control / Info");		
	}
	
	public GUIStyle timeLabelStyle;
	
	void WindowFunction(int windowId) {				
		GUILayout.BeginVertical();
			GUILayout.Label(SimulationState.getInstance().gameTime.ToString(), timeLabelStyle);
			foreach (AgentState agentState in SimulationState.getInstance().agents.Values) {
				AgentPanel(agentState.agentController);
			}
		GUILayout.EndVertical();
	}
	
	public Texture2D goldIcon;
		
	void AgentPanel(Agent agent) {		
		GUILayout.BeginVertical();	
			//GUILayout.Box(agent._name, GUILayout.Height(100f));			
		    GUILayout.Box(agent._name);			
			GUILayout.Label("skill: "+agent.skill);
			GUILayout.BeginHorizontal();
			 GUILayout.Label("backpack:");				
			 foreach (EObject obj in agent.backpack) {
		 		GUILayout.Label(goldIcon);
			 }
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
}

