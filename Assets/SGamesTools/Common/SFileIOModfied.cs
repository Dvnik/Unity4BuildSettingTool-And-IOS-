#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * 檔案的輸入輸出，甚至改寫內容
 */
public class SFileIOModfied
{
	// 副檔名
	private const string cDotTxt = ".txt";
	private const string cDotMeta = ".meta";
	private const string cDotJson = ".json";
	private const string cDotCs = ".cs";

	private static char mSlashMark = '\\';
	/// <summary>
	/// 儲存成Txt副檔名的檔案
	/// </summary>
	public static void SaveNewTxtFile(string iPath, string iFileName, string iWord)
	{
		string aFilePath = Path.Combine(iPath, iFileName + cDotTxt);
		aFilePath = aFilePath.Replace(mSlashMark, '/');
		
		FileInfo aFI = new FileInfo(aFilePath);
		if(aFI.Exists)
			aFI.Delete();
		
		FileStream aFS = new FileStream(aFilePath, FileMode.OpenOrCreate, FileAccess.Write);
		StreamWriter aSW = new StreamWriter(aFS, System.Text.Encoding.GetEncoding("UTF-8"));
		
		aSW.Write(iWord);
		aSW.Close();
		aFS.Close();
		AssetDatabase.Refresh();
		
		Debug.Log("FilePath = " + aFilePath);
	}
	/// <summary>
	/// 儲存成Json副檔名的檔案
	/// </summary>
	public static void SaveNewJsonFile(string iPath, string iFileName, string iContent)
	{
		if(!Directory.Exists(iPath))// Check Path Exists
			Directory.CreateDirectory(iPath);
		SetSlash();
		string aFilePath = Path.Combine(iPath, iFileName + cDotJson);
		aFilePath = aFilePath.Replace(mSlashMark, '/');

		FileInfo aFI = new FileInfo(aFilePath);
		if(aFI.Exists)
			aFI.Delete();
		
		FileStream aFS = new FileStream(aFilePath, FileMode.OpenOrCreate, FileAccess.Write);
		StreamWriter aSW = new StreamWriter(aFS, Encoding.GetEncoding("UTF-8"));
		
		aSW.Write(iContent);
		aSW.Close();
		aFS.Close();
		AssetDatabase.Refresh();

		Debug.Log("SaveTxtPath = " + aFilePath);
	}
	/// <summary>
	/// 讀取Json檔案
	/// </summary>
	public static string LoadFromJsonFile(string iPath, string iFileName)
	{
		SetSlash();
		string aFilePath = Path.Combine(iPath, iFileName + cDotJson);
		aFilePath = aFilePath.Replace(mSlashMark, '/');

		FileInfo aFI = new FileInfo(aFilePath);
		if(!aFI.Exists)
		{
			Debug.Log("File Not Found, Path :" + aFilePath);
			return string.Empty;
		}
		
		FileStream aFS = new FileStream(aFilePath, FileMode.Open, FileAccess.Read);
		StreamReader aSR = new StreamReader(aFS, Encoding.GetEncoding("UTF-8"));
		
		string aResult = aSR.ReadToEnd();
		
		return aResult;
	}
	/// <summary>
	/// 取得資料夾底下所有特定副檔名的檔案名稱，並彙整成列表返回
	/// </summary>
	/// <returns>檔案名稱列表</returns>
	/// <param name="iFolderPath">資料夾路徑</param>
	/// <param name="iDotFileName">篩選的副檔名</param>
	private static List<string> GetFileNameLsit(string iFolderPath, string iDotFileName)
	{
		List<string> aResult = new List<string>();
		// Get Folders File name
		if(!Directory.Exists(iFolderPath))// Check Path Exists
			Directory.CreateDirectory(iFolderPath);
		
		string[] aFileList =  Directory.GetFiles(iFolderPath);
		// If Not Get File Name List, Return null;
		if(aFileList.Length <= 0)
		{
			Debug.LogError("No Get Any File Name.");
			return aResult;
		}
		string aLog = "";
		// Make File Name List
		for(int i = 0; i < aFileList.Length;i++)
		{
			// Remove File Path, Reserved File Name
			string aFileFullName = aFileList[i].Remove(0, (iFolderPath.Length + 1));
			aLog +="FileFullName." + i + ":" + aFileFullName + "\n";
			// Check File Is Txt
			int aCheckNum = aFileFullName.IndexOf(iDotFileName, System.StringComparison.OrdinalIgnoreCase);
			if(aCheckNum < 0)
				continue;
			// Exclude MetaFile, Not Use.
			aCheckNum = aFileFullName.IndexOf(cDotMeta, System.StringComparison.OrdinalIgnoreCase);
			if(aCheckNum < 0)
			{
				int aStarIndex = aFileFullName.Length - (iDotFileName.Length);
				string aResultName = aFileFullName.Remove(aStarIndex);
				aResult.Add(aResultName);
				aLog += aResultName + " Be Add.\n";
			}
		}
		Debug.Log(aLog);
		return aResult;
	}
	/// <summary>
	/// 取得資料夾底下所有json檔的檔案名稱，並彙整成列表返回(List)
	/// </summary>
	public static List<string> GetJsonFileNameList(string iFolderPath)
	{
		return GetFileNameLsit(iFolderPath, cDotJson);
	}
	/// <summary>
	/// 取得資料夾底下所有json檔的檔案名稱，並彙整成列表返回(Array)
	/// </summary>
	public static string[] GetJsonFileNameArray(string iFolderPath)
	{
		List<string> aTmpList = GetJsonFileNameList(iFolderPath);
		string[] aResult = null;
		if(aTmpList == null)
			aResult = null;
		else
			aResult = aTmpList.ToArray();

		return aResult;
	}
	/// <summary>
	/// 取得資料夾底下所有C#檔的檔案名稱，並彙整成列表返回(List)
	/// </summary>
	public static List<string> GetCSFileNameList(string iFolderPath)
	{
		return GetFileNameLsit(iFolderPath, cDotCs);
	}

