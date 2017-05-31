namespace sapHowmuch.Base.Services
{
	public interface ISettingService
	{
		/// <summary>
		/// Create empty setting if not exists
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		void InitSetting<T>(string key, string name, T defaultValue = default(T));

		/// <summary>
		/// Get setting for current user
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="askIfNotFound"></param>
		/// <returns></returns>
		T GetCurrentUserSettingByKey<T>(string key, T defaultValue = default(T), bool askIfNotFound = false);

		/// <summary>
		/// Save settingf for current user
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void SaveCurrentUserSetting<T>(string key, T value);

		/// <summary>
		/// Get Setting
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="userCode"></param>
		/// <param name="askIfNotFound"></param>
		/// <returns></returns>
		T GetSettingByKey<T>(string key, T defaultValue = default(T), string userCode = null, bool askIfNotFound = false);

		/// <summary>
		/// Save setting
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="userCode"></param>
		/// <param name="name"></param>
		void SaveSetting<T>(string key, T value = default(T), string userCode = null, string name = null);
	}
}