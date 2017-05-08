#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * 自動產檔相關選單
 * 自動產檔就是整合其他功能
 * 省去介面選單設定
 * 然後讓程式碼去跑整個檔案產生流程
 */
public class SAMenu
{
	#region Base Function
	[MenuItem("SuperGame/Tools/AutoBuild/刷新檔案移動紀錄列表")]
	private static void SASFGetCustomUpdate()
	{
		SFileIOModfied.ReWriteSFGetCustomInfo();
	}
	[MenuItem("SuperGame/Tools/AutoBuild/刷XCode改寫設定紀錄列表")]
	private static void SASXGetCustomUpdate()
	{
		SFileIOModfied.ReWriteSXGetCustomInfo();
	}
	[MenuItem("SuperGame/Tools/AutoBuild/自動產檔視窗")]
	private static void SABuildWindow()
	{
		EditorWindow.GetWindow(typeof(SAWindow));
	}
	#endregion
	#region ExtraMenu Demo
	// Android
	[MenuItem("SuperGame/QuickBuild/SGQC/Android/DemoOne")]
	public static void DEBUGTWAndroidBuild()
	{
		BuildProcess(SAConst.cQC + SAConst.cDEBUG, "STQCDEBUG_Android", BuildTarget.Android, new SFDemoSet());
	}
	// IOS
	[MenuItem("SuperGame/QuickBuild/SGQC/IOS/DemoTwo")]
	public static void DEBUGTWIOSBuild()
	{
		BuildProcess(SAConst.cXcodeProject + SAConst.cDEBUG, "STQCDEBUG_IOS", BuildTarget.iPhone, new SFDemoSet());
	}
	#endregion
	/// <summary>
	/// 產黨流程
	/// </summary>
	/// <param name="iBuildFileName">檔案產出的檔案名稱</param>
	/// <param name="iDefineSetName">DefineSetting功能的設定檔名稱</param>
	/// <param name="iTarget">是由哪個版本輸出</param>
	/// <param name="iFileChange">檔案搬移方法</param>
	private static void BuildProcess(string iBuildFileName, string iDefineSetName,
	                                 BuildTarget iTarget, SFChangeBase iFileChange)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(iTarget); // 切換Platform
		SDOverride.OverridePlayerSet(iDefineSetName);// 設定Define檔
		iFileChange.DoFileMove();// 搬移檔案
		// 執行產檔
		if( iTarget == BuildTarget.Android )
			SBuildFile.BuildActionAndroid(iBuildFileName);
		else if(iTarget == BuildTarget.iPhone)
			SBuildFile.BuildActionIOS(iBuildFileName);
	}
}
#endif