using sapHowmuch.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace sapHowmuch.Base.Helpers
{
	public class SboRecordsetQuery : IDisposable
	{
		private SAPbobsCOM.Recordset _recordset;

		public SboRecordsetQuery(string query, params object[] args)
		{
			_recordset = SapStream.DICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
			_recordset.DoQuery(string.Format(query, args));
		}

		public int Count => _recordset.RecordCount;

		public SAPbobsCOM.Recordset Recordset => _recordset;

		public IEnumerable<SAPbobsCOM.Fields> Result
		{
			get
			{
				while (!_recordset.EoF)
				{
					yield return _recordset.Fields;
					_recordset.MoveNext();
				}
			}
		}

		public void Dispose()
		{
			if (_recordset != null) _recordset.ReleaseComObject();

			_recordset = null;
		}
	}

	public class SboRecordsetQuery<T> : IDisposable
	{
		private SAPbobsCOM.Recordset _recordset;
		private dynamic _businessObject;

		public SboRecordsetQuery(string query, SAPbobsCOM.BoObjectTypes boObjectTypes, params object[] args)
		{
			_recordset = SapStream.DICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
			_businessObject = SapStream.DICompany.GetBusinessObject(boObjectTypes);

			_recordset.DoQuery(string.Format(query, args));
			_businessObject.Browser.Recordset = _recordset;
		}

		public int Count => _recordset.RecordCount;

		public T BusinessObject => _businessObject;
		public SAPbobsCOM.Recordset Recordset => _recordset;

		public IEnumerable<T> Result
		{
			get
			{
				while (!_businessObject.Browser.Eof)
				{
					yield return _businessObject;
					_businessObject.Browser.MoveNext();
				}
			}
		}

		public void Dispose()
		{
			if (_recordset != null) _recordset.ReleaseComObject();
			if (_businessObject != null) _businessObject.ReleaseComObject();

			_recordset = null;
			_businessObject = null;
		}
	}

	public static class SboRecordset
	{
		public static int NonQuery(string sql, params object[] args)
		{
			var recordset = SapStream.DICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

			try
			{
				if (recordset == null)
					throw new ArgumentNullException(nameof(recordset));

				recordset.DoQuery(string.Format(sql, args));

				return recordset.RecordCount;
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Debug($"NonQuery error: {ex.Message}{Environment.NewLine}SQL={sql}");
				throw;
			}
			finally
			{
				if (recordset != null) recordset.ReleaseComObject();
			}
		}
	}

	public class SboSqlConnection : IDisposable
	{
		private readonly SqlConnection _sqlConnection;
		private readonly SqlDataReader _reader;

		public SboSqlConnection(string query = null)
		{
			var dbPassword = ConfigurationManager.AppSettings["sapDbPassword"];
			var connectionString = $"Server={SapStream.DICompany.Server};Initial Catalog={SapStream.DICompany.CompanyDB};User ID={SapStream.DICompany.DbUserName};Password={dbPassword}";
			_sqlConnection = new SqlConnection(connectionString);

			if (string.IsNullOrWhiteSpace(query))
				return;

			var command = new SqlCommand(query, _sqlConnection);
			_sqlConnection.Open();
			_reader = command.ExecuteReader();
		}

		public SqlConnection SqlConnection => _sqlConnection;
		public bool HasRows => _reader.HasRows;

		public IEnumerable<SqlDataReader> Result
		{
			get
			{
				while (_reader.Read())
				{
					yield return _reader;
				}

				_reader.Close();
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}