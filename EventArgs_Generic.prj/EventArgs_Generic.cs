using System;

namespace EventArgsUtilities
{
	public class EventArgs_Generic<t> : EventArgs
	{
		public EventArgs_Generic(t Target)
		{
			TargetObject = Target;
		}

		public t TargetObject { get; set; }
	}
}