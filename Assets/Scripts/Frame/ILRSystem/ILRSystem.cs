﻿#if USE_ILRUNTIME
using System.IO;
using ILRuntime.Mono.Cecil.Pdb;
using ILRAppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using UnityEngine;
using System.Threading;
using System.Collections;

public class ILRSystem : FrameSystem
{
	protected ILRAppDomain mAppDomain;
	protected MemoryStream mDllFile;
	protected MemoryStream mPDBFile;
	public void launchILR()
	{
		destroyILR();
		mAppDomain = new ILRAppDomain();
		mGameFramework.StartCoroutine(loadILRuntime());
	}
	public override void destroy()
	{
		destroyILR();
	}
	public ILRAppDomain getAppDomain() { return mAppDomain; }
	public void destroyILR()
	{
		mAppDomain?.Dispose();
		mDllFile?.Dispose();
		mPDBFile?.Dispose();
		mAppDomain = null;
		mDllFile = null;
		mPDBFile = null;
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected IEnumerator loadILRuntime()
	{
		// 下载dll文件
		string dllDownloadPath = availablePath(FrameDefine.ILR_FILE);
		checkDownloadPath(ref dllDownloadPath);
		WWW wwwDll = new WWW(dllDownloadPath);
		while (!wwwDll.isDone)
		{
			yield return null;
		}
		if (!string.IsNullOrEmpty(wwwDll.error))
		{
			logError(wwwDll.error + ": " + dllDownloadPath);
		}
		mDllFile = new MemoryStream(wwwDll.bytes);
		wwwDll.Dispose();
		// 下载pdb文件
#if UNITY_EDITOR
		string pdbDownloadPath = availablePath(FrameDefine.ILR_PDB_FILE);
		checkDownloadPath(ref pdbDownloadPath);
		WWW wwwPDB = new WWW(pdbDownloadPath);
		while (!wwwPDB.isDone)
		{
			yield return null;
		}
		if (!string.IsNullOrEmpty(wwwPDB.error))
		{
			logError(wwwPDB.error + ": " + pdbDownloadPath);
		}
		mPDBFile = new MemoryStream(wwwPDB.bytes);
		wwwPDB.Dispose();
#endif
		// 加载dll
		mAppDomain.LoadAssembly(mDllFile, mPDBFile, new PdbReaderProvider());
#if UNITY_EDITOR
		// 固定绑定56000端口,用于ILRuntime调试
		mAppDomain.DebugService.StartDebugService(56000);
#endif
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
		// 由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
		mAppDomain.UnityMainThreadID = Thread.CurrentThread.ManagedThreadId;
#endif
		ILRLaunchFrame.OnILRuntimeInitialized(mAppDomain);

		// 初始化完毕后开始执行热更工程中的逻辑
		ILRFrameUtility.start();
		mGameFramework.hotFixInited();
	}
}
#endif