//-----------------------------------------------------------------
//  Copyright 2009 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using UnityEditor;


public class SM2_Ver : ScriptableObject
{
	public static string version = "1.83";


	[MenuItem("Tools/A&B Software/About SpriteManager 2")]
	static void ShowVersion()
	{
		EditorUtility.DisplayDialog("SpriteManager 2 v" + version, "SpriteManager 2 Version " + version + "\n\nCopyright 2009 Above and Beyond Software\nAll rights reserved.", "OK");
	}
}
