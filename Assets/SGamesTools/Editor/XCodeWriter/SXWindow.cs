#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using System.IO;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * XCode設定視窗選單
 */
public class SXWindow : EditorWindow
{
	private enum eFileStatus
	{
		Save,
		Load,
	}

	private const string cSaveAssetsPath = "SGamesTools/Editor/XCodeWriter";
	private const string cSavePBXTxtName = "PBXSaveWords";

	private string mXCodeProjctPath;
	private eFileStatus mDoStatus;

	private bool mInitStatus;// 初始化狀態

	#region Unity Base
	private void OnGUI()
	{
		SettingInit();
		ShowUI ();
	}
	#endregion
	/// <summary>
	/// 初始化
	/// </summary>
	private void SettingInit()
	{
		if(mInitStatus)
			return;
		
		
		mInitStatus = true;
	}
	/// <summary>
	/// UI顯示
	/// </summary>
	private void ShowUI()
	{
		mXCodeProjctPath = EditorGUILayout.TextField("Xcode Project Path:", mXCodeProjctPath);
		GUILayout.Label("");
		mDoStatus = (eFileStatus)EditorGUILayout.EnumPopup("PBXData Doing:", mDoStatus);

		GUILayout.Label("");
		GUILayout.Label("");
		if(GUILayout.Button("PBX設定檔存取"))
			DoChangePBX();
		if (GUILayout.Button ("取消"))
			Close ();
	}
	/*
	 * 有些XCodeProject的設定
		無法透過XCodeEditor的函式設定
		這個方法是把xcodeproj檔底下的"project.pbxproj"以文字形式打開
		全部儲存到一個文字文件
		等要讀取的時候
		再把檔案寫回去
		但是有些需要的額外檔案會在複寫完成之後可能會缺少
		要再這動作之後加回去
		才不會無法產檔
	 */
	#region PBX Writer
	/// <summary>
	/// 執行PBX設定檔存取
	/// </summary>
	private void DoChangePBX()
	{
		switch(mDoStatus)
		{
		case eFileStatus.Save: SavePBXData(mXCodeProjctPath); break;
		case eFileStatus.Load: LoadPBXData(mXCodeProjctPath); break;
		}

		Close();
	}
	/// <summary>
	/// 儲存PBX設定檔
	/// </summary>
	private static void SavePBXData(string iProjectPath)
	{
		string aPbxPath = string.Format("{0}/{1}", iProjectPath, "Unity-iPhone.xcodeproj/project.pbxproj");
		FileInfo aFI = new FileInfo(aPbxPath);
		if(!aFI.Exists)
		{
			Debug.Log("File Not Found, Path :" + aPbxPath);
			return;
		}

		PBXProject aPBXProject = new PBXProject();
		aPBXProject.ReadFromFile(aPbxPath);

		string aPBXSavePath = string.Format ("{0}/{1}", Application.dataPath, cSaveAssetsPath);
		SFileIOModfied.SaveNewTxtFile(aPBXSavePath , cSavePBXTxtName, aPBXProject.WriteToString());
		
	}
	/// <summary>
	/// 讀取PBX設定檔
	/// </summary>
	public static void LoadPBXData(string iProjectPath)
	{
		string aPBXSavePath = string.Format ("{0}/{1}", Application.dataPath, cSaveAssetsPath);
		string aFullPath = string.Format("{0}/{1}", aPBXSavePath, cSavePBXTxtName + ".txt");
		
		FileInfo aFI = new FileInfo(aFullPath);
		if(!aFI.Exists)
		{
			Debug.Log("File Not Found, Path :" + aFullPath);
			return;
		}
		// Read Setting
		FileStream aFS = new FileStream(aFullPath, FileMode.Open, FileAccess.Read);
		StreamReader aSR = new StreamReader(aFS, System.Text.Encoding.GetEncoding("UTF-8"));
		string aAllWord = aSR.ReadToEnd();
		// Override Setting
		string aPbxPath = string.Format("{0}/{1}", iProjectPath, "Unity-iPhone.xcodeproj/project.pbxproj");
		aFI = new FileInfo(aPbxPath);
		if(!aFI.Exists)
		{
			Debug.Log("File Not Found, Path :" + aFullPath);
			return;
		}

		File.WriteAllText(aPbxPath, aAllWord);
	}
	#endregion
}
#endif