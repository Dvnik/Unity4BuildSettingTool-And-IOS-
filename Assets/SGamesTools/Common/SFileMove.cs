#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * 檔案搬移，以Unity/Assets底下的檔案搬移為主
 */
public class SFileMove
{
	private static SFileMove Instance;
	public static SFileMove Self 
	{ 
		set
		{
			Instance = value;
		}
		get
		{
			if( Instance == null )
				Instance = new SFileMove();
			
			return Instance;
		}
	}

	public const string cDotMeta = ".meta";
	public const string cDotSvn = ".svn";
	/// <summary>
	/// 複製檔案與資料夾，並過濾".svn"資料夾
	/// 因為舊版的烏龜SVN會在每一個資料夾中設置".svn"資料夾做版本控管(對新版本無影響)
	/// </summary>
	/// <param name="iFrom">來源位置</param>
	/// <param name="iTo">目標位置</param>
	public void CopyFolder(string iFrom, string iTo)
	{
		try
		{
			if(CheckFilePathEmpty( iFrom ))
				return;
			
			string[] aTmpPaths = Directory.GetDirectories(iFrom);
			string aLog = "Path : " + iFrom + ".\nStart Copy.\n";
			int i, k;
			// Check Other Folder
			for(i = 0; i < aTmpPaths.Length; i++)
			{
				string aDircPath = aTmpPaths[i].Replace("\\", "/");
				string[] aDotSVNCheck = aDircPath.Split('/');
				bool aSVNResult = false;
				for(k = 0; k < aDotSVNCheck.Length; k++)
				{
					if(aDotSVNCheck[k].Equals(cDotSvn))
					{
						aSVNResult = true;
						break;
					}
				}
				aLog += "\tNO." + i + " DotSVNCheck Log :\n\t\t" +
					"Path : " + aDircPath + ".\n\t\tResult : " + aSVNResult + "\n";
				if(aSVNResult)
					continue;
				
				string aNextFolder = iTo + aDircPath.Substring(iFrom.Length);// 組出下一個要拷貝的資料夾
				aLog += "\tNext Copy Folder : " + aNextFolder + "\n";
				CopyFolder(aDircPath, aNextFolder);// 遞回拷貝子資料夾
			}
			// 目標資料夾沒有就產生一個
			if(Directory.Exists(iTo) == false)
			{
				aLog += "Create Folder Path :"+ iTo +"\n";
				Directory.CreateDirectory(iTo);
				string aFromMeta = iFrom + cDotMeta;
				string aToMeta = iTo + cDotMeta;
				if(File.Exists(aFromMeta))
					File.Copy (aFromMeta, aToMeta, true);
			}
			// Copy File
			aLog += "Copy File Log :\n";
			string[] aTmpFiles = Directory.GetFiles(iFrom);
			for(i = 0; i < aTmpFiles.Length; i++)
			{
				string aFilePath = aTmpFiles[i];
				aFilePath = aFilePath.Replace("\\", "/");// 修正路徑斜線
				string aFileName = Path.GetFileName(aFilePath);
				string aTargetFile = Path.Combine(iTo, aFileName).Replace("\\", "/");
				File.Copy (aFilePath, aTargetFile, true);
				aLog += "NO."+ i + "\n" +
					"\tFrome Path:" + aFilePath + "\n" +
						"\tFileName:" + aFileName + "\n" +
						"\tTo Path:" + aTargetFile + "\n";
				// Show Wait Bar
				EditorUtility.DisplayProgressBar("拷貝檔案", "路徑 : " + aFilePath,  (float)( i / aTmpFiles.Length ) * 10 );
			}
			EditorUtility.ClearProgressBar();// Clearn Bar
			Debug.Log(aLog);
		}
		catch (System.Exception iExcpt)
		{
			EditorUtility.ClearProgressBar ();// Clearn Bar
			Debug.Log (iExcpt);
		}
	}	
	/// <summary>
	/// 深度刪除資料夾
	/// </summary>
	/// <param name="iLists">I lists.</param>
	public void RemoveFolder(List<string> iList)
	{
		for (int i = 0; i < iList.Count; i++)
			RemoveFolder (iList [i] );
	}
	/// <summary>
	/// 深度刪除資料夾
	/// 透過搜尋每一個檔案，並過濾".svn"資料夾
	/// 因為舊版的烏龜SVN會在每一個資料夾中設置".svn"資料夾做版本控管(對新版本無影響)
	/// </summary>
	/// <param name="iPath">I path.</param>
	public void RemoveFolder(string iPath)
	{
		try
		{
			if(CheckFilePathEmpty( iPath ))
				return;
			
			string[] aTmpPaths = Directory.GetDirectories(iPath);
			string aLog = "Path : " + iPath + ".\nStart Delete.\n";
			int i, k;
			// Check Other Folder
			for(i = 0; i < aTmpPaths.Length; i++)
			{
				string aDircPath = aTmpPaths[i].Replace("\\", "/");
				string[] aDotSVNCheck = aDircPath.Split('/');
				bool aSVNResult = false;
				for(k = 0; k < aDotSVNCheck.Length; k++)
				{
					if(aDotSVNCheck[k].Equals(cDotSvn))
					{
						aSVNResult = true;
						break;
					}
				}
				aLog += "\tNO." + i + " DotSVNCheck Log :\n\t\t" +
					"Path : " + aDircPath + ".\n\t\tResult : " + aSVNResult + "\n";
				if(aSVNResult)
					continue;
				
				RemoveFolder(aDircPath);// 遞回刪除子資料夾
			}
			// Delete File
			aLog += "Delete File Log :\n";
			string[] aTmpFiles = Directory.GetFiles(iPath);
			for(i = 0; i < aTmpFiles.Length; i++)
			{
				string aFilePath = aTmpFiles[i];
				aFilePath = aFilePath.Replace("\\", "/");// 修正路徑斜線
				File.Delete(aFilePath);
				aLog += "\t" + aFilePath + "\n";
				// Show Wait Bar
				EditorUtility.DisplayProgressBar("刪除檔案", "路徑 : " + aFilePath,  (float)( i / aTmpFiles.Length ) * 10 );
			}
			// Check Empty
			aTmpPaths = Directory.GetDirectories(iPath);
			if(aTmpPaths.Length <= 0)
			{
				Directory.Delete (iPath, true);
				File.Delete(iPath + cDotMeta);
				aLog += "Clear No Child Folder.\n\t" + iPath + "\n\t" + iPath + cDotMeta;
			}
			EditorUtility.ClearProgressBar();// Clearn Bar
			Debug.Log( aLog );
		}
		catch(System.Exception iExcpt)
		{
			EditorUtility.ClearProgressBar();// Clearn Bar
			Debug.Log(iExcpt);
		}
	}
	/// <summary>
	/// 刪除某個資料夾底下的檔案，並不包含子資料夾
	/// </summary>
	/// <param name="iPath">I path.</param>
	public void RemoveFolderFiles(string iFolderPath, string[] iSkipArray)
	{
		List<string> aSADataFiles = GetTargetFilesList (iFolderPath);
		for(int i = 0; i < aSADataFiles.Count; i++)
		{
			string iTargetPath = aSADataFiles[i];
			iTargetPath = iTargetPath.Replace("\\", "/");
			bool aCheck = false;
			foreach(string aNames in iSkipArray)
			{
				string aFilePath = Path.Combine(iFolderPath, aNames);
				aFilePath = aFilePath.Replace("\\", "/");
				if(aFilePath.Equals(iTargetPath))
				{
					aCheck = true;
					break;
				}
			}

			if(aCheck == false)
				File.Delete(iTargetPath);
		}
	}
	/// <summary>
	/// 搬移檔案並複製一份備份位置(List列表處理)
	/// </summary>
	/// <param name="iFromList">來源位置(List列表)</param>
	/// <param name="iToList">目標位置(List列表)</param>
	/// <param name="iBackList">備援位置(List列表)</param>
	public void CopyMoveFolderLists(List<string> iFromList, List<string> iToList, List<string> iBackList)
	{
		for(int i = 0; i < iFromList.Count; i++)
			CopyMoveFolderLists(iFromList[i], iToList[i], iBackList[i]);
	}
	/// <summary>
	/// 搬移檔案並複製一份備份位置
	/// </summary>
	/// <param name="iFrom">來源位置</param>
	/// <param name="iTo">目標位置</param>
	/// <param name="iBack">備援位置</param>
	public void CopyMoveFolderLists(string iFrom, string iTo, string iBack)
	{
		// 移回來
		if(Directory.Exists(iBack))
			MoveFolder(iBack, iFrom);
		// 拷貝
		CopyFolder (iFrom, iTo);
		// 移開
		if(Directory.Exists(iFrom))
			MoveFolder(iFrom, iBack);
	}
	/// <summary>
	/// 移動資料夾
	/// </summary>
	/// <param name="iFrom">來源位置</param>
	/// <param name="iTo">目標位置</param>
	public void MoveFolder(string iFrom, string iTo)
	{
		if(CheckFilePathEmpty( iFrom ))
			return;
		
		if ( Directory.Exists (iTo) )
		{
			Directory.Delete (iTo, true);
			File.Delete(iTo + cDotMeta);
		}
		Directory.Move (iFrom, iTo);
		File.Delete (iFrom + cDotMeta);
	}
	/// <summary>
	/// 執行覆蓋檔案，並將備份檔案刪除
	/// </summary>
	/// <param name="iSourcePath">要覆蓋原始檔的檔案位置</param>
	/// <param name="iOriginalPath">被覆蓋的原始檔位置</param>
	/// <param name="aBackupPath">備援檔案(會被刪除)</param>
	public void ReplaceFile(string iSourcePath, string iOriginalPath, string iBackupPath)
	{
		if (File.Exists (iSourcePath))
		{
			if(File.Exists(iOriginalPath))//如果目標路徑存在，就覆蓋
			{
				File.Replace (iSourcePath, iOriginalPath, iBackupPath);
				RemoveSourceAndMetaFile (iBackupPath);
			}
			else// 反之則直接移動
				File.Move(iSourcePath, iOriginalPath);
		}
		else
			Debug.Log("This Source File >> \"" + iSourcePath + "\" << Is Empty");
	}
	/// <summary>
	/// 確認路徑是否是空的
	/// 空的話要顯示Log訊息
	/// </summary>
	/// <returns><c>true</c>, 路徑不存在, <c>false</c> 路徑存在.</returns>
	/// <param name="iPath">I path.</param>
	public bool CheckFilePathEmpty(string iPath)
	{
		bool aResult = !Directory.Exists (iPath);// Exists是確認是否存在，所以要反轉結果
		if(aResult)
			Debug.Log("This Path >> \"" + iPath + "\" << Is Empty");
		
		return aResult;
	}
	/// <summary>
	/// 取得指定路徑下的檔案列表
	/// </summary>
	/// <returns>檔案名稱(List)</returns>
	/// <param name="iTargetPath">資料夾路徑</param>
	public List<string> GetTargetFilesList(string iTargetPath)
	{
		if (Directory.Exists (iTargetPath) == false)
			return new List<string> ();
		
		List<string> iDir = new List<string>();
		string[] iGetFiles = Directory.GetFiles(iTargetPath);
		
		foreach(string iFile in iGetFiles)
			iDir.Add(iFile);
		
		return iDir;
	}
	/// <summary>
	/// 刪除指定的檔案與Unity meta檔
	/// </summary>
	public void RemoveSourceAndMetaFile(string iPath)
	{
		if (File.Exists (iPath))
		{
			File.Delete (iPath);
			File.Delete (iPath + cDotMeta);
		}
		else
			Debug.Log("This Source File >> \"" + iPath + "\" << Is Empty");
	}
}
#endif