using Plugin.Settings;
using Plugin.Settings.Abstractions;

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
			get
			{
				return CrossSettings.Current;
			}
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
            set => AppSettings.AddOrUpdateValue(MasterVolumeKey, value);
        }

        public static float PadVolume
        {
            get => AppSettings.GetValueOrDefault(PadVolumeKey, PadVolumeDefault);
            set => AppSettings.AddOrUpdateValue(PadVolumeKey, value);
        }

        public static float ClickVolume
        {
            get => AppSettings.GetValueOrDefault(ClickVolumeKey, ClickVolumeDefault);
            set => AppSettings.AddOrUpdateValue(ClickVolumeKey, value);
        }

        public static int LastBPM
        {
            get => AppSettings.GetValueOrDefault(LastBPMKey, LastBPMDefault);
            set => AppSettings.AddOrUpdateValue(LastBPMKey, value);
        }
	}
}