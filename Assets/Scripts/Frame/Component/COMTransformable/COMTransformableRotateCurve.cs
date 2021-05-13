﻿using UnityEngine;
using System;

public class COMTransformableRotateCurve : ComponentCurve, IComponentModifyRotation
{
	//-------------------------------------------------------------------------------------------------------------
	protected override void setValue(Vector3 value)
	{
		(mComponentOwner as Transformable).setRotation(value);
	}
}