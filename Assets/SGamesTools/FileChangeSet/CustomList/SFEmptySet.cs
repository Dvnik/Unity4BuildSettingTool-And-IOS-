#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFEmptySet : SFChangeBase
{
	public SFEmptySet() : base()
	{
		Init ();
	}
	public override string ToString ()
	{
		return string.Format ("NotDoAny");
	}
	
	protected override void MakeMoveList()
	{

	}
}
#endif