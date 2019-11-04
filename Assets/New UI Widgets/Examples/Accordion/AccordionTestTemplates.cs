﻿namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Test accordion.
	/// </summary>
	public class AccordionTestTemplates : MonoBehaviour
	{
		/// <summary>
		/// Accordion.
		/// </summary>
		[SerializeField]
		public Accordion Accordion;

		/// <summary>
		/// Header template.
		/// </summary>
		[SerializeField]
		public GameObject HeaderTemplate;

		/// <summary>
		/// Content1.
		/// </summary>
		[SerializeField]
		public GameObject ContentTemplate;

		/// <summary>
		/// Set accordion items.
		/// </summary>
		public void SetItems()
		{
			var items = new ObservableList<AccordionItem>();

			// instead int you can use list with yours data
			for (int i = 0; i < 10; i++)
			{
				var item = CreateItem(i);
				items.Add(item);
			}

			Accordion.DataSource = items;
		}

		AccordionItem CreateItem(int i)
		{
			var header = Compatibility.Instantiate(HeaderTemplate);
			header.transform.SetParent(Accordion.transform);
			header.gameObject.SetActive(true);

			// set header data
			header.GetComponentInChildren<Text>().text = "Header " + i;

			var content = Compatibility.Instantiate(ContentTemplate);
			content.transform.SetParent(Accordion.transform);
			content.gameObject.SetActive(true);

			// set content data
			content.GetComponentInChildren<Text>().text = "Content " + i;

			return new AccordionItem()
			{
				ToggleObject = header,
				ContentObject = content,
				Open = i == 0, // only first item will be opened
			};
		}
	}
}