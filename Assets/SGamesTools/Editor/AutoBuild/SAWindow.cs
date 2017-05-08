#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * 手動設定自動產檔的詢問視窗
 * 自動產檔是「設定>搬移檔案>產出」的自動化，呼叫可以由外部方法執行
 * 或是由開啟視窗設定執行
 */
public class SAWindow : EditorWindow
{
	private enum eBuildTarget
	{
		Android,
		IOS,
	}
	private List<SFChangeBase> mChangeInfoList;// 檔案移動設定檔
	private string[] mChangeInfoPop;// 檔案移動顯示列表
	private string[] mDefineShowPop;// PlayerSetting顯示列表

	private List<SXWriteBase> mXCodeWriteList;// XCode設定檔
	private string[] mXCodeWritePop;// XCode設定顯示列表
	
	private bool mInitStatus;// 初始化狀態
	// 是否顯示設定列表
	private bool mChangeInfoNeedSet;
	private bool mDefineNeedSet;
	private bool mXCodeNeedSet;
	// UI列表Index
	private int mChangeInfoIndex = 0;
	private int mDefineIndex = 0;
	private int mXcodeWriteIndex = 0;
	// 
	private string mSaveFileName;// 檔案名稱
	private eBuildTarget mBuildTarget;// 產檔目標

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

		mChangeInfoList = SFGetCustom.GetBaseTypeList();
		mDefineShowPop = SDDataMove.GetSaveDataNames();
		mXCodeWriteList = SXGetCustom.GetBaseTypeList();

		mInitStatus = true;
	}
	/// <summary>
	/// UI顯示
	/// </summary>
	private void ShowUI()
	{
		CheckListStatus();

		mSaveFileName = EditorGUILayout.TextField("Save File Name:", mSaveFileName);
		GUILayout.Label("");
		// 
		mBuildTarget = (eBuildTarget)EditorGUILayout.EnumPopup("BuildTarget:", mBuildTarget);
		// Define
		mDefineNeedSet = EditorGUILayout.Toggle("Need Set Define ?", mDefineNeedSet);
		if(mDefineNeedSet)
			mDefineIndex = EditorGUILayout.Popup("DefineSet List", mDefineIndex, mDefineShowPop);
		// ChangeInfo
		mChangeInfoNeedSet = EditorGUILayout.Toggle("Need Set Change Info ?", mChangeInfoNeedSet);
		if(mChangeInfoNeedSet)
			mChangeInfoIndex = EditorGUILayout.Popup("CheangeInfo List", mChangeInfoIndex, mChangeInfoPop);
		// XCode Write
		mXCodeNeedSet = EditorGUILayout.Toggle("XCode Need Setting ?", mXCodeNeedSet);
		if(mXCodeNeedSet)
			mXcodeWriteIndex = EditorGUILayout.Popup("XCode Write List", mXcodeWriteIndex, mXCodeWritePop);
		GUILayout.Label("");
		GUILayout.Label("");
		if(GUILayout.Button("依設定檔產出檔案"))
			DoBuildFile();
		if (GUILayout.Button ("取消"))
			Close ();
	}
	/// <summary>
	/// 執行產檔
	/// </summary>
	private void DoBuildFile()
	{
		if(string.IsNullOrEmpty(mSaveFileName))
		{
			EditorUtility.DisplayDialog("警告", "需要檔案名稱", "確定");
			return;
		}

		SetDefineFile();
		SetChangeInfoFile();
		SetXCodeSettingFile();

		switch(mBuildTarget)
		{
		case eBuildTarget.Android: SBuildFile.BuildActionAndroid(mSaveFileName); break;
		case eBuildTarget.IOS: SBuildFile.BuildActionIOS(mSaveFileName); break;
		}
		Close();
	}
	/// <summary>
	/// 依照DefineSetting檔設定當前PlayerSetting環境
	/// </summary>
	private void SetDefineFile()
	{
		if(!mDefineNeedSet)
			return;

		if(mDefineShowPop.Length > 0)
			SDOverride.OverridePlayerSet(mDefineShowPop[mDefineIndex]);
	}
	/// <summary>
	/// 依照FileMove檔案設定執行檔案搬移
	/// </summary>
	private void SetChangeInfoFile()
	{
		if(!mChangeInfoNeedSet)
			return;

		if(mChangeInfoPop.Length > 0)
			mChangeInfoList[mChangeInfoIndex].DoFileMove();
	}
	/// <summary>
	/// (IOS)依照XCodeWrite設定在XCode專案產出後的XCode專案設定
	/// </summary>
	private void SetXCodeSettingFile()
	{
		if(!mXCodeNeedSet)
			return;

		if(mXCodeWritePop.Length > 0)
			SXMenu.XCodeWriteBase = mXCodeWriteList[mXcodeWriteIndex];
	}
	/// <summary>
	/// 確認列表狀態
	/// </summary>
	private void CheckListStatus()
	{
		if(mChangeInfoList == null)
			mChangeInfoList = new List<SFChangeBase>();

		if(mChangeInfoPop == null)
		{
			mChangeInfoPop = new string[mChangeInfoList.Count];
			for(int i = 0; i < mChangeInfoPop.Length; i++)
				mChangeInfoPop[i] = mChangeInfoList[i].ToString();
		}

		if(mDefineShowPop == null)
			mDefineShowPop = new string[0];

		if(mXCodeWritePop == null)
		{
			mXCodeWritePop = new string[mXCodeWriteList.Count];
			for(int i = 0; i < mXCodeWritePop.Length; i++)
				mXCodeWritePop[i] = mXCodeWriteList[i].ToString();
		}
	}
}
#endif