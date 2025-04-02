using Godot;
using System;
using System.Collections.Generic;
using Temptic404Overlay.Scripts;
using Temptica.TwitchBot.Shared.enums;

public partial class SpamAudioPlayer : AudioStreamPlayer3D
{
    private Dictionary<AudioEffects, AudioStream> _audioStreams => AudioPlayer.AudioStreams;
    
    public static List<AudioEffects> NextStream { get; set; } = [];
    private AudioStreamPlayback _stream;
    
    public static AudioEffects[] Spamables =
    [
        AudioEffects.Screenshot, AudioEffects.Pop, AudioEffects.Otter0, AudioEffects.Otter1,AudioEffects.Otter2,AudioEffects.Otter3,AudioEffects.Fireworks
    ];

    public override void _Ready()
    {
        Stream = new AudioStreamPolyphonic();
        ((AudioStreamPolyphonic)Stream).Polyphony = 20;
    }

    public override void _Process(double delta)
    {
        if(NextStream.Count == 0) return;
        if (!_audioStreams.TryGetValue(NextStream[0], out var stream)) return;

        if (!Playing)
        {
            Play();
        }
        
        if (_stream == null) _stream = GetStreamPlayback();
        
        if (_stream is not AudioStreamPlaybackPolyphonic polyphonic) return;
        
        polyphonic.PlayStream(stream);
        NextStream.RemoveAt(0);

    }
}
