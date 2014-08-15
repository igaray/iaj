using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.Collections;

public class PostBuildScript : MonoBehaviour {

	[MenuItem("MyTools/Windows Build With Postprocess")]
	public static void BuildGame ()
	{
		// Copy a file from the project folder to the build folder, alongside the built game.
		//FileUtil.CopyFileOrDirectory("C:\\Mauro\\Docencia\\IA\\GameUnity\\iaj\\unity\\IAJ\\Assets\\Code\\config.xml", Application.dataPath + "Resources\\config.xml");			
		//FileUtil.CopyFileOrDirectory("C:\\Mauro\\Docencia\\IA\\GameUnity\\iaj\\unity\\IAJ\\Assets\\Code\\config.xml", "C:\\Mauro\\Docencia\\IA\\GameUnity\\iaj\\unity\\IAJ\\Assets\\Code\\temp\\config.xml");			
	}
}

