#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

public class SFGetCustom
{
	public static List<SFChangeBase> GetBaseTypeList()
	{
		List<SFChangeBase> aResult = new List<SFChangeBase>();

		aResult.Add(new SFDemoSet());
		aResult.Add(new SFEmptySet());
		return aResult;
	}
}
#endif
