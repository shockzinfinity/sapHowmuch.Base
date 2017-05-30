namespace sapHowmuch.Base.Dialogs.Inputs
{
	public interface IDialogInput
	{
		string Id { get; }
		string Title { get; }
		bool Required { get; }
		SAPbouiCOM.Item Item { set; }
		SAPbouiCOM.BoFormItemTypes ItemType { get; }
		SAPbouiCOM.BoDataType DataType { get; }
		bool Validated { get; }
		int Length { get; }
		string DefaultValue { get; }

		object GetValue();

		void Extras(SAPbouiCOM.Form form, int yPos);
	}
}