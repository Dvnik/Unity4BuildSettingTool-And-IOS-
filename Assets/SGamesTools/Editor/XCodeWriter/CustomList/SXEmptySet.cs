#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class SXEmptySet : SXWriteBase
{

	public override string ToString ()
	{
		return string.Format ("NotDoAny");
	}

	protected override void DoChangeXSetting()
	{
		
	}
}
#endif