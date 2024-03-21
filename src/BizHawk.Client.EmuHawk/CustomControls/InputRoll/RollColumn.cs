﻿namespace BizHawk.Client.EmuHawk
{
	public class RollColumn
	{
		public int Width { get; set; }
		public int Left { get; set; }
		public int Right { get; set; }
		public string Name { get; set; }
		public string Text { get; set; }
		
		// Is this the default we want? ColumnType.Text is the most common.
		public ColumnType Type { get; set; } = ColumnType.Boolean;
		public bool Visible { get; set; } = true;

		/// <summary>
		/// Column will be drawn with an emphasized look, if true
		/// </summary>
		public bool Emphasis { get; set; }

		/// <summary>
		/// Column header text will be drawn rotated, if true
		/// </summary>
		public bool Rotatable { get; set; }

		/// <summary>
		/// Sets the desired width as appropriate for a display with no scaling. If display
		/// scaling is enabled, the actual column width will be scaled accordingly.
		/// </summary>
		public int UnscaledWidth
		{
			get => UIHelper.UnscaleX(Width);
			set => Width = UIHelper.ScaleX(value);
		}
	}
}
