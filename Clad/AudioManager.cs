using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Foundation;
using AVFoundation;

namespace Clad
{
    [Register(nameof(AudioManager))]
    public class AudioManager : NSObject
    {
        private readonly static List<string> KEYS = new List<string>()
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
            Pad
        }

        public static AudioManager Instance
        {
            get;
            private set;
        }

        private Dictionary<string, AVAudioPlayer> _audioPlayers = new Dictionary<string, AVAudioPlayer>();

        private Dictionary<SoundType, AVAudioPlayer> _activePlayers = new Dictionary<SoundType, AVAudioPlayer>();

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

        private float _volume = 0.85F;

        [Export(nameof(Volume))]
        public float Volume
        {
            get => _volume;
            set
            {
                WillChangeValue(nameof(Volume));
                _volume = value;
                DidChangeValue(nameof(Volume));
            }
        }

        public static void Initialize(PadSounds padSound)
        {
            //TODO: Different sounds
            Instance = new AudioManager();
            Instance.PadSound = padSound;
            //TODO: Loop
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
            if (_activePlayers.ContainsKey(SoundType.Pad))
                await StopAsync(SoundType.Pad);

            if(_audioPlayers.TryGetValue(key, out AVAudioPlayer player))
            {
                player.Volume = Volume;
                player.Play();
                _activePlayers.Add(SoundType.Pad, player);
            }
        }

        public async Task StopAsync(SoundType soundType)
        {
            if(_activePlayers.TryGetValue(SoundType.Pad, out AVAudioPlayer player))
            {
                //Fade
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
                _activePlayers.Remove(SoundType.Pad);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                foreach(var item in _activePlayers)
                {
                    item.Value.Stop();
                    item.Value.Dispose();
                }
                _activePlayers.Clear();
                _activePlayers = null;

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
