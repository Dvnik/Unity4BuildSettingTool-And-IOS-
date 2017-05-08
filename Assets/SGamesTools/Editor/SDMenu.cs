#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class SDMenu 
{
	[MenuItem("SuperGame/Tools/Define/Create(建置)")]
	private static void SDCreate()
	{
		EditorWindow.GetWindow(typeof(SDCreate));
	}

	[MenuItem("SuperGame/Tools/Define/Revise(修改)")]
	private static void SDRevise()
	{
		EditorWindow.GetWindow(typeof(SDRevise));
	}

	[MenuItem("SuperGame/Tools/Define/Override(更新)")]
	private static void SDOverride()
	{
		EditorWindow.GetWindow(typeof(SDOverride));
	}


}
#endif