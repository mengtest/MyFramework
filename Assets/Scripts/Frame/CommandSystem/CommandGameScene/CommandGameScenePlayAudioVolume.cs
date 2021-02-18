﻿using System;

public class CommandGameSceneAudioVolume : Command
{
	public KeyFrameCallback mFadingCallback;
	public KeyFrameCallback mFadeDoneCallback;
	public SOUND_DEFINE mSoundVolumeCoe;    // 如果为有效值,则启用音量系数
	public KEY_FRAME mKeyframe;
	public float mStartVolume;
	public float mTargetVolume;
	public float mOnceLength;   // 持续时间
	public float mOffset;
	public float mAmplitude;
	public bool mLoop;
	public bool mFullOnce;
	public override void init()
	{
		base.init();
		mFadingCallback = null;
		mFadeDoneCallback = null;
		mSoundVolumeCoe = SOUND_DEFINE.MIN;
		mKeyframe = KEY_FRAME.NONE;
		mStartVolume = 0.0f;
		mTargetVolume = 0.0f;
		mOnceLength = 0.0f;
		mOffset = 0.0f;
		mAmplitude = 1.0f;
		mLoop = false;
		mFullOnce = true;
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		GameSceneComponentVolume component = gameScene.getComponent(out component);
		if (mSoundVolumeCoe != SOUND_DEFINE.MIN)
		{
			float volumeCoe = mAudioManager.getVolumeScale(mSoundVolumeCoe);
			mStartVolume *= volumeCoe;
			mTargetVolume *= volumeCoe;
		}
		component.setTremblingCallback(mFadingCallback);
		component.setTrembleDoneCallback(mFadeDoneCallback);
		component.setActive(true);
		component.setStartVolume(mStartVolume);
		component.setTargetVolume(mTargetVolume);
		component.play((int)mKeyframe, mLoop, mOnceLength, mOffset, mFullOnce, mAmplitude);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ": mKeyframe:" + mKeyframe + ", mOnceLength:" + mOnceLength + ", mOffset:" + mOffset + ", mStartVolume:" + mStartVolume +
			", mTargetVolume:" + mTargetVolume + ", mLoop:" + mLoop + ", mAmplitude:" + mAmplitude + ", mFullOnce:" + mFullOnce;
	}
}