using System;
using System.Collections.Generic;

namespace SummerGUI.Scheduling.Data
{
	public class WorkingHour
	{		
		public double StartHour { get; set; }
		public double EndHour { get; set; }
	}

	public class WorkingHourCollection
	{		
		public static bool IsWorkingDay(DateTime day) 
		{			
			switch (day.DayOfWeek) {
			case DayOfWeek.Saturday:
			case DayOfWeek.Sunday:
				return false;
			default:
				return true;
			}
		}

		public WorkingHour[] WorkingHours { get; set; }

		public static IEnumerable<WorkingHour> SampleWorkingHours()
		{
			yield return new WorkingHour {
				StartHour = 9,
				EndHour = 12
			};

			yield return new WorkingHour {
				StartHour = 13,
				EndHour = 17.5
			};
		}
	}		
}

