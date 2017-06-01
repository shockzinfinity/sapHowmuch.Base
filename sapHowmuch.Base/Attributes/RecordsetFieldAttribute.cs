using System;

namespace sapHowmuch.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
	public class RecordsetFieldAttribute : Attribute
	{
		public string FieldName { get; private set; }

		public RecordsetFieldAttribute(string fieldName)
		{
			this.FieldName = fieldName;
		}
	}
}