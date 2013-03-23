using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// SIMULATION ENGINE CORE

// Runs within Unity3D
public class SimulationEngine {

    public int               currentAgentID = 1;
    public int               currentRespawn = 0;
    public ConnectionHandler connectionHandler;
    public SimulationState   simulationState;

    public SimulationEngine(SimulationState ss) {
        simulationState   = ss;

        connectionHandler = new ConnectionHandler(ss);
    }

    /* Don't get confused by the fact that SimulationEngine has start()
     * and stop() methods, it does not run its own thread. 
     * It just encapsulates all the other objects and threads that 
     * make up the simulation engine. 
     * Correspondingly, it has no run() method.
     */
    public void start() {
        // Tell all the threads to start.
        connectionHandler.start();
    }

    public void stop() {
        // Tell all the threads to stop.
        connectionHandler.stop();
    }

    public void generatePercepts() {

        MailBox<PerceptRequest> requests = simulationState.perceptRequests;

        PerceptRequest request;
        Percept        percept;

        // aca hay que sacar todos los requests de percepciones de la cola 
        // y para cada uno, generar la percepcion correspondiente
        while (requests.NotEmpty()) {
            if (requests.NBRecv(out request)) {
                percept = new Percept(simulationState, request.agentID);
                request.agentPerceptMailbox.Send(percept);
            } 
        } 
    }

    public void handleActions() {

        Action                      currentAction;
        MailBox<Action>             raq    = simulationState.readyActionQueue;
        Dictionary<int, AgentState> agents = simulationState.agents;

        while (raq.NotEmpty()) {
            // get action from the ready action queue
            // if the action is executable,
            //     put it in unity's action queue
            //     let the agent know that its action was executable
            // else
            //     let the agent know that its action was not executable
            simulationState.stdout.Send(String.Format("AH: there are actions to process...\n"));
            if (raq.NBRecv(out currentAction)) {
                int agentID = currentAction.agentID;
                try {
					agents[agentID].lastAction = currentAction;
                    if (simulationState.executableAction(currentAction)) {
                        simulationState.stdout.Send(String.Format("AH: the action is executable.\n"));
                        //agents[agentID].results.Send(ActionResult.success);
                        agents[agentID].lastActionResult = ActionResult.success;
                        simulationState.applyActionEffects(currentAction);
                    }
                    else {
                        simulationState.stdout.Send(String.Format("AH: the action is not executable.\n"));
                        agents[agentID].results.Send(ActionResult.failure);
                        agents[agentID].lastActionResult = ActionResult.failure;
                    }
                }
                catch (System.Collections.Generic.KeyNotFoundException e) {
                    simulationState.stdout.Send(String.Format("AH: Error: agent id {0} not present in agent database.\n", agentID));
					Debug.LogError(e.ToString());
                }
            }
        }
    }

    public void instantiateAgents(GameObject prefab) {
		
		InstantiateRequest          request;
		int                         agentID;
		string                      name;
		AgentState                  state;	
        Vector3                     spawnPosition;	
		
        /*
        Instantiating an agent in the simulation consists of associating the 
        AgentConnection with an AgentID and an AgentState.
        If the agent has not been previously instantiated, a new AgentState must 
        be created, including an AgentController which will represent it graphically.
        If the agent has already been instantiated, then its old AgentID and AgentState
        must be recovered from the SimulationState, and associated to the new AgentConnection.
        */

        /* Esto lo que hace es elegir una posicion donde va a spawnear el agente.
           Cada vez que un agente se instancia (por primera vez), se incrementa en uno 
           modulo la cantidad de sitios de instanciacion. 
        */
		while (simulationState.instantiateRequests.NotEmpty()) {
			if (simulationState.instantiateRequests.NBRecv(out request)) {
				name = request.agentConnection.name;

                spawnPosition = GameObject.FindGameObjectsWithTag("Respawn")[currentRespawn].transform.position;
                currentRespawn = (currentRespawn + 1) % GameObject.FindGameObjectsWithTag("Respawn").Length;

				if (simulationState.agentIDs.TryGetValue(name, out agentID)) {
					// esta
                    request.agentConnection.agentState = simulationState.agents[agentID];
                }
                else {
                    // no esta
                    agentID = currentAgentID;
                    state   = new AgentState(simulationState, name, agentID, spawnPosition, prefab);
                    request.agentConnection.agentState = state;
                    
                    simulationState.agentIDs.Add(name, agentID);
                    simulationState.agents.Add(agentID, state); 
                    
                    currentAgentID++;
                }
                connectionHandler.instantiationResults.Send(true);
			}
		} 
    }
}
