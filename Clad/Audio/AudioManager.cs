using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Foundation;
using AVFoundation;

namespace Clad.Audio
{
    [Register(nameof(AudioManager))]
    public class AudioManager : NSObject
    {
        public readonly static List<string> KEYS = new List<string>()
        {
            "C",
            "C#",
            "D",
            "D#",
            "E",
            "F",
            "F#",
            "G",
            "Ab",
            "A",
            "B",
            "Bb"
        };

        public enum PadSounds
        {
            Unknown,
            Classic
        }

        public enum SoundType
        {
            Unknown,
            Click,
            Pad,
            All
        }

        public static AudioManager Instance
        {
            get;
            private set;
        }

        private Dictionary<string, AVAudioPlayer> _audioPlayers = new Dictionary<string, AVAudioPlayer>();

        private (AVAudioPlayer pad, AVAudioPlayer click) _activePlayers = (null, null);

        private PadSounds _padSound;

        [Export(nameof(PadSound))]
        public PadSounds PadSound
        {
            get => _padSound;
            set
            {
                WillChangeValue(nameof(PadSound));
                _padSound = value;
                DidChangeValue(nameof(PadSound));
            }
        }

        private float _masterVolume = 0.85F;

        [Export(nameof(MasterVolume))]
        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                WillChangeValue(nameof(MasterVolume));
                _masterVolume = value;
                UpdateVolume(SoundType.All);
                DidChangeValue(nameof(MasterVolume));
            }
        }

        private float _padVolume = 0.85F;

        [Export(nameof(PadVolume))]
        public float PadVolume
        {
            get => _padVolume;
            set
            {
                WillChangeValue(nameof(PadVolume));
                _padVolume = value;
                UpdateVolume(SoundType.Pad);
                DidChangeValue(nameof(PadVolume));
            }
        }

        private float _clickVolume = 0.85F;

        [Export(nameof(ClickVolume))]
        public float ClickVolume
        {
            get => _clickVolume;
            set
            {
                WillChangeValue(nameof(ClickVolume));
                _clickVolume = value;
                UpdateVolume(SoundType.Click);
                DidChangeValue(nameof(ClickVolume));
            }
        }

        public static void Initialize(PadSounds padSound)
        {
            //TODO: Different sounds
            Instance = new AudioManager();
            Instance.PadSound = padSound;
            foreach (var key in KEYS)
            {
                var audioPlayer = new AVAudioPlayer(GetSoundFile(padSound, key), key, out NSError error);
                audioPlayer.NumberOfLoops = -1;
                audioPlayer.Pan = 1;
                audioPlayer.PrepareToPlay();
                audioPlayer.SoundSetting.AudioQuality = AVAudioQuality.High;
                Instance._audioPlayers.Add(key, audioPlayer);
                break;
            }
        }

        public async Task PlayAsync(string key)
        {
            //Check if running first, then pause
            if (_activePlayers.pad != null)
                await StopAsync(SoundType.Pad);

            if(_audioPlayers.TryGetValue(key, out AVAudioPlayer player))
            {
                player.Volume = MasterVolume * PadVolume;
                player.Play();
                _activePlayers.pad = player;
            }
        }

        public async Task StopAsync(SoundType soundType)
        {
            if (soundType == SoundType.Pad && _activePlayers.pad != null)
            {
                //Fade
                var player = _activePlayers.pad;
                await Task.Run(() =>
                {
                    var currentVolume = player.Volume;
                    for (var i = currentVolume; i >= 0; i = i - 0.05F)
                    {
                        player.Volume = i;
                        Thread.Sleep(150);
                    }
                });
                player.Stop();
                player.CurrentTime = 0;
                player.PrepareToPlay();
                _activePlayers.pad = null;
            }
            else
                throw new NotSupportedException("Not supported yet");
        }

        private void UpdateVolume(SoundType soundType)
        {
            if (soundType == SoundType.Pad || soundType == SoundType.All)
                _activePlayers.pad?.SetVolume((MasterVolume * PadVolume), 0);
            if (soundType == SoundType.Click || soundType == SoundType.All)
                _activePlayers.click?.SetVolume((MasterVolume * ClickVolume), 0);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _activePlayers.pad?.Stop();
                _activePlayers.pad?.Dispose();
                _activePlayers.click?.Stop();
                _activePlayers.click?.Dispose();
                _activePlayers = (null, null);

                foreach(var item in _audioPlayers)
                    item.Value.Dispose();
                _audioPlayers.Clear();
                _audioPlayers = null;
            }

            base.Dispose(disposing);
        }

        static NSUrl GetSoundFile(PadSounds padSounds, string key) => new NSUrl($"Sounds/{padSounds.ToString()}/{key}.m4a");
    }
}
