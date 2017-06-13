using sapHowmuch.Base.Enums;
using System;

namespace sapHowmuch.Base.Attributes
{
	public class SrfControlAttribute : Attribute
	{
		public string UniqueId { get; private set; }
		public SrfControlType ControlType { get; private set; }
		private bool _affectsFormMode = true;
		public bool AffectsFormMode { get { return _affectsFormMode; } set { _affectsFormMode = value; } }

		public SrfControlAttribute(string uniqueId, SrfControlType controlType)
		{
			UniqueId = uniqueId;
			ControlType = controlType;
		}
	}
}