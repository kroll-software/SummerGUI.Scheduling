using System;
using System.Drawing;
using SummerGUI;

namespace SummerGUI.Scheduling
{
	public static class Theme
	{
		static Theme ()
		{
		}

		public static Color LightFillGray = Color.FromArgb(206, 212, 223);

		//public static Color HourLabelColor = Color.FromArgb (85, 85, 85);
		public static Color HourLabelColor = SummerGUI.Theme.Colors.Base02;

		public static void InitTheme(IGUIContext ctx)
		{
			string latoSemiPath = "Fonts/Lato-Semibold.ttf".FixedExpandedPath ();

			FontManager.Manager.AddFontConfig (new GUIFontConfiguration (
				"HourFont",
				latoSemiPath,
				16,
				GlyphFilterFlags.Numeric));

			FontManager.Manager.AddFontConfig (new GUIFontConfiguration (
				"MinuteFont",
				latoSemiPath,
				8,
				GlyphFilterFlags.Numeric));
		}
	}
}

