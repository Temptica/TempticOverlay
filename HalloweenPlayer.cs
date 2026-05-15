using System.Collections.Generic;
using Godot;

namespace Temptica.Overlay;

public partial class HalloweenPlayer : AudioStreamPlayer
{
    private List<AudioStream> _audioStreams = [];
    private static HalloweenPlayer _instance;
    
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_instance = this;
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/ack.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/bruh-sound-effect_WstdzdM.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/catnap-jumpscare.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/click-nice.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/five-nights-at-freddys-full-scream-sound_2.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/jump-blackpink.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/metal-pipe-clang.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/my-movie-6_0RlWMvM.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/punch-gaming-sound-effect-hd_RzlG1GE.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/relaxing-car-jumpscare-loud.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/tattletail-mama-jumpscare.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/techno-halloween.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/terror-5.mp3"));
		_audioStreams.Add(GD.Load<AudioStream>("res://AudioFiles/HalloweenSounds/yipeeeee.mp3"));
	}

	public static void PlayRandomSound()
	{
		_instance.PlaySound();
	}
	
	public void PlaySound()
	{
		var random = _audioStreams[GD.RandRange(0, _audioStreams.Count-1)];
		Stream = random;
		Play();
	}
	
}