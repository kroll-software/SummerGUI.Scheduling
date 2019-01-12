/* Developed by Ertan Tike (ertan.tike@moreum.com) */

// Remarks by Kroll-Software, March 2010:
// We found this code licensed under the Code Project Open License (CPOL) at
// http://www.codeproject.com/KB/selection/Calendardayview.aspx
// More infos about the license can be found at
// http://www.codeproject.com/info/cpol10.aspx
// "Source Code and Executable Files can be used in commercial applications"
// Many thanks and respect to the Developer
// This is a slightly modified version which implements KS.Gantt.Calendars-WorkingHours.


using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
//using System.Drawing;
using OpenTK;
using OpenTK.Input;
using SummerGUI;
using SummerGUI.Scheduling.Data;

namespace SummerGUI.Scheduling
{
	public class DayViewStyle : WidgetStyle
	{
		public override void InitStyle ()
		{
			SetBackColor (SummerGUI.Theme.Colors.Base3);
			SetForeColor (SummerGUI.Theme.Colors.Black);
			SetBorderColor (System.Drawing.Color.Empty);
		}
	}

	public class DayView : ScrollableContainer
    {
		public event EventHandler<EventArgs> SelectionChanged;
		public void OnSelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged (this, EventArgs.Empty);
		}

		/***
		public event ResolveAppointmentsEventHandler ResolveAppointments;
		public event NewAppointmentEventHandler NewAppointment;
		public event EventHandler<AppointmentEventArgs> EditAppointment;
		public event EventHandler<AppointmentEventArgs> DeleteAppointment;
		***/
		public event EventHandler<AppointmentEventArgs> AppoinmentMove;

        // ToolTips
        //private ToolTip m_Tooltip = new ToolTip();
        //private Timer m_TimerTooltip = new Timer();
		/***
        private bool m_TooltipShown = false;
        private int m_TooltipX = 0;
        private int m_TooltipY = 0;
        private string m_TooltipText = "";
        ***/

        private bool m_ToolTipEnabled = true;
        [DefaultValue(true)]
        public bool ToolTipEnabled
        {
            get
            {
                return m_ToolTipEnabled;
            }
            set
            {
                if (m_ToolTipEnabled != value)
                {
                    m_ToolTipEnabled = value;
                    if (!value)
                    {
                        //m_TimerTooltip.Stop();
                        //HideTooltip();                        
                    }
                }
            }
        }


        private SortedDictionary<int, string> m_Owners = new SortedDictionary<int, string>();
        public SortedDictionary<int, string> Owners
        {
            get
            {
                return m_Owners;
            }            
        }

		/**
        private ImageList m_ImageList = null;
        public ImageList ImageList
        {
            get
            {
                return m_ImageList;
            }
            set
            {
                m_ImageList = value;
            }
        }
        **/



        

        //private TextBox m_EditBox;
        //private VScrollBar m_ScrollBar;
        private DrawTool m_DrawTool;
        private SelectionTool m_SelectionTool;
        private int m_AllDayEventsHeaderHeight = 0;

                
		private int m_HourLabelWidth = 56;
        private int m_HourLabelIndent = 2;
        private int m_DayHeadersHeight = 24;
        private int m_AppointmentGripWidth = 5;
        // private int horizontalAppointmentHeight = 20;		       

		protected void AutoScaleControl()
		{
			if (Renderer == null)
				return;

			//System.Drawing.SizeF sz = Renderer.BaseFont.Measure ("MyQgK");
			//m_HalfHourHeight = (int)Math.Round(sz.Height * 1.67);
			//m_DayHeadersHeight = (int)Math.Round(sz.Height * 1.67);

			m_HalfHourHeight = (int)(Renderer.BaseFont.TextBoxHeight + 0.5f);

			m_DayHeadersHeight =  m_HalfHourHeight;
			//if (Renderer.HeaderFont != null)
			//	m_DayHeadersHeight = Math.Max(m_HalfHourHeight, (int)(Renderer.HeaderFont.CaptionHeight));

			SizeF szHour = Renderer.HourFont.Measure ("87");
			SizeF szMinute = Renderer.MinuteFont.Measure ("30");

			float w = szHour.Width * 1f + szMinute.Width * 1.54f;

			m_HourLabelWidth = (int)Math.Round((w) + 3);
			m_HourLabelIndent = (int)(szHour.Width * 0.12f);

			System.Drawing.SizeF szChar = Renderer.BaseFont.Measure ("a");
			m_AppointmentGripWidth = (int)(szChar.Width / 2f + 0.5f);	

			//VScrollBar.Margin = new Padding (0, (this.HeaderHeight / ScaleFactor).Ceil(), 0, VScrollBar.Margin.Bottom);
			VScrollBar.Margin = Padding.Empty;
		}

		protected override void OnScaleWidget (IGUIContext ctx, float absoluteScaleFactor, float relativeScaleFactor)
		{
			base.OnScaleWidget (ctx, absoluteScaleFactor, relativeScaleFactor);
			AutoScaleControl ();
		}

		/*
        private KS.Gantt.Calendars.IGanttCalendar m_GanttCalendar = null;
        public KS.Gantt.Calendars.IGanttCalendar GanttCalendar
        {
            get
            {
                return m_GanttCalendar;
            }
            set
            {
                if (m_GanttCalendar != value)
                {
                    m_GanttCalendar = value;
                    this.Invalidate();
                }
            }
        }
        */

        private bool m_ShowDateRangeSelection = false;
        public bool ShowDateRangeSelection
        {
            get
            {
                return m_ShowDateRangeSelection;
            }
            set
            {
                m_ShowDateRangeSelection = value;
            }
        }
		        
