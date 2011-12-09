using System;

namespace EventArgsUtilities
{
	public class EventArgsGeneric<t> : EventArgs
	{
		public EventArgsGeneric(t Target)
		{
			TargetObject = Target;
		}

		public t TargetObject { get; set; }
	}
}