	/// <summary>
	/// 設定檔案斜線，在直接操作外部檔案時會用到
	/// </summary>
	private static void SetSlash()
	{
		if(Application.platform == RuntimePlatform.WindowsEditor)
			mSlashMark = '\\';
		else
			mSlashMark = '/';
	}
	/// <summary>
	/// 文字檔替換覆蓋，透過標記行判斷
	/// </summary>
	/// <param name="iAssetPath">Unity工作區底下的路徑</param>
	/// <param name="iWrite">改寫內容行</param>
	/// <param name="iMark">改寫標記行，透過標記將改寫內容放入標記的下一行</param>
	public static void FileWirteLineByMark(string iAssetPath, string iWrite, string iMark)
	{
		string aGDefinePath = string.Format ("{0}/{1}", Application.dataPath, iAssetPath);
		FileInfo aFI = new FileInfo(aGDefinePath);
		if(aFI.Exists == false)
		{
			Debug.Log("This file is Not Exists. File Path = " + aGDefinePath);
			return;
		}
		int aInfoCount = 0;
		List<string> aWriteList = new List<string> ();// 寫檔List
		try
		{
			// 讀取檔案
			FileStream aFS = new FileStream (aGDefinePath, FileMode.Open, FileAccess.Read);
			StreamReader aSR = new StreamReader(aFS, Encoding.GetEncoding("UTF-8"));
			//開始讀取
			string aRLine;
			while ( aSR.Peek() != -1)
			{
				aRLine = aSR.ReadLine();//讀取一列 
				if(aRLine.Equals(iMark))// 檢查是否到執行更換內容的標記行
				{
					aWriteList.Add(aRLine);
					aRLine = aSR.ReadLine();//多讀取一列，並將該行取代掉
					aRLine = iWrite;
				}
				
				aWriteList.Add(aRLine);
				// Show Wait Bar
				EditorUtility.DisplayProgressBar("讀取進度", "讀取行數 : " + aInfoCount++,  (float)( aInfoCount / aWriteList.Count ) );
			}
			EditorUtility.ClearProgressBar();// Clearn Bar
			// 結束讀取
			aSR.Close ();
			aFS.Close();
			aInfoCount = 0;
			// 寫入檔案
			aFS = new FileStream (aGDefinePath, FileMode.Create, FileAccess.Write);
			StreamWriter aSW = new StreamWriter(aFS, Encoding.GetEncoding("UTF-8"));
			
			// 開始寫入
			foreach (string e in aWriteList)
			{
				aSW.WriteLine (e);
				// Show Wait Bar
				EditorUtility.DisplayProgressBar("寫入進度", "寫入行數 : " + aInfoCount++,  (float)( aInfoCount / aWriteList.Count ) );
			}
			EditorUtility.ClearProgressBar();// Clearn Bar
			// 結束寫入
			aSW.Close();
			aFS.Close();
			// Down
			Debug.Log("修改完成");
			// Refresh
			AssetDatabase.Refresh();
		}
		catch(IOException e)
		{
			EditorUtility.ClearProgressBar();// Clearn Bar
			EditorUtility.DisplayDialog("失敗", "訊息:\n" + e.ToString(), "確定");
			Debug.Log(e.ToString());
		}
	}
	/// <summary>
	/// 重寫一個取得其他工具修改名稱的列表內容
	/// </summary>
	public static void ReWriteGetCustomInfo(string iFullPath, string iFNListPath,
	                                        string iClassName, string iTypeClassName)
	{
		// Check File Exists
		FileInfo aFI = new FileInfo(iFullPath);
		if(aFI.Exists == false)
		{
			Debug.Log("This file is Not Exists. File Path = " + iFullPath);
			return;
		}

		try
		{
			List<string> aFileList = GetCSFileNameList(iFNListPath);
			// ReWrite a New Data
			FileStream aFS = new FileStream (iFullPath, FileMode.Create, FileAccess.Write);
			StreamWriter aSW = new StreamWriter(aFS, Encoding.GetEncoding("UTF-8"));
			// Write Data
			aSW.WriteLine ("#if UNITY_EDITOR");
			aSW.WriteLine ("using System.Collections;");
			aSW.WriteLine ("using System.Collections.Generic;\n");
			aSW.WriteLine ("public class " + iClassName);
			aSW.WriteLine ("{");
			aSW.WriteLine ("\tpublic static List<" + iTypeClassName + "> GetBaseTypeList()");
			aSW.WriteLine ("\t{");
			aSW.WriteLine ("\t\tList<" + iTypeClassName + "> aResult = new List<" + iTypeClassName + ">();\n");
			// ReWrite Lists
			for(int i = 0; i < aFileList.Count; i++)
				aSW.WriteLine ("\t\taResult.Add(new " + aFileList[i] + "());");

			aSW.WriteLine ("\t\treturn aResult;");
			aSW.WriteLine ("\t}");
			aSW.WriteLine ("}");
			aSW.WriteLine ("#endif");
			// 結束寫入
			aSW.Close();
			aFS.Close();
			// Down
			Debug.Log("修改完成");

		}
		catch(IOException e)
		{
			EditorUtility.DisplayDialog("失敗", "訊息:\n" + e.ToString(), "確定");
			Debug.Log(e.ToString());
		}
		// Refresh
		AssetDatabase.Refresh();
	}
	/// <summary>
	/// 重寫檔案搬移列表
	/// </summary>
	public static void ReWriteSFGetCustomInfo()
	{
		string aFullFilePath = string.Format ("{0}/{1}", Application.dataPath, "SGamesTools/FileChangeSet/SFGetCustom.cs");
		string aCustomListFullPath = string.Format ("{0}/{1}", Application.dataPath, "SGamesTools/FileChangeSet/CustomList");
		
		ReWriteGetCustomInfo(aFullFilePath, aCustomListFullPath, "SFGetCustom", "SFChangeBase");
	}
	/// <summary>
	/// 重寫XCode編輯內容
	/// </summary>
	public static void ReWriteSXGetCustomInfo()
	{
		string aFullFilePath = string.Format ("{0}/{1}", Application.dataPath, "SGamesTools/Editor/XCodeWriter/SXGetCustom.cs");
		string aCustomListFullPath = string.Format ("{0}/{1}", Application.dataPath, "SGamesTools/Editor/XCodeWriter/CustomList");
		
		ReWriteGetCustomInfo(aFullFilePath, aCustomListFullPath, "SXGetCustom", "SXWriteBase");
	}
	/// <summary>
	/// 新增一個XCode PushNotification 所需要的ios.entitlements檔案與Push設定
	/// </summary>
	public static void AddPushNotificationEntitlements(string iProjectPath)
	{
		string aFullPath = string.Format("{0}/{1}", iProjectPath, "Unity-iPhone/ios.entitlements");
		try
		{
			// ReWrite a New Data
			FileStream aFS = new FileStream (aFullPath, FileMode.Create, FileAccess.Write);
			StreamWriter aSW = new StreamWriter(aFS, Encoding.GetEncoding("UTF-8"));
			// Write Data
			aSW.WriteLine ("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			aSW.WriteLine ("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
			aSW.WriteLine ("<plist version=\"1.0\">");
			aSW.WriteLine ("<dict>");
			aSW.WriteLine ("\t<key>aps-environment</key>");
			aSW.WriteLine ("\t<string>development</string>");
			aSW.WriteLine ("</dict>");
			aSW.WriteLine ("</plist>");
			// 結束寫入
			aSW.Close();
			aFS.Close();
			// Down
			Debug.Log("修改完成");
		}
		catch(IOException e)
		{
			Debug.Log(e.ToString());
		}
	}
}
#endif