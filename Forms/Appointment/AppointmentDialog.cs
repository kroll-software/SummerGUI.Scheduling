using System;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using SummerGUI;
using KS.Foundation;

namespace SummerGUI.Scheduling
{
	public class AppointmentDialog : ChildFormWindow
	{
		public AppointmentForm Form { get; private set; }

		public AppointmentDialog (string name, SummerGUIWindow parent)
			: base(name, "Edit Appointment", 360, 480, parent, true)
		{
			Form = new AppointmentForm ("apppointment");
			this.AddChild (Form);
		}
	}
}

