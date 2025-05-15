using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using Godot;

namespace Temptica.Overlay.Scripts.Services;
using System.Speech;
#pragma warning disable CA1416
public class TextToSpeechService : IDisposable
{
    private static TextToSpeechService _textToSpeechService;
    public static TextToSpeechService Instance => _textToSpeechService ??= new TextToSpeechService();
    
    public bool IsSpeaking { get; private set; }
    public bool IsPaused => _synth.State == SynthesizerState.Paused;

    private readonly SpeechSynthesizer _synth;
    private readonly List<string> _queue = [];
    private TextToSpeechService()
    {
        _synth = new SpeechSynthesizer();
        _synth.SpeakCompleted += OnSpeakCompleted;
    }

    private void OnSpeakCompleted(object sender, SpeakCompletedEventArgs e)
    {
        if(_queue.Count > 0 )
        {
            var text = _queue[0];
            _queue.RemoveAt(0);
            StartSpeaking(text);
        }
        else
        {
            IsSpeaking = false;
        }
    }

    public void Speak(string text)
    {
        if(_queue.Count == 0 && !IsSpeaking && !IsPaused)
        {
            StartSpeaking(text);
        }
        else
        {
            _queue.Add(text);
        }
    }
    
    private void StartSpeaking(string text)
    {
        IsSpeaking = true;
        Task.Run(()=>_synth.SpeakAsync(text));
    }
    
    public void Dispose()
    {
        _synth?.Dispose();
        _queue.Clear();
        IsSpeaking = false;
    }

    public void Stop()
    {
        _synth.SpeakAsyncCancelAll();
        IsSpeaking = false;
    }
    
    public void Pause()
    {
        _synth.Pause();
    }
    
    public void Resume()
    {
        _synth.Resume();
    }
}
#pragma warning restore CA1416