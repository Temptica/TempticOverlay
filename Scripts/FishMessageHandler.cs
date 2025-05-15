using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

namespace Temptica.Overlay.Scripts;

public partial class FishMessageHandler : MeshInstance3D
{
	public override void _Ready()
	{
		FishMessageListener.FishMessage += FishMessage;
	}

	private void FishMessage(object sender, string e)
	{
		// Creates a Node3D object with a width of 200. Which will have a Label3D with the message in it. It has some margin around the text.
		// text is autowrapped and added to list. messages despawn after 10 second. new messages get added under it.
		var label = new Label3D();
		label.Text = e;
		label.Width = 190;
		label.AutowrapMode = TextServer.AutowrapMode.Word;
		label.Set("position", new Vector3(5, 5, 0));
		AddChild(label);
		
		var messages = GetChildren().Select(node=>(Label3D)node).ToList();
		var newHeight = messages.Sum(x => x.GetAabb().Size.Y);
		var oldSize = label.Get("size").AsVector3();
		Mesh.Set("size", new Vector3(oldSize.X, newHeight, oldSize.Z));
		
		//remove messages that are older than 10 seconds
		Task.Run(async () =>
		{
			await Task.Delay(10000);
			RemoveChild(label);
		});
	}
}
