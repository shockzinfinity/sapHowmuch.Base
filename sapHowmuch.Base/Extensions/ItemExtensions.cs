using System.Text.RegularExpressions;

namespace sapHowmuch.Base.Extensions
{
	public static class ItemExtensions
	{
		public static MoneyField GetMoney(this SAPbouiCOM.EditText editText)
		{
			var moneyString = editText.Value.Substring(0, editText.Value.Length - 4);
			var money = decimal.Parse(moneyString, System.Globalization.NumberStyles.Any);
			var currencyCode = editText.Value.Substring(editText.Value.Length - 3);

			return new MoneyField
			{
				Money = money,
				CurrencyCode = currencyCode
			};
		}

		public static int GetWeightInMg(this SAPbouiCOM.EditText editText)
		{
			var weightUnit = decimal.Parse(Regex.Match(editText.Value.Replace(".", string.Empty), @"[\d.]+").Value, System.Globalization.NumberStyles.Any);
			var unit = Regex.Match(editText.Value, @"[A-Za-z]+").Value;

			switch (unit)
			{
				case "g": return (int)weightUnit * 1000;
				case "kg": return (int)weightUnit * 1000000;
			}

			return 0;
		}
	}

	public class MoneyField
	{
		public decimal Money { get; set; }
		public string CurrencyCode { get; set; }
	}
}