		public DayView(string name) : base(name, Docking.Fill, new DayViewStyle())
        {
			/**
            m_EditBox = new TextBox();
            m_EditBox.Multiline = true;
            m_EditBox.Visible = false;
            m_EditBox.BorderStyle = BorderStyle.None;
            m_EditBox.KeyUp += new KeyEventHandler(editbox_KeyUp);
            m_EditBox.Margin = Padding.Empty;

            this.Controls.Add(m_EditBox);
            **/

			ScrollBars = ScrollBars.Vertical;

            m_DrawTool = new DrawTool
            {
                DayView = this
            };

            m_SelectionTool = new SelectionTool
            {
                DayView = this
            };

            m_SelectionTool.Complete += new EventHandler(selectionTool_Complete);

			m_StartDate = DateTime.Now;

            //activeTool = drawTool;            
			            
            this.Renderer = new FlatRenderer();
			CanFocus = true;

			/**
            // Tooltips
            m_Tooltip.AutoPopDelay = 15000;
            m_Tooltip.InitialDelay = 20000;
            m_TimerTooltip.Interval = 500;
            m_TimerTooltip.Tick += new EventHandler(tmrToolTip_Tick);
			**/		
        }
			
		protected override void OnParentChanged ()
		{	
			base.OnParentChanged ();
			if (Parent == null)
				return;

			AutoScaleControl ();
			AdjustScrollbar();
			//VScrollBar.Scroll += new EventHandler<EventArgs>(scrollbar_Scroll);
			this.MinSize = new System.Drawing.SizeF (m_HourLabelWidth + m_HourLabelIndent + VScrollBar.Width + 12, m_DayHeadersHeight + 4);
		}

        void Renderer_BaseFontChanged(object sender, EventArgs e)
        {
            //OnFontChanged(EventArgs.Empty);
        }
		  
        private int m_HalfHourHeight = 22;
        [DefaultValue(22)]
        public int HalfHourHeight
        {
            get
            {
                return m_HalfHourHeight;
            }
            set
            {
                if (m_HalfHourHeight != value)
                {
                    m_HalfHourHeight = value;
                    OnHalfHourHeightChanged();
                }
            }
        }

		/**
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            float FontHeight = 18;

            if (Renderer == null)
                FontHeight = this.Font.GetHeight();
            else
                FontHeight = Renderer.BaseFont.GetHeight();

            float factor = 1.5f;
            HalfHourHeight = Math.Max(22, (int)Math.Ceiling(Font.GetHeight() * factor));

            //newVal += newVal % 2;
            //HalfHourHeight = newVal;
        }
        **/

        private void OnHalfHourHeightChanged()
        {
            AdjustScrollbar();
            Invalidate();
        }

        private AbstractRenderer m_Renderer;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AbstractRenderer Renderer
        {
            get
            {
                return m_Renderer;
            }
            set
            {
                m_Renderer = value;
                OnRendererChanged();
            }
        }

        private void OnRendererChanged()
        {
            //this.Font = m_Renderer.BaseFont;
            this.Invalidate();
        }

        private int m_DaysToShow = 5;
        [DefaultValue(1)]
        public int DaysToShow
        {
            get
            {
                return m_DaysToShow;
            }
            set
            {
                m_DaysToShow = value;
                OnDaysToShowChanged();
            }
        }

        protected virtual void OnDaysToShowChanged()
        {
            if (this.CurrentlyEditing)
                FinishEditing(true);

            Invalidate();
        }

        private SelectionType m_Selection;
        [Browsable(false)]
        public SelectionType Selection
        {
            get
            {
                return m_Selection;
            }
        }

        private DateTime m_StartDate;
        public DateTime StartDate
        {
            get
            {
                return m_StartDate;
            }
            set
            {
                if (m_StartDate.Date != value.Date)
                {
                    m_StartDate = value.Date;
                    OnStartDateChanged();
                }
            }
        }

        protected virtual void OnStartDateChanged()
        {
            m_StartDate = m_StartDate.Date;

            m_SelectedAppointment = null;
            m_SelectedAppointmentIsNew = false;
            m_Selection = SelectionType.DateRange;
            OnSelectionChanged();

            Invalidate();
        }

        private int m_StartHour = 8;
        [DefaultValue(8)]
        public int StartHour
        {
            get
            {
                return m_StartHour;
            }
            set
            {
                m_StartHour = value;
                OnStartHourChanged();
            }
        }


        protected virtual void OnStartHourChanged()
        {
            //scrollbar.Value = (startHour * 2 * halfHourHeight);            
            //m_ScrollBar.Value = m_ScrollBar.Minimum + Math.Max(0, Math.Min(m_ScrollBar.Maximum - m_ScrollBar.LargeChange, (m_StartHour * 2 * m_HalfHourHeight) + m_HalfHourHeight - (ClientRectangle.Height - HeaderHeight)));

			VScrollBar.Value = VScrollBar.Minimum + Math.Max(0, Math.Min(VScrollBar.Maximum - VScrollBar.LargeChange, (m_StartHour * 2 * m_HalfHourHeight) + m_HalfHourHeight - (ClientRectangle.Height - HeaderHeight)));

            Invalidate();
        }
		/*
        */

        private Appointment m_SelectedAppointment;
        [Browsable(false)]
        public Appointment SelectedAppointment
        {
            get 
            { 
                return m_SelectedAppointment; 
            }
            set
            {
                m_SelectedAppointment = value;
            }
        }

        private DateTime m_SelectionStart;
        public DateTime SelectionStart
        {
            get { return m_SelectionStart; }
            set { m_SelectionStart = value; }
        }

        private DateTime m_SelectionEnd;
        public DateTime SelectionEnd
        {
            get { return m_SelectionEnd; }
            set { m_SelectionEnd = value; }
        }

        private ITool m_ActiveTool;

        [Browsable(false)]
        public ITool ActiveTool
        {
            get { return m_ActiveTool; }
            set { m_ActiveTool = value; }
        }

        [Browsable(false)]
        public bool CurrentlyEditing
        {
            get
            {
                //return m_EditBox.Visible;
				return false;
            }
        }        

        private bool m_SelectedAppointmentIsNew;
        public bool SelectedAppointmentIsNew
        {
            get
            {
                return m_SelectedAppointmentIsNew;
            }
        }

        private bool m_AllowScroll = true;
        [DefaultValue(true)]
        public bool AllowScroll
        {
            get
            {
                return m_AllowScroll;
            }
            set
            {
                m_AllowScroll = value;
            }
        }

