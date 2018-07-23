using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Foundation;
using AVFoundation;

namespace Clad
{
    public class AudioManager
    {
        private Dictionary<string, AVAudioPlayer> _audioPlayers = new Dictionary<string, AVAudioPlayer>();

        public static Task<AudioManager> InitializeAsync()
        {
            return null;
        }
    }
}
