using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class SimulationConfig {
    // these are the default values
    public int simulation_duration  = 10;
    public int vision_length        = 2;
    public float unconscious_time   = 1f;
    public Dictionary<string, float> actionDurations = new Dictionary<string, float>();

    public SimulationConfig(string configPath) {
        try {
            XmlDocument document = new XmlDocument();
            document.Load(new StreamReader(configPath));

            simulation_duration = Convert.ToInt32(document.SelectSingleNode("/config/simulation_duration").InnerText);
            vision_length       = Convert.ToInt32(document.SelectSingleNode("/config/vision_length").InnerText);
            unconscious_time    = Convert.ToSingle(document.SelectSingleNode("/config/unconscious_time").InnerText);

            // Load action durations and save them in the action duration dictionary
            XmlNodeList xnl = document.SelectNodes("/config/actions/action");
            foreach (XmlNode action in xnl) {
                string name           = action.Attributes["name"].Value;
                string duration       = action.Attributes["duration"].Value;
                actionDurations[name] = Convert.ToSingle(duration); 
            }

            Debug.LogError("Loaded the configuration file.");
        }
        catch (Exception) {
            Debug.LogError("No config file found.");
            actionDurations["noop"]       = 1f;
            actionDurations["move"]       = 1f;
            actionDurations["pickup"]     = 1f;
            actionDurations["drop"]       = 1f;
            actionDurations["attack"]     = 1f;
            actionDurations["cast_spell"] = 1f;			
        }
    }
}
