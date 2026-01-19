using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
//using System.Drawing;
//using System.Drawing.Drawing2D;
using System.Drawing.Text;
using SummerGUI;

namespace SummerGUI.Scheduling
{
    public class FlatRenderer : AbstractRenderer
    {
		public FlatRenderer()
		{
			InitStringFormats ();
		}

		/***
		private System.Drawing.StringFormat m_UserHeaderFormat;
		private System.Drawing.StringFormat m_DayHeaderFormat;
		private System.Drawing.StringFormat m_HourLabelFormat;
		private System.Drawing.StringFormat m_AppointmentStringFormat;
		***/

		private FontFormat m_UserHeaderFormat;
		private FontFormat m_DayHeaderFormat;
		private FontFormat m_HourLabelFormat;
		private FontFormat m_AppointmentStringFormat;

		private void InitStringFormats()
		{			
			m_UserHeaderFormat = new FontFormat (Alignment.Center, Alignment.Center, FontFormatFlags.Elipsis);
			m_DayHeaderFormat = new FontFormat (Alignment.Center, Alignment.Center, FontFormatFlags.None);
			m_HourLabelFormat = new FontFormat (Alignment.Near, Alignment.Center, FontFormatFlags.None);
			m_AppointmentStringFormat = new FontFormat (Alignment.Near, Alignment.Center, FontFormatFlags.WrapText | FontFormatFlags.Elipsis);
				
			//HourFont = HeaderFont;
			//MinuteFont = HeaderFont;
		}

		public override System.Drawing.Color HourColor
        {
            get
            {
                //return Color.FromArgb(230, 237, 247);
				//return System.Drawing.Color.FromArgb(206, 212, 223);                
				return Theme.LightFillGray;
            }
        }

		public override System.Drawing.Color GripperBorderColor
		{
			get
			{
				//return System.Drawing.Color.FromArgb(194, 200, 208);
				return SummerGUI.Theme.Colors.Base1;
			}
		}

		public override System.Drawing.Color HalfHourSeperatorColor
        {
            get
            {
				return System.Drawing.Color.FromArgb(180, SummerGUI.Theme.Colors.Base1);
            }
        }

		public override System.Drawing.Color HourSeperatorColor
        {
            get
            {                
				return SummerGUI.Theme.Colors.Base0;			
            }
        }

		public override System.Drawing.Color WorkingHourColor
        {
            get
            {				
				return SummerGUI.Theme.Colors.Base3;
            }
        }

		public override System.Drawing.Color BackColor
        {
            get
            {				
				return System.Drawing.SystemColors.Window;
            }
        }

		public override System.Drawing.Color HoursBackColor
        {
            get
            {				
				//return System.Drawing.Color.FromArgb(206, 212, 223);
				//return SolarizedColors.SolarizedBase1;
				return SummerGUI.Theme.Colors.Silver;
            }
        }

		public override System.Drawing.Color SelectionColor
        {
            get
            {
                //return Color.FromArgb(241, 243, 248);
				return System.Drawing.Color.FromArgb(241, 243, 248);
                //return SystemColors.Highlight;
            }
        }

		public override void DrawHourRange(IGUIContext gfx, System.Drawing.RectangleF rect, bool drawBorder, bool highlight, bool active)
        {			
			System.Drawing.Color c = highlight ? this.SelectionColor : this.WorkingHourColor;
			if (!active)
				c = c.ToGray ();

			using (var brush = new SolidBrush(c))
            {
				gfx.FillRectangle(brush, rect);
            }

            if (drawBorder)
            {
				using (var pen = new Pen(System.Drawing.Color.FromArgb(194, 200, 208)))
                {
					gfx.DrawRectangle(pen, rect);
                }
            }
        }

		public override void DrawDayGripper(IGUIContext gfx, RectangleF rect, int gripWidth)
        {
			using (var m_Brush = new SolidBrush (BackColor)) {
				gfx.FillRectangle (m_Brush, rect.Left, rect.Top - 1, gripWidth, rect.Height + 1);
			}

			using (var m_Pen = new Pen (GripperBorderColor)) {
				gfx.DrawRectangle (m_Pen, rect.Left - 0.5f, rect.Top - 1, gripWidth, rect.Height + 1);
			}
        }

