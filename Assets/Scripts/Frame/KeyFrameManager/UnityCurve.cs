﻿using System;
using UnityEngine;

public class UnityCurve : MyCurve
{
	protected AnimationCurve mCurve;
	public override void resetProperty()
	{
		base.resetProperty();
		mCurve = null;
	}
	public void setCurve(AnimationCurve curve) { mCurve = curve; }
	public override float Evaluate(float time)
	{
		if(mCurve == null)
		{
			return 0.0f;
		}
		return mCurve.Evaluate(time);
	}
	public override float getLength()
	{
		if (mCurve == null)
		{
			return 0.0f;
		}
		return mCurve.length;
	}
}