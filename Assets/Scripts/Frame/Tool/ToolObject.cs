﻿using UnityEngine;
using System.Collections.Generic;

// 全部都是对MovableObject的操作,部分Transformable的通用操作在ToolFrame中
public class OT : FrameBase
{
	//------------------------------------------------------------------------------------------------------------------------------
	// 摄像机视角
	#region 摄像机视角
	public static void FOV(GameCamera obj, float fov)
	{
		if (obj == null)
		{
			return;
		}
		CMD(out CmdGameCameraFOV cmd, LOG_LEVEL.LOW);
		cmd.mStartFOV = fov;
		cmd.mTargetFOV = fov;
		cmd.mOnceLength = 0.0f;
		pushCommand(cmd, obj);
	}
	public static void FOV(GameCamera obj, float start, float target, float onceLength)
	{
		FOV_EX(obj, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void FOV(GameCamera obj, int keyframe, float start, float target, float onceLength)
	{
		FOV_EX(obj, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void FOV_EX(GameCamera obj, float start, float target, float onceLength, KeyFrameCallback doneCallback)
	{
		FOV_EX(obj, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, doneCallback);
	}
	public static void FOV_EX(GameCamera obj, int keyframe, float start, float target, float onceLength, bool loop, float offset, KeyFrameCallback doingCallBack, KeyFrameCallback doneCallback)
	{
		if (obj == null)
		{
			return;
		}
		CMD(out CmdGameCameraFOV cmd, LOG_LEVEL.LOW);
		cmd.mKeyframe = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mStartFOV = start;
		cmd.mTargetFOV = target;
		cmd.mOffsetTime = offset;
		cmd.mLoop = loop;
		cmd.mDoingCallback = doingCallBack;
		cmd.mDoneCallback = doneCallback;
		pushCommand(cmd, obj);
	}
	public static void ORTHO_SIZE(GameCamera obj, float start, float target, float onceLength)
	{
		ORTHO_SIZE_EX(obj, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void ORTHO_SIZE_EX(GameCamera obj, float start, float target, float onceLength, KeyFrameCallback doneCallback)
	{
		ORTHO_SIZE_EX(obj, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, doneCallback);
	}
	public static void ORTHO_SIZE_EX(GameCamera obj, int keyframe, float startFOV, float targetFOV, float onceLength, bool loop, float offset, KeyFrameCallback doingCallBack, KeyFrameCallback doneCallback)
	{
		if (obj == null)
		{
			return;
		}
		CMD(out CmdGameCameraOrthoSize cmd, LOG_LEVEL.LOW);
		cmd.mKeyframe = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mStartOrthoSize = startFOV;
		cmd.mTargetOrthoSize = targetFOV;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mDoingCallback = doingCallBack;
		cmd.mDoneCallback = doneCallback;
		pushCommand(cmd, obj);
	}
	#endregion
	//------------------------------------------------------------------------------------------------------------------------------
	// 显示
	#region 物体的显示和隐藏
	public static void ACTIVE(MovableObject obj, bool active = true)
	{
		obj?.setActive(active);
	}
	public static CmdMovableObjectActive ACTIVE_DELAY(DelayCmdWatcher watcher, MovableObject obj, bool active, float delayTime)
	{
		return ACTIVE_DELAY_EX(watcher, obj, active, delayTime, null);
	}
	public static CmdMovableObjectActive ACTIVE_DELAY_EX(DelayCmdWatcher watcher, MovableObject obj, bool active, float dealyTime, CommandCallback startCallback)
	{
		if (obj == null)
		{
			return null;
		}
		CMD_DELAY(out CmdMovableObjectActive cmd, LOG_LEVEL.LOW);
		cmd.mActive = active;
		cmd.addStartCommandCallback(startCallback);
		pushDelayCommand(cmd, obj, dealyTime, watcher);
		return cmd;
	}
	#endregion
	//------------------------------------------------------------------------------------------------------------------------------
	// 时间缩放
	#region 时间缩放
	public static void TIME(float scale)
	{
		CMD(out CmdTimeManagerScaleTime cmd, LOG_LEVEL.FORCE);
		cmd.mOnceLength = 0.0f;
		cmd.mStartScale = scale;
		cmd.mTargetScale = scale;
		pushCommand(cmd, mTimeManager);
	}
	public static void TIME(float start, float target, float onceLength)
	{
		TIME_EX(KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void TIME(int keyframe, float start, float target, float onceLength)
	{
		TIME_EX(keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void TIME(int keyframe, float start, float target, float onceLength, bool loop)
	{
		TIME_EX(keyframe, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static void TIME(int keyframe, float start, float target, float onceLength, bool loop, float offset)
	{
		TIME_EX(keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static void TIME_EX(float start, float target, float onceLength, KeyFrameCallback doneCallback)
	{
		TIME_EX(KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, doneCallback);
	}
	public static void TIME_EX(float start, float target, float onceLength, KeyFrameCallback doingCallBack, KeyFrameCallback doneCallback)
	{
		TIME_EX(KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, doingCallBack, doneCallback);
	}
	public static void TIME_EX(float start, float target, float onceLength, float offsetTime, KeyFrameCallback doneCallback)
	{
		TIME_EX(KEY_CURVE.ZERO_ONE, start, target, onceLength, false, offsetTime, null, doneCallback);
	}
	public static void TIME_EX(float start, float target, float onceLength, float offsetTime, KeyFrameCallback doingCallBack, KeyFrameCallback doneCallback)
	{
		TIME_EX(KEY_CURVE.ZERO_ONE, start, target, onceLength, false, offsetTime, doingCallBack, doneCallback);
	}
	public static void TIME_EX(int keyframe, float start, float target, float onceLength, KeyFrameCallback doingCallBack, KeyFrameCallback doneCallback)
	{
		TIME_EX(keyframe, start, target, onceLength, false, 0.0f, doingCallBack, doneCallback);
	}
	public static void TIME_EX(int keyframe, float start, float target, float onceLength, bool loop, KeyFrameCallback doingCallBack, KeyFrameCallback doneCallback)
	{
		TIME_EX(keyframe, start, target, onceLength, loop, 0.0f, doingCallBack, doneCallback);
	}
	public static void TIME_EX(int keyframe, float start, float target, float onceLength, bool loop, float offset, KeyFrameCallback doingCallBack, KeyFrameCallback doneCallback)
	{
		CMD(out CmdTimeManagerScaleTime cmd, LOG_LEVEL.LOW);
		cmd.mKeyframe = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mDoingCallBack = doingCallBack;
		cmd.mDoneCallBack = doneCallback;
		pushCommand(cmd, mTimeManager);
	}
	public static CmdTimeManagerScaleTime TIME_DELAY(DelayCmdWatcher watcher, float delayTime, float scale)
	{
		CMD_DELAY(out CmdTimeManagerScaleTime cmd, LOG_LEVEL.LOW);
		cmd.mStartScale = scale;
		cmd.mTargetScale = scale;
		cmd.mOnceLength = 0.0f;
		pushDelayCommand(cmd, mTimeManager, delayTime, watcher);
		cmd.setIgnoreTimeScale(true);
		return cmd;
	}
	public static CmdTimeManagerScaleTime TIME_DELAY(DelayCmdWatcher watcher, float delayTime, float start, float target, float onceLength)
	{
		return TIME_DELAY_EX(watcher, delayTime, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CmdTimeManagerScaleTime TIME_DELAY_EX(DelayCmdWatcher watcher, float delayTime, float start, float target, float onceLength, KeyFrameCallback moveDoneCallback)
	{
		return TIME_DELAY_EX(watcher, delayTime, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, moveDoneCallback);
	}
	public static CmdTimeManagerScaleTime TIME_DELAY_EX(DelayCmdWatcher watcher, float delayTime, float start, float target, float onceLength, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		return TIME_DELAY_EX(watcher, delayTime, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, movingCallback, moveDoneCallback);
	}
	public static CmdTimeManagerScaleTime TIME_DELAY_EX(DelayCmdWatcher watcher, float delayTime, int keyframe, float start, float target, float onceLength)
	{
		return TIME_DELAY_EX(watcher, delayTime, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CmdTimeManagerScaleTime TIME_DELAY_EX(DelayCmdWatcher watcher, float delayTime, int keyframe, float start, float target, float onceLength, bool loop)
	{
		return TIME_DELAY_EX(watcher, delayTime, keyframe, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static CmdTimeManagerScaleTime TIME_DELAY_EX(DelayCmdWatcher watcher, float delayTime, int keyframe, float start, float target, float onceLength, bool loop, float offset)
	{
		return TIME_DELAY_EX(watcher, delayTime, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static CmdTimeManagerScaleTime TIME_DELAY_EX(DelayCmdWatcher watcher, float delayTime, int keyframe, float start, float target, float onceLength, bool loop, float offset, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		CMD_DELAY(out CmdTimeManagerScaleTime cmd, LOG_LEVEL.LOW);
		cmd.mKeyframe = keyframe;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mDoingCallBack = movingCallback;
		cmd.mDoneCallBack = moveDoneCallback;
		pushDelayCommand(cmd, mTimeManager, delayTime, watcher);
		cmd.setIgnoreTimeScale(true);
		return cmd;
	}
	#endregion
	//------------------------------------------------------------------------------------------------------------------------------
	// 透明度
	#region 透明度
	public static void ALPHA(MovableObject obj, float alpha = 1.0f)
	{
		if (obj == null)
		{
			return;
		}
		CMD(out CmdMovableObjectAlpha cmd, LOG_LEVEL.LOW);
		cmd.mOnceLength = 0.0f;
		cmd.mStartAlpha = alpha;
		cmd.mTargetAlpha = alpha;
		pushCommand(cmd, obj);
	}
	public static void ALPHA(MovableObject obj, float start, float target, float onceLength)
	{
		ALPHA_EX(obj, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void ALPHA(MovableObject obj, int keyframe, float start, float target, float onceLength)
	{
		ALPHA_EX(obj, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void ALPHA(MovableObject obj, int keyframe, float start, float target, float onceLength, bool loop)
	{
		ALPHA_EX(obj, keyframe, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static void ALPHA(MovableObject obj, int keyframe, float start, float target, float onceLength, bool loop, float offset)
	{
		ALPHA_EX(obj, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static void ALPHA_EX(MovableObject obj, float start, float target, float onceLength, KeyFrameCallback doneCallback)
	{
		ALPHA_EX(obj, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, doneCallback);
	}
	public static void ALPHA_EX(MovableObject obj, float start, float target, float onceLength, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		ALPHA_EX(obj, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, doingCallback, doneCallback);
	}
	public static void ALPHA_EX(MovableObject obj, int keyframe, float start, float target, float onceLength, KeyFrameCallback doneCallback)
	{
		ALPHA_EX(obj, keyframe, start, target, onceLength, false, 0.0f, null, doneCallback);
	}
	public static void ALPHA_EX(MovableObject obj, int keyframe, float start, float target, float onceLength, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		ALPHA_EX(obj, keyframe, start, target, onceLength, false, 0.0f, doingCallback, doneCallback);
	}
	public static void ALPHA_EX(MovableObject obj, int keyframe, float start, float target, float onceLength, bool loop, float offset, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		if (obj == null)
		{
			return;
		}
		if (keyframe == KEY_CURVE.NONE || isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,请使用void ALPHA(MovableObject obj, float alpha)");
			return;
		}
		CMD(out CmdMovableObjectAlpha cmd, LOG_LEVEL.LOW);
		cmd.mKeyframe = keyframe;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.mDoingCallback = doingCallback;
		cmd.mDoneCallback = doneCallback;
		pushCommand(cmd, obj);
	}
	public static CmdMovableObjectAlpha ALPHA_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, float alpha)
	{
		if (obj == null)
		{
			return null;
		}
		CMD_DELAY(out CmdMovableObjectAlpha cmd, LOG_LEVEL.LOW);
		cmd.mOnceLength = 0.0f;
		cmd.mStartAlpha = alpha;
		cmd.mTargetAlpha = alpha;
		pushDelayCommand(cmd, obj, delayTime, watcher);
		return cmd;
	}
	public static CmdMovableObjectAlpha ALPHA_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, float start, float target, float onceLength)
	{
		return ALPHA_DELAY_EX(watcher, obj, delayTime, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CmdMovableObjectAlpha ALPHA_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, int keyframe, float start, float target, float onceLength)
	{
		return ALPHA_DELAY_EX(watcher, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CmdMovableObjectAlpha ALPHA_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, int keyframe, float start, float target, float onceLength, bool loop)
	{
		return ALPHA_DELAY_EX(watcher, obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static CmdMovableObjectAlpha ALPHA_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, int keyframe, float start, float target, float onceLength, bool loop, float offset)
	{
		return ALPHA_DELAY_EX(watcher, obj, delayTime, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static CmdMovableObjectAlpha ALPHA_DELAY_EX(DelayCmdWatcher watcher, MovableObject obj, float delayTime, float start, float target, float onceLength, KeyFrameCallback doneCallback)
	{
		return ALPHA_DELAY_EX(watcher, obj, delayTime, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, null, doneCallback);
	}
	public static CmdMovableObjectAlpha ALPHA_DELAY_EX(DelayCmdWatcher watcher, MovableObject obj, float delayTime, float start, float target, float onceLength, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		return ALPHA_DELAY_EX(watcher, obj, delayTime, KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, doingCallback, doneCallback);
	}
	public static CmdMovableObjectAlpha ALPHA_DELAY_EX(DelayCmdWatcher watcher, MovableObject obj, float delayTime, int keyframe, float start, float target, float onceLength, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		return ALPHA_DELAY_EX(watcher, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, doingCallback, doneCallback);
	}
	public static CmdMovableObjectAlpha ALPHA_DELAY_EX(DelayCmdWatcher watcher, MovableObject obj, float delayTime, int keyframe, float start, float target, float onceLength, bool loop, float offset, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		if (obj == null)
		{
			return null;
		}
		if (keyframe == KEY_CURVE.NONE || isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,CommandMovableObjectAlpha ALPHA_DELAY(MovableObject obj, float delayTime, float alpha)");
			return null;
		}
		CMD_DELAY(out CmdMovableObjectAlpha cmd, LOG_LEVEL.LOW);
		cmd.mKeyframe = keyframe;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.mDoingCallback = doingCallback;
		cmd.mDoneCallback = doneCallback;
		pushDelayCommand(cmd, obj, delayTime, watcher);
		return cmd;
	}
	#endregion
	//------------------------------------------------------------------------------------------------------------------------------
	// 以指定点列表以及时间点的路线设置物体透明度
	#region 以指定点列表以及时间点的路线设置物体透明度
	public static void ALPHA_PATH(MovableObject obj)
	{
		if (obj == null)
		{
			return;
		}
		pushCommand<CmdMovableObjectAlphaPath>(obj, LOG_LEVEL.LOW);
	}
	public static void ALPHA_PATH(MovableObject obj, Dictionary<float, float> valueKeyFrame)
	{
		ALPHA_PATH_EX(obj, valueKeyFrame, 1.0f, 1.0f, false, 0.0f, null, null);
	}
	public static void ALPHA_PATH(MovableObject obj, Dictionary<float, float> valueKeyFrame, float valueOffset)
	{
		ALPHA_PATH_EX(obj, valueKeyFrame, valueOffset, 1.0f, false, 0.0f, null, null);
	}
	public static void ALPHA_PATH(MovableObject obj, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed)
	{
		ALPHA_PATH_EX(obj, valueKeyFrame, valueOffset, speed, false, 0.0f, null, null);
	}
	public static void ALPHA_PATH(MovableObject obj, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed, bool loop)
	{
		ALPHA_PATH_EX(obj, valueKeyFrame, valueOffset, speed, loop, 0.0f, null, null);
	}
	public static void ALPHA_PATH_EX(MovableObject obj, Dictionary<float, float> valueKeyFrame, float valueOffset, KeyFrameCallback doneCallback)
	{
		ALPHA_PATH_EX(obj, valueKeyFrame, valueOffset, 1.0f, false, 0.0f, null, doneCallback);
	}
	public static void ALPHA_PATH_EX(MovableObject obj, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed, KeyFrameCallback doneCallback)
	{
		ALPHA_PATH_EX(obj, valueKeyFrame, valueOffset, speed, false, 0.0f, null, doneCallback);
	}
	public static void ALPHA_PATH_EX(MovableObject obj, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed, bool loop, float offset, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		if (obj == null)
		{
			return;
		}
		CMD(out CmdMovableObjectAlphaPath cmd, LOG_LEVEL.LOW);
		cmd.mValueKeyFrame = valueKeyFrame;
		cmd.mValueOffset = valueOffset;
		cmd.mSpeed = speed;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mDoingCallBack = doingCallback;
		cmd.mDoneCallBack = doneCallback;
		pushCommand(cmd, obj);
	}
	public static CmdMovableObjectAlphaPath ALPH_PATH_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime)
	{
		if (obj == null)
		{
			return null;
		}
		return pushDelayCommand<CmdMovableObjectAlphaPath>(watcher, obj, delayTime, LOG_LEVEL.LOW);
	}
	public static CmdMovableObjectAlphaPath ALPH_PATH_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, Dictionary<float, float> valueKeyFrame)
	{
		return ALPH_PATH_DELAY_EX(watcher, obj, delayTime, valueKeyFrame, 1.0f, 1.0f, false, 0.0f, null, null);
	}
	public static CmdMovableObjectAlphaPath ALPH_PATH_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, Dictionary<float, float> valueKeyFrame, float valueOffset)
	{
		return ALPH_PATH_DELAY_EX(watcher, obj, delayTime, valueKeyFrame, valueOffset, 1.0f, false, 0.0f, null, null);
	}
	public static CmdMovableObjectAlphaPath ALPH_PATH_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed)
	{
		return ALPH_PATH_DELAY_EX(watcher, obj, delayTime, valueKeyFrame, valueOffset, speed, false, 0.0f, null, null);
	}
	public static CmdMovableObjectAlphaPath ALPH_PATH_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed, bool loop)
	{
		return ALPH_PATH_DELAY_EX(watcher, obj, delayTime, valueKeyFrame, valueOffset, speed, loop, 0.0f, null, null);
	}
	public static CmdMovableObjectAlphaPath ALPH_PATH_DELAY(DelayCmdWatcher watcher, MovableObject obj, float delayTime, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed, bool loop, float offset)
	{
		return ALPH_PATH_DELAY_EX(watcher, obj, delayTime, valueKeyFrame, valueOffset, speed, loop, offset, null, null);
	}
	public static CmdMovableObjectAlphaPath ALPH_PATH_DELAY_EX(DelayCmdWatcher watcher, MovableObject obj, float delayTime, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed, KeyFrameCallback doneCallback)
	{
		return ALPH_PATH_DELAY_EX(watcher, obj, delayTime, valueKeyFrame, valueOffset, speed, false, 0.0f, null, doneCallback);
	}
	public static CmdMovableObjectAlphaPath ALPH_PATH_DELAY_EX(DelayCmdWatcher watcher, MovableObject obj, float delayTime, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		return ALPH_PATH_DELAY_EX(watcher, obj, delayTime, valueKeyFrame, valueOffset, speed, false, 0.0f, doingCallback, doneCallback);
	}
	public static CmdMovableObjectAlphaPath ALPH_PATH_DELAY_EX(DelayCmdWatcher watcher, MovableObject obj, float delayTime, Dictionary<float, float> valueKeyFrame, float valueOffset, float speed, bool loop, float offset, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		if (obj == null)
		{
			return null;
		}
		CMD_DELAY(out CmdMovableObjectAlphaPath cmd, LOG_LEVEL.LOW);
		cmd.mValueKeyFrame = valueKeyFrame;
		cmd.mValueOffset = valueOffset;
		cmd.mSpeed = speed;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mDoingCallBack = doingCallback;
		cmd.mDoneCallBack = doneCallback;
		pushDelayCommand(cmd, obj, delayTime, watcher);
		return cmd;
	}
	#endregion
	//------------------------------------------------------------------------------------------------------------------------------
	// 基础数据类型的渐变,Tweener的操作由于暂时没有合适的地方放,所以放在这里
	public static MyTweenerFloat TWEEN_FLOAT(float start, float target, float onceLength, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		return TWEEN_FLOAT_EX(KEY_CURVE.ZERO_ONE, start, target, onceLength, false, 0.0f, doingCallback, doneCallback);
	}
	public static MyTweenerFloat TWEEN_FLOAT_EX(int keyframe, float start, float target, float onceLength, bool loop, float offset, KeyFrameCallback doingCallback, KeyFrameCallback doneCallback)
	{
		if (keyframe == KEY_CURVE.NONE || isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,请使用void ALPHA(MovableObject obj, float alpha)");
			return null;
		}
		MyTweenerFloat tweenerFloat = mTweenerManager.createTweenerFloat();
		CMD(out CmdMyTweenerFloat cmd, LOG_LEVEL.LOW);
		cmd.mKeyframeID = keyframe;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStart = start;
		cmd.mTarget = target;
		cmd.mDoingCallBack = doingCallback;
		cmd.mDoneCallBack = doneCallback;
		pushCommand(cmd, tweenerFloat);
		return tweenerFloat;
	}
}