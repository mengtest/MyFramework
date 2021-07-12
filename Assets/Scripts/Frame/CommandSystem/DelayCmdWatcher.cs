﻿using System;
using System.Collections.Generic;

public class DelayCmdWatcher : FrameBase
{
	protected HashSet<long> mDelayCmdList;  // 布局显示和隐藏时的延迟命令列表,当命令执行时,会从列表中移除该命令
	protected CommandCallback mCmdCallback;
	public DelayCmdWatcher()
	{
		mDelayCmdList = new HashSet<long>();
		mCmdCallback = onCmdStarted;
	}
	public void addDelayCmd(Command cmd)
	{
		mDelayCmdList.Add(cmd.getAssignID());
		cmd.addStartCommandCallback(mCmdCallback);
	}
	public void interruptCommand(long assignID, bool showError = true)
	{
		if (mDelayCmdList.Remove(assignID))
		{
			mCommandSystem.interruptCommand(assignID, showError);
		}
	}
	public virtual void onCmdStarted(Command cmd)
	{
		if (!mDelayCmdList.Remove(cmd.getAssignID()))
		{
			logError("命令执行后移除命令失败!");
		}
	}
	public void interruptAllCommand()
	{
		foreach (var item in mDelayCmdList)
		{
			mCommandSystem.interruptCommand(item, false);
		}
		mDelayCmdList.Clear();
	}
}