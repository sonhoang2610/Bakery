#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	
	/// <summary>
	/// Set the WholeNumberOfSteps of a CenteredSliderVertical depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/UIWidgets/Setters/[DB] CenteredSliderVertical WholeNumberOfSteps Setter")]
	public class CenteredSliderVerticalWholeNumberOfStepsSetter : ComponentSingleSetter<UIWidgets.CenteredSliderVertical, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CenteredSliderVertical target, System.Boolean value)
		{
			target.WholeNumberOfSteps = value;
		}
	}
}
#endif