﻿using UnityEngine;
using System.Net.Sockets;

public class CommandSocketConnectServerAcceptClient : Command
{
	public Socket mSocket;
	public string mIP;
	public override void resetProperty()
	{
		base.resetProperty();
		mSocket = null;
		mIP = null;
	}
	public override void execute()
	{
		var connectServer = mReceiver as SocketConnectServer;
		connectServer?.notifyAcceptedClient(mSocket, mIP);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ": mIP:" + mIP;
	}
}