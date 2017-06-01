using sapHowmuch.Base.Helpers;
using System;
using System.Linq;
using System.Threading;

namespace sapHowmuch.Base.Extensions
{
	public static class TransactionExtensions
	{
		/// <summary>
		/// Wait for open transaction to complete
		/// Useful when using BeginTransaction
		/// </summary>
		/// <param name="company"></param>
		/// <param name="sleep"></param>
		/// <param name="tryCount"></param>
		public static void WaitForOpenTransaction(this SAPbobsCOM.Company company, int sleep = 500, int tryCount = 10)
		{
			for (int i = 0; i < tryCount; i++)
			{
				using (var query = new SboRecordsetQuery($"SELECT hostname, loginname FROM sys.sysprocesses WHERE open_tran = 1 AND  dbid = DB_ID('{company.CompanyDB}')"))
				{
					if (query.Count == 0) return;

					var openTransaction = query.Result.First();

					sapHowmuchLogger.Trace($"Open transaction by {openTransaction.Item("hostname").Value}, waiting {sleep} ms...");
				}

				Thread.Sleep(sleep);
			}

			throw new Exception($"Waiting for open transactions too long ({sleep * tryCount} ms)");
		}
	}
}