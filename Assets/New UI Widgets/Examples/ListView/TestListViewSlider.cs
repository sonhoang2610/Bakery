﻿namespace UIWidgets.Examples
{
	using System.Linq;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test ListView with Slider.
	/// </summary>
	public class TestListViewSlider : MonoBehaviour
	{
		/// <summary>
		/// ListViewSlider.
		/// </summary>
		[SerializeField]
		protected ListViewSlider ListView;

		/// <summary>
		/// Set ListView DataSource.
		/// </summary>
		public void SetList()
		{
			ListView.DataSource = Utilites.CreateList(100, x => new ListViewSliderItem() { Value = Random.Range(0, 100) });
		}
	}
}