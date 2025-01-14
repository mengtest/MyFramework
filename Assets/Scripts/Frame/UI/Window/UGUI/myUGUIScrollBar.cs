﻿using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

// 对UGUI的ScrollBar的封装
public class myUGUIScrollBar : myUGUIObject
{
	protected Action<float, myUGUIScrollBar> mCallBack;	// 值改变的回调
	protected UnityAction<float> mThisValueCallback;	// 避免GC的委托
	protected Scrollbar mScrollBar;						// UGUI的ScrollBar组件
	public myUGUIScrollBar()
	{
		mThisValueCallback = onValueChangeCallBack;
	}
	public override void init()
	{
		base.init();
		mScrollBar = mObject.GetComponent<Scrollbar>();
		if (mScrollBar == null)
		{
			mScrollBar = mObject.AddComponent<Scrollbar>();
			// 添加UGUI组件后需要重新获取RectTransform
			mRectTransform = mObject.GetComponent<RectTransform>();
			mTransform = mRectTransform;
		}
		if (mScrollBar == null)
		{
			logError(Typeof(this) + " can not find " + typeof(Scrollbar) + ", window:" + mName + ", layout:" + mLayout.getName());
		}
	}
	public void setValue(float value) { mScrollBar.value = value; }
	public float getValue() { return mScrollBar.value; }
	public void setCallBack(Action<float, myUGUIScrollBar> callBack)
	{
		mCallBack = callBack;
		mScrollBar.onValueChanged.AddListener(mThisValueCallback);
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected void onValueChangeCallBack(float value)
	{
		mCallBack?.Invoke(value, this);
	}
}