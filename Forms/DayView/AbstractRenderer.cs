using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
//using System.Drawing;
using SummerGUI;

namespace SummerGUI.Scheduling
{
    public abstract class AbstractRenderer
    {		
        protected readonly object m_SyncObj = new object();
                
		public virtual System.Drawing.Color AllDayEventsBackColor
        {
            get
            {
				return InterpolateColors(this.BackColor, System.Drawing.Color.Black, 0.5f);
            }
        }

		public virtual System.Drawing.Color GripperBorderColor
		{
			get
			{
				//return System.Drawing.Color.FromArgb(194, 200, 208);
				return SummerGUI.Theme.Colors.Base1;
			}
		}
			
		public virtual IGUIFont BaseFont
        {
            get
            {				
				return SummerGUIWindow.CurrentContext.FontManager.DefaultFont;
            }            
        }

		public virtual IGUIFont HeaderFont
		{
			get
			{				
				return SummerGUIWindow.CurrentContext.FontManager.StatusFont;
			}            
		}

		public virtual IGUIFont HourFont
		{
			get
			{								
				return SummerGUIWindow.CurrentContext.FontManager.FontByTag("HourFont");
			}            
		}

		public virtual IGUIFont MinuteFont
		{
			get
			{					
				return SummerGUIWindow.CurrentContext.FontManager.FontByTag("MinuteFont");
			}            
		}
			
		public virtual System.Drawing.Color HourSeperatorColor
        {
            get
            {
                //return System.Drawing.Color.FromArgb(234, 208, 152);
				return System.Drawing.Color.FromArgb(34, 08, 52);
            }
        }

		public virtual System.Drawing.Color HalfHourSeperatorColor
        {
            get
            {
                //return System.Drawing.Color.FromArgb(243, 228, 177);
				return System.Drawing.Color.FromArgb(43, 28, 77);
            }
        }

		public virtual System.Drawing.Color HourColor
        {
            get
            {
				/**
                // ToDo: Set Weekend Color from Gantt-Control
                if (m_GanttControl != null)
                    return m_GanttControl.GanttSettings.WeekendFormatStyle.BackgroundStyle.Color;
                else
					**/
                    return System.Drawing.Color.FromArgb(255, 244, 188);
            }
        }

		public virtual System.Drawing.Color WorkingHourColor
        {
            get
            {
                // ToDo: Set WorkingHour / Background Color from Gantt Control
				/**
                if (m_GanttControl != null)
                    return m_GanttControl.BackColor;
                else
                **/
                    return System.Drawing.Color.FromArgb(255, 255, 213);
            }
        }

		public virtual System.Drawing.Color BackColor
        {
            get
            {
				return System.Drawing.SystemColors.Control;
            }
        }

		public virtual System.Drawing.Color HoursBackColor
        {
            get
            {
				/**
                if (m_GanttControl != null)
                    return m_GanttControl.GanttSettings.WeekendFormatStyle.BackgroundStyle.Color;
                else
                **/
                    return BackColor;
            }
        }

		public virtual System.Drawing.Color SelectionColor
        {
            get
            {
				return System.Drawing.SystemColors.Highlight;                
            }
        }
			        
		/**
        public virtual QFont HourFont
        {
            get
            {
				return Styles.HourFont;
            }            
        }			                
			        
        public virtual QFont MinuteFont
        {
            get
            {
				return QFontFactory.MinuteFont;
            }            
        }
        **/

		public abstract void DrawHourLabel(IGUIContext gfx, System.Drawing.RectangleF rect, int hour);

		public abstract void DrawDayHeader(IGUIContext gfx, System.Drawing.RectangleF rect, DateTime date);

		public abstract void DrawUserHeader(IGUIContext gfx, System.Drawing.RectangleF rect, string UserName);

		public abstract void DrawDayBackground(IGUIContext gfx, System.Drawing.RectangleF rect, bool active);

		public virtual void DrawHourRange(IGUIContext gfx, System.Drawing.RectangleF rect, bool drawBorder, bool hilight, bool active)
        {
			using (var brush = new SolidBrush(hilight ? this.SelectionColor : this.WorkingHourColor))
            {
                gfx.FillRectangle(brush, rect);
            }

			if (drawBorder) 
			{
				using (var pen = new Pen(System.Drawing.Color.Gray))
					gfx.DrawRectangle (pen, rect);
			}
        }

		public virtual void DrawDayGripper(IGUIContext gfx, RectangleF rect, int gripWidth)
        {
			using (var m_Brush = new SolidBrush(Color.White))
                gfx.FillRectangle(m_Brush, rect.Left, rect.Top - 1, gripWidth, rect.Height);

			using (var m_Pen = new Pen(Color.Black))
                gfx.DrawRectangle(m_Pen, rect.Left, rect.Top - 1, gripWidth, rect.Height);
        }

        //public abstract void DrawAppointment(Graphics g, Rectangle rect, Appointment appointment, bool isSelected, int gripWidth, ImageList imageList);
		public abstract void DrawAppointment(IGUIContext gfx, RectangleF rect, Appointment appointment, bool isSelected, int gripWidth);

		public void DrawAllDayBackground(IGUIContext gfx, Rectangle rect)
        {
			using (var brush = new SolidBrush(InterpolateColors(this.BackColor, Color.Black, 0.5f)))
                gfx.FillRectangle(brush, rect);
        }

		public static Color InterpolateColors(Color color1, Color color2, float percentage)
        {
            int num1 = ((int)color1.R);
            int num2 = ((int)color1.G);
            int num3 = ((int)color1.B);
            int num4 = ((int)color2.R);
            int num5 = ((int)color2.G);
            int num6 = ((int)color2.B);
            byte num7 = Convert.ToByte(((float)(((float)num1) + (((float)(num4 - num1)) * percentage))));
            byte num8 = Convert.ToByte(((float)(((float)num2) + (((float)(num5 - num2)) * percentage))));
            byte num9 = Convert.ToByte(((float)(((float)num3) + (((float)(num6 - num3)) * percentage))));
			return System.Drawing.Color.FromArgb(num7, num8, num9);
        }
    }
}
