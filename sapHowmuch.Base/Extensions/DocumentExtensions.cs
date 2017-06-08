using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace sapHowmuch.Base.Extensions
{
	/// <summary>
	/// SAP Business One Document extension methods
	/// </summary>
	public static class DocumentExtensions
	{
		public static int AddEx(this SAPbobsCOM.IDocuments documents)
		{
			var returnCode = documents.Add();
			ErrorHelper.HandleErrorWithException(returnCode, "Could not add document");

			return int.Parse(SapStream.DICompany.GetNewObjectKey());
		}

		public static void AddAndLoadEx(this SAPbobsCOM.IDocuments documents)
		{
			var docEntryKey = documents.AddEx();
			if (!documents.GetByKey(docEntryKey))
				throw new Exception($"Could not load document with docEntry {docEntryKey}");
		}

		public static void UpdateEx(this SAPbobsCOM.IDocuments documents)
		{
			var returnCode = documents.Update();
			ErrorHelper.HandleErrorWithException(returnCode, "Could not update document");
		}

		public static IEnumerable<SAPbobsCOM.Document_Lines> AsEnumerable(this SAPbobsCOM.Document_Lines lines)
		{
			var line = -1;
			while (++line < lines.Count)
			{
				lines.SetCurrentLine(line);
				yield return lines;
			}
		}

		// 운송비 (e.g. INV3 : AR Freight (운송비 처리부분)
		public static IEnumerable<SAPbobsCOM.DocumentsAdditionalExpenses> AsEnumerable(this SAPbobsCOM.DocumentsAdditionalExpenses expenses)
		{
			var line = -1;
			while (++line < expenses.Count)
			{
				expenses.SetCurrentLine(line);
				yield return expenses;
			}
		}

		public static bool Search(this SAPbobsCOM.IDocuments document, string table, string where)
		{
			if (string.IsNullOrWhiteSpace(table))
				throw new ArgumentNullException(nameof(table));

			var recordSet = SapStream.DICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
			recordSet.DoQuery($"SELECT * FROM [{table}] WHERE {where}");
			document.Browser.Recordset = recordSet;
			return recordSet.RecordCount != 0;
		}

		public static int CopyTo(this SAPbobsCOM.IDocuments sourceDocument, SAPbobsCOM.BoObjectTypes copyToType, bool copyExpenses = true, Action<SAPbobsCOM.Documents> setObjectProperties = null)
		{
			using (var copyTo = SapStream.DICompany.GetBusinessObject<SAPbobsCOM.Documents>(copyToType))
			{
				var targetDocument = copyTo.Object;

				targetDocument.CardCode = sourceDocument.CardCode;

				setObjectProperties?.Invoke(targetDocument);

				foreach (var sourceLine in sourceDocument.Lines.AsEnumerable())
				{
					sapHowmuchLogger.Debug($"CopyTo.Line: DocEntry={sourceLine.DocEntry}, LineNum={sourceLine.LineNum}, DocObjectCode={sourceDocument.DocObjectCode}");
					targetDocument.Lines.BaseEntry = sourceLine.DocEntry;
					targetDocument.Lines.BaseLine = sourceLine.LineNum;
					targetDocument.Lines.BaseType = (int)sourceDocument.DocObjectCode;
					targetDocument.Lines.Add();
				}

				if (copyExpenses)
				{
					foreach (var sourceExpense in sourceDocument.Expenses.AsEnumerable().Where(e => e.LineTotal > 0))
					{
						sapHowmuchLogger.Debug($"CopyTo.Expense: DocEntry={sourceDocument.DocEntry}, LineNum={sourceExpense.LineNum}, DocObjectCode={sourceDocument.DocObjectCode}");
						targetDocument.Expenses.BaseDocEntry = sourceDocument.DocEntry;
						targetDocument.Expenses.BaseDocLine = sourceExpense.LineNum;
						targetDocument.Expenses.BaseDocType = (int)sourceDocument.DocObjectCode;
						targetDocument.Expenses.Add();
					}
				}

				targetDocument.AddAndLoadEx();

				return targetDocument.DocEntry;
			}
		}

		public static decimal GetWeightInMg(this SAPbobsCOM.Document_Lines line)
		{
			using (var weightMeasures = SapStream.DICompany.GetBusinessObject<SAPbobsCOM.WeightMeasures>(SAPbobsCOM.BoObjectTypes.oWeightMeasures))
			{
				if (weightMeasures.Object.GetByKey(line.Weight1Unit))
					return (decimal)(line.Weight1 * weightMeasures.Object.UnitWeightinmg);

				return 0;
			}
		}

		public static decimal GetWeightInMg(this SAPbobsCOM.IDocumentPackages line)
		{
			using (var weightMeasures = SapStream.DICompany.GetBusinessObject<SAPbobsCOM.WeightMeasures>(SAPbobsCOM.BoObjectTypes.oWeightMeasures))
			{
				if (weightMeasures.Object.GetByKey(line.Units))
					return (decimal)(line.TotalWeight * weightMeasures.Object.UnitWeightinmg);

				return 0;
			}
		}

		public static string GetTableName(this SAPbobsCOM.BoObjectTypes boObjectType)
		{
			var tableConstant = typeof(SboTable).GetFields().FirstOrDefault(x => x.GetCustomAttribute<DescriptionAttribute>().Description == ((int)boObjectType).ToString());

			return tableConstant?.GetRawConstantValue().ToString();
		}

		public static void AddComment(this SAPbobsCOM.IDocuments documents, string comment)
		{
			documents.Comments = documents.Comments.AddNewLine(comment);
		}

		public static string AddNewLine(this string existingText, string newLine)
		{
			var lines = existingText.Split(new[] { "\n" }, StringSplitOptions.None);
			if (lines[lines.Length - 1] != "")
				existingText += $"{Environment.NewLine}{newLine}{Environment.NewLine}";
			else
				existingText += $"{newLine}{Environment.NewLine}";

			return existingText;
		}

		public static int? GetDocEntry(this int docNum, string table)
		{
			using (var query = new SboRecordsetQuery($"SELECT [DocEntry] FROM [{table}] WHERE [DocNum] = {docNum}"))
			{
				if (query.Count == 0) return null;

				return int.Parse(query.Result.First().Item("DocEntry").Value.ToString());
			}
		}
	}
}