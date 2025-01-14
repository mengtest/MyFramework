﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 从AssetBundle中加载资源
public class AssetBundleLoader : FrameBase
{
	protected Dictionary<Object, AssetBundleInfo> mAssetToAssetBundleInfo;	// 根据加载的Asset查找所属AssetBundle的列表
	protected Dictionary<string, AssetBundleInfo> mAssetBundleInfoList;		// 根据名字查找AssetBundle的列表,此名字不含后缀
	protected Dictionary<string, AssetBundleInfo> mRequestAssetList;		// 请求异步加载的Asset列表
	protected Dictionary<string, AssetInfo> mAssetToBundleInfo;				// 根据资源文件名查找Asset信息的列表,初始化时就会填充此列表
	protected List<AssetBundleInfo> mRequestBundleList;						// 请求异步加载的AssetBundle列表
	protected HashSet<Coroutine> mCoroutineList;							// 当前的协程列表
	protected WaitForEndOfFrame mWaitForEndOfFrame;							// 用于避免GC
	protected int mAssetBundleCoroutineCount;								// 当前加载AssetBundle的协程数量
	protected int mAssetCoroutineCount;										// 当前加载Asset的协程数量
	protected bool mAutoLoad;												// 当资源可用时是否自动初始化AssetBundle
	protected bool mInited;													// AssetBundleLoader是否已经初始化
	protected const int ASSET_BUNDLE_COROUTINE = 8;							// 加载AssetBundle的协程最大数量
	protected const int ASSET_COROUTINE = 4;								// 加载Asset的协程最大数量
	public AssetBundleLoader()
	{
		mAssetToAssetBundleInfo = new Dictionary<Object, AssetBundleInfo>();
		mAssetBundleInfoList = new Dictionary<string, AssetBundleInfo>();
		mRequestAssetList = new Dictionary<string, AssetBundleInfo>();
		mAssetToBundleInfo = new Dictionary<string, AssetInfo>();
		mRequestBundleList = new List<AssetBundleInfo>();
		mCoroutineList = new HashSet<Coroutine>();
		mWaitForEndOfFrame = new WaitForEndOfFrame();
		mAutoLoad = true;
	}
	public override void resetProperty()
	{
		base.resetProperty();
		mAssetToAssetBundleInfo.Clear();
		mAssetBundleInfoList.Clear();
		mRequestAssetList.Clear();
		mAssetToBundleInfo.Clear();
		mRequestBundleList.Clear();
		mCoroutineList.Clear();
		// mWaitForEndOfFrame不重置
		// mWaitForEndOfFrame = null;
		mAutoLoad = true;
		mInited = false;
		mAssetBundleCoroutineCount = 0;
		mAssetCoroutineCount = 0;
	}
	public void resourceAvailable()
	{
		if (!mAutoLoad)
		{
			return;
		}

		// 卸载所有已加载的AssetBundle
		unloadAll();
		// 加载AssetBundle的配置文件
		int fileSize = openFile(availableReadPath("StreamingAssets.bytes"), out byte[] fileBuffer, false);
		if (fileBuffer != null)
		{
			initAssetConfig(fileBuffer, fileSize);
		}
		releaseFile(fileBuffer);
	}
	public void setAutoLoad(bool autoLoad) { mAutoLoad = autoLoad; }
	public void update()
	{
		if (!mInited)
		{
			return;
		}
		// 处理资源包异步加载请求
		if (mRequestBundleList.Count > 0 && mAssetBundleCoroutineCount < ASSET_BUNDLE_COROUTINE)
		{
			// 找到第一个依赖项已经加载完毕的资源
			for (int i = 0; i < mRequestBundleList.Count; ++i)
			{
				// 因为新的加载请求是加入到列表的末尾,所以不会影响当前的遍历顺序
				AssetBundleInfo info = mRequestBundleList[i];
				info.loadParentAsync();
				if (info.isAllParentLoaded())
				{
					mCoroutineList.Add(mGameFramework.StartCoroutine(loadAssetBundleCoroutine(info)));
					mRequestBundleList.RemoveAt(i);
					break;
				}
			}
		}
		// 处理资源异步加载请求
		if (mRequestAssetList.Count > 0 && mAssetCoroutineCount < ASSET_COROUTINE)
		{
			foreach (var item in mRequestAssetList)
			{
				mCoroutineList.Add(mGameFramework.StartCoroutine(loadAssetCoroutine(item.Value, item.Key)));
				mRequestAssetList.Remove(item.Key);
				break;
			}
		}
	}
	public void destroy()
	{
		unloadAll();
	}
	public void unloadAll()
	{
		foreach (var item in mCoroutineList)
		{
			mGameFramework.StopCoroutine(item);
		}
		mCoroutineList.Clear();
		mAssetBundleCoroutineCount = 0;
		mAssetCoroutineCount = 0;
		// 还未开始加载的异步加载资源需要从等待列表中移除
		mRequestAssetList.Clear();
		mRequestBundleList.Clear();
		mAssetToAssetBundleInfo.Clear();
		foreach (var item in mAssetBundleInfoList)
		{
			item.Value.unload();
		}
	}
	public bool unloadAsset<T>(ref T asset) where T : Object
	{
		// 查找对应的AssetBundle
		if (!mAssetToAssetBundleInfo.TryGetValue(asset, out AssetBundleInfo info))
		{
			return false;
		}
		bool ret = info.unloadAsset(asset);
		if (ret)
		{
			mAssetToAssetBundleInfo.Remove(asset);
			asset = null;
		}
		return ret;
	}
	public bool isInited() { return mInited; }
	public Dictionary<string, AssetBundleInfo> getAssetBundleInfoList() { return mAssetBundleInfoList; }
	public AssetBundleInfo getAssetBundleInfo(string name)
	{
		// 因为在初始化过程中需要调用该函数,所以此处不检测是否初始化完成
		mAssetBundleInfoList.TryGetValue(name, out AssetBundleInfo info);
		return info;
	}
	public void unloadAssetBundle(string bundleName)
	{
		if (!mAssetBundleInfoList.TryGetValue(bundleName, out AssetBundleInfo info))
		{
			return;
		}
		var assetList = info.getAssetList();
		foreach (var item in assetList)
		{
			mAssetToAssetBundleInfo.Remove(item.Value.getAsset());
		}
		info.unload();
	}
	// 卸载指定路径中的所有资源包
	public void unloadPath(string path)
	{
		LIST(out List<string> tempList);
		foreach (var item in mAssetBundleInfoList)
		{
			if (!startWith(item.Key, path))
			{
				continue;
			}
			tempList.Clear();
			// 还未开始加载的异步加载资源需要从等待列表中移除
			tempList.AddRange(mRequestAssetList.Keys);
			int count = tempList.Count;
			for (int i = 0; i < count; ++i)
			{
				string assetName = tempList[i];
				if (mRequestAssetList[assetName] == item.Value)
				{
					mRequestAssetList.Remove(assetName);
				}
			}
			mRequestBundleList.Remove(item.Value);
			item.Value.unload();
		}
		UN_LIST(tempList);
	}
	// 得到文件夹中的所有文件,文件夹被打包成一个AssetBundle,返回AssetBundle中的所有资源名
	public void getFileList(string path, List<string> list)
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return;
		}
		removeEndSlash(ref path);
		list.Clear();
		// 该文件夹被打包成一个AssetBundle
		foreach (var item in mAssetBundleInfoList)
		{
			if (!startWith(item.Key, path))
			{
				continue;
			}
			var assetList = item.Value.getAssetList();
			foreach (var asset in assetList)
			{
				list.Add(getFileNameNoSuffix(asset.Key));
			}
		}
	}
	// 资源是否已经加载
	public bool isAssetLoaded<T>(string fileName) where T : UnityEngine.Object
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return false;
		}
		LIST(out List<string> fileNameList);
		mResourceManager.adjustResourceName<T>(fileName, fileNameList);
		// 只返回第一个找到的资源
		int count = fileNameList.Count;
		for (int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			// 找不到资源则直接返回
			if (!mAssetToBundleInfo.TryGetValue(fileNameWithSuffix, out AssetInfo asset))
			{
				continue;
			}
			UN_LIST(fileNameList);
			AssetBundleInfo bundleInfo = asset.getAssetBundle();
			if (bundleInfo.getLoadState() != LOAD_STATE.LOADED)
			{
				return false;
			}
			return bundleInfo.getAssetInfo(fileNameWithSuffix).isLoaded();
		}
		UN_LIST(fileNameList);
		return false;
	}
	// 获得资源,如果资源包未加载,则返回空
	public T getAsset<T>(string fileName) where T : UnityEngine.Object
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return null;
		}
		LIST(out List<string> fileNameList);
		mResourceManager.adjustResourceName<T>(fileName, fileNameList);
		// 只返回第一个找到的资源
		T res = null;
		int count = fileNameList.Count;
		for (int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			if (mAssetToBundleInfo.TryGetValue(fileNameWithSuffix, out AssetInfo info))
			{
				res = info.getAssetBundle().getAssetInfo(fileNameWithSuffix).getAsset() as T;
				break;
			}
		}
		UN_LIST(fileNameList);
		return res;
	}
	// 检查指定的已加载的AssetBundle的依赖项是否有未加载的情况,如果有未加载的则同步加载
	public void checkAssetBundleDependenceLoaded(string bundleName)
	{
		if (!mInited)
		{
			return;
		}
		mAssetBundleInfoList.TryGetValue(bundleName, out AssetBundleInfo info);
		info?.checkAssetBundleDependenceLoaded();
	}
	// 异步加载资源包
	public void loadAssetBundleAsync(string bundleName, AssetBundleLoadCallback callback, object userData = null)
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			callback?.Invoke(null, userData);
			return;
		}
		if (mAssetBundleInfoList.TryGetValue(bundleName, out AssetBundleInfo info))
		{
			info.loadAssetBundleAsync(callback, userData);
		}
		else
		{
			logError("can not find AssetBundle : " + bundleName);
		}
	}
	// 同步加载资源包
	public void loadAssetBundle(string bundleName, List<UnityEngine.Object> assetList)
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return;
		}
		if (mAssetBundleInfoList.TryGetValue(bundleName, out AssetBundleInfo bundleInfo))
		{
			if (bundleInfo.getLoadState() == LOAD_STATE.LOADING ||
				bundleInfo.getLoadState() == LOAD_STATE.WAIT_FOR_LOAD)
			{
				logError("asset bundle is loading or waiting for load, can not load again! name : " + bundleName);
				return;
			}
			// 如果还未加载,则加载资源包
			if (bundleInfo.getLoadState() == LOAD_STATE.UNLOAD)
			{
				bundleInfo.loadAssetBundle();
			}
			// 加载完毕,返回资源列表
			if (bundleInfo.getLoadState() == LOAD_STATE.LOADED)
			{
				if (assetList == null)
				{
					return;
				}
				var bundleAssetlist = bundleInfo.getAssetList();
				foreach (var item in bundleAssetlist)
				{
					if (item.Value.isLoaded())
					{
						assetList.Add(item.Value.getAsset());
					}
				}
				return;
			}
		}
		return;
	}
	// 异步加载资源,文件名称,不带后缀,Resources下的相对路径
	public bool loadSubAssetAsync<T>(string fileName, AssetLoadDoneCallback callback, object userData = null) where T : UnityEngine.Object
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return false;
		}
		LIST(out List<string> fileNameList);
		mResourceManager.adjustResourceName<T>(fileName, fileNameList);
		// 只加载第一个找到的资源,所以不允许有重名的同类资源
		int loadedCount = 0;
		int count = fileNameList.Count;
		for (int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			if (!mAssetToBundleInfo.TryGetValue(fileNameWithSuffix, out AssetInfo asset))
			{
				continue;
			}
			if (asset.getAssetBundle().loadSubAssetAsync(ref fileNameWithSuffix, callback, fileName, userData))
			{
				++loadedCount;
				break;
			}
		}
		UN_LIST(fileNameList);
		return loadedCount != 0;
	}
	// 同步加载资源,文件名称,不带后缀,Resources下的相对路径
	public UnityEngine.Object[] loadSubAsset<T>(string fileName) where T : UnityEngine.Object
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return null;
		}
		LIST(out List<string> fileNameList);
		mResourceManager.adjustResourceName<T>(fileName, fileNameList);
		// 只加载第一个找到的资源,所以不允许有重名的同类资源
		UnityEngine.Object[] res = null;
		int count = fileNameList.Count;
		for (int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			if (!mAssetToBundleInfo.TryGetValue(fileNameWithSuffix, out AssetInfo asset))
			{
				continue;
			}
			res = asset.getAssetBundle().loadSubAsset(ref fileNameWithSuffix);
			if (res != null)
			{
				break;
			}
		}
		UN_LIST(fileNameList);
		return res;
	}
	// 同步加载资源,文件名称,不带后缀,Resources下的相对路径
	public T loadAsset<T>(string fileName) where T : UnityEngine.Object
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return null;
		}
		LIST(out List<string> fileNameList);
		mResourceManager.adjustResourceName<T>(fileName, fileNameList);
		// 只加载第一个找到的资源,所以不允许有重名的同类资源
		T res = null;
		int count = fileNameList.Count;
		for (int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			if (!mAssetToBundleInfo.TryGetValue(fileNameWithSuffix, out AssetInfo asset))
			{
				continue;
			}
			res = asset.getAssetBundle().loadAsset<T>(ref fileNameWithSuffix);
			if (res != null)
			{
				break;
			}
		}
		UN_LIST(fileNameList);
		return res;
	}
	// 异步加载资源
	public bool loadAssetAsync<T>(string fileName, AssetLoadDoneCallback doneCallback, object userData = null) where T : UnityEngine.Object
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return false;
		}
		LIST(out List<string> fileNameList);
		mResourceManager.adjustResourceName<T>(fileName, fileNameList);
		// 只加载第一个找到的资源,所以不允许有重名的同类资源
		int loadedCount = 0;
		int count = fileNameList.Count;
		for (int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			if (!mAssetToBundleInfo.TryGetValue(fileNameWithSuffix, out AssetInfo asset))
			{
				continue;
			}
			if (asset.getAssetBundle().loadAssetAsync(fileNameWithSuffix, doneCallback, fileName, userData))
			{
				++loadedCount;
				break;
			}
		}
		UN_LIST(fileNameList);
		return loadedCount != 0;
	}
	// 请求异步加载资源包
	public void requestLoadAssetBundle(AssetBundleInfo bundleInfo)
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return;
		}
		if (!mRequestBundleList.Contains(bundleInfo))
		{
			mRequestBundleList.Add(bundleInfo);
		}
	}
	public void requestLoadAsset(AssetBundleInfo bundleInfo, string fileNameWithSuffix)
	{
		if (!mInited)
		{
			logError("AssetBundleLoader is not inited!");
			return;
		}
		if (!mRequestAssetList.ContainsKey(fileNameWithSuffix))
		{
			mRequestAssetList.Add(fileNameWithSuffix, bundleInfo);
		}
	}
	public void notifyAssetLoaded(Object asset, AssetBundleInfo bundle)
	{
		// 保存加载出的资源与资源包的信息
		if (asset != null && !mAssetToAssetBundleInfo.ContainsKey(asset))
		{
			mAssetToAssetBundleInfo.Add(asset, bundle);
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected IEnumerator loadAssetBundleCoroutine(AssetBundleInfo bundleInfo)
	{
		++mAssetBundleCoroutineCount;
		// 先确保依赖项全部已经加载完成,才能开始加载当前请求的资源包
		while (!bundleInfo.isAllParentLoaded())
		{
			bundleInfo.loadParentAsync();
			yield return null;
		}
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		log(bundleInfo.getBundleFileName() + " start load bundle", LOG_LEVEL.NORMAL);
#endif
		bundleInfo.setLoadState(LOAD_STATE.LOADING);
		AssetBundle assetBundle = null;
		string bundleFileName = bundleInfo.getBundleFileName();
		AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(availableReadPath(bundleFileName));
		if (request != null)
		{
			yield return request;
			assetBundle = request.assetBundle;
		}
		else
		{
			logError("can not load asset bundle async : " + bundleFileName);
		}
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		if (assetBundle != null)
		{
			log(bundleFileName + " load bundle done", LOG_LEVEL.NORMAL);
		}
#endif
		yield return mWaitForEndOfFrame;
		// 通知AssetBundleInfo
		bundleInfo.notifyAssetBundleAsyncLoaded(assetBundle);
		--mAssetBundleCoroutineCount;
	}
	protected IEnumerator loadAssetCoroutine(AssetBundleInfo bundle, string fileNameWithSuffix)
	{
		++mAssetCoroutineCount;
		// 只有等资源所属的AssetBundle加载完毕以后才能开始加载其中的单个资源
		if (bundle.getLoadState() != LOAD_STATE.LOADED)
		{
			logError("asset bundle is not loaded, can not load asset async!");
			--mAssetCoroutineCount;
			yield break;
		}

		// 异步从资源包中加载资源
		bundle.getAssetInfo(fileNameWithSuffix).setLoadState(LOAD_STATE.LOADING);
		if (bundle.getAssetBundle() != null)
		{
			string assetPath = FrameDefine.P_GAME_RESOURCES_PATH + fileNameWithSuffix;
			AssetBundleRequest assetRequest = bundle.getAssetBundle().LoadAssetWithSubAssetsAsync(assetPath);
			if (assetRequest == null)
			{
				logError("can not load asset async : " + fileNameWithSuffix);
				--mAssetCoroutineCount;
				yield break;
			}
			yield return assetRequest;
			bundle.notifyAssetLoaded(fileNameWithSuffix, assetRequest.allAssets);
		}
		else
		{
			bundle.notifyAssetLoaded(fileNameWithSuffix, null);
		}
		--mAssetCoroutineCount;
	}
	protected void initAssetConfig(byte[] fileBuffer, int fileSize)
	{
		mInited = false;
		ARRAY(out byte[] tempStringBuffer, 256);
		mAssetBundleInfoList.Clear();
		mAssetToBundleInfo.Clear();
		CLASS_ONCE(out SerializerRead serializer);
		serializer.init(fileBuffer, fileSize);
		serializer.read(out int assetBundleCount);
		for (int i = 0; i < assetBundleCount; ++i)
		{
			// AssetBundle名字
			serializer.readString(tempStringBuffer, tempStringBuffer.Length);
			string bundleName = getFileNameNoSuffix(bytesToString(tempStringBuffer));
			if (!mAssetBundleInfoList.TryGetValue(bundleName, out AssetBundleInfo bundleInfo))
			{
				bundleInfo = new AssetBundleInfo(bundleName);
				mAssetBundleInfoList.Add(bundleName, bundleInfo);
			}
			// AssetBundle包含的所有Asset的名字
			serializer.read(out int assetCount);
			for (int k = 0; k < assetCount; ++k)
			{
				serializer.readString(tempStringBuffer, tempStringBuffer.Length);
				string assetName = bytesToString(tempStringBuffer);
				bundleInfo.addAssetName(assetName);
				mAssetToBundleInfo.Add(assetName, bundleInfo.getAssetInfo(assetName));
			}
			// AssetBundle的所有依赖项
			serializer.read(out int depCount);
			for (int j = 0; j < depCount; ++j)
			{
				serializer.readString(tempStringBuffer, tempStringBuffer.Length);
				bundleInfo.addParent(getFileNameNoSuffix(bytesToString(tempStringBuffer)));
			}
		}
		UN_CLASS(serializer);
		UN_ARRAY(tempStringBuffer);
		// 配置清单解析完毕后,为每个AssetBundleInfo查找对应的依赖项
		foreach (var info in mAssetBundleInfoList)
		{
			info.Value.findAllDependence();
		}
		mInited = true;
		logForce("AssetBundle初始化完成, AssetBundle count : " + mAssetBundleInfoList.Count);
	}
}