/* Developed by Ertan Tike (ertan.tike@moreum.com) */

using System;
using System.Collections.Generic;
using System.Text;

namespace SummerGUI.Scheduling
{
    public class NewAppointmentEventArgs : EventArgs
    {
        public NewAppointmentEventArgs( string title, DateTime start, DateTime end, int ownerID)
        {
            m_Title = title;
            m_StartDate = start;
            m_EndDate = end;
            m_OwnerID = ownerID;
        }

        private string m_Title;

        public string Title
        {
            get { return m_Title; }
        }

        private DateTime m_StartDate;

        public DateTime StartDate
        {
            get { return m_StartDate; }
        }

        private DateTime m_EndDate;

        public DateTime EndDate
        {
            get { return m_EndDate; }
        }

        private int m_OwnerID = -1;
        public int OwnerID
        {
            get
            {
                return m_OwnerID;
            }
            set
            {
                m_OwnerID = value;
            }
        }

    }

    public delegate void NewAppointmentEventHandler( object sender, NewAppointmentEventArgs args );
}
