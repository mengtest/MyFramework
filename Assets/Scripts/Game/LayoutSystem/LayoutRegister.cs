﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LayoutRegister : GameBase
{
	public static void registeAllLayout()
	{
		registeLayout<ScriptDemoStart>(LAYOUT.DEMO_START, "UIDemoStart");
		registeLayout<ScriptDemo>(LAYOUT.DEMO, "UIDemo");
		if (mLayoutManager.getLayoutCount() < (int)LAYOUT.MAX)
		{
			logError("error : not all script added! max count : " + (int)LAYOUT.MAX + ", added count :" + mLayoutManager.getLayoutCount());
		}
		GameLayout.mLayoutScriptCallback = onScriptChanged;
	}
	public static void onScriptChanged(LayoutScript script, bool created = true)
	{
		// 只有布局与脚本唯一对应的才能使用变量快速访问
		if (mLayoutManager.getScriptMappingCount(script.GetType()) > 1)
		{
			return;
		}
		if (assign(ref mScriptDemo, script, created)) return;
		if (assign(ref mScriptDemoStart, script, created)) return;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	protected static void registeLayout<T>(LAYOUT layout, string name) where T : LayoutScript
	{
		mLayoutManager.registeLayout(typeof(T), layout, name);
	}
	protected static bool assign<T>(ref T thisScript, LayoutScript value, bool created) where T : LayoutScript
	{
		if (typeof(T) == value.GetType())
		{
			thisScript = created ? value as T : null;
			return true;
		}
		return false;
	}
}