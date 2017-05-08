#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Newtonsoft.Json;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * 設定上和檔案有點相關的處理
 */
public class SDDataMove
{
	public const string cSavePath = "SGamesTools/DefineSetting/DataSave";// 存檔路徑(比較會要修改的位置)
	public static char mSlash = '\\';
	/// <summary>
	/// 取得存檔的完整路徑
	/// 檔案儲存引用的是System.IO，所以儲存的路徑是需要完整化
	/// </summary>
	private static string GetSaveFullPath()
	{
		SetSlash();
		string aSavePath = Path.Combine(Application.dataPath, cSavePath);
		aSavePath = aSavePath.Replace(mSlash, '/');
		
		return aSavePath;
	}
	/// <summary>
	/// 儲存檔案
	/// </summary>
	public static void SaveData(SDefineSet iSetData)
	{
		string aSavePath = GetSaveFullPath();
		try
		{
			string aJsonString = JsonConvert.SerializeObject(iSetData, Formatting.Indented);
			
			Debug.Log("TestSave cahge to Json string\n"+ aJsonString);
			SFileIOModfied.SaveNewJsonFile(aSavePath, iSetData.SettingName, aJsonString);
			AssetDatabase.Refresh();
			EditorUtility.DisplayDialog("成功", "資料建置完畢", "確定");
		}
		catch(Exception e)
		{
			Debug.LogError("Save temp data error:\n" + e.Message);
		}
	}
	/// <summary>
	/// 依照檔案名稱尋找並讀取檔案
	/// </summary>
	public static SDefineSet LoadData(string iFileName)
	{
		string aSavePath = GetSaveFullPath();
		try
		{
			string aLoadedJson = SFileIOModfied.LoadFromJsonFile(aSavePath, iFileName);
			SDefineSet aResult = JsonConvert.DeserializeObject<SDefineSet>(aLoadedJson);
			Debug.Log("LoadFinish.");
			return aResult;
		}
		catch(Exception e)
		{
			Debug.LogError("Load temp data error:\n" + e.Message);
			return new SDefineSet();
		}
	}
	/// <summary>
	/// 取得所有檔案名稱
	/// </summary>
	public static string[] GetSaveDataNames()
	{
		string aSavePath = GetSaveFullPath();
		try
		{
			string[] aSaveFileNames = SFileIOModfied.GetJsonFileNameArray(aSavePath);

			Debug.Log("GetFinish.");
			return aSaveFileNames;
		}
		catch(Exception e)
		{
			Debug.LogError("Get DataNames temp data error:\n" + e.Message);
			return null;
		}

	}
	/// <summary>
	/// 讀取Icon圖
	/// </summary>
	public static Texture2D[] LoadIconTexture(eSettingPage iPage, ref SDefineSet iSetting, bool aLoadUnitySet = true)
	{
		string[] aSelectIcons = null;
		Texture2D[] aResultTexture = null;
		BuildTargetGroup iPSet = BuildTargetGroup.Unknown;
		switch(iPage)
		{
		case eSettingPage.android:
			aSelectIcons = iSetting.AndroidSet.DefIcons;
			iPSet = BuildTargetGroup.Android;
			break;
		case eSettingPage.ios:
			aSelectIcons = iSetting.IOSSet.DefIcons;
			iPSet = BuildTargetGroup.iPhone;
			break;
		default:
			aSelectIcons = iSetting.DefIcons;
			break;
		}
		if(aLoadUnitySet)
		if(CheckImagePathIsEmpty(aSelectIcons))
		{
			GetIconsGroup(iPSet, ref aResultTexture, ref aSelectIcons);
			return aResultTexture;
		}
		try
		{
			aResultTexture = new Texture2D[aSelectIcons.Length];
			for(int i = 0; i < aSelectIcons.Length; i++)
			{
				if(string.IsNullOrEmpty(aSelectIcons[i]))
					continue;

				Texture2D aTmpIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(aSelectIcons[i], typeof(System.Object));
				aResultTexture[i] = aTmpIcon;
			}

			Debug.Log("GetFinish.");
			return aResultTexture;
		}
		catch(Exception e)
		{
			Debug.LogError("Load Icon temp data error:\n" + e.Message);
			return aResultTexture;
		}
	}
	/// <summary>
	/// 讀取Splash圖示
	/// </summary>
	public static Texture2D[] LoadSplashTexture(ref SDefineSet iSetting, bool aLoadUnitySet = true)
	{
		string[] aSelectSplashs = iSetting.MobileSplashImages;
		Texture2D[] aResultTexture = null;
		if(aLoadUnitySet)
		if(CheckImagePathIsEmpty(aSelectSplashs))
		{
			int aEmumTotal = Enum.GetNames(typeof(eMobileSplashScreen)).Length;
			aResultTexture = new Texture2D[aEmumTotal];
			aSelectSplashs = new string[aEmumTotal];
			for(int i = 0; i < aEmumTotal; i++)
				aSelectSplashs[i] = SDDataMove.GetSplashScreenPath((eMobileSplashScreen)i, ref aResultTexture[i]);

			iSetting.MobileSplashImages = aSelectSplashs;
			return aResultTexture;
		}

		try
		{
			aResultTexture = new Texture2D[aSelectSplashs.Length];
			for(int i = 0; i < aSelectSplashs.Length; i++)
			{
				if(string.IsNullOrEmpty(aSelectSplashs[i]))
					continue;
				
				Texture2D aTmpSplash = (Texture2D)AssetDatabase.LoadAssetAtPath(aSelectSplashs[i], typeof(System.Object));
				aResultTexture[i] = aTmpSplash;
			}
			Debug.Log("GetFinish.");
			iSetting.MobileSplashImages = aSelectSplashs;
			return aResultTexture;
		}
		catch(Exception e)
		{
			Debug.LogError("Load Icon temp data error:\n" + e.Message);
			return aResultTexture;
		}
	}
	/// <summary>
	/// 確認圖片陣列路徑有沒有空的檔案
	/// </summary>
	public static bool CheckImagePathIsEmpty(string[] iImageArray)
	{
		// Cehck Is Null
		if(iImageArray == null)
		{
			Debug.Log("No Any Icons.");
			return true;
		}
		// Check Is Empty
		bool aIsEmpty = true;
		for(int i = 0; i < iImageArray.Length; i++)
		{
			if(!string.IsNullOrEmpty(iImageArray[i]))
				aIsEmpty = false;
		}

		if(aIsEmpty)
		{
			Debug.Log("No Any Icon path.");
			return aIsEmpty;
		}

		return aIsEmpty;
	}
	/// <summary>
	/// 取得PlayerSetting上的Icon圖片
	/// </summary>
	public static void GetIconsGroup(BuildTargetGroup iTarget, ref Texture2D[] iIcons, ref string[] iIconNames)
	{
		iIcons = PlayerSettings.GetIconsForTargetGroup(iTarget);
		string[] aDefNames = new string[iIcons.Length];
		for(int i = 0; i < iIcons.Length; i++)
		{
			if(iIcons[i] != null)
				aDefNames[i] = iIcons[i].name;
			else
				aDefNames[i] = string.Empty;
		}
		
		iIconNames = aDefNames;
	}
	/// <summary>
	/// 把Icon圖片設定至PlayerSetting
	/// </summary>
	public static void SetIconsGroup(BuildTargetGroup iTarget, string[] iIconPath)
	{
		Texture2D[] aIconGroup = new Texture2D[iIconPath.Length];
		for(int i = 0; i< aIconGroup.Length; i++)
			aIconGroup[i] = (Texture2D)AssetDatabase.LoadAssetAtPath(iIconPath[i], typeof(System.Object));

		PlayerSettings.SetIconsForTargetGroup(iTarget, aIconGroup);
	}
	/// <summary>
	/// 清空PlayeSetting的Icon圖片
	/// </summary>
	public static void ClearnIconsGroup(BuildTargetGroup iTarget)
	{
		PlayerSettings.SetIconsForTargetGroup(iTarget, new Texture2D[0]);
	}
	// 這邊用到的方法是直接提取PlayerSettings設定檔的內容
	// 設定名稱可以從Edit/ProjectSetting/Editor中把AssetSerialization轉成文字模式
	// 再用記事本檔案查看PlayerSetting是甚麼名字
	// 當PlayerSettings沒有提供相關方法時所採用的最後手段
	#region SerializedObject
	/// <summary>
	/// 取得PlayerSetting上的Splash圖片
	/// </summary>
	public static Texture2D GetSplashScreen(eMobileSplashScreen iScrType)
	{
		Texture2D aResult = null;
		PlayerSettings[] aAllSet = Resources.FindObjectsOfTypeAll<PlayerSettings> ();
		for(int i = 0; i < aAllSet.Length; i++)
		{
			SerializedObject aSO = new SerializedObject(aAllSet[i]);
			aSO.Update();
			aResult = aSO.FindProperty (iScrType.ToString()).objectReferenceValue as Texture2D;
		}
		return aResult;
	}
	/// <summary>
	/// 取得Splash圖片路徑
	/// </summary>
	public static string GetSplashScreenPath(eMobileSplashScreen iScrType, ref Texture2D iImage)
	{
		string aResult = string.Empty;
		iImage = GetSplashScreen(iScrType);
		if(iImage != null)
			aResult = AssetDatabase.GetAssetPath(iImage);

		return aResult;
	}
	/// <summary>
	/// 設置Splash圖片
	/// </summary>
	public static void SetSplashScreen(eMobileSplashScreen iScrType, string iPath )
	{
		foreach (var player in Resources.FindObjectsOfTypeAll<PlayerSettings> ())
		{
			var so = new SerializedObject (player);
			so.Update ();
			so.FindProperty (iScrType.ToString()).objectReferenceValue = AssetDatabase.LoadAssetAtPath (iPath, typeof(Texture2D));
			so.ApplyModifiedProperties ();

			EditorUtility.SetDirty (player);
		}
		AssetDatabase.SaveAssets ();
	}
	/// <summary>
	/// 取得PlayerSetting上的Bool參數
	/// </summary>
	public static bool GetBoolPlayerSetting(string iOptionName)
	{
		bool aResult = false;
		PlayerSettings[] aAllSet = Resources.FindObjectsOfTypeAll<PlayerSettings> ();
		for(int i = 0; i < aAllSet.Length; i++)
		{
			SerializedObject aSO = new SerializedObject(aAllSet[i]);
			aSO.Update();
			aResult = aSO.FindProperty (iOptionName).boolValue;
		}
		return aResult;
	}
	/// <summary>
	/// 設置PlayerSetting上的Bool參數
	/// </summary>
	public static void SetBoolPalyerSetting(string iOptionName, bool iValue)
	{
		foreach (var player in Resources.FindObjectsOfTypeAll<PlayerSettings> ())
		{
			var so = new SerializedObject (player);
			so.Update ();
			so.FindProperty (iOptionName).boolValue = iValue;
			so.ApplyModifiedProperties ();
			
			EditorUtility.SetDirty (player);
		}
		AssetDatabase.SaveAssets ();
	}
	#endregion
	/// <summary>
	/// Sets the slash.
	/// </summary>
	private static void SetSlash()
	{
		if(Application.platform == RuntimePlatform.WindowsEditor)
			mSlash = '\\';
		else
			mSlash = '/';
	}
}
#endif