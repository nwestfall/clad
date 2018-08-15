using Plugin.Settings;
using Plugin.Settings.Abstractions;

using Foundation;

namespace Clad.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get => CrossSettings.Current;
		}

        private static NSUbiquitousKeyValueStore _keyStore = new NSUbiquitousKeyValueStore();

        public static NSUbiquitousKeyValueStore KeyStore
        {
            get => _keyStore;
        }

		#region Setting Constants

		private const string MasterVolumeKey = "mastervolume_key";
        private static readonly float MasterVolumeDefault = 0.85F;
        private const string PadVolumeKey = "padvolume_key";
        private static readonly float PadVolumeDefault = 0.85F;
        private const string ClickVolumeKey = "clickvolume_key";
        private static readonly float ClickVolumeDefault = 0.85F;
        private const string LastBPMKey = "lastbpm_key";
        private static readonly int LastBPMDefault = 125;

		#endregion

        public static float MasterVolume
        {
            get => AppSettings.GetValueOrDefault(MasterVolumeKey, MasterVolumeDefault);
            set
            {
                AppSettings.AddOrUpdateValue(MasterVolumeKey, value);
                KeyStore.SetDouble(MasterVolumeKey, value);
                KeyStore.Synchronize();
            }
        }

        public static float PadVolume
        {
            get => AppSettings.GetValueOrDefault(PadVolumeKey, PadVolumeDefault);
            set
            {
                AppSettings.AddOrUpdateValue(PadVolumeKey, value);
                KeyStore.SetDouble(PadVolumeKey, value);
                KeyStore.Synchronize();
            }
        }

        public static float ClickVolume
        {
            get => AppSettings.GetValueOrDefault(ClickVolumeKey, ClickVolumeDefault);
            set
            {
                AppSettings.AddOrUpdateValue(ClickVolumeKey, value);
                KeyStore.SetDouble(ClickVolumeKey, value);
                KeyStore.Synchronize();
            }
        }

        public static int LastBPM
        {
            get => AppSettings.GetValueOrDefault(LastBPMKey, LastBPMDefault);
            set
            {
                AppSettings.AddOrUpdateValue(LastBPMKey, value);
                KeyStore.SetLong(LastBPMKey, value);
                KeyStore.Synchronize();
            }
        }

        public static void SyncFromCloud()
        {
            var dictionary = KeyStore.ToDictionary();
            if (dictionary.ContainsKey(new NSString(MasterVolumeKey)))
                AppSettings.AddOrUpdateValue(MasterVolumeKey, (float)KeyStore.GetDouble(MasterVolumeKey));
            if (dictionary.ContainsKey(new NSString(PadVolumeKey)))
                AppSettings.AddOrUpdateValue(PadVolumeKey, (float)KeyStore.GetDouble(PadVolumeKey));
            if (dictionary.ContainsKey(new NSString(ClickVolumeKey)))
                AppSettings.AddOrUpdateValue(ClickVolumeKey, (float)KeyStore.GetDouble(ClickVolumeKey));
            if (dictionary.ContainsKey(new NSString(LastBPMKey)))
                AppSettings.AddOrUpdateValue(LastBPMKey, (int)KeyStore.GetLong(LastBPMKey));
        }

        public static void SyncToCloud() => KeyStore.Synchronize();
	}
}