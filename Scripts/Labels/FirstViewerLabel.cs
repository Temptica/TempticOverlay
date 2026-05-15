using Godot;
using Temptica.Overlay.Scripts.Services;


namespace Temptica.Overlay.Scripts.Labels;

public partial class FirstViewerLabel : Label3D
{
	private static string _textToSet = "It's definitely Noor!";
	private static bool _textSet;
	
	public override async void _Ready()
	{
		ChatListener.Instance.StreamNewChatMessage += SetFirst;
		Text = await new ApiService().GetFirstViewer();
	}

	private void SetFirst(string userName)
	{
		if(_textSet) return;
		_textToSet = userName;
		Text = _textToSet;
		_textSet = true;
	}
}
