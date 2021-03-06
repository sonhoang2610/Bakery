#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the ValueMin of a RangeSlider depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/UIWidgets/Setters/[DB] RangeSlider ValueMin Setter")]
	public class RangeSliderValueMinSetter : ComponentSingleSetter<UIWidgets.RangeSlider, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSlider target, System.Int32 value)
		{
			target.ValueMin = value;
		}
	}
}
#endif