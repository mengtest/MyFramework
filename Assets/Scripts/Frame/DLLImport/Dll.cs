﻿using System;
using System.Collections.Generic;
#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
#endif

// 表示一个通过Kernel32加载的动态库对象
public class Dll : FrameBase
{
	protected Dictionary<string, Delegate> mFunctionList;	// 已经获取的动态库中的函数列表
	protected IntPtr mHandle;								// 动态库句柄
	protected string mLibraryName;							// 动态库名字
	public Dll()
	{
		mFunctionList = new Dictionary<string, Delegate>();
	}
	public void init(string name)
	{
		mLibraryName = name;
#if UNITY_STANDALONE_WIN
		mHandle = Kernel32.LoadLibrary(FrameDefine.F_PLUGINS_PATH + mLibraryName);
#endif
	}
	public void destroy()
	{
#if UNITY_STANDALONE_WIN
		Kernel32.FreeLibrary(mHandle);
#endif
	}
	public override void resetProperty()
	{
		base.resetProperty();
		mFunctionList.Clear();
		mHandle = IntPtr.Zero;
		mLibraryName = null;
	}
	public string getName() { return mLibraryName; }
	public T getFunction<T>(string funcName, Type t) where T : Delegate
	{
#if UNITY_STANDALONE_WIN
		if (!mFunctionList.TryGetValue(funcName, out Delegate value))
		{
			IntPtr api = Kernel32.GetProcAddress(mHandle, funcName);
			if(api == IntPtr.Zero)
			{
				logError("can not find function, name : " + funcName);
				return default(T);
			}
			value = Marshal.GetDelegateForFunctionPointer(api, t);
			mFunctionList.Add(funcName, value);
		}
		return value as T;
#else
		return default(T);
#endif
	}
}