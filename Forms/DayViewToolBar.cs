using System;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using SummerGUI;
using KS.Foundation;

namespace SummerGUI.Scheduling
{
	public class DayViewToolBar : ComponentToolBar
	{
		private DayView m_ActiveDayView;
		public DayView ActiveDayView 
		{ 
			get {
				return m_ActiveDayView;
			}
			set {
				m_ActiveDayView = value;
				EnableControls ();
			}
		}

		IGuiMenuItem subAddColumn;
		IGuiMenuItem subRemoveColumn;
		IGuiMenuItem subToday;
		IGuiMenuItem subSplitView;
		IGuiMenuItem subSidebar;
		IGuiMenuItem subToolbar;

		DayViewEnsamble Owner;

		public DayViewToolBar (string name, IGuiMenu menu, DayViewEnsamble owner)
			: base(name, menu)
		{
			Owner = owner;

			subToday = menu.FindItem ("Today");
			subAddColumn = menu.FindItem ("AddColumn");
			subRemoveColumn = menu.FindItem ("RemoveColumn");
			subSplitView = menu.FindItem ("SplitView");
			subSidebar = menu.FindItem ("Sidebar");
			subToolbar = menu.FindItem ("Toolbar");

			Padding = new Padding (54, 2, 4, 2);
			Children.OfType<Button> ().ForEach (btn => {
				btn.DisplayStyle = ButtonDisplayStyles.ImageAndText;
				btn.Text = btn.Tooltip;
				btn.Tooltip = null;
				btn.CanFocus = false;
				btn.Styles.SetStyle (new ComponentToolBarButtonStyle (), WidgetStates.Pressed);
			});			

			subAddColumn.Click += delegate {
				ActiveDayView.Do(dv => {
					if (dv.DaysToShow < 5)
						dv.DaysToShow++;
					SetupMenu();
				});
			};

			subRemoveColumn.Click += delegate {
				ActiveDayView.Do(dv => {
					if (dv.DaysToShow > 1)
						dv.DaysToShow--;
					SetupMenu();
				});
			};

			subSidebar.Click += delegate {
				Owner.Panel2Collapsed = !subSidebar.Checked;
				Owner.Panel2.Update(true);
				SetupMenu();
			};

			subSplitView.Click += delegate {
				Owner.SplitScreen = subSplitView.Checked;
				SetupMenu();
			};

			subToolbar.Click += delegate {
				Visible = subToolbar.Checked;
				SetupMenu();
			};

			subToday.Click += delegate {
				Owner.MonthCalendar.Do(m => m.CurrentDate = DateTime.Now.Date);
			};					

			Owner.MonthCalendar.SelectionChanged += (object sender, EventArgs ev) => 
			{
				if (Owner.MonthCalendar.CurrentDate != DateTime.MinValue) {
					ActiveDayView.StartDate = Owner.MonthCalendar.CurrentDate;
					SetupMenu();
				}
			};

			Owner.MonthCalendar.ContextMenu = Menu.Children;
			Owner.MonthCalendar.SetupContextMenu += (sender, e) => SetupMenu();

			Owner.DayView1.ContextMenu = Menu.Children;
			Owner.DayView1.SetupContextMenu += (sender, e) => SetupMenu();

			Owner.DayView2.ContextMenu = Menu.Children;
			Owner.DayView2.SetupContextMenu += (sender, e) => SetupMenu();

			Owner.DayView1.GotFocus += (object sender, EventArgs e) => {
				Owner.MonthCalendar.CurrentDate = Owner.DayView1.StartDate;	
				ActiveDayView = Owner.DayView1;
				SetupMenu();
			};

			Owner.DayView2.GotFocus += (object sender, EventArgs e) => {
				Owner.MonthCalendar.CurrentDate = Owner.DayView2.StartDate;		
				ActiveDayView = Owner.DayView2;
				SetupMenu();
			};

			Owner.DayView1.SelectionChanged += (object sender, EventArgs e) => {
				if (Owner.DayView1.IsFocused) {
					Owner.MonthCalendar.CurrentDate = Owner.DayView1.StartDate;
					SetupMenu();
				}
			};

			Owner.DayView2.SelectionChanged += (object sender, EventArgs e) => {				
				if (Owner.DayView2.IsFocused) {
					Owner.MonthCalendar.CurrentDate = Owner.DayView2.StartDate;
					SetupMenu();
				}
			};

			Owner.DayView1.LostFocus += delegate {
				ActiveDayView = Owner.DayView2;
				SetupMenu();
			};

			Owner.DayView2.LostFocus += delegate {
				ActiveDayView = Owner.DayView1;
				SetupMenu();
			};
		}			

		public virtual void SetupMenu()
		{
			DayView activeDV = ActiveDayView;
			subAddColumn.Enabled = activeDV != null && activeDV.DaysToShow < 5;
			subRemoveColumn.Enabled = activeDV != null && activeDV.DaysToShow > 1;
			subSidebar.Checked = !Owner.Panel2Collapsed;
			subSplitView.Checked = Owner.SplitScreen;
			subToday.Enabled = Owner.MonthCalendar.CurrentDate.Date != DateTime.Now.Date;
			subToolbar.Checked = Visible;

			this.Update ();
			EnableControls ();
		}

		public void EnableControls()
		{
			Enabled = m_ActiveDayView != null;
			if (!Enabled)
				return;
		}

		protected override void CleanupManagedResources ()
		{
			Owner = null;
			subAddColumn = null;
			subRemoveColumn = null;
			subToday = null;
			subSplitView = null;
			subSidebar = null;
			subToolbar = null;

			base.CleanupManagedResources ();
		}
	}
}