        private bool m_AllowInplaceEditing = true;
        [DefaultValue(true)]
        public bool AllowInplaceEditing
        {
            get
            {
                return m_AllowInplaceEditing;
            }
            set
            {
                m_AllowInplaceEditing = value;
            }
        }

        private bool m_AllowNew = true;

        [DefaultValue(true)]
        public bool AllowNew
        {
            get
            {
                return m_AllowNew;
            }
            set
            {
                m_AllowNew = value;
            }
        }

        private int HeaderHeight
        {
            get
            {
                if (m_Owners.Count == 0)
                    return m_DayHeadersHeight + m_AllDayEventsHeaderHeight;
                else
                    return (m_DayHeadersHeight * 2) + m_AllDayEventsHeaderHeight;
            }
        }
        
		/**
        void editbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                FinishEditing(true);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                FinishEditing(false);
            }
        }
		**/

        void selectionTool_Complete(object sender, EventArgs e)
        {
            if (m_SelectedAppointment != null && m_AllowInplaceEditing)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(EnterEditMode));
            }
        }
			
        void scrollbar_Scroll(object sender, EventArgs e)
        {
            //Invalidate();

			/*
            if (m_EditBox.Visible)
                //scroll text box too
                m_EditBox.Top += e.OldValue - e.NewValue;
            */
        } 		


        

		/***
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            //m_HalfHourHeight = 22 * Helpers.FontScaleFactor
            m_HalfHourHeight = (int)Math.Round((double)SystemFonts.IconTitleFont.GetHeight() * 1.5);
            m_DayHeadersHeight = (int)Math.Round((double)SystemFonts.IconTitleFont.GetHeight() * 1.5);
            m_HourLabelWidth = (int)Math.Round(((double)52));
            m_HourLabelIndent = (int)Math.Round(((double)2));

            if (factor.Height != 1f && Renderer != null)
                Renderer.ResetFonts();

            base.ScaleControl(factor, specified);
            AdjustScrollbar();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            AdjustScrollbar();
        }

        private bool NeedsScrollbar
        {
            get
            {                
                return (48 * this.m_HalfHourHeight >= ClientRectangle.Height - this.HeaderHeight);                
            }
        }
        ***/

		/**
		**/

		// ToDo: This should later work automatically (without overriding)
		// when Scrollbar.Size = WindowSize, what is true in very most cases
		public override void OnResize()
		{
			//base.OnResize();
			AdjustScrollbar();
			//this.Invalidate();
		}
			     

        public void AdjustScrollbar()
        {            
            // ToDo                        

			if (Parent == null)
				return;

			float windowSize = this.Bounds.Height - HeaderHeight;
			if (windowSize <= 0)
				return;

			int documentSize = this.m_HalfHourHeight * 48;
			VScrollBar.SetUp (windowSize, documentSize, m_HalfHourHeight * 2);
        }


		/**

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Flicker free
        }        


        Point m_LastMouseDown;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Capture focus
            this.Focus();

            m_LastMouseDown = new Point(e.X, e.Y);

            HideTooltip();

            if (CurrentlyEditing)
            {
                FinishEditing(false);
            }

            if (m_SelectedAppointmentIsNew)
            {
                RaiseNewAppointment();
            }

            if (e.Clicks > 1)
                return;

            ITool newTool = null;

            Appointment appointment = GetAppointmentAt(e.X, e.Y);

            if (appointment == null)
            {
                if (m_SelectedAppointment != null)
                {
                    m_SelectedAppointment = null;
                    OnSelectionChanged(EventArgs.Empty);
                    Invalidate();
                }

                // ToDo ??
                newTool = m_DrawTool;
                //newTool = null;
                m_ActiveTool = null;
                m_Selection = SelectionType.DateRange;
            }
            else
            {
                newTool = m_SelectionTool;
                m_SelectedAppointment = appointment;
                m_Selection = SelectionType.Appointment;
                OnSelectionChanged(EventArgs.Empty);

                Invalidate();
            }

            if (m_ActiveTool != null)
            {
                m_ActiveTool.MouseDown(e);
            }

            if ((m_ActiveTool != newTool) && (newTool != null))
            {
                newTool.Reset();
                newTool.MouseDown(e);
            }

            m_ActiveTool = newTool;

            base.OnMouseDown(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (m_LastMouseDown.X < m_HourLabelWidth || m_LastMouseDown.X > this.ClientRectangle.Width - m_ScrollBar.Width)
                return;

            if (m_LastMouseDown.Y <= HeaderHeight)
                return;

            if (CurrentlyEditing)
                return;

            Appointment appointment = GetAppointmentAt(m_LastMouseDown.X, m_LastMouseDown.Y);
            if (appointment != null)
            {
                if (EditAppointment != null)
                {
                    EditAppointment(this, new AppointmentEventArgs(appointment));
                }
            }
            else
            {
                if (NewAppointment != null)
                {
                    DateTime dtStart = GetTimeAt(m_LastMouseDown.X, m_LastMouseDown.Y);
                    int OwnerID = GetOwnerAt(m_LastMouseDown.X);

                    int minute = dtStart.Minute;
                    if (minute < 30)
                        minute = 0;
                    else
                        minute = 30;

                    dtStart = dtStart.Date.AddHours(dtStart.Hour).AddMinutes(minute);

                    NewAppointmentEventArgs args = new NewAppointmentEventArgs("", dtStart, dtStart.AddMinutes(30), OwnerID);
                    NewAppointment(this, args);

                    m_SelectedAppointment = null;
                    m_SelectedAppointmentIsNew = false;
                    OnSelectionChanged(EventArgs.Empty);
                }
            }
        }

        private string GetTooltipText(Appointment appointment)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(appointment.StartDate.ToString("f"));
            sb.Append("\r\n");
            sb.Append(appointment.EndDate.ToString("f"));
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append(appointment.Title);

            if (!String.IsNullOrEmpty(appointment.Description))
            {
                sb.Append("\r\n");
                sb.Append("\r\n");

                if (appointment.Description.Length > 1000)
                {
                    sb.Append(KS.Foundation.Strings.StrLeft(appointment.Description, 1000));
                    sb.Append("...");                    
                }
                else
                    sb.Append(appointment.Description);                
            }

            return sb.ToString();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_ToolTipEnabled)
            {
                if (Math.Abs(e.X - m_TooltipX) > 4 || Math.Abs(e.Y - m_TooltipY) > 4)
                {
                    m_TimerTooltip.Stop();

                    m_TooltipX = e.X;
                    m_TooltipY = e.Y;

                    Appointment appointment = GetAppointmentAt(e.X, e.Y);
                    if (appointment != null)
                    {
                        m_TooltipText = GetTooltipText(appointment);
                        if (m_TooltipText != "")
                            m_TimerTooltip.Start();
                    }
                    else
                    {
                        HideTooltip();
                    }
                }
            }

            if (m_ActiveTool != null)
                m_ActiveTool.MouseMove(e);            

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (m_ActiveTool != null)
                m_ActiveTool.MouseUp(e);

            base.OnMouseUp(e);
        }

        public void HideTooltip()
        {
            if (m_TooltipShown)
            {
                m_Tooltip.Hide(this);
                m_TooltipShown = false;
            }
        }

        void tmrToolTip_Tick(object sender, EventArgs e)
        {
            m_TimerTooltip.Stop();
            
            this.m_Tooltip.Show(m_TooltipText, this, m_TooltipX + 6, m_TooltipY + 6);

            m_TooltipShown = true;
        }
		*/

        System.Collections.Hashtable cachedAppointments = new System.Collections.Hashtable();
        private DateTime m_CachedStartDate = DateTime.MinValue;
        private DateTime m_CachedEndDate = DateTime.MinValue;
        private Appointment m_CachedSelectedAppointment = null;

        public void ResetCache()
        {
            m_CachedStartDate = DateTime.MinValue;
            m_CachedEndDate = DateTime.MinValue;
            m_CachedSelectedAppointment = null;
        }

        protected virtual void OnResolveAppointments(ResolveAppointmentsEventArgs args)
        {
            if (m_CachedStartDate == args.StartDate && m_CachedEndDate == args.EndDate && m_CachedSelectedAppointment == m_SelectedAppointment)
                return;

            //System.Diagnostics.Debug.WriteLine("Resolve app");

			/**
            if (ResolveAppointments != null)
                ResolveAppointments(this, args);
            **/

            this.m_AllDayEventsHeaderHeight = 0;

            // cache resolved appointments in hashtable by days.
            cachedAppointments.Clear();

            if ((m_SelectedAppointmentIsNew) && (m_SelectedAppointment != null))
            {
                if ((m_SelectedAppointment.StartDate > args.StartDate) && (m_SelectedAppointment.StartDate < args.EndDate))
                {
                    args.Appointments.Add(m_SelectedAppointment);
                }
            }

			/**
            foreach (Appointment appointment in args.Appointments)
            {
                int key = -1;
                AppointmentList list;

                if (appointment.StartDate.Day == appointment.EndDate.Day)
                {
                    key = appointment.StartDate.Day;
                }
                else
                {
                    // use -1 for exceeding one more than day
                    key = -1;

                    // ALL DAY EVENTS IS NOT COMPLETE
                    //this.allDayEventsHeaderHeight += horizontalAppointmentHeight;
                     //
                }

                list = (AppointmentList)cachedAppointments[key];

                if (list == null)
                {
                    list = new AppointmentList();
                    cachedAppointments[key] = list;
                }

                list.Add(appointment);
            }
			**/

            m_CachedStartDate = args.StartDate;
            m_CachedEndDate = args.EndDate;
            m_CachedSelectedAppointment = m_SelectedAppointment;
        }
			
        public void OnAppointmentMove(AppointmentEventArgs e)
        {
            if (AppoinmentMove != null)
				AppoinmentMove(this, e);
        }

		/**
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if ((m_AllowNew) && char.IsLetterOrDigit(e.KeyChar))
            {
                if ((this.Selection == SelectionType.DateRange))
                {
                    if (!m_SelectedAppointmentIsNew)
                        EnterNewAppointmentMode(e.KeyChar);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {            
            if (e.KeyCode == Keys.Delete)
            {
                if (!this.CurrentlyEditing && m_SelectedAppointment != null && DeleteAppointment != null)
                {
                    DeleteAppointment(this, new AppointmentEventArgs(m_SelectedAppointment));
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {                
                if (!this.CurrentlyEditing && m_SelectedAppointment != null && EditAppointment != null)
                {
                    EditAppointment(this, new AppointmentEventArgs(m_SelectedAppointment));
                    e.Handled = true;
                }
            }

            base.OnKeyDown(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!CurrentlyEditing)
            {
                switch (keyData)
                {
                    case Keys.Up:
                        ScrollUp();
                        return true;

                    case Keys.Down:
                        ScrollDown();
                        return true;

                    case Keys.PageUp:
                        ScrollPageUp();
                        return true;

                    case Keys.PageDown:
                        ScrollPageDown();
                        return true;

                    case Keys.Home:
                        this.StartHour = 0;
                        return true;

                    case Keys.End:
                        this.StartHour = 24;
                        return true;                    
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        **/

		public override bool OnKeyDown (OpenTK.Input.KeyboardKeyEventArgs e)
		{
			if (!Visible || !Enabled)
				return false;

			bool handled = false;

			switch (e.Key) {
			case Key.Left:
				StartDate = StartDate.AddDays(-1).Date;
				handled = true;
				break;
			case Key.Right:
				StartDate = StartDate.AddDays(1).Date;
				handled = true;
				break;
			case Key.Escape:
				StartDate = DateTime.Now.Date;
				handled = true;
				break;
			}

			if (handled) {
				Invalidate ();
				return true;
			}				

			return base.OnKeyDown (e);
		}

        private void EnterNewAppointmentMode(char key)
        {
            Appointment appointment = new Appointment();
            appointment.StartDate = m_SelectionStart;
            appointment.EndDate = m_SelectionEnd;
            appointment.Title = key.ToString();

            m_SelectedAppointment = appointment;
            m_SelectedAppointmentIsNew = true;
            OnSelectionChanged();

            m_ActiveTool = m_SelectionTool;

            Invalidate();

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(EnterEditMode));
        }

        private delegate void StartEditModeDelegate(object state);

        private void EnterEditMode(object state)
        {
            if (!m_AllowInplaceEditing)
                return;

			/**
            if (this.InvokeRequired)
            {
                Appointment selectedApp = m_SelectedAppointment;

                System.Threading.Thread.Sleep(200);

                if (selectedApp == m_SelectedAppointment)
                    this.Invoke(new StartEditModeDelegate(EnterEditMode), state);
            }
            else
            {
                StartEditing();
            }
			**/
        }

        internal void RaiseNewAppointment()
        {            
			/**
			NewAppointmentEventArgs args = new NewAppointmentEventArgs(m_SelectedAppointment.Title, m_SelectedAppointment.StartDate, m_SelectedAppointment.EndDate, m_SelectedAppointment.OwnerID);
            if (NewAppointment != null)
            {
                NewAppointment(this, args);
            }
            **/

            m_SelectedAppointment = null;
            m_SelectedAppointmentIsNew = false;            

            Invalidate();
        }

        public void StartEditing()
        {
            if (!m_SelectedAppointment.Locked && appointmentViews.ContainsKey(m_SelectedAppointment))
            {
				RectangleF editBounds = appointmentViews[m_SelectedAppointment].Rectangle;

                editBounds.Inflate(-3, -3);
                editBounds.X += m_AppointmentGripWidth - 2;
                editBounds.Width -= m_AppointmentGripWidth - 5;

				/**
                m_EditBox.Bounds = editBounds;
                m_EditBox.Text = m_SelectedAppointment.Title;
                m_EditBox.Visible = true;
                m_EditBox.SelectionStart = m_EditBox.Text.Length;
                m_EditBox.SelectionLength = 0;

                m_EditBox.Focus();
                **/
            }
        }

        public void FinishEditing(bool cancel)
        {
            //m_EditBox.Visible = false;

            if (!cancel)
            {
				/**
                if (m_SelectedAppointment != null)
                    m_SelectedAppointment.Title = m_EditBox.Text;
                    **/
            }
            else
            {
                if (m_SelectedAppointmentIsNew)
                {
                    m_SelectedAppointment = null;
                    m_SelectedAppointmentIsNew = false;
                    OnSelectionChanged();
                }
            }

            Invalidate();
            //this.Focus();
        }

        public DateTime GetTimeAt(int x, int y)
        {			
			int dayWidth = (int)((this.ClientRectangle.Width - m_HourLabelWidth) / m_DaysToShow);

			// ToDo: Check ScrollOffsetY or -ScrollOffsetY
			int hour = (int)((y - this.HeaderHeight + ScrollOffsetY) / m_HalfHourHeight);
			x -= m_HourLabelWidth;
			DateTime date = m_StartDate;
			date = date.Date;
			date = date.AddDays(x / dayWidth);
			if ((hour > 0) && (hour < 24 * 2))
				date = date.AddMinutes((hour * 30));
			return date;
        }

		//public int Width { get; set; }

        public int GetOwnerAt(int x)
        {            
            if (m_Owners.Count == 0)
                return 0;
				            
			float dayWidth = (this.ClientRectangle.Width - m_HourLabelWidth) / m_DaysToShow;

			int ownerWidth = (int)(dayWidth + 0.5f);
            if (m_Owners.Count > 0)
                ownerWidth /= m_Owners.Count;

            int OwnerID = 0;
            int column = 0;
            for (int i = m_HourLabelWidth; i < this.Width; i += ownerWidth)
            {
                if (i > x)
                {
                    int OwnerIndex = (column - 1) % m_Owners.Count;                    
                    
                    // Cannot access Keys by index, so we must enumerate through them
                    int k = 0;
                    foreach (int ownerID in m_Owners.Keys)
                    {
                        if (k == OwnerIndex)
                        {
                            OwnerID = ownerID;
                            break;
                        }
                        k++;
                    }
                    
                    break;
                }

                column++;
            }            

            return OwnerID;
        }

        public Appointment GetAppointmentAt(int x, int y)
        {
            if (y < this.HeaderHeight)
                return null;

            foreach (AppointmentView view in appointmentViews.Values)
                if (view.Rectangle.Contains(x, y))
                    return view.Appointment;

            return null;
        }			
        
		public override void OnPaint(IGUIContext ctx, RectangleF bounds)
        {
            // resolve appointments on visible date range.
            ResolveAppointmentsEventArgs args = new ResolveAppointmentsEventArgs(this.StartDate, this.StartDate.AddDays(m_DaysToShow));

            try
            {
                OnResolveAppointments(args);
            }
            catch (Exception)
            {                
            }
				            
			//System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, (int)(bounds.Width), (int)bounds.Height);
			//System.Drawing.Rectangle rectangle = bounds;
			RectangleF rectangle = new RectangleF(bounds.Left, bounds.Top, bounds.Width - VScrollBar.Width, bounds.Height);

			RectangleF hourBackRectangle = rectangle;
            hourBackRectangle.Width = m_HourLabelWidth;

			using (var backBrush = new  SolidBrush (m_Renderer.HoursBackColor)) {
				ctx.FillRectangle (backBrush, hourBackRectangle);            
			}

			RectangleF hourLabelRectangle = rectangle;
            hourLabelRectangle.Y += this.HeaderHeight;

            DrawHourLabels(ctx, hourLabelRectangle);

			RectangleF daysRectangle = rectangle;
            daysRectangle.X += m_HourLabelWidth;
            daysRectangle.Y += this.HeaderHeight;
            daysRectangle.Width -= m_HourLabelWidth;

            if (bounds.IntersectsWith(daysRectangle))
            {
                DrawDays(ctx, daysRectangle);
            }

			RectangleF headerRectangle = rectangle;

            headerRectangle.X += m_HourLabelWidth;
            headerRectangle.Width -= m_HourLabelWidth;
            headerRectangle.Height = m_DayHeadersHeight;

            if (bounds.IntersectsWith(headerRectangle))
                DrawDayHeaders(ctx, headerRectangle);            
        }

		public float ScrollOffsetY 
		{ 
			get {
				return VScrollBar.Value;
			}
			set {
				System.Diagnostics.Debug.WriteLine ("****** DayView ScrollOffsetY not implemented. ******");
			}
		}

		private void DrawHourLabels(IGUIContext gfx, RectangleF rect)
        {            
			//gfx.SetClip (rect);

            int i = 0;
            for (int m_Hour = 0; m_Hour < 24; m_Hour++)
            {
				RectangleF hourRectangle = rect;

				hourRectangle.Y = (float)(rect.Y + (m_Hour * 2 * m_HalfHourHeight) - ScrollOffsetY);
                hourRectangle.X += m_HourLabelIndent;
                hourRectangle.Width = m_HourLabelWidth;
                                
                hourRectangle.Height = 2 * m_HalfHourHeight;

				if (hourRectangle.Bottom > this.HeaderHeight) {
					m_Renderer.DrawHourLabel (gfx, hourRectangle, m_Hour);
				}

                // Alternating Row Colors
                //if (i % 2 > 0)
                //{
                //    Rectangle rLine = new Rectangle(this.m_HourLabelWidth, hourRectangle.Y, rect.Width - m_HourLabelWidth, 2 * m_HalfHourHeight);
                    
                //    using (SolidBrush brush = new SolidBrush(KS.Gantt.Dialogs.Globals.GridAlternatingRowColor))
                //    {
                //        e.Graphics.FillRectangle(brush, rLine);
                //    }
                //}

                i++;
            }

			float currentTime = (float)(HeaderHeight + DateTime.Now.TimeOfDay.TotalMinutes / 30d * m_HalfHourHeight) - ScrollOffsetY + Bounds.Top;
			using (var pen = new Pen (System.Drawing.Color.FromArgb (200, SummerGUI.Theme.Colors.Orange), 3f)) {
				gfx.DrawLine (pen, 0, currentTime, this.m_HourLabelWidth, currentTime);
			}				
        }

		private void DrawDayHeaders(IGUIContext gfx, RectangleF rect)
        {
			float dayWidth = rect.Width / m_DaysToShow;
			float extraWidth = 0;

            // one day header rectangle
			RectangleF dayHeaderRectangle = new RectangleF(rect.Left, rect.Top, dayWidth, rect.Height);
            DateTime headerDate = m_StartDate;

            for (int day = 0; day < m_DaysToShow; day++)
            {
				// fill up last rectangle
				if (m_DaysToShow > 1 && day == m_DaysToShow - 1) {
					extraWidth = rect.Width.Ceil() % m_DaysToShow;
					dayHeaderRectangle.Width += extraWidth;
				}

				m_Renderer.DrawDayHeader(gfx, dayHeaderRectangle, headerDate);

                if (m_Owners.Count > 0)
                {
					float userWidth = dayWidth / m_Owners.Count;
					RectangleF userHeaderRectangle = new RectangleF(dayHeaderRectangle.Left, dayHeaderRectangle.Top + dayHeaderRectangle.Height, userWidth, dayHeaderRectangle.Height);

                    int i = 0;
                    foreach (int ownerID in m_Owners.Keys)
                    {
                        if (i == m_Owners.Count - 1)
                        {
                            userHeaderRectangle.Width += (dayHeaderRectangle.Right - userHeaderRectangle.Right);
                        }

						DrawUserHeader(gfx, userHeaderRectangle, ownerID);
                        userHeaderRectangle.X += userWidth;

                        i++;                        
                    }
                }

				dayHeaderRectangle.X += dayWidth + extraWidth;
                headerDate = headerDate.AddDays(1);
            }

			// fill the space above the scrollbar
			//m_Renderer.DrawDayGripper(gfx, dayHeaderRectangle, m_AppointmentGripWidth);
			dayHeaderRectangle.Width = Bounds.Width - dayHeaderRectangle.X;
			m_Renderer.DrawDayHeader(gfx, dayHeaderRectangle, DateTime.MinValue);

			/*
			System.Drawing.Rectangle rlast = new System.Drawing.Rectangle(dayHeaderRectangle.Left, HeaderHeight, m_AppointmentGripWidth, ClientRectangle.Height - HeaderHeight);
			using (var m_Pen = new Pen (m_Renderer.GripperBorderColor)) {
				gfx.DrawLine (m_Pen, rect.Left + 0.5f, rect.Top - 1, rect.Left + 0.5f, rect.Height);
			}
			m_Renderer.DrawDayGripper(gfx, rlast, m_AppointmentGripWidth);
			*/
        }

		private void DrawUserHeader(IGUIContext gfx, RectangleF rect, int OwnerID)
        {
            string strUserName = "";
            if (m_Owners.ContainsKey(OwnerID))
                strUserName = m_Owners[OwnerID];
            
            m_Renderer.DrawUserHeader(gfx, rect, strUserName);
        }

		private RectangleF GetHourRangeRectangle(DateTime start, DateTime end, RectangleF baseRectangle)
        {
			RectangleF rect = baseRectangle;            

			float startY;
			float endY;            

            startY = (start.Hour * m_HalfHourHeight * 2) + ((start.Minute * m_HalfHourHeight) / 30);
            endY = (end.Hour * m_HalfHourHeight * 2) + ((end.Minute * m_HalfHourHeight) / 30);
            
			rect.Y = startY - this.ScrollOffsetY + this.HeaderHeight + Top;
            rect.Height = endY - startY;

            return rect;
        }

		private void DrawDay(IGUIContext gfx, RectangleF rect, DateTime time, int OwnerID)
        {			
			m_Renderer.DrawDayBackground(gfx, rect, IsFocused);

            // Draw WorkingHours

			IEnumerable<WorkingHour> whc = WorkingHourCollection.SampleWorkingHours ();
			if (whc != null)
			{
				foreach (WorkingHour wh in whc)
				{
					// Kroll
					DateTime dtWhStart = time.Date.AddHours(wh.StartHour);
					DateTime dtWhEnd = time.Date.AddHours(wh.EndHour).AddMilliseconds(-1); // To handle EndHour == 24.0
					RectangleF workingHoursRectangle = GetHourRangeRectangle(dtWhStart, dtWhEnd, rect);

					//workingHoursRectangle.Offset (0, rect.Top);

					if (workingHoursRectangle.Y < this.HeaderHeight)
					{
						float iDelta = this.HeaderHeight - workingHoursRectangle.Y;
						workingHoursRectangle.Y = this.HeaderHeight;
						workingHoursRectangle.Height -= iDelta;
					}

					// Kroll
					if (WorkingHourCollection.IsWorkingDay(time))
						m_Renderer.DrawHourRange(gfx, workingHoursRectangle, false, false, this.IsFocused);

					if (m_ShowDateRangeSelection && (m_Selection == SelectionType.DateRange) && (time.Day == m_SelectionStart.Day))
					{
						RectangleF selectionRectangle = GetHourRangeRectangle(m_SelectionStart, m_SelectionEnd, rect);
						m_Renderer.DrawHourRange(gfx, selectionRectangle, false, true, this.IsFocused);
					}
				}
			}
		


			/**
            if (m_GanttCalendar != null && m_GanttCalendar.IsWorkingDay(time))
            {
                KS.Gantt.Calendars.WorkingHourCollection whc = m_GanttCalendar.InheritedWorkingHours(time);
                if (whc != null)
                {
                    foreach (KS.Gantt.Calendars.WorkingHour wh in whc)
                    {
                        // Kroll
                        DateTime dtWhStart = time.Date.AddHours(wh.StartHour);
                        DateTime dtWhEnd = time.Date.AddHours(wh.EndHour).AddMilliseconds(-1); // To handle EndHour == 24.0
                        Rectangle workingHoursRectangle = GetHourRangeRectangle(dtWhStart, dtWhEnd, rect);

                        if (workingHoursRectangle.Y < this.HeaderHeight)
                        {
                            int iDelta = this.HeaderHeight - workingHoursRectangle.Y;
                            workingHoursRectangle.Y = this.HeaderHeight;
                            workingHoursRectangle.Height -= iDelta;
                        }

                        // Kroll
                        if (m_GanttCalendar.IsWorkingDay(time))
                            m_Renderer.DrawHourRange(e.Graphics, workingHoursRectangle, false, false);

                        if (m_ShowDateRangeSelection && (m_Selection == SelectionType.DateRange) && (time.Day == m_SelectionStart.Day))
                        {
                            Rectangle selectionRectangle = GetHourRangeRectangle(m_SelectionStart, m_SelectionEnd, rect);
                            m_Renderer.DrawHourRange(e.Graphics, selectionRectangle, false, true);
                        }
                    }
                }
            }
            **/
			            
			int startHour = GetTimeAt (0, 0).Hour;

			using (var hourPen = new Pen (m_Renderer.HourSeperatorColor))
			using (var halfHourPen = new Pen (m_Renderer.HalfHourSeperatorColor, 1f))
			using (var HourLineBuf = new LineDrawingBuffer (gfx))
			using (var HalfHourLineBuf = new LineDrawingBuffer (gfx, LineStyles.Dotted)) {

				for (int hour = startHour * 2; hour < 24 * 2; hour++) {
					float y = rect.Top + (hour * m_HalfHourHeight) - ScrollOffsetY;
					if (y < rect.Top)
						continue;
					if (y > rect.Bottom)
						break;
					if ((hour % 2) == 0)
						HourLineBuf.AddLine (hourPen, rect.Left, y, rect.Right, y);
					else
						HalfHourLineBuf.AddLine (halfHourPen, rect.Left, y, rect.Right, y);					
				}
			}

            m_Renderer.DrawDayGripper(gfx, rect, m_AppointmentGripWidth);
			            
            DrawAppointments(gfx, rect, time, OwnerID);
        }

        internal Dictionary<Appointment, AppointmentView> appointmentViews = new Dictionary<Appointment, AppointmentView>();

		private void DrawAppointments(IGUIContext gfx, RectangleF rect, DateTime time, int OwnerID)
        {            
            DateTime timeStart = time.Date;
            DateTime timeEnd = timeStart.AddHours(24);
            timeEnd = timeEnd.AddSeconds(-1);

            AppointmentList appointments = null;

            if (OwnerID < 0)
                appointments = (AppointmentList)cachedAppointments[time.Day];
            else
            {
                appointments = new AppointmentList();

                if (cachedAppointments.ContainsKey(time.Day))
                {
                    foreach (Appointment app in (AppointmentList)cachedAppointments[time.Day])
                    {
                        if (app.OwnerID == OwnerID)                        
                            appointments.Add(app);
                    }
                }
            }

            if (appointments != null && appointments.Count > 0)
            {
                HalfHourLayout[] layout = GetMaxParalelAppointments(appointments);
                List<Appointment> drawnItems = new List<Appointment>();

                for (int halfHour = 0; halfHour < 24 * 2; halfHour++)
                {
                    HalfHourLayout hourLayout = layout[halfHour];

                    if ((hourLayout != null) && (hourLayout.Count > 0))
                    {
                        for (int appIndex = 0; appIndex < hourLayout.Count; appIndex++)
                        {
                            Appointment appointment = hourLayout.Appointments[appIndex];

                            if (drawnItems.IndexOf(appointment) < 0)
                            {
								RectangleF appRect = rect;
								float appointmentWidth;
                                AppointmentView view;

                                appointmentWidth = rect.Width / appointment.m_ConflictCount;

								float lastX = 0;

                                foreach (Appointment app in hourLayout.Appointments)
                                {
                                    if ((app != null) && (appointmentViews.ContainsKey(app)))
                                    {
                                        view = appointmentViews[app];

                                        if (lastX < view.Rectangle.X)
                                            lastX = view.Rectangle.X;
                                    }
                                }

                                if ((lastX + (appointmentWidth * 2f)) > (rect.X + rect.Width))
                                    lastX = 0;

                                //appRect.Width = appointmentWidth - 5;
                                appRect.Width = appointmentWidth;

                                if (lastX > 0)
                                    appRect.X = lastX + appointmentWidth;

                                appRect = GetHourRangeRectangle(appointment.StartDate, appointment.EndDate, appRect);

                                view = new AppointmentView();
                                view.Rectangle = appRect;
                                view.Appointment = appointment;

                                appointmentViews[appointment] = view;

                                //gfx.SetClip(rect);

                                // Kroll
                                //appRect.Offset(5, 0);
                                //appRect.Offset(5, 0);

                                bool bSelected = false;
                                if (m_SelectedAppointment != null)
                                {
                                    bSelected = m_SelectedAppointment == appointment;
                                    if (!bSelected && m_SelectedAppointment.Tag != null && appointment.Tag != null)
                                        bSelected = m_SelectedAppointment.Tag.ToString() == appointment.Tag.ToString();
                                }

                                //renderer.DrawAppointment(e.Graphics, appRect, appointment, appointment == selectedAppointment, appointmentGripWidth);
                                m_Renderer.DrawAppointment(gfx, appRect, appointment, bSelected, m_AppointmentGripWidth);

                                //gfx.ResetClip();

                                drawnItems.Add(appointment);
                            }
                        }
                    }
                }
            }            
        }

        private HalfHourLayout[] GetMaxParalelAppointments(List<Appointment> appointments)
        {
            HalfHourLayout[] appLayouts = new HalfHourLayout[24 * 2];

            foreach (Appointment appointment in appointments)
            {
                appointment.m_ConflictCount = 1;
            }

            foreach (Appointment appointment in appointments)
            {
                int firstHalfHour = appointment.StartDate.Hour * 2 + (appointment.StartDate.Minute / 30);
                int lastHalfHour = appointment.EndDate.Hour * 2 + (appointment.EndDate.Minute / 30);                

                for (int halfHour = firstHalfHour; halfHour < lastHalfHour; halfHour++)
                {
                    HalfHourLayout layout = appLayouts[halfHour];

                    if (layout == null)
                    {
                        layout = new HalfHourLayout();
                        
                        //layout.Appointments = new Appointment[20];
                        
                        // Kroll
                        layout.Appointments = new Appointment[appointments.Count + 1];

                        appLayouts[halfHour] = layout;
                    }

                    layout.Appointments[layout.Count] = appointment;
                    layout.Count++;                    

                    // update conflicts
                    foreach (Appointment app2 in layout.Appointments)
                    {
                        if (app2 != null)
                            if (app2.m_ConflictCount < layout.Count)
                                app2.m_ConflictCount = layout.Count;
                    }
                }
            }

            return appLayouts;
        }

		private void DrawDays(IGUIContext gfx, RectangleF rect)
        {
			float dayWidth = rect.Width / m_DaysToShow;
            
            //int userWith = dayWidth;
            if (m_Owners.Count > 0)
                //userWith = dayWidth / m_Owners.Count;
                dayWidth /= m_Owners.Count;



			/* ALL DAY EVENTS IS NOT COMPLETE
			
            AppointmentList longAppointments = (AppointmentList)cachedAppointments[-1];

            int y = dayHeadersHeight;

            if (longAppointments != null)
            {
                Rectangle backRectangle = rect;
                backRectangle.Y = y;
                backRectangle.Height = allDayEventsHeaderHeight;

                renderer.DrawAllDayBackground(e.Graphics, backRectangle);

                foreach (Appointment appointment in longAppointments)
                {
                    Rectangle appointmenRect = rect;

                    appointmenRect.Width = (dayWidth * (appointment.EndDate.Day - appointment.StartDate.Day));
                    appointmenRect.Height = horizontalAppointmentHeight;
                    appointmenRect.X += (appointment.StartDate.Day - startDate.Day) * dayWidth;
                    appointmenRect.Y = y;

                    renderer.DrawAppointment(e.Graphics, appointmenRect, appointment, appointment == selectedAppointment, appointmentGripWidth);

                    y += horizontalAppointmentHeight;
                }
            }
            */

            DateTime time = m_StartDate;
			RectangleF rectangle = rect;
            rectangle.Width = dayWidth;

			float extraWidth = 0;

            appointmentViews.Clear();
            
            for (int day = 0; day < m_DaysToShow; day++)
            {
				if (m_DaysToShow > 1 && day == m_DaysToShow - 1)
				{
					extraWidth = rect.Width % m_DaysToShow;
					rectangle.Width += extraWidth;
				}

                if (m_Owners.Count == 0)
                {
                    DrawDay(gfx, rectangle, time, -1);
                    rectangle.X += dayWidth;                    
                }
                else
                {
                    foreach (int ownerID in m_Owners.Keys)
                    {
                        DrawDay(gfx, rectangle, time, ownerID);
                        rectangle.X += dayWidth;                        
                    }
                }

                time = time.AddDays(1);
            }
        }

        class HalfHourLayout
        {
            public int Count;
            public Appointment[] Appointments;
        }

        internal class AppointmentView
        {
            public Appointment Appointment;
			public RectangleF Rectangle;
        }
			
        class AppointmentList : List<Appointment>
        {
        }		       		       

		protected override void CleanupManagedResources ()
		{
			/**
                if (m_TimerTooltip != null)
                {
                    m_TimerTooltip.Dispose();
                    m_TimerTooltip = null;
                }

                if (m_Tooltip != null)
                {
                    m_Tooltip.Dispose();
                    m_Tooltip = null;
                }
                **/

			if (m_DrawTool != null)
			{
				m_DrawTool.Dispose();
				m_DrawTool = null;
			}
			
			base.CleanupManagedResources ();
		}
    }
}
