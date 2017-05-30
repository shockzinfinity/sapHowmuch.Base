using System;

namespace sapHowmuch.Base.Helpers
{
	public class SboTransaction : IDisposable
	{
		private readonly SAPbobsCOM.Company _company;
		private bool _transactionEnded;

		public SboTransaction(SAPbobsCOM.Company company)
		{
			if (company == null)
				throw new ArgumentNullException(nameof(company));

			_company = company;

			if (_company.InTransaction)
				throw new Exception("Already in transaction");

			_company.StartTransaction();
			sapHowmuchLogger.Debug("StartTransaction");
		}

		public void Rollback()
		{
			_company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
			_transactionEnded = true;
			sapHowmuchLogger.Debug("Rollback");
		}

		public void Commit()
		{
			if (!_transactionEnded && _company.InTransaction)
			{
				_company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
				sapHowmuchLogger.Debug("Commit");
			}
		}

		public void Dispose()
		{
			Commit();
		}
	}
}