		public override void DrawHourLabel(IGUIContext gfx, RectangleF rect, int hour)
        {
			lock (m_SyncObj)
            {
				using (var pen = new Pen(Theme.HourLabelColor, 1.5f))
				using (var brush = new SolidBrush(Theme.HourLabelColor))
                {										                
                    rect.X += 2;
                    rect.Width -= 2;

					RectangleF HourRectangle = rect;
					//HourRectangle.Offset(0, 1);
					HourRectangle.Width = rect.Width;
					gfx.DrawString(hour.ToString ("##00"), HourFont, brush, HourRectangle, m_HourLabelFormat);

					float delta = (rect.Width * 2f / 3f) - 4;
					rect.X += delta;
					rect.Width -= delta;
					rect.Height = rect.Height / 2f;
                    //rect.Y += 1;

					gfx.DrawString("00", MinuteFont, brush, rect, m_HourLabelFormat);
					gfx.DrawLine(pen, rect.Left + 2, rect.Bottom - 1, rect.Right - 7, rect.Bottom - 1);

                    rect.Y += rect.Height - 1;
					gfx.DrawString("30", MinuteFont, brush, rect, m_HourLabelFormat);
                }
            }				
        }
			
		public override void DrawUserHeader(IGUIContext gfx, System.Drawing.RectangleF rect, string UserName)
        {
			gfx.DrawGrayButton(rect);	

            if (rect.Width > 4)
            {
                rect.Offset(2, 1);
                rect.Inflate(-2, 0);
				gfx.DrawString (UserName, BaseFont, SummerGUI.Theme.Brushes.Base03, rect, m_UserHeaderFormat);
            }
        }

