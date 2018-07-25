/*
 * Ported from https://github.com/MasterEx/BeatKeeper/blob/master/src/pntanasis/android/metronome/Metronome.java
 * 
 * 07/24/2018
 * */

using System;

namespace Clad.Audio
{
    /// <summary>
    /// Metronome.
    /// </summary>
    public class Metronome
    {
        const int TICK = 1000;

        /// <summary>
        /// Gets or sets the bpm.
        /// </summary>
        /// <value>The bpm.</value>
        public double BPM { get; set; }
        /// <summary>
        /// Gets or sets the beat.
        /// </summary>
        /// <value>The beat.</value>
        public int Beat { get; set; }
        /// <summary>
        /// Gets or sets the note value.
        /// </summary>
        /// <value>The note value.</value>
        public int NoteValue { get; set; }
        /// <summary>
        /// Gets or sets the beat sound.
        /// </summary>
        /// <value>The beat sound.</value>
        public double BeatSound { get; set; }
        /// <summary>
        /// Gets or sets the sound.
        /// </summary>
        /// <value>The sound.</value>
        public double Sound { get; set; }

        int _silence;
        bool _play = true;

        AudioGenerator _audioGenerator = new AudioGenerator(8000);
        double[] _soundTickArray;
        double[] _soundTockArray;
        double[] _soundSilenceArray;
        int _currentBeat = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Clad.Audio.Metronome"/> class.
        /// </summary>
        public Metronome()
        {
            _audioGenerator.CreatePlayer();
        }

        /// <summary>
        /// Calculates the silence.
        /// </summary>
        public void CalculateSilence()
        {
            _silence = (int)(((60 / BPM) * 8000) - TICK);
            _soundTickArray = new double[TICK];
            _soundTockArray = new double[TICK];
            _soundSilenceArray = new double[_silence];

            double[] tick = _audioGenerator.GetSineWave(TICK, 8000, BeatSound);
            double[] tock = _audioGenerator.GetSineWave(TICK, 8000, Sound);
            for (var i = 0; i < TICK; i++)
            {
                //TODO: Span or array copy
                _soundTickArray[i] = tick[i];
                _soundTockArray[i] = tock[i];
            }
            for (var i = 0; i < _silence; i++)
                _soundSilenceArray[i] = 0;
        }

        /// <summary>
        /// Play this instance.
        /// </summary>
        public void Play()
        {
            CalculateSilence();
            do
            {
                if (_currentBeat == 1)
                    _audioGenerator.WriteSound(_soundTockArray);
                else
                    _audioGenerator.WriteSound(_soundTickArray);
                _audioGenerator.WriteSound(_soundSilenceArray);
                _currentBeat++;
                if (_currentBeat > Beat)
                    _currentBeat = 1;
            } while (_play);
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop()
        {
            _play = false;
            _audioGenerator?.Dispose();
        }
    }
}
