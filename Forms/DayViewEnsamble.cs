using System;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using KS.Foundation;
using SummerGUI;

namespace SummerGUI.Scheduling
{
	public class DayViewEnsamble : SplitContainer
	{
		public DayViewToolBar Tools { get; private set; }
		public DayView DayView1 { get; private set; }
		public DayView DayView2 { get; private set; }
		public MonthCalendar MonthCalendar;
		public SplitContainer DayViewSplitter { get; private set; }
		public SplitContainer CalendarSplitter { get; private set; }
		public DataGridView GrdTasks { get; private set; }

		public IGuiMenuItem Menu { get; private set; }
		const float defaultMonthCalendarWidth = 204f;



		public DayViewEnsamble (string name)
			: base(name, SplitOrientation.Vertical, -defaultMonthCalendarWidth)
		{

			Menu = new GuiMenuItem (name + "menu", "Schedule");
			Menu.AddChild ("Today", "Today", (char)FontAwesomeIcons.fa_calendar_check_o);
			Menu.AddSeparator ();
			Menu.AddChild ("AddColumn", "Add Column", (char)FontAwesomeIcons.fa_calendar_plus_o);
			Menu.AddChild ("RemoveColumn", "Remove Column", (char)FontAwesomeIcons.fa_calendar_minus_o);
			Menu.AddSeparator ();
			Menu.AddChild ("SplitView", "Split View").SetChecked(false).ShowOnToolbar();
			Menu.AddChild ("Sidebar", "Sidebar").SetChecked(false).ShowOnToolbar();
			Menu.AddChild ("Toolbar", "Toolbar").SetChecked(true);

			Splitter.Style.BackColorBrush.Color = SummerGUI.Theme.CurrentTheme.StatusBar.BackColor;
			Panel2.Style.BorderColorPen.Color = SummerGUI.Theme.Colors.Base0;
			Panel2.Style.BackColorBrush.Color = SummerGUI.Theme.Colors.Base01;

			CalendarSplitter = new SplitContainer ("calendarsplit", SplitOrientation.Horizontal, defaultMonthCalendarWidth);

			this.Panel2.AddChild (CalendarSplitter);

			MonthCalendar = new MonthCalendar ("monthcalendar1",
				FontManager.Manager.StatusFont,
				FontManager.Manager.BoldFont
				);
			CalendarSplitter.Panel1.AddChild (MonthCalendar);

			GrdTasks = new DataGridView ("tasks");
			CalendarSplitter.Panel2.AddChild (GrdTasks);

			DayView1 = new DayView ("dayview1");
			DayView2 = new DayView ("dayview2");

			DayViewSplitter = new SplitContainer ("dayviewsplitter", SplitOrientation.Horizontal, 0.5f);
			DayViewSplitter.Panel1.AddChild (DayView1);
			DayViewSplitter.Panel2.AddChild (DayView2);
			DayViewSplitter.Panel2Collapsed = true;

			Panel1.AddChild (DayViewSplitter);

			Tools = Panel1.AddChild (new DayViewToolBar ("dayviewtoolbar", Menu.Children, this));

			Menu.Expanding += delegate {
				Tools.SetupMenu();
			};

			CanFocus = true;
			DayView1.Focus ();
		}

		public void LoadSettings(string section)
		{
			ConfigurationService.Instance.ConfigFile.Do (cfg => {
				Panel2Collapsed = cfg.GetSetting(section, "Schedule.Panel2Collapsed", Panel2Collapsed).SafeBool();
				Splitter.Distance = cfg.GetSetting(section, "Schedule.Splitter", Splitter.Distance).SafeFloat();
				SplitScreen = cfg.GetSetting(section, "Schedule.SplitScreen", SplitScreen).SafeBool();
				DayView1.DaysToShow = cfg.GetSetting(section, "Schedule.DaysToShow1", DayView1.DaysToShow).SafeInt(4);
				DayView2.DaysToShow = cfg.GetSetting(section, "Schedule.DaysToShow2", DayView2.DaysToShow).SafeInt(4);
				Tools.Visible = cfg.GetSetting(section, "Schedule.Toolbar", Tools.Visible).SafeBool();
			});

			Tools.SetupMenu ();
		}

		public void SaveSettings(string section)
		{
			ConfigurationService.Instance.ConfigFile.Do (cfg => {
				cfg.SetSetting (section, "Schedule.Panel2Collapsed", Panel2Collapsed.ToLowerString());
				cfg.SetSetting (section, "Schedule.Splitter", Splitter.Distance / ScaleFactor);
				cfg.SetSetting (section, "Schedule.SplitScreen", SplitScreen.ToLowerString());
				cfg.SetSetting (section, "Schedule.DaysToShow1", DayView1.DaysToShow);
				cfg.SetSetting (section, "Schedule.DaysToShow2", DayView2.DaysToShow);
				cfg.SetSetting (section, "Schedule.Toolbar", Tools.Visible.ToLowerString());
			});
		}

		public bool SplitScreen
		{
			get{
				return !DayViewSplitter.Panel2Collapsed;
			}
			set{
				DayViewSplitter.Panel2Collapsed = !value;
			}
		}

		public void MovePrev()
		{			
			ActiveDayView.StartDate = ActiveDayView.StartDate.AddDays(-1).Date;
		}

		public void MoveNext()
		{			
			ActiveDayView.StartDate = ActiveDayView.StartDate.AddDays(1).Date;
		}			

		private DayView ActiveDayView
		{
			get{
				if (DayView2.IsFocused)
					return DayView2;
				return DayView1;
			}
		}

		public override bool OnKeyDown (KeyboardKeyEventArgs e)
		{	
			if (!Visible || !Enabled)
				return false;

			// Nichts davon funktioniert-
			// gar nichts.

			switch (e.Key) {
			case Keys.Down:
				if (e.Control) {
					DayViewSplitter.Panel2Collapsed = false;
					return true;
				}
				break;
			case Keys.Up:
				if (e.Control) {
					DayViewSplitter.Panel2Collapsed = true;
					return true;
				}
				break;
			}

			return base.OnKeyDown (e);
		}			

		public override void Focus ()
		{
			DayView1.Focus ();
		}
	}
}

