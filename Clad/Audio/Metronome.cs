/*
 * Ported/Recreated from https://github.com/dvinesett/Ronome/blob/master/Ronome/ViewController.swift
 * 
 * 07/24/2018
 * */

using System;

using Clad.Models;
using Foundation;
using AVFoundation;

namespace Clad.Audio
{
    /// <summary>
    /// Metronome.
    /// </summary>
    public class Metronome : NSObject
    {
        AVAudioPlayer _soundPlayer;
        AVAudioPlayer _accentPlayer;
        NSTimer _metronomeTimer;
        volatile int _count = 0;

        volatile static bool _isRunning = false;

        BPMModel _model;

        public Metronome(float volume, ref BPMModel model)
        {
            SetModel(ref model);

            var soundFile = new NSUrl($"Sounds/DefaultClick/sound.wav");
            var accentFile = new NSUrl($"Sounds/DefaultClick/accent.wav");

            NSError error;
            _soundPlayer = new AVAudioPlayer(soundFile, "sound", out error);
            _soundPlayer.Volume = volume;
            _soundPlayer.Pan = -1;
            _soundPlayer.PrepareToPlay();
            _accentPlayer = new AVAudioPlayer(accentFile, "sound", out error);
            _accentPlayer.Volume = volume;
            _accentPlayer.Pan = -1;
            _accentPlayer.PrepareToPlay();
            // TODO: Check error
        }

        public void Start()
        {
            _isRunning = true;
            var metronomeTimeInterval = (240.0 / (double)_model.Lower) / _model.CurrentBPM;
            _metronomeTimer = NSTimer.CreateScheduledTimer(metronomeTimeInterval, true, PlaySound);
            _metronomeTimer?.Fire();
        }

        public void Stop()
        {
            _metronomeTimer?.Invalidate();
            _metronomeTimer?.Dispose();
            _metronomeTimer = null;
            _count = 0;
            _isRunning = false;
        }

        public void Restart()
        {
            if(_isRunning)
            {
                Stop();
                Start();
            }
        }

        public void Restart(ref BPMModel model)
        {
            SetModel(ref model);
            if (_isRunning)
            {
                Stop();
                Start();
            }
        }

        private void Restart(NSObservedChange change)
        {
            if (_isRunning)
            {
                Stop();
                Start();
            }
        }

        public void SetVolume(float volume)
        {
            _soundPlayer.Volume = volume;
            _accentPlayer.Volume = volume;
        }

        private void SetModel(ref BPMModel model)
        {
            _model?.RemoveObserver(this, nameof(BPMModel.CurrentBPM));
            _model?.RemoveObserver(this, nameof(BPMModel.Upper));
            _model?.RemoveObserver(this, nameof(BPMModel.Lower));
            _model = model;
            _model?.AddObserver(this, nameof(BPMModel.CurrentBPM), NSKeyValueObservingOptions.New, IntPtr.Zero);
            _model?.AddObserver(this, nameof(BPMModel.Upper), NSKeyValueObservingOptions.New, IntPtr.Zero);
            _model?.AddObserver(this, nameof(BPMModel.Lower), NSKeyValueObservingOptions.New, IntPtr.Zero);
        }

        private void PlaySound(NSTimer timer)
        {
            _count++;
            if (_count == 1)
                _accentPlayer.Play();
            else
            {
                _soundPlayer.Play();
                if(_count == _model.Upper) {
                    _count = 0;
                }
            }
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (ofObject.GetType().Name == nameof(BPMModel))
                Restart();
            else
                base.ObserveValue(keyPath, ofObject, change, context);
        }

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
                _soundPlayer?.Dispose();
                _soundPlayer = null;
                _accentPlayer?.Dispose();
                _accentPlayer = null;
                _model?.RemoveObserver(this, nameof(BPMModel.CurrentBPM));
                _model?.RemoveObserver(this, nameof(BPMModel.Upper));
                _model?.RemoveObserver(this, nameof(BPMModel.Lower));
                _model = null;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
