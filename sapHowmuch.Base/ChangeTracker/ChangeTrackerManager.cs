using SAPbobsCOM;
using sapHowmuch.Base.Helpers;
using sapHowmuch.Base.Setup;
using System.Collections.Generic;
using System.Linq;

namespace sapHowmuch.Base.ChangeTracker
{
	/// <summary>
	/// This provides interfaces to the <c>ChangeTrackerManager</c> class.
	/// </summary>
	public interface IChangeTrackerManager
	{
		/// <summary>
		/// Get changed items for object since timestamp.
		/// </summary>
		/// <param name="timestamp">Timestamp (seconds from a given startpoint)</param>
		/// <param name="objectType">SBO DI object type</param>
		/// <returns>Collection of key and timestamp for updated objects</returns>
		ICollection<KeyAndTimeStampModel> GetChanged(int timestamp, BoObjectTypes objectType);
	}

	/// <summary>
	/// This represents the change tracker manager.
	/// TODO: 테스트 필요, PreTransactionNotification 과의 연계 정책 필요
	/// 이 부분이 masstransit 을 이용한 전략을 적용해야 하는 부분이 될듯
	/// </summary>
	public class ChangeTrackerManager : IChangeTrackerManager
	{
		public static ChangeTrackerManager _instance;

		/// <summary>
		/// Get static instance of <c>ChangeTrackerManager</c> class.
		/// </summary>
		public static ChangeTrackerManager Instance => _instance ?? (_instance = new ChangeTrackerManager());

		/// <summary>
		/// Run setup via SetupManager
		/// </summary>
		public static void RunSetup() => SetupManager.RunSetup(new ChangeTrackerSetup());

		/// <summary>
		/// Get changed items for object since timestamp.
		/// </summary>
		/// <param name="timestamp">Timestamp (seconds from a given startpoint)</param>
		/// <param name="objectType">SBO DI object type</param>
		/// <returns>Collection of key and timestamp for updated objects</returns>
		public ICollection<KeyAndTimeStampModel> GetChanged(int timestamp, BoObjectTypes objectType)
		{
			string trackingQuery = $@"SELECT DISTINCT
[U_{sapHowmuchConstants.CT_UdfKey}] AS [Key]
, CAST([Code] AS INT) AS [Timestamp]
FROM [@{sapHowmuchConstants.CT_UdtName}]
WHERE 1 = 1
AND [U_{sapHowmuchConstants.CT_UdfObj}] = {(int)objectType}
AND CAST([Code] AS INT) > {timestamp}
ORDER BY CAST([Code] AS INT) ASC";

			using (var query = new SboRecordsetQuery(trackingQuery))
			{
				if (query.Count == 0)
					return new List<KeyAndTimeStampModel>();

				return query.Result.Select(r => new KeyAndTimeStampModel
				{
					Key = r.Item(0).Value.ToString(),
					Timestamp = int.Parse(r.Item(1).Value.ToString())
				}).ToList();
			}
		}
	}

	/// <summary>
	/// Change tracker model
	/// </summary>
	public class KeyAndTimeStampModel
	{
		/// <summary>
		/// Object key
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Timestamp in seconds
		/// </summary>
		public int Timestamp { get; set; }
	}
}