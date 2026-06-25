using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Enums;
using Temptica.Overlay.Scripts.Alerts;
using Temptica.Overlay.Scripts.Services;
using Temptica.Overlay.Scripts.Spotify;

namespace Temptica.Overlay.Scripts;

public partial class AudioPlayer : AudioStreamPlayer
{
    public static Dictionary<AudioEffects, AudioStream> AudioStreams;
    private double deltaCumulitveTime;
    private Glow _glow;
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
            { AudioEffects.Fireworks, GD.Load<AudioStream>("res://AudioFiles/firework.mp3") },
            { AudioEffects.Screenshot, GD.Load<AudioStream>("res://AudioFiles/Screenshot.mp3") },
            { AudioEffects.HaveYouEverHadADream, GD.Load<AudioStream>("res://AudioFiles/HaveYouEverHadADream.mp3") },
            { AudioEffects.Quack, GD.Load<AudioStream>("res://AudioFiles/quack_5.mp3") },
            { AudioEffects.Toothless, GD.Load<AudioStream>("res://AudioFiles/Toothless.mp3") },
            { AudioEffects.ILoveRepo, GD.Load<AudioStream>("res://AudioFiles/ILoveRepo.mp3") },
            { AudioEffects.Meow, GD.Load<AudioStream>("res://AudioFiles/meow.mp3") },
            { AudioEffects.Panda, GD.Load<AudioStream>("res://AudioFiles/panda.mp3") },
            { AudioEffects.Mags, GD.Load<AudioStream>("res://AudioFiles/TRON-a-user_onlyvocals.mp3") },
            { AudioEffects.IntoTheThickOfIt, GD.Load<AudioStream>("res://AudioFiles/IntoTheThickOfIt.mp3") },
            { AudioEffects.IHeartRaid, GD.Load<AudioStream>("res://AudioFiles/IHeartRaid.mp3") },
        };
        
        _glow = GetNode<Glow>("%Glow");

        MaxPolyphony = 5;

        SetViewerSounds();
    }

    private void SetViewerSounds()
    {
        ChatListener.Instance.StreamNewChatMessage += userName =>
        {
            if (userName.Equals("qmezu", StringComparison.OrdinalIgnoreCase))
            {
                PlayAudio(AudioEffects.Meow);
                return;
            }

            if (userName.Equals("booooooooooooooooooom", StringComparison.OrdinalIgnoreCase))
            {
                PlayAudio(AudioEffects.Fbi);
                return;
            }

            if (userName.Equals("iheartfunnyboys", StringComparison.OrdinalIgnoreCase))
            {
                PlayAudio(AudioEffects.IHeartRaid);
                return;
            }

            if (userName.Equals("xfoff", StringComparison.OrdinalIgnoreCase))
            {
                PlayAudio(AudioEffects.Bober);
            }

            if (userName.Equals("pandacoder", StringComparison.OrdinalIgnoreCase))
            {
                PlayAudio(AudioEffects.Panda);
            }
        };
    }

    private static AudioEffects[] _spookySounds =
    [
        AudioEffects.Vine, AudioEffects.Wow, AudioEffects.Pew, AudioEffects.Punch, AudioEffects.Pikmin,
        AudioEffects.Whip
    ];

    private void PlayAudio(string soundName)
    {
        if (soundName.Equals("Spooky", StringComparison.OrdinalIgnoreCase))
        {
            var random = _spookySounds[new Random().Next(0, _spookySounds.Length)];

            NextStream.Add(random);
            return;
        }

        if (Enum.TryParse(soundName, out AudioEffects effect2))
        {
            if (SpamAudioPlayer.Spamables.Contains(effect2))
            {
                SpamAudioPlayer.NextStream.Add(effect2);
                return;
            }

            NextStream.Add(effect2);
            return;
        }

        GD.Print($"Audio effect {soundName} not found");
    }

    public static void PlayAudio(AudioEffects e)
    {
        NextStream.Add(e);
    }

    public override void _Process(double delta)
    {
        if (NextStream.Count == 0 || GetPlaybackPosition() > 0) return;
        if (!AudioStreams.TryGetValue(NextStream[0], out var stream)) return;
        GD.Print($"Audio stream {stream}");
        SetStream(stream);
        Play();
        var timeout = (int)Math.Round(stream.GetLength() * 1000);
        GD.Print(timeout);
        switch (NextStream[0])
        {
            case AudioEffects.Flash:
                OverlayColorManager.Instance.PlayFlashBang();
                break;
            case AudioEffects.Eww:
            case AudioEffects.HaveYouEverHadADream:
                _ = Task.Run(async () =>
                {
                    await SpotifyService.Pause();
                    timeout = (int)Math.Round(stream.GetLength() * 1000);
                    GD.Print(timeout);
                    await Task.Delay(timeout);
                    await SpotifyService.Resume();
                });
                break;
        }

        NextStream.RemoveAt(0);
    }
}