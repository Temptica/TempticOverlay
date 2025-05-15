using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.SignalR.Listeners;
using Temptica.Overlay.Scripts.Spotify;
using Temptica.TwitchBot.Shared.enums;

namespace Temptica.Overlay.Scripts;

public partial class AudioPlayer : AudioStreamPlayer
{
    public static Dictionary<AudioEffects, AudioStream> AudioStreams;
    private double deltaCumulitveTime = 0;
    private Alerts.Glow _glow;
    private static List<AudioEffects> NextStream { get; set; } = [];

    [Export] SpamAudioPlayer SpamAudioPlayer { get; set; }

    public override void _Ready()
    {
        AudioStreams = new Dictionary<AudioEffects, AudioStream>
        {
            { AudioEffects.Flash, GD.Load<AudioStream>("res://AudioFiles/flashbang.wav") },
            { AudioEffects.GeneralKenobi, GD.Load<AudioStream>("res://AudioFiles/general-kenobi.wav") },
            { AudioEffects.Follow, GD.Load<AudioStream>("res://AudioFiles/hey_listen.wav") },
            { AudioEffects.Horn, GD.Load<AudioStream>("res://AudioFiles/J0kerzzHorn.wav") },
            { AudioEffects.KEKW0, GD.Load<AudioStream>("res://AudioFiles/KEKW0.wav") },
            { AudioEffects.KEKW1, GD.Load<AudioStream>("res://AudioFiles/KEKW1.wav") },
            { AudioEffects.KEKW2, GD.Load<AudioStream>("res://AudioFiles/KEKW2.wav") },
            { AudioEffects.Laugh, GD.Load<AudioStream>("res://AudioFiles/Laugh.wav") },
            { AudioEffects.Bits, GD.Load<AudioStream>("res://AudioFiles/lilbits.wav") },
            { AudioEffects.Mlem, GD.Load<AudioStream>("res://AudioFiles/mlem.wav") },
            { AudioEffects.Nom, GD.Load<AudioStream>("res://AudioFiles/Nom.wav") },
            { AudioEffects.Otter0, GD.Load<AudioStream>("res://AudioFiles/Otter0.wav") },
            { AudioEffects.Otter1, GD.Load<AudioStream>("res://AudioFiles/Otter1.wav") },
            { AudioEffects.Otter2, GD.Load<AudioStream>("res://AudioFiles/Otter2.wav") },
            { AudioEffects.Otter3, GD.Load<AudioStream>("res://AudioFiles/Otter3.wav") },
            { AudioEffects.Raid, GD.Load<AudioStream>("res://AudioFiles/Raid.wav") },
            { AudioEffects.Subscription, GD.Load<AudioStream>("res://AudioFiles/sub_alert.wav") },
            { AudioEffects.Water, GD.Load<AudioStream>("res://AudioFiles/water.wav") },
            { AudioEffects.Eww, GD.Load<AudioStream>("res://AudioFiles/BrotherEw.wav") },
            { AudioEffects.Fart, GD.Load<AudioStream>("res://AudioFiles/fart.mp3") },
            { AudioEffects.Bell, GD.Load<AudioStream>("res://AudioFiles/bell.mp3") },
            { AudioEffects.Vine, GD.Load<AudioStream>("res://AudioFiles/vine.mp3") },
            { AudioEffects.Wow, GD.Load<AudioStream>("res://AudioFiles/wow.mp3") },
            { AudioEffects.Pew, GD.Load<AudioStream>("res://AudioFiles/pew.mp3") },
            { AudioEffects.Punch, GD.Load<AudioStream>("res://AudioFiles/punch.mp3") },
            { AudioEffects.Pikmin, GD.Load<AudioStream>("res://AudioFiles/pikmin.mp3") },
            { AudioEffects.Whip, GD.Load<AudioStream>("res://AudioFiles/whip.mp3") },
            { AudioEffects.Bober, GD.Load<AudioStream>("res://AudioFiles/bober.mp3") },
            { AudioEffects.Fbi, GD.Load<AudioStream>("res://AudioFiles/fbi.mp3") },
            { AudioEffects.Pop, GD.Load<AudioStream>("res://AudioFiles/pop.mp3") },
            { AudioEffects.Fireworks, GD.Load<AudioStream>("res://AudioFiles/Firework.mp3") },
            { AudioEffects.Screenshot, GD.Load<AudioStream>("res://AudioFiles/Screenshot.mp3") },
            { AudioEffects.HaveYouEverHadADream, GD.Load<AudioStream>("res://AudioFiles/HaveYouEverHadADream.mp3") },
            { AudioEffects.Quack, GD.Load<AudioStream>("res://AudioFiles/quack_5.mp3") },
        };
        _glow = GetNode<Alerts.Glow>("%Glow");

        PlayAudioListener.PlayAudio += PlayAudio;
        MaxPolyphony = 5;
    }

    private static AudioEffects[] _spookySounds =
    [
        AudioEffects.Vine, AudioEffects.Wow, AudioEffects.Pew, AudioEffects.Punch, AudioEffects.Pikmin,
        AudioEffects.Whip
    ];

    private void PlayAudio(object sender, string e)
    {
        if (e.Equals("Spooky", StringComparison.OrdinalIgnoreCase))
        {
            var random = _spookySounds[new Random().Next(0, _spookySounds.Length)];

            NextStream.Add(random);
            return;
        }

        if (Enum.TryParse(e, out AudioEffects effect2))
        {
            if (SpamAudioPlayer.Spamables.Contains(effect2))
            {
                SpamAudioPlayer.NextStream.Add(effect2);
                return;
            }

            NextStream.Add(effect2);
            return;
        }

        GD.Print($"Audio effect {e} not found");
    }

    public static void PlayAudio(AudioEffects e)
    {
        NextStream.Add(e);
    }

    public override void _Process(double delta)
    {
        if (NextStream.Count == 0 || GetPlaybackPosition() > 0) return;
        if (!AudioStreams.TryGetValue(NextStream[0], out var stream)) return;

        SetStream(stream);
        Play();
        var timeout = (int)Math.Round(stream.GetLength() * 1000);
        GD.Print(timeout);
        switch (NextStream[0])
        {
            case AudioEffects.Flash:
                _glow.PlayFlashBang();
                break;
            case AudioEffects.Eww:
            case AudioEffects.HaveYouEverHadADream:
                _ = Task.Run(async () =>
                {
                    await SpotifyService.Pause();
                    var timeout = (int)Math.Round(stream.GetLength() * 1000);
                    GD.Print(timeout);
                    await Task.Delay(timeout);
                    await SpotifyService.Resume();
                });
                break;
        }

        NextStream.RemoveAt(0);
    }
}