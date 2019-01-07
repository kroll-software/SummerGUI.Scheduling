/* Developed by Ertan Tike (ertan.tike@moreum.com) */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SummerGUI.Scheduling
{
    public class Appointment
    {
        public Appointment()
        {
            color = Color.White;
            m_BorderColor = Color.FromArgb(112, 160, 240);
            //m_BorderColor = Color.FromArgb(51, 153, 255);
            m_Title = "New Appointment";
        }

        public object Tag;

        private DateTime m_StartDate;
        public DateTime StartDate
        {
            get
            {
                return m_StartDate;
            }
            set
            {
                if (m_StartDate != value)
                {
                    m_StartDate = value;
                    OnStartDateChanged();
                }

            }
        }
        protected virtual void OnStartDateChanged()
        {
        }

        private DateTime m_EndDate;
        public DateTime EndDate
        {
            get
            {
                return m_EndDate;
            }
            set
            {
                if (m_EndDate != value)
                {
                    m_EndDate = value;
                    OnEndDateChanged();
                }
            }
        }

        protected virtual void OnEndDateChanged()
        {
        }

        private int m_OwnerID;
        public int OwnerID
        {
            get
            {
                return m_OwnerID;
            }
            set
            {
                if (m_OwnerID != value)
                {
                    m_OwnerID = value;
                    OnOwnderIDChanged();
                }
            }
        }

        private string m_CreateUser;
        public string CreateUser
        {
            get
            {
                return m_CreateUser;
            }
            set
            {
                if (m_CreateUser != value)
                {
                    m_CreateUser = value;                    
                }
            }
        }        

        protected virtual void OnOwnderIDChanged()
        {
        }

        private int[] m_ImageIndices;
        public int[] ImageIndices
        {
            get
            {
                return m_ImageIndices;
            }
            set
            {
                if (m_ImageIndices != value)
                {
                    m_ImageIndices = value;
                    OnImageIndicesChanged();
                }
            }
        }

        protected virtual void OnImageIndicesChanged()
        {
        }


        private bool m_Locked = false;
        [System.ComponentModel.DefaultValue(false)]
        public bool Locked
        {
            get { return m_Locked; }
            set
            {
                m_Locked = value;
                OnLockedChanged();
            }
        }

        protected virtual void OnLockedChanged()
        {
        }

        private bool m_Selected = false;
        [System.ComponentModel.DefaultValue(false)]
        public bool Selected
        {
            get { return m_Selected; }
            set
            {
                m_Selected = value;
                OnSelectedChanged();
            }
        }

        protected virtual void OnSelectedChanged()
        {
        }

        private Color color = Color.White;
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        private Color textColor = Color.Black;

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        private Color m_BorderColor = Color.Blue;

        public Color BorderColor
        {
            get
            {
                return m_BorderColor;
            }
            set
            {
                m_BorderColor = value;
            }
        }

        private string m_Title = "";
        [System.ComponentModel.DefaultValue("")]
        public string Title
        {
            get
            {
                return m_Title;
            }
            set
            {
                m_Title = value;
                OnTitleChanged();
            }
        }
        protected virtual void OnTitleChanged()
        {
        }

        private string m_Description = "";
        [System.ComponentModel.DefaultValue("")]
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
                OnDescriptionChanged();
            }
        }
        protected virtual void OnDescriptionChanged()
        {
        }

        internal int m_ConflictCount;
    }
}
