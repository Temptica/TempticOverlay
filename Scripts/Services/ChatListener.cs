using System.Collections.Generic;
using Godot;
using TwitcherSharp;
using TwitcherSharp.Chat;
using TwitcherSharp.EventSub;
using TwitcherSharp.EventSub.Generated.ChannelChatMessage;

namespace Temptica.Overlay.Scripts.Services;

public partial class ChatListener : Node
{
	public static ChatListener Instance { get; private set; }
	public List<string> UsersJoined { get; set; } = [];
	public TwitchEventListener<TwitchChannelChatMessageEvent> TwitchChatListener { get; private set; }
	
	[Signal] public delegate void StreamNewChatMessageEventHandler(string userName);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		TwitchChatListener = TwitchEventListener<TwitchChannelChatMessageEvent>.FromObject(GetChild(0));
		TwitchChatListener.Received += OnChatMessage;
	}

	private void OnChatMessage(TwitchChannelChatMessageEvent messageEvent)
	{
		var userName = messageEvent.ChatterUserName;
		
		if (UsersJoined.Contains(userName)) return;
		
		UsersJoined.Add(userName);
		EmitSignalStreamNewChatMessage(userName);
	}
}