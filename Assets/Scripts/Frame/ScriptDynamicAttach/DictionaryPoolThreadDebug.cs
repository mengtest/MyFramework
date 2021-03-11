﻿using System.Collections.Generic;
using UnityEngine;

public class DictionaryPoolThreadDebug : MonoBehaviour
{
	public List<string> PersistentInuseList = new List<string>();
	public List<string> InuseList = new List<string>();
	public List<string> UnuseList = new List<string>();
	public void Update()
	{
		if (!FrameBase.mGameFramework.isEnableScriptDebug())
		{
			return;
		}
		PersistentInuseList.Clear();
		var persistentInuse = FrameBase.mDictionaryPoolThread.getPersistentInusedList();
		foreach (var item in persistentInuse)
		{
			PersistentInuseList.Add(item.Key + ":" + item.Value.Count);
		}

		InuseList.Clear();
		var inuse = FrameBase.mDictionaryPoolThread.getInusedList();
		foreach(var item in inuse)
		{
			InuseList.Add(item.Key + ":" + item.Value.Count);
		}

		UnuseList.Clear();
		var unuse = FrameBase.mDictionaryPoolThread.getUnusedList();
		foreach (var item in unuse)
		{
			UnuseList.Add(item.Key + ":" + item.Value.Count);
		}
	}
	//-------------------------------------------------------------------------------------------------------
}