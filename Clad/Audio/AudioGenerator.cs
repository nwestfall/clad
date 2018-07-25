/*
 * Ported from https://github.com/MasterEx/BeatKeeper/blob/master/src/pntanasis/android/metronome/AudioGenerator.java
 * 
 * 07/24/2018
 * 
 * Some other shit here https://stackoverflow.com/questions/28058777/generating-a-tone-in-ios-with-16-bit-pcm-audioengine-connect-throws-ausetform
 * */

using System;

using Foundation;
using AVFoundation;

namespace Clad.Audio
{
    public class AudioGenerator : IDisposable
    {
        int _sampleRate;
        AVAudioPlayerNode _audioPlayerNode;
        AVAudioPlayer _audioPlayer;
        AVAudioEngine _audioEngine;

        public AudioGenerator(int sampleRate)
        {
            _sampleRate = sampleRate;
        }

        public double[] GetSineWave(int samples, int sampleRate, double frequencyOfTone)
        {
            double[] sample = new double[samples];
            for (var i = 0; i < samples; i++)
                sample[i] = Math.Sin(2 * Math.PI * i / (sampleRate / frequencyOfTone));
            return sample;
        }

        public byte[] Get16BitPcm(double[] samples)
        {
            byte[] generatedSound = new byte[2 * samples.Length];
            int index = 0;
            foreach (var sample in samples)
            {
                short maxSample = (short)(sample * short.MaxValue);
                generatedSound[index++] = (byte)(maxSample & 0x00ff);
                generatedSound[index++] = (byte)((maxSample & 0xff00) >> 8);
            }
            return generatedSound;
        }

        public void CreatePlayer()
        {
            _audioPlayerNode = new AVAudioPlayerNode();
            _audioEngine = new AVAudioEngine();
            _audioEngine.AttachNode(_audioPlayerNode);

            var format = new AVAudioFormat(AVAudioCommonFormat.PCMInt16, _sampleRate, 1, false);
            var buffer = new AVAudioPcmBuffer(format, 0);
            buffer.FrameLength = 0;
        }

        public void WriteSound(double[] samples)
        {
            byte[] generatedSnd = Get16BitPcm(samples);
            //TODO
        }

        #region IDisposable Support
        /// <summary>
        /// Releases all resource used by the <see cref="T:Clad.Audio.AudioGenerator"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:Clad.Audio.AudioGenerator"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="T:Clad.Audio.AudioGenerator"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:Clad.Audio.AudioGenerator"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:Clad.Audio.AudioGenerator"/> was occupying.</remarks>
        public void Dispose()
        {
            
        }
        #endregion

    }
}
