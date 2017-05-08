#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * 2017/04 Merge By SuperGame
 * programmer : dvnik147
 * 
 * 檔案搬移的基本格式
 */
public abstract class SFChangeBase
{
	protected string mPluginPath        = string.Format ("{0}/{1}", Application.dataPath, SFConst.cPPlugins);
	protected string mEditorPath        = string.Format ("{0}/{1}", Application.dataPath, SFConst.cPEditor);
	protected string mStreamAssetsPath  = string.Format ("{0}/{1}", Application.dataPath, SFConst.cPStreamingAssets);
	/*
	 * 基本上只要在這些List中添加需要刪除的檔案路徑
	 * DoFileMove()就會掃過這些List的內容做刪除的動作
	 * Base的Function很多都是節省添加列表的重複輸入
	 */
	protected List<string> mRemoveFolderList;// 要刪除的資料夾
	protected List<string> mRemoveFileList;// 要刪除的檔案
	protected List<SFSinglFRFInfo> mRemoveSFFList;// 要刪除的資料夾，但跳過特定檔案
	protected List<SFMovePlaceInfo> mCopyFileList;// 複製移動的檔案
	protected List<SFMovePlaceInfo> mMoveFileList;// 純移動的檔案
	protected List<SFMovePlaceInfo> mReplaceFileList;// 要覆蓋的檔案, FromePath = 要覆蓋的檔案, ToPath = 被覆蓋的原始擋
	
	protected abstract void MakeMoveList();// 讓繼承的物件記得去寫搬移的詳細內容
	/// <summary>
	/// 執行檔案搬移
	/// </summary>
	public void DoFileMove()
	{
		SFileMove.Self.RemoveFolder(mRemoveFolderList);
		
		foreach(var vInfo in mRemoveFileList)
			SFileMove.Self.RemoveSourceAndMetaFile(vInfo);
		
		foreach(var vInfo in mRemoveSFFList)
			SFileMove.Self.RemoveFolderFiles(vInfo.FolderPath, vInfo.SkipFiles);
		
		foreach(var vInfo in mCopyFileList)
			SFileMove.Self.CopyMoveFolderLists(vInfo.FromePath, vInfo.ToPath, vInfo.BakupPath);
		
		foreach(var vInfo in mMoveFileList)
			SFileMove.Self.MoveFolder(vInfo.FromePath, vInfo.ToPath);
		
		foreach(var vInfo in mReplaceFileList)
			SFileMove.Self.ReplaceFile(vInfo.FromePath, vInfo.ToPath, vInfo.BakupPath);
		
		AssetDatabase.Refresh();
	}
	/// <summary>
	/// 初始化
	/// </summary>
	protected virtual void Init()
	{
		mRemoveFolderList = new List<string>();
		mRemoveFileList = new List<string>();
		mRemoveSFFList = new List<SFSinglFRFInfo>();
		mCopyFileList  = new List<SFMovePlaceInfo>();
		mMoveFileList  = new List<SFMovePlaceInfo>();
		mReplaceFileList  = new List<SFMovePlaceInfo>();
		
		MakeMoveList();
	}
	/// <summary>
	/// 刪除列表加入一個StramAssets中要刪除的資料夾
	/// 有的專案會設計StramAssets底下有Android和IOS兩個不同的資源檔結構
	/// 為這樣的設計寫的一個刪除方式
	/// </summary>
	protected void AddRemoveBaseStreamAssets(BuildTarget iTarget)
	{
		switch(iTarget)
		{
		case BuildTarget.Android: mRemoveFolderList.Add(string.Format("{0}/{1}", mStreamAssetsPath, SFConst.cPIOS)); break;
		case BuildTarget.iPhone: mRemoveFolderList.Add(string.Format("{0}/{1}", mStreamAssetsPath, SFConst.cPAndroid)); break;
		}
		AddRemovePerPluginList();
	}
	/// <summary>
	/// 範例：刪除專案底下的資源檔
	/// </summary>
	protected void AddDemoFTPRemoveMode()
	{
		// Demo Remove Folder
		mRemoveFolderList.Add(GetFilePath(mStreamAssetsPath, SFConst.cPAndroid + "/Animation"));
		mRemoveFolderList.Add(GetFilePath(mStreamAssetsPath, SFConst.cPAndroid + "/Room"));
		// Demo Remove Folder By Files
		string aFilePath = "";
		List<string> aTmpSkipFile = new List<string>();
		aFilePath = GetFilePath(mStreamAssetsPath, SFConst.cPAndroid);
		mRemoveSFFList.Add(new SFSinglFRFInfo(aFilePath, aTmpSkipFile.ToArray()));
		// Demo Remove Folder By Files add SkipFile
		aTmpSkipFile.Clear();
		aFilePath = GetFilePath(mStreamAssetsPath, SFConst.cPAndroid + "/Sound");
		aTmpSkipFile.Add("BGM005.unity3d");
		mRemoveSFFList.Add(new SFSinglFRFInfo(aFilePath, aTmpSkipFile.ToArray()));
	}
	/// <summary>
	/// 範例：刪除Plugin/Android底下的特定資料夾
	/// </summary>
	protected void AddRemovePerPluginList()
	{
		mRemoveFolderList.Add(GetFilePath(mPluginPath, SFConst.cPAndroid + "/assets"));
		mRemoveFolderList.Add(GetFilePath(mPluginPath, SFConst.cPAndroid + "/libs/arm64-v8a"));
		mRemoveFolderList.Add(GetFilePath(mPluginPath, SFConst.cPAndroid + "/libs/armeabi"));
		mRemoveFolderList.Add(GetFilePath(mPluginPath, SFConst.cPAndroid + "/libs/armeabi-v7a"));
		mRemoveFolderList.Add(GetFilePath(mPluginPath, SFConst.cPAndroid + "/libs/x86"));
	}
	/// <summary>
	/// 檔案路徑合併
	/// </summary>
	protected string GetFilePath(string iPath1, string iPath2)
	{
		string aResult = string.Format("{0}/{1}", iPath1, iPath2);
		
		return aResult;
	}
	/// <summary>
	/// 範例：取得某路徑下的AndroidManifest檔
	/// </summary>
	/// <returns>返回結果</returns>
	/// <param name="iPath">路徑</param>
	/// <param name="iULTag">如果有多個AndroidManifest，或許會用AndroidManifest_XXX的加上Tag</param>
	protected string GetManifestPath(string iPath, string iULTag)
	{
		string aResult = string.Format("{0}/{1}", iPath, SFConst.cAndroidManifest + iULTag + SFConst.cDotXML);
		return aResult;
	}
}

public class SFSinglFRFInfo// SinglFolderRemoveFiles
{
	public string FolderPath;
	public string[] SkipFiles;
	
	public SFSinglFRFInfo(string iPath, string[] iSkipFiles)
	{
		FolderPath = iPath;
		SkipFiles = iSkipFiles;
	}
	
}

public class SFMovePlaceInfo
{
	public string FromePath;
	public string ToPath;
	public string BakupPath;
	
	public SFMovePlaceInfo(string iFrome, string iTo, string iBackup)
	{
		FromePath = iFrome;
		ToPath = iTo;
		BakupPath = iBackup;
	}
}
#endif