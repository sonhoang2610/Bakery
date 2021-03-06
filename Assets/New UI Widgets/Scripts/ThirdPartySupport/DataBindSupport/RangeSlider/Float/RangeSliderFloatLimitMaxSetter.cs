#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the LimitMax of a RangeSliderFloat depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/UIWidgets/Setters/[DB] RangeSliderFloat LimitMax Setter")]
	public class RangeSliderFloatLimitMaxSetter : ComponentSingleSetter<UIWidgets.RangeSliderFloat, System.Single>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSliderFloat target, System.Single value)
		{
			target.LimitMax = value;
		}
	}
}
#endif