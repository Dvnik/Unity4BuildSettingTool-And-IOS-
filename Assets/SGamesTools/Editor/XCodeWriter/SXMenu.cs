#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * XCode視窗選單
 */
public class SXMenu
{

	private static SXWriteBase mXCodeWriteBase;
	public static SXWriteBase XCodeWriteBase
	{
		get{return mXCodeWriteBase;}
		set
		{
			mXCodeWriteBase = value;
		}
	}

	[PostProcessBuild(0)]
	public static void OnPostProcessBuild(BuildTarget iTarget, string pathToBuiltProject)
	{
		if(iTarget == BuildTarget.iPhone)
		{
			if(XCodeWriteBase != null)
			{
				XCodeWriteBase.SetXCodeFile(pathToBuiltProject);
				XCodeWriteBase = null;
			}
		}
	}

	[MenuItem("SuperGame/Tools/XCodeWriter/改寫PBXProj設定")]
	private static void SABuildWindow()
	{
		EditorWindow.GetWindow(typeof(SXWindow));
	}

}
#endif