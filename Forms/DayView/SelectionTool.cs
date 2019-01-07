/* Developed by Ertan Tike (ertan.tike@moreum.com) */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using KS.Foundation;

namespace SummerGUI.Scheduling
{
    public class SelectionTool : DisposableObject, ITool, IDisposable
    {        
		public DayView DayView { get; set; }
        
		/***
        private TimeSpan length;
        private Mode mode;
        private TimeSpan delta;
        ***/

        public void Reset()
        {
            //length = TimeSpan.Zero;
            //delta = TimeSpan.Zero;
        }

		/**
        public void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            Appointment selection = m_DayView.SelectedAppointment;

            if ((selection != null) && (!selection.Locked))
            {
                switch (e.Button)
                {
                    case System.Windows.Forms.MouseButtons.Left:

                        // Get time at mouse position
                        DateTime m_Date = m_DayView.GetTimeAt(e.X, e.Y);

                        switch (mode)
                        {
                            case Mode.Move:

                                // add delta value
                                m_Date = m_Date.Add(delta);

                                if (length == TimeSpan.Zero)
                                {
                                    startDate = selection.StartDate;
                                    length = selection.EndDate - startDate;
                                }
                                else
                                {
                                    DateTime m_EndDate = m_Date.Add(length);

                                    if (m_EndDate.Day == m_Date.Day)
                                    {
                                        selection.StartDate = m_Date;
                                        selection.EndDate = m_EndDate;
                                        m_DayView.Invalidate();
                                        m_DayView.RaiseAppointmentMove(new AppointmentEventArgs(selection));
                                    }
                                }

                                break;

                            case Mode.ResizeBottom:

                                if (m_Date > selection.StartDate)
                                {
                                    if (selection.EndDate.Day == m_Date.Day)
                                    {
                                        selection.EndDate = m_Date;
                                        m_DayView.Invalidate();
                                        m_DayView.RaiseAppointmentMove(new AppointmentEventArgs(selection));
                                    }
                                }

                                break;

                            case Mode.ResizeTop:

                                if (m_Date < selection.EndDate)
                                {
                                    if (selection.StartDate.Day == m_Date.Day)
                                    {
                                        selection.StartDate = m_Date;
                                        m_DayView.Invalidate();
                                        m_DayView.RaiseAppointmentMove(new AppointmentEventArgs(selection));
                                    }
                                }

                                break;
                        }

                        break;

                    default:

                        Mode tmpNode = GetMode(e);

                        switch (tmpNode)
                        {
                            case Mode.Move:
                                m_DayView.Cursor = System.Windows.Forms.Cursors.Default;
                                break;
                            case Mode.ResizeBottom:
                            case Mode.ResizeTop:
                                m_DayView.Cursor = System.Windows.Forms.Cursors.SizeNS;
                                break;
                        }

                        break;
                }
            }
        }

        private Mode GetMode(System.Windows.Forms.MouseEventArgs e)
        {
            if (m_DayView.SelectedAppointment == null)
                return Mode.None;

            if (m_DayView.appointmentViews.ContainsKey(m_DayView.SelectedAppointment))
            {
                DayView.AppointmentView view = m_DayView.appointmentViews[m_DayView.SelectedAppointment];

                Rectangle topRect = view.Rectangle;
                Rectangle bottomRect = view.Rectangle;

                bottomRect.Y = bottomRect.Bottom - 5;
                bottomRect.Height = 5;
                topRect.Height = 5;

                if (topRect.Contains(e.Location))
                    return Mode.ResizeTop;
                else if (bottomRect.Contains(e.Location))
                    return Mode.ResizeBottom;
                else
                    return Mode.Move;
            }

            return Mode.None;
        }

        public void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (Complete != null)
                    Complete(this, EventArgs.Empty);
            }

            m_DayView.RaiseSelectionChanged(EventArgs.Empty);            

            mode = Mode.Move;

            delta = TimeSpan.Zero;
        }

        public void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (m_DayView.SelectedAppointmentIsNew)
            {
                m_DayView.RaiseNewAppointment();
            }

            if (m_DayView.CurrentlyEditing)
                m_DayView.FinishEditing(false);

            mode = GetMode(e);

            if (m_DayView.SelectedAppointment != null)
            {
                DateTime downPos = m_DayView.GetTimeAt(e.X, e.Y);
                // Calculate delta time between selection and clicked point
                delta = m_DayView.SelectedAppointment.StartDate - downPos;
            }
            else
            {
                delta = TimeSpan.Zero;
            }

            length = TimeSpan.Zero;
        }
        **/

        public event EventHandler Complete;

        enum Mode
        {
            ResizeTop,
            ResizeBottom,
            Move,
            None
        }

		protected override void CleanupUnmanagedResources ()
		{
			DayView = null;
			base.CleanupUnmanagedResources ();
		}
    }
}
