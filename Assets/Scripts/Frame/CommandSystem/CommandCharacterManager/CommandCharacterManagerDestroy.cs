﻿using System;

public class CommandCharacterManagerDestroy : Command
{
	public ulong mGUID;
	public override void resetProperty()
	{
		base.resetProperty();
		mGUID = 0;
	}
	public override void execute()
	{
		if(mGUID != 0)
		{
			mCharacterManager.destroyCharacter(mGUID);
		}
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ": mGUID:" + mGUID;
	}
}