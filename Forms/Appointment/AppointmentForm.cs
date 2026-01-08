using System;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using SummerGUI;
using KS.Foundation;

namespace SummerGUI.Scheduling
{
	public class AppointmentForm : TableLayoutContainer
	{
		public AppointmentForm (string name)
			: base(name)
		{
			InitControls ();
		}

		protected virtual void InitControls()
		{
			
		}
	}
}

