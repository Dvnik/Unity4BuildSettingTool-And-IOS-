#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

public class SXGetCustom
{
	public static List<SXWriteBase> GetBaseTypeList()
	{
		List<SXWriteBase> aResult = new List<SXWriteBase>();

		aResult.Add(new SXDemoSet());
		aResult.Add(new SXEmptySet());
		return aResult;
	}
}
#endif
