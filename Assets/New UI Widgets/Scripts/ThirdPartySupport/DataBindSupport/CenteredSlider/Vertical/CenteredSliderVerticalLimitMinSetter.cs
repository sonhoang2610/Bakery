#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the LimitMin of a CenteredSliderVertical depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/UIWidgets/Setters/[DB] CenteredSliderVertical LimitMin Setter")]
	public class CenteredSliderVerticalLimitMinSetter : ComponentSingleSetter<UIWidgets.CenteredSliderVertical, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CenteredSliderVertical target, System.Int32 value)
		{
			target.LimitMin = value;
		}
	}
}
#endif