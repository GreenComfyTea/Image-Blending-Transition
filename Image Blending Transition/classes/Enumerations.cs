using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlendingTransition
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum BlendingModes
	{
		[Description("Opacity")]
		Opacity,
		[Description("Darken")]
		Darken,
		[Description("Multiply")]
		Multiply,
		[Description("Lighten")]
		Lighten,
		[Description("Screen")]
		Screen,
		[Description("Color Burn")]
		ColorBurn,
		[Description("Linear Burn")]
		LinearBurn,
		[Description("Color Dodge")]
		ColorDodge,
		[Description("Linear Dodge (Add)")]
		LinearDodge,
		[Description("Overlay")]
		Overlay,
		[Description("Soft Light")]
		SoftLight,
		[Description("Hard Light")]
		HardLight,
		[Description("Difference")]
		Difference
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum TransitionModes
	{
		[Description("No Transition")]
		NoTransition,
		[Description("Wipe (Left to Right)")]
		WipeLeftToRight,
		[Description("Wipe (Right to Left)")]
		WipeRightToLeft,
		[Description("Wipe (Top to Bottom)")]
		WipeTopToBottom,
		[Description("Wipe (Bottom to Top)")]
		WipeBottomToTop,
		[Description("Split (Horizontal)")]
		HorizontalSplit,
		[Description("Split (Vertical)")]
		VerticalSplit,
		[Description("Fade")]
		Fade,
		[Description("Rectangle")]
		Rectangle,
		[Description("Circle")]
		Circle
	}
}
