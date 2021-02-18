﻿using UnityEngine;

public class CommandMovableObjectRotateSpeedPhysics : Command
{
	public Vector3 mStartAngle;
	public Vector3 mRotateSpeed;
	public Vector3 mRotateAcceleration;
	public override void init()
	{
		base.init();
		mStartAngle = Vector3.zero;
		mRotateSpeed = Vector3.zero;
		mRotateAcceleration = Vector3.zero;
	}
	public override void execute()
	{
		Transformable obj = mReceiver as Transformable;
		TransformableComponentRotateSpeedPhysics component = obj.getComponent(out component);
		component.setActive(true);
		component.startRotateSpeed(mStartAngle, mRotateSpeed, mRotateAcceleration);
		// 需要启用组件更新时,则开启组件拥有者的更新,后续也不会再关闭
		obj.setEnable(true);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ": mStartAngle:" + mStartAngle + ", mRotateSpeed:" + mRotateSpeed + ", mRotateAcceleration:" + mRotateAcceleration;
	}
}