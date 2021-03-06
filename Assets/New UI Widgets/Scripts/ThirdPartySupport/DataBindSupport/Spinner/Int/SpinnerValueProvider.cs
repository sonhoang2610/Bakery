#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;
	
	/// <summary>
	/// Provides the Value of an Spinner.
	/// </summary>
	[AddComponentMenu("Data Bind/UIWidgets/Getters/[DB] Spinner Value Provider")]
	public class SpinnerValueProvider : ComponentDataProvider<UIWidgets.Spinner, System.Int32>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.Spinner target)
		{
			target.onValueChangeInt.AddListener(OnValueChangeIntSpinner);
		}

		/// <inheritdoc />
		protected override System.Int32 GetValue(UIWidgets.Spinner target)
		{
			return target.Value;
		}
		
		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.Spinner target)
		{
			target.onValueChangeInt.RemoveListener(OnValueChangeIntSpinner);
		}
		
		void OnValueChangeIntSpinner(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif