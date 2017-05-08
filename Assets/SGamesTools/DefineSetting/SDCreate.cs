#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * 新建一個Setting檔
 */
public class SDCreate : SDBaseUI
{
	/// <summary>
	/// 初始化介面資料設定
	/// </summary>
	protected override void SettingInit()
	{
		if(mInitStatus)
			return;
		// Set Default Info
		DefaultSetCommon();
		DefaultSetAndroid();
		DefaultSetIOS();
		
		mInitStatus = true;
	}
	/// <summary>
	/// 顯示GUI內容
	/// </summary>
	protected override void ShowUI()
	{
		// Start Scroll View
		mEditorScrollView = EditorGUILayout.BeginScrollView(mEditorScrollView);
		UITopSettingShow();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("一般", ButtonMyStyle(eButtonPos.left)))
			mNowPage = eSettingPage.common;
		if (GUILayout.Button ("Android", ButtonMyStyle(eButtonPos.mid)))
			mNowPage = eSettingPage.android;
		if (GUILayout.Button ("IOS", ButtonMyStyle(eButtonPos.right)))
			mNowPage = eSettingPage.ios;
		EditorGUILayout.EndHorizontal();
		switch(mNowPage)
		{
		case eSettingPage.android: UIAndoridShow(); break;
		case eSettingPage.ios:     UIIOSShow();     break;
		default:                   UISettingShow(); break;
		}
		// Button Horizontal
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("存檔", ButtonMyStyle(eButtonPos.left)))
			DoCreate();
		if (GUILayout.Button ("取消", ButtonMyStyle(eButtonPos.right)))
			Close ();
		EditorGUILayout.EndHorizontal();
		// End Scroll View
		EditorGUILayout.EndScrollView ();
	}
	/// <summary>
	/// 執行產生一個新的檔案
	/// </summary>
	private void DoCreate()
	{
		if(string.IsNullOrEmpty(ShowSetInfo.SettingName))
		{
			EditorUtility.DisplayDialog("警告", "需要檔案名稱", "確定");
			return;
		}

		if(CheckFileNameRepeat())// 確認有沒有重複名稱
			return;

		SDDataMove.SaveData(ShowSetInfo);// 存檔

		Close();
	}
	/// <summary>
	/// 預設共通資料(取現在的PlayerSetting)
	/// </summary>
	private void DefaultSetCommon()
	{
		mShowSetInfo = new SDefineSet();
		mShowSetInfo.CompanyName = PlayerSettings.companyName;
		mShowSetInfo.ProductName = PlayerSettings.productName;
		// Screen Rotate
		mShowSetInfo.UIOrientation = PlayerSettings.defaultInterfaceOrientation;
		mShowSetInfo.OrienRoatable = new bool[4];
		mShowSetInfo.OrienRoatable[0] = PlayerSettings.allowedAutorotateToPortrait;
		mShowSetInfo.OrienRoatable[1] = PlayerSettings.allowedAutorotateToPortraitUpsideDown;
		mShowSetInfo.OrienRoatable[2] = PlayerSettings.allowedAutorotateToLandscapeLeft;
		mShowSetInfo.OrienRoatable[3] = PlayerSettings.allowedAutorotateToLandscapeRight;
		// Bundle
		mShowSetInfo.BundleID = PlayerSettings.bundleIdentifier;
		mShowSetInfo.BundleVer = PlayerSettings.bundleVersion;
		mShowSetInfo.ShortBundleVer = PlayerSettings.shortBundleVersion;
		// Other
		mShowSetInfo.StatusBarHidden = PlayerSettings.statusBarHidden;
		mShowSetInfo.Use32BitDisplayBuffer = PlayerSettings.use32BitDisplayBuffer;
		mShowSetInfo.ApiCompatibilityLevel = PlayerSettings.apiCompatibilityLevel;
		mShowSetInfo.StrippingLevel = PlayerSettings.strippingLevel;
		// Icon
		SDDataMove.GetIconsGroup(BuildTargetGroup.Unknown, ref mUIUseImages.DefaultIcon, ref mShowSetInfo.DefIcons);
		// Splash Images
		int aEmumTotal = Enum.GetNames(typeof(eMobileSplashScreen)).Length;
		mUIUseImages.MobileSplashImages = new Texture2D[aEmumTotal];
		mShowSetInfo.MobileSplashImages = new string[aEmumTotal];
		for(int i = 0; i < aEmumTotal; i++)
			mShowSetInfo.MobileSplashImages[i] = SDDataMove.GetSplashScreenPath((eMobileSplashScreen)i, ref mUIUseImages.MobileSplashImages[i]);
	}
	/// <summary>
	/// 預設Android資料(取現在的PlayerSetting)
	/// </summary>
	private void DefaultSetAndroid()
	{
		SDAndroidSet aTmpSet = new SDAndroidSet();
		aTmpSet.ShowActivityIndicatorOnLoading = PlayerSettings.Android.showActivityIndicatorOnLoading;
		aTmpSet.SplashScreenScale = PlayerSettings.Android.splashScreenScale;
		aTmpSet.BundleCode = PlayerSettings.Android.bundleVersionCode;
		aTmpSet.SdkVersions = PlayerSettings.Android.minSdkVersion;
		aTmpSet.TargetDevice = PlayerSettings.Android.targetDevice;
		aTmpSet.TargetGraphice = PlayerSettings.targetGlesGraphics;

		aTmpSet.ForceInternet = PlayerSettings.Android.forceInternetPermission;
		aTmpSet.ForceSDCard = PlayerSettings.Android.forceSDCardPermission;
		// PlayerSettings 的keystoreName是完整的Keystore路徑
		// 撰寫工具上我希望盡可能簡單的把檔案都放在Assets裡面
		// 所以在取路徑的時候檢查拿掉專案Assets之前的路徑
		string aKeyStorePath = PlayerSettings.Android.keystoreName;
		int aCheckNum = aKeyStorePath.IndexOf("Assets", System.StringComparison.OrdinalIgnoreCase);
		if(aCheckNum > 0)
			aKeyStorePath = aKeyStorePath.Remove(0, Application.dataPath.Length + 1);

		aTmpSet.KeyStorePath = aKeyStorePath;
		aTmpSet.KeyStorePassword = PlayerSettings.Android.keystorePass;
		aTmpSet.KeyAlialsName = PlayerSettings.Android.keyaliasName;
		aTmpSet.KeyAlialsPassword = PlayerSettings.Android.keyaliasPass;
		// Icon
		aTmpSet.IconOverride = true;
		SDDataMove.GetIconsGroup(BuildTargetGroup.Android, ref mUIUseImages.AndroidIcons, ref aTmpSet.DefIcons);
		// Set Down
		mShowSetInfo.AndroidSet = aTmpSet;
	}
	/// <summary>
	/// 預設IOS資料(取現在的PlayerSetting)
	/// </summary>
	private void DefaultSetIOS()
	{
		SDIOSSet aTmpSet = new SDIOSSet();
		aTmpSet.StatusBarStyle = PlayerSettings.iOS.statusBarStyle;
		aTmpSet.ShowActivityIndicatorOnLoading = PlayerSettings.iOS.showActivityIndicatorOnLoading;
		aTmpSet.TargetDevice = PlayerSettings.iOS.targetDevice;
		aTmpSet.TargetResolution = PlayerSettings.iOS.targetResolution;
		aTmpSet.TargetGraphics = PlayerSettings.targetIOSGraphics;

		aTmpSet.SDKVersion = PlayerSettings.iOS.sdkVersion;
		aTmpSet.TargetOSVersion = PlayerSettings.iOS.targetOSVersion;
		aTmpSet.ScriptCallOptimizationLevel = PlayerSettings.iOS.scriptCallOptimization;

		aTmpSet.OverrideIPodMusic = SDDataMove.GetBoolPlayerSetting("Override IPod Music");
		aTmpSet.PrepareIOSForRecording = SDDataMove.GetBoolPlayerSetting("Prepare IOS For Recording");
		aTmpSet.RequiresPersistentWiFi = PlayerSettings.iOS.requiresPersistentWiFi;
		aTmpSet.ExitOnSuspend = PlayerSettings.iOS.exitOnSuspend;

		aTmpSet.PrerenderedIcon = PlayerSettings.iOS.prerenderedIcon;
		aTmpSet.ScriptingBackend = (ScriptingImplementation)PlayerSettings.GetPropertyInt("ScriptingBackend", BuildTargetGroup.iPhone);
		if(aTmpSet.ScriptingBackend == ScriptingImplementation.IL2CPP)
			aTmpSet.Architecture = (iPhoneArchitecture)PlayerSettings.GetPropertyInt("Architecture", BuildTargetGroup.iPhone);
		// Icon
		aTmpSet.IconOverride = true;
		SDDataMove.GetIconsGroup(BuildTargetGroup.iPhone, ref mUIUseImages.IosIcons, ref aTmpSet.DefIcons);
		// Set Down
		mShowSetInfo.IOSSet = aTmpSet;
	}
}
#endif