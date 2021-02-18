﻿using UnityEngine;

public class CommandTransformableMovePhysics : Command
{
	public KeyFrameCallback mTremblingCallBack;
	public KeyFrameCallback mTrembleDoneCallBack;
	public Vector3 mStartPos;
	public Vector3 mTargetPos;
	public KEY_FRAME mKeyframe;
	public float mOnceLength;
	public float mOffset;
	public float mAmplitude;
	public bool mFullOnce;
	public bool mLoop;
	public override void init()
	{
		base.init();
		mTremblingCallBack = null;
		mTrembleDoneCallBack = null;
		mStartPos = Vector3.zero;
		mTargetPos = Vector3.zero;
		mKeyframe = KEY_FRAME.NONE;
		mOnceLength = 1.0f;
		mOffset = 0.0f;
		mAmplitude = 1.0f;
		mFullOnce = false;
		mLoop = false;
	}
	public override void execute()
	{
		Transformable obj = mReceiver as Transformable;
		TransformableComponentMovePhysics component = obj.getComponent(out component);
		component.setTremblingCallback(mTremblingCallBack);
		component.setTrembleDoneCallback(mTrembleDoneCallBack);
		component.setActive(true);
		component.setTargetPos(mTargetPos);
		component.setStartPos(mStartPos);
		component.play((int)mKeyframe, mLoop, mOnceLength, mOffset, mFullOnce, mAmplitude);
		if (component.getState() == PLAY_STATE.PLAY)
		{
			// 需要启用组件更新时,则开启组件拥有者的更新,后续也不会再关闭
			obj.setEnable(true);
		}
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ": mKeyframe:" + mKeyframe + ", mOnceLength:" + mOnceLength + ", mOffset:" + mOffset + ", mStartPos:" + mStartPos +
			", mTargetPos:" + mTargetPos + ", mLoop:" + mLoop + ", mAmplitude:" + mAmplitude + ", mFullOnce:" + mFullOnce;
	}
}