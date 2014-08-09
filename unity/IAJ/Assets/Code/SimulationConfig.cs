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

	public enum AgAttributes {HP, XP};
	public static Dictionary<ActionType, Dictionary<AgAttributes, float>> actionEffectsOnAttributes = new Dictionary<ActionType, Dictionary<AgAttributes, float>>();


    public SimulationConfig() {
		XmlDocument document = new XmlDocument();

		FileInfo externalConfigFile = new FileInfo(Application.dataPath+"/Resources/config.xml");
		if ( externalConfigFile != null && externalConfigFile.Exists ) {
			document.Load(externalConfigFile.OpenText());
			Debug.LogError("Config will be loaded from external file: "+externalConfigFile);
		} else {
			TextAsset internalConfigFile = (TextAsset)Resources.Load("config", typeof(TextAsset));
			document.Load(new StringReader(internalConfigFile.text));
		}
        try {
            simulation_duration = Convert.ToInt32(document.SelectSingleNode("/config/simulation_duration").InnerText);
            vision_length       = Convert.ToInt32(document.SelectSingleNode("/config/vision_length").InnerText);
            unconscious_time    = Convert.ToSingle(document.SelectSingleNode("/config/unconscious_time").InnerText);

            // Load action durations and save them in the action duration dictionary
            XmlNodeList xnl = document.SelectNodes("/config/actions/action");
            foreach (XmlNode actionXML in xnl) {
                string name           = actionXML.Attributes["name"].Value;
				ActionType action = (ActionType)Enum.Parse(typeof(ActionType), actionXML.Attributes["name"].Value, true);
                string duration       = actionXML.Attributes["duration"].Value;
                actionDurations[name] = Convert.ToSingle(duration);
				actionEffectsOnAttributes[action] = new Dictionary<AgAttributes, float>();
				XmlNodeList effectNodeList = actionXML.SelectNodes("effects/effect");
				foreach (XmlNode effect in effectNodeList) {
					AgAttributes effectAttr = (AgAttributes)Enum.Parse(typeof(AgAttributes), effect.Attributes["attr"].Value, true);
					float effectValue = Convert.ToSingle(effect.Attributes["value"].Value);
					actionEffectsOnAttributes[action].Add(effectAttr, effectValue);
				}
            }

            Debug.LogError("Config loaded.");
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
