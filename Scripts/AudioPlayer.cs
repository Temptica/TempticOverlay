using System;
using System.Collections.Generic;
using Godot;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts;

public partial class AudioPlayer : AudioStreamPlayer
{
	private Dictionary<AudioEffects, AudioStream> _audioStreams;
	private double deltaCumulitveTime = 0;
	public override void _Ready()
	{
		_audioStreams = new Dictionary<AudioEffects, AudioStream>
		{
			{ AudioEffects.Flash, GD.Load<AudioStream>("res://AudioFiles/flashbang.wav") },
			{ AudioEffects.GeneralKenobi,  GD.Load<AudioStream>("res://AudioFiles/general-kenobi.wav")},
			{ AudioEffects.Follow,  GD.Load<AudioStream>("res://AudioFiles/hey_listen.wav")},
			{ AudioEffects.Horn,  GD.Load<AudioStream>("res://AudioFiles/J0kerzzHorn.wav")},
			{ AudioEffects.KEKW0,  GD.Load<AudioStream>("res://AudioFiles/KEKW0.wav")},
			{ AudioEffects.KEKW1,  GD.Load<AudioStream>("res://AudioFiles/KEKW1.wav")},
			{ AudioEffects.KEKW2,  GD.Load<AudioStream>("res://AudioFiles/KEKW2.wav")},
			{ AudioEffects.Laugh,  GD.Load<AudioStream>("res://AudioFiles/Laugh.wav")},
			{ AudioEffects.Bits,  GD.Load<AudioStream>("res://AudioFiles/lilbits.wav")},
			{ AudioEffects.Mlem,  GD.Load<AudioStream>("res://AudioFiles/mlem.wav")},
			{ AudioEffects.Nom,  GD.Load<AudioStream>("res://AudioFiles/Nom.wav")},
			{ AudioEffects.Otter0,  GD.Load<AudioStream>("res://AudioFiles/Otter0.wav")},
			{ AudioEffects.Otter1,  GD.Load<AudioStream>("res://AudioFiles/Otter1.wav")},
			{ AudioEffects.Otter2,  GD.Load<AudioStream>("res://AudioFiles/Otter2.wav")},
			{ AudioEffects.Otter3,  GD.Load<AudioStream>("res://AudioFiles/Otter3.wav")},
			{ AudioEffects.Raid,  GD.Load<AudioStream>("res://AudioFiles/Raid.wav")},
			{ AudioEffects.Subscription,  GD.Load<AudioStream>("res://AudioFiles/sub_alert.wav")},
			{ AudioEffects.Water,  GD.Load<AudioStream>("res://AudioFiles/water.wav") }
		};
	}
	public void PlayAudio(AudioEffects e)
	{
		if (!_audioStreams.TryGetValue(e, out var stream)) return;
		Stream = stream;
		Play();
	}
}
