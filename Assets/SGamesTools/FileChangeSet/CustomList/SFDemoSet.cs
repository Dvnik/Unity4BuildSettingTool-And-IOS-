#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFDemoSet : SFChangeBase
{
	public SFDemoSet() : base()
	{
		Init ();
	}

	public override string ToString ()
	{
		return string.Format ("CHANGE_DEMO");
	}
	protected override void MakeMoveList()
	{
		BuildTarget aTarget = BuildTarget.Android;
		if( Application.platform == RuntimePlatform.Android )
			aTarget = BuildTarget.Android;
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
			aTarget = BuildTarget.iPhone;
		
		AddRemoveBaseStreamAssets(aTarget);// 
		AddDemoFTPRemoveMode();// 
		// Demo Remove AndroidManifest
		mRemoveFileList.Add(GetManifestPath(GetFilePath(mPluginPath, SFConst.cPAndroid + "/DemoFolder"), "_NoFTP"));
		// Demo RemoveFolder List
		mRemoveFolderList.Add(GetFilePath(mPluginPath, SFConst.cPAndroid + "/DemoFolder"));
		mRemoveFolderList.Add(GetFilePath(mEditorPath, "/DemoFolder"));
		// Demo ReplaceFile AndoidManifest
		string aPluginAPath = GetFilePath(mPluginPath, SFConst.cPAndroid);
		mReplaceFileList.Add(new SFMovePlaceInfo(GetManifestPath(aPluginAPath, "_Demo"),
		                                         GetManifestPath(aPluginAPath, ""),
		                                         GetManifestPath(aPluginAPath, "_BK")));
	}
}
#endif