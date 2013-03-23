using UnityEngine;
using System;
using System.Collections.Generic;

public class PerceptRequest {

    public int agentID;
    public MailBox<Percept> agentPerceptMailbox;

    public PerceptRequest(int AID, MailBox<Percept> APM) {
        // Aca va informacion que usa Unity para generar la percepcion
        // principalmente, para que agente se esta generando
        // con esto, unity identifica el gameobject correspondiente 
        // al agente, y una vez obtenido eso, corre los spherecast, etc
        // ademas, tiene una referencia al mailbox de percepciones del agente
        // en la cual unity va a insertar la percepcion generada
        this.agentID             = AID;
        this.agentPerceptMailbox = APM;
    }
}

