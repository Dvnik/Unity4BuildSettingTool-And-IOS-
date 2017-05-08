using UnityEngine;
using System.Collections;

public class SXDemoSet : SXWriteBase
{

	public override string ToString ()
	{
		return string.Format ("XCODE_DEMO");
	}

	protected override void DoChangeXSetting()
	{
		AddTBDFrameWork("libz");
		SXWindow.LoadPBXData(mXCodeProjectPath);
		SFileIOModfied.AddPushNotificationEntitlements(mXCodeProjectPath);
	}
}
