#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * 寫檔內容設定基本
要產生一個客製化設定內容
應該是要繼承這個再改寫要修改的內容
 */
public abstract class SXWriteBase
{
	protected const string cOutputProj = "Unity-iPhone";// 產出後的XCodeProject名稱
	protected const string cInfoPlist = "Info.plist";// XCodePlist檔案
	protected const string cPbxProjPath = "Unity-iPhone.xcodeproj/project.pbxproj";// PBXProj的位置

	protected static string mXCodeProjectPath;// 產出XCodeProject檔案路徑
	protected static string mInfoPlistFullPath;// InfoPlist檔案路徑
	protected static string mPBXProjFullPath;// PBXProj的檔案路徑

	protected abstract void DoChangeXSetting();// 虛擬化>>執行客製化設定的函式
	/// <summary>
	/// Sets the build projet path.
	/// </summary>
	/// <param name="iOutputPath">I output path.</param>
	public void SetXCodeFile(string iOutputPath)
	{
		// Set Path
		mXCodeProjectPath = iOutputPath;
		mInfoPlistFullPath = string.Format("{0}/{1}", mXCodeProjectPath, cInfoPlist);
		mPBXProjFullPath = string.Format("{0}/{1}", mXCodeProjectPath, cPbxProjPath);
		// Do Change Data
		BaseChageInfo();
		DoChangeXSetting();
	}
	/// <summary>
	/// 基礎設定
	/// </summary>
	protected virtual void BaseChageInfo()
	{
		AddGameKitToPlist();
		AddDescriptionToPlist();
		SetRequireFullScreen();
		AddTransPortSSToPlist();
		SetDebugInformationToDWRF();
	}
	/// <summary>
	/// 設定GameKit到Plist上
	/// </summary>
	protected void AddGameKitToPlist()
	{
		Debug.Log("Start AddGameKitToPlist");
		PlistDocument aPDoc = GetProjectPlist();
		PlistElementDict aRootDict = aPDoc.root;

		PlistElementArray aDeviceCapArray = aRootDict.CreateArray("UIRequiredDeviceCapabilities");
		aDeviceCapArray.AddString("armv7");
		aDeviceCapArray.AddString("gamekit");

		File.WriteAllText(mInfoPlistFullPath, aPDoc.WriteToString());
	}
	/// <summary>
	/// 設定相機相簿訪問到Plist上
	/// </summary>
	protected void AddDescriptionToPlist()
	{
		Debug.Log("Start AddDescriptionToPlist");
		PlistDocument aPDoc = GetProjectPlist();
		PlistElementDict aRootDict = aPDoc.root;

		aRootDict.SetString("NSCameraUsageDescription", "相機訪問");
		aRootDict.SetString("NSPhotoLibraryUsageDescription", "相冊訪問");
		
		File.WriteAllText(mInfoPlistFullPath, aPDoc.WriteToString());
	}
	/// <summary>
	/// 加入Plist設定:NSAppTransportSecurity
	/// </summary>
	protected void AddTransPortSSToPlist()
	{
		Debug.Log("Start AddTransPortSSToPlist");
		PlistDocument aPDoc = GetProjectPlist();
		PlistElementDict aRootDict = aPDoc.root;

		PlistElementDict aTransPort = aRootDict.CreateDict("NSAppTransportSecurity");
		aTransPort.SetBoolean("NSAllowsArbitraryLoads", true);

		File.WriteAllText(mInfoPlistFullPath, aPDoc.WriteToString());
	}
	/// <summary>
	/// 設定UIRequiresFullScreen
	/// </summary>
	protected void SetRequireFullScreen()
	{
		Debug.Log("Start SetRequireFullScreen");
		PlistDocument aPDoc = GetProjectPlist();
		PlistElementDict aRootDict = aPDoc.root;

		aRootDict.SetBoolean("UIRequiresFullScreen", true);

		File.WriteAllText(mInfoPlistFullPath, aPDoc.WriteToString());
	}
	/// <summary>
	/// 設定DEBUG模式全為DWARF
	/// </summary>
	protected void SetDebugInformationToDWRF()
	{
		Debug.Log("Start SetDebugInformationToDWRF");
		PBXProject aPBXProject = GetPBXProject();
		string aTargetGUID = aPBXProject.TargetGuidByName(cOutputProj);
		
		aPBXProject.SetBuildProperty(aTargetGUID, "DEBUG_INFORMATION_FORMAT", "DWARF");
		
		File.WriteAllText(mPBXProjFullPath, aPBXProject.WriteToString());
	}
	/// <summary>
	/// GGCPreprocessor設定(選填)
	/// </summary>
	protected void AddGGCPreprocessor()
	{
		Debug.Log("Start AddGGCPreprocessor");
		PBXProject aPBXProject = GetPBXProject();
		string aTargetGUID = aPBXProject.TargetGuidByName(cOutputProj);
		string aDebugConfGUID = aPBXProject.BuildConfigByName(aTargetGUID, "Debug");
		string aReleaseConfGUID = aPBXProject.BuildConfigByName(aTargetGUID, "Release");
		
		aPBXProject.AddBuildProperty(aTargetGUID, "GCC_PREPROCESSOR_DEFINITIONS", "PLATFORM_IOS=1");
		aPBXProject.AddBuildPropertyForConfig(aDebugConfGUID, "GCC_PREPROCESSOR_DEFINITIONS", "DEBUG=1");
		aPBXProject.AddBuildPropertyForConfig(aReleaseConfGUID, "GCC_PREPROCESSOR_DEFINITIONS", "RELEASE=1");
		
		File.WriteAllText(mPBXProjFullPath, aPBXProject.WriteToString());
	}
	/// <summary>
	/// 加入一個普通的FrameWork
	/// </summary>
	protected void AddNromalFrameWork(string iFrameWorkName, bool iWeak = false)
	{
		Debug.Log("Start AddNromalFrameWork, FrameWorkName : " + iFrameWorkName);
		PBXProject aPBXProject = GetPBXProject();
		string aTargetGUID = aPBXProject.TargetGuidByName(cOutputProj);

		aPBXProject.AddFrameworkToProject(aTargetGUID, iFrameWorkName, iWeak);

		File.WriteAllText(mPBXProjFullPath, aPBXProject.WriteToString());
	}
	/// <summary>
	/// 加入一個tbd函式庫名稱
	/// </summary>
	protected void AddTBDFrameWork(string iTBDName)
	{
		Debug.Log("Start AddTBDFrameWork, TBDName : " + iTBDName);
		PBXProject aPBXProject = GetPBXProject();
		string aTargetGUID = aPBXProject.TargetGuidByName(cOutputProj);

		string aTBDName = iTBDName + ".tbd";
		aPBXProject.AddFileToBuild(aTargetGUID, aPBXProject.AddFile("usr/lib/" + aTBDName, "Frameworks/" + aTBDName, PBXSourceTree.Sdk) );
		string projText = addTbdLibrary(aTargetGUID, aPBXProject.WriteToString(), aTBDName);
		
		File.WriteAllText(mPBXProjFullPath, projText);
	}
	// 參考網址http://qiita.com/tkyaji/items/19dfff4afe228c7f34a1
	// 網路上找到的
	// .pbxprojファイルのtbdの記述を修正して返却
	private static string addTbdLibrary(string target, string projText, string tbdName)
	{
		string[] lines = projText.Split ('\n');
		List<string> newLines = new List<string> ();
		
		string refId = null;
		bool editFinish = false;
		
		for (int i = 0; i < lines.Length; i++) {
			
			string line = lines [i];
			
			if (editFinish) {
				newLines.Add (line);
				
			} else if (line.IndexOf (tbdName) > -1) {
				if (refId == null && line.IndexOf ("PBXBuildFile") > -1) {
					refId = line.Substring (0, line.IndexOf ("/*")).Trim ();
				} else if (line.IndexOf ("lastKnownFileType") > -1) {
					line = line.Replace ("lastKnownFileType = file;", "lastKnownFileType = \"sourcecode.text-based-dylib-definition\";");
				}
				newLines.Add (line);
				
			} else if (line.IndexOf ("isa = PBXFrameworksBuildPhase;") > -1) {
				do {
					newLines.Add (line);
					line = lines [++i];
				} while (line.IndexOf("files = (") == -1);
				
				while (true) {
					if (line.IndexOf (")") > -1) {
						newLines.Add (refId + ",");
						newLines.Add (line);
						break;
					} else if (line.IndexOf (refId) > -1) {
						newLines.Add (line);
						break;
					} else {
						newLines.Add (line);
						line = lines [++i];
					}
				}
				editFinish = true;
				
			} else {
				newLines.Add (line);
			}
		}
		
		return string.Join ("\n", newLines.ToArray ());
	}
	/// <summary>
	/// 由Plist檔案取得PlistDocument的Class內容
	/// </summary>
	protected static PlistDocument GetProjectPlist()
	{
		PlistDocument aPDoc = new PlistDocument();
		aPDoc.ReadFromString(File.ReadAllText(mInfoPlistFullPath));
		
		return aPDoc;
	}
	/// <summary>
	/// 由PBX檔取得PBXProject的Class內容
	/// </summary>
	protected static PBXProject GetPBXProject()
	{
		PBXProject aPBXProject = new PBXProject();
		aPBXProject.ReadFromFile(mPBXProjFullPath);
		
		return aPBXProject;
	}
}
#endif