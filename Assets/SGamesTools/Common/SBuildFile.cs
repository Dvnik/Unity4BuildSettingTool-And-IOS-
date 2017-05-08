#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * 控管執行UnityBuild指令的地方
 */
public class SBuildFile
{
	public const string cDotApk = ".apk";
	public const string cBaseFileName = "CI_SG";// 預設檔案名稱, CI=持續整合(Continuous integration), SG=SuperGame
	/// <summary>
	/// 執行產檔的動作
	/// </summary>
	public static void BuildAction(BuildTarget iTarget, string iExportName, BuildOptions iOptions = BuildOptions.None)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(iTarget); // 切換Platform
		// Get Scenes
		string[] aLevels = GetBuildScenes();
		// Start Build
		BuildPipeline.BuildPlayer(aLevels,// 場景
		                          iExportName, // 檔案產出名稱
		                          iTarget, // Build Platform
		                          iOptions); // Options

	}
	/// <summary>
	/// IOS包檔方法
	/// </summary>
	/// <param name="iApkFileName">自定義檔名</param>
	public static void BuildActionIOS(string iXcodeFolderName)
	{
		BuildAction(BuildTarget.iPhone, iXcodeFolderName);
	}
	/// <summary>
	/// Android包檔方法
	/// </summary>
	/// <param name="iApkFileName">自定義檔名</param>
	public static void BuildActionAndroid(string iVerName)
	{
		string aApkFileName = GetApkFileName(iVerName);
		BuildAction(BuildTarget.Android, aApkFileName);
	}
	/// <summary>
	/// 換算Android檔案名稱
	/// </summary>
	/// <returns>The super TS file name.</returns>
	/// <param name="iVerStr">I ver string.</param>
	public static string GetApkFileName(string iVerStr)
	{
		string aFileName = cBaseFileName +
			DateTime.Now.ToString ("yyyyMMddHHmmss") + "_" +
				iVerStr + cDotApk;
		
		return aFileName;
	}
	/// <summary>
	/// 此方法可回傳所有Build Setting 引用到的場景
	/// 可在做檢驗自動產檔的時候有沒有確實引用到場景的參考依據
	/// </summary>
	/// <returns></returns>
	private static string[] GetBuildScenes()
	{
		List<string> names = new List<string>();
		
		foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
		{
			if (e == null)
				continue;
			if (e.enabled)
				names.Add(e.path);
		}
		string aDLog = "This BuildScenes have : \n";
		foreach (string s in names)
			aDLog += s + "\n";
		
		Debug.Log(aDLog);
		return names.ToArray();
	}
}
#endif