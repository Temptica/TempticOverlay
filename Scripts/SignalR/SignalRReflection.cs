using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.SignalR.Listeners;

namespace Temptica.Overlay.Scripts.SignalR;

public class SignalRReflection
{
	public static List<ISignalRListener> RegisterListeners(HubConnection connection)
	{
		return Assembly
			.GetExecutingAssembly()
			.GetTypes()
			.Where(t => t
				.GetInterfaces()
				.Contains(typeof(ISignalRListener)))
			.Select(i => Activator.CreateInstance(i, connection))
			.Cast<ISignalRListener>()
			.ToList();
	}
	
}