		public override void DrawDayHeader(IGUIContext gfx, System.Drawing.RectangleF rect, DateTime date)
        {	
			if (date.Date.Equals(DateTime.Now.Date))
            {
				gfx.DrawHighlightButton (rect);			
            }
            else
            {
				gfx.DrawGrayButton (rect);                
            }

			if (rect.Width < 2 || date == DateTime.MinValue)
                return;

            IGUIFont fntDay = BaseFont;

            string strDate;
            int StringWith = 0;

            try
            {
                strDate = date.ToString("D");                
				StringWith = (int)fntDay.Measure(strDate).Width;

                if (StringWith > rect.Width - 5)
                {
                    strDate = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek) + ", " + date.ToString("d");
					StringWith = (int)fntDay.Measure(strDate).Width;

                    if (StringWith > rect.Width - 5)
                    {
                        strDate = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek) + ", " + date.ToString("d");
						StringWith = (int)fntDay.Measure(strDate).Width;

                        if (StringWith > rect.Width - 5)
                        {
                            strDate = date.ToString("d");
							StringWith = (int)fntDay.Measure(strDate).Width;

                            if (StringWith > rect.Width - 5)
                            {
                                strDate = date.ToString("m");
								StringWith = (int)fntDay.Measure(strDate).Width;

                                if (StringWith > rect.Width - 3)
                                {
                                    strDate = date.Day.ToString() + " " + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);
									StringWith = (int)fntDay.Measure(strDate).Width;

                                    if (StringWith > rect.Width - 3)
                                    {
                                        strDate = date.Day.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                strDate = "?";
            }

			//gfx.DrawString (strDate, BaseFont, SummerGUI.Theme.Brushes.Base02, rect, m_DayHeaderFormat);	

			gfx.DrawString (strDate, HeaderFont, SummerGUI.Theme.Brushes.Base01, rect, m_DayHeaderFormat);	
        }

		private SolidBrush NonWorkingTimeBrush = new SolidBrush (System.Drawing.Color.FromArgb (236, 224, 190));

		public override void DrawDayBackground(IGUIContext gfx, RectangleF rect, bool active)
        {
			/**
            if (m_GanttControl != null)
                m_GanttControl.GanttSettings.NonWorkingTimeFormatStyle.BackgroundStyle.FillRectangle(g, rect);
           **/

			// *** NonWorkingTime

			//gfx.FillRectangle (SolarizedBrushes.SolarizedBase2, rect);


			if (active)
				gfx.FillRectangle (NonWorkingTimeBrush, rect);
			else {
				using (Brush b = new SolidBrush (NonWorkingTimeBrush.Color.ToGray ())) {
					gfx.FillRectangle (b, rect);
				}
			}
        }

		//public override void DrawAppointment(OpenGlGdiContext gfx, Rectangle rect, Appointment appointment, bool isSelected, int gripWidth, ImageList imageList)
		public override void DrawAppointment(IGUIContext gfx, RectangleF rect, Appointment appointment, bool isSelected, int gripWidth)
        {
            if (rect.Width == 0 || rect.Height == 0)
                //if (rect.Width < 6 || rect.Height < 3)
                return;

            //Color start = InterpolateColors(appointment.Color, Color.White, 0.4f);            
            //Color end = InterpolateColors(appointment.Color, Color.White, 0.7f);

            // better swap start and end color, text is more readable
			System.Drawing.Color end = InterpolateColors(appointment.Color, System.Drawing.Color.White, 0.4f);
			System.Drawing.Color start = InterpolateColors(appointment.Color, System.Drawing.Color.White, 0.7f);

            if ((appointment.Locked) || isSelected)
            {
                // Draw back                
				using (var m_Brush = new HatchBrush ("LargeConfetti", System.Drawing.Color.FromArgb (70, 90, 125), appointment.Color)) {
					gfx.FillRectangle (m_Brush, rect);
				}

                // little transparent
				start = System.Drawing.Color.FromArgb(230, start);
				end = System.Drawing.Color.FromArgb(180, end);

				using (var aGB = new LinearGradientBrush (start, end, GradientDirections.ForwardDiagonal)) {
					gfx.FillRectangle (aGB, rect);
				}
            }
            else
            {
                // Draw back
				using (var aGB = new LinearGradientBrush (start, end, GradientDirections.Vertical)) {
					gfx.FillRectangle (aGB, rect);
				}
            }

            if (isSelected)
            {
				RectangleF m_BorderRectangle = rect;
                m_BorderRectangle.Offset(-1, 0);

				using (var m_Pen = new Pen (appointment.BorderColor, 4)) {
					gfx.DrawRectangle (m_Pen, m_BorderRectangle);
				}
            }
            else
            {
                // Draw shadow lines
				float xLeft = rect.X + 6;
				float xRight = rect.Right + 1;
				float yTop = rect.Y + 1;
				float yButton = rect.Bottom + 1;

                for (int i = 0; i < 5; i++)
                {
					using (var shadow_Pen = new Pen(System.Drawing.Color.FromArgb(70 - 12 * i, System.Drawing.Color.Black)))
                    {					
                        gfx.DrawLine(shadow_Pen, xLeft + i, yButton + i, xRight + i - 1, yButton + i); //horisontal lines
                        gfx.DrawLine(shadow_Pen, xRight + i, yTop + i, xRight + i, yButton + i); //vertical
                    }
                }

                rect.Width -= 1;
				using (var m_Pen = new Pen (Color.FromArgb (70, 90, 125), 1)) {
					gfx.DrawRectangle (m_Pen, rect);
				}
                rect.Width += 1;
            }

            // Draw gripper *****
			RectangleF m_GripRectangle = rect;
            m_GripRectangle.Width = gripWidth + 1;

			using (var aGB = new SolidBrush (appointment.BorderColor)) {
				gfx.FillRectangle (aGB, m_GripRectangle);
			}


            // Draw Icons *****            
            rect.X += gripWidth + 2;
            rect.Width -= gripWidth + 2;

            if (rect.Height > 56)
            {
                rect.Y += 6;
                rect.Height -= 10;
            }
            else if (rect.Height > 38)
            {
                rect.Y += 4;
                rect.Height -= 8;
            }
            else
            {
                rect.Y += 3;
                rect.Height -= 6;
            }

			float LeftX = rect.X;
			float IconWidth = 0;
			/**
            if (appointment.ImageIndices != null && appointment.ImageIndices.Length > 0 && imageList != null && rect.Width >= 15 && rect.Height >= 10)
            {
                int imagecount = 0;

                int startX = rect.X;
                foreach (int ImageIndex in appointment.ImageIndices)
                {
                    //imageList.Draw(g, new Point(startX, rect.Y), ImageIndex);
                    imagecount++;
                    IconWidth += 18;
                    startX = rect.X + (imagecount * 18);

                    if (startX + 18 > rect.Right - (gripWidth))
                    {
                        startX = rect.X;
                        rect.Y += 18;
                        rect.Height -= 18;
                        IconWidth = 0;

                        if (rect.Y + 18 > rect.Height)
                            break;
                    }
                }

                if (IconWidth > 0)
                    IconWidth += 2;
            }
            **/

            rect.Width -= 2;
            //rect.Y -= 1;

            // Draw Text *****
			System.Drawing.SizeF TextSize = gfx.MeasureString(appointment.Title, this.BaseFont, rect, m_AppointmentStringFormat);
            int TextLength = (int)TextSize.Width;
            int TextHeight = (int)TextSize.Height;

            if (TextLength + IconWidth + gripWidth + 2 > rect.Width && IconWidth > 20 && rect.Height > 36)
            {
                rect.Y += 20;
                rect.Height -= 20;
            }
            else
            {
                rect.X += IconWidth;
                rect.Width -= IconWidth;
            }

           	gfx.DrawString(appointment.Title, this.BaseFont, SystemBrushes.WindowText, rect, m_AppointmentStringFormat);

            if (!String.IsNullOrEmpty(appointment.Description))
            {
                rect.X = LeftX;
                rect.Y += TextHeight + 16;
                rect.Height -= TextHeight + 14;

                if (rect.Height > 12)
					gfx.DrawString(appointment.Description, this.BaseFont, SystemBrushes.WindowText, rect, m_AppointmentStringFormat);
            }
        }
    }
}
