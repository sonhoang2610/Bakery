﻿namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListView with dynamic items heights.
	/// </summary>
	public class ListViewHeight : ListView
	{
		/// <summary>
		/// Items heights.
		/// </summary>
		protected Dictionary<string, float> Heights = new Dictionary<string, float>();

		ListViewStringComponent defaultItemCopy;

		RectTransform defaultItemCopyRect;

		/// <summary>
		/// Gets the default item copy.
		/// </summary>
		/// <value>The default item copy.</value>
		protected ListViewStringComponent DefaultItemCopy
		{
			get
			{
				if (defaultItemCopy == null)
				{
					var copy = Compatibility.Instantiate(DefaultItem);
					copy.gameObject.SetActive(true);
					defaultItemCopy = copy.GetComponent<ListViewStringComponent>();
					defaultItemCopy.transform.SetParent(DefaultItem.transform.parent, false);
					defaultItemCopy.gameObject.name = "DefaultItemCopy";
					defaultItemCopy.gameObject.SetActive(false);

					Utilites.FixInstantiated(DefaultItem, defaultItemCopy);
				}

				return defaultItemCopy;
			}
		}

		/// <summary>
		/// Gets the RectTransform of DefaultItemCopy.
		/// </summary>
		/// <value>RectTransform.</value>
		protected RectTransform DefaultItemCopyRect
		{
			get
			{
				if (defaultItemCopyRect == null)
				{
					defaultItemCopyRect = defaultItemCopy.transform as RectTransform;
				}

				return defaultItemCopyRect;
			}
		}

		/// <summary>
		/// Sets the default item.
		/// </summary>
		/// <param name="newDefaultItem">New default item.</param>
		protected override void SetDefaultItem(Image newDefaultItem)
		{
			if (newDefaultItem != null)
			{
				if (defaultItemCopy != null)
				{
					Destroy(defaultItemCopy.gameObject);
					defaultItemCopy = null;
				}

				if (defaultItemCopyRect != null)
				{
					Destroy(defaultItemCopyRect.gameObject);
					defaultItemCopyRect = null;
				}
			}

			base.SetDefaultItem(newDefaultItem);
		}

		/// <summary>
		/// Calculates the maximum count of the visible items.
		/// </summary>
		protected override void CalculateMaxVisibleItems()
		{
			SetItemsHeight(DataSource);

			maxVisibleItems = GetMaxVisibleItems();
		}

		/// <summary>
		/// Get the max count of the visible items.
		/// </summary>
		/// <returns>Maximum count of the visible items,</returns>
		protected int GetMaxVisibleItems()
		{
			var spacing = LayoutBridge.GetSpacing();
			var min = MinSize();

			var size = GetScrollSize();
			var result = 0;
			for (; ;)
			{
				size -= min;
				if (result > 0)
				{
					size -= spacing;
				}

				if (size < 0)
				{
					break;
				}

				result += 1;
			}

			return result + 2;
		}

		/// <summary>
		/// Get the minimal size of the specified items.
		/// </summary>
		/// <returns>Minimal size.</returns>
		protected virtual float MinSize()
		{
			if (DataSource.Count == 0)
			{
				return 0f;
			}

			var result = Heights[DataSource[0]];

			for (int i = 1; i < DataSource.Count; i++)
			{
				result = Mathf.Min(result, Heights[DataSource[i]]);
			}

			return result;
		}

		/// <summary>
		/// Calculates the size of the item.
		/// </summary>
		protected override void CalculateItemSize()
		{
			var rect = DefaultItem.transform as RectTransform;
			#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			var layout_elements = rect.GetComponents<ILayoutElement>();
			#else
			var layout_elements = rect.GetComponents<Component>().OfType<ILayoutElement>();
			#endif

			if (ItemSize.y == 0)
			{
				var preffered_height = layout_elements.Max(x => Mathf.Max(x.minHeight, x.preferredHeight));
				ItemSize.y = (preffered_height > 0) ? preffered_height : rect.rect.height;
			}

			if (ItemSize.x == 0)
			{
				var preffered_width = layout_elements.Max(x => Mathf.Max(x.minWidth, x.preferredWidth));
				ItemSize.x = (preffered_width > 0) ? preffered_width : rect.rect.width;
			}
		}

		/// <summary>
		/// Scrolls to item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		public override void ScrollTo(int index)
		{
			if (!CanOptimize())
			{
				return;
			}

			var top = GetScrollValue();
			var bottom = GetScrollValue() + GetScrollSize();

			var item_starts = ItemStartAt(index);

			var item_ends = ItemEndAt(index) + LayoutBridge.GetMargin();

			if (item_starts < top)
			{
				SetScrollValue(item_starts);
			}
			else if (item_ends > bottom)
			{
				SetScrollValue(item_ends - GetScrollSize());
			}
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		protected override float CalculateBottomFillerSize()
		{
			if (bottomHiddenItems == 0)
			{
				return 0f;
			}

			var height = 0f;
			for (int i = topHiddenItems + visibleItems; i < topHiddenItems + visibleItems + bottomHiddenItems; i++)
			{
				height += GetItemHeight(DataSource[i]);
			}

			return Mathf.Max(0, height + (LayoutBridge.GetSpacing() * (bottomHiddenItems - 1)));
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected override float CalculateTopFillerSize()
		{
			if (topHiddenItems == 0)
			{
				return 0f;
			}

			var height = 0f;
			for (int i = 0; i < topHiddenItems; i++)
			{
				height += GetItemHeight(DataSource[i]);
			}

			return Mathf.Max(0, height + (LayoutBridge.GetSpacing() * (topHiddenItems - 1)));
		}

		float GetItemHeight(string item)
		{
			return Heights[item];
		}

		/// <summary>
		/// Gets the item position.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPosition(int index)
		{
			var height = 0f;
			var n = Mathf.Min(index, DataSource.Count);
			for (int i = 0; i < n; i++)
			{
				height += GetItemHeight(DataSource[i]);
			}

			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Gets the item position bottom.
		/// </summary>
		/// <returns>The item position bottom.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionBottom(int index)
		{
			return GetItemPosition(index) + GetItemHeight(DataSource[index]) + LayoutBridge.GetFullMargin() - GetScrollSize();
		}

		/// <summary>
		/// Total height of items before specified index.
		/// </summary>
		/// <returns>Height.</returns>
		/// <param name="index">Index.</param>
		float ItemStartAt(int index)
		{
			var height = 0f;
			for (int i = 0; i < index; i++)
			{
				height += GetItemHeight(DataSource[i]);
			}

			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Total height of items before and with specified index.
		/// </summary>
		/// <returns>The <see cref="int"/>.</returns>
		/// <param name="index">Index.</param>
		float ItemEndAt(int index)
		{
			var height = 0f;
			for (int i = 0; i < index + 1; i++)
			{
				height += GetItemHeight(DataSource[i]);
			}

			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public override int Add(string item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item", "Item is null.");
			}

			if (!Heights.ContainsKey(item))
			{
				Heights.Add(item, CalculateItemHeight(item));
			}

			return base.Add(item);
		}

		/// <summary>
		/// Calculate and sets the height of the items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
		void SetItemsHeight(ObservableList<string> items, bool forceUpdate = true)
		{
			if (forceUpdate)
			{
				Heights.Clear();
			}

			items.Except(Heights.Keys).Distinct().ForEach(x =>
			{
				Heights.Add(x, CalculateItemHeight(x));
			});
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		public override void Resize()
		{
			SetItemsHeight(DataSource, true);

			base.Resize();
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view.</param>
		protected override void SetNewItems(ObservableList<string> newItems, bool updateView = true)
		{
			// SetItemsHeight(newItems);
			// CalculateMaxVisibleItems();
			base.SetNewItems(newItems, updateView);
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		protected override void Update()
		{
			if (DataSourceSetted || DataSourceChanged)
			{
				var reset_scroll = DataSourceSetted;

				DataSourceSetted = false;
				DataSourceChanged = false;

				lock (DataSource)
				{
					SetItemsHeight(DataSource);
					CalculateMaxVisibleItems();

					UpdateView();
				}

				if (reset_scroll)
				{
					SetScrollValue(0f);
				}
			}

			if (NeedResize)
			{
				Resize();
			}
		}

		/// <summary>
		/// Gets the height of the index by.
		/// </summary>
		/// <returns>The index by height.</returns>
		/// <param name="height">Height.</param>
		int GetIndexByHeight(float height)
		{
			var spacing = LayoutBridge.GetSpacing();

			var result = 0;
			for (int index = 0; index < DataSource.Count; index++)
			{
				height -= Heights[DataSource[index]];
				if (index > 0)
				{
					height -= spacing;
				}

				if (height < 0)
				{
					break;
				}

				result += 1;
			}

			return result;
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetLastVisibleIndex(bool strict = false)
		{
			var last_visible_index = GetIndexByHeight(GetScrollValue() + GetScrollSize());

			return strict ? last_visible_index : last_visible_index + 2;
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetFirstVisibleIndex(bool strict = false)
		{
			var first_visible_index = GetIndexByHeight(GetScrollValue());

			if (strict)
			{
				return first_visible_index;
			}

			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}

		LayoutGroup defaultItemLayoutGroup;

		/// <summary>
		/// Gets the height of the item.
		/// </summary>
		/// <returns>The item height.</returns>
		/// <param name="item">Item.</param>
		float CalculateItemHeight(string item)
		{
			if (defaultItemLayoutGroup == null)
			{
				defaultItemLayoutGroup = DefaultItemCopy.GetComponent<LayoutGroup>();
			}

			float height = 0f;
			if (defaultItemLayoutGroup != null)
			{
				DefaultItemCopy.gameObject.SetActive(true);

				DefaultItemCopy.SetData(item);
				LayoutUtilites.UpdateLayout(defaultItemLayoutGroup);

				height = LayoutUtility.GetPreferredHeight(DefaultItemCopyRect);

				DefaultItemCopy.gameObject.SetActive(false);
			}

			return height;
		}

		/// <summary>
		/// Calls specified function with each component.
		/// </summary>
		/// <param name="func">Func.</param>
		public override void ForEachComponent(Action<ListViewItem> func)
		{
			base.ForEachComponent(func);
			func(DefaultItemCopy);
		}

		#region ListViewPaginator support

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		public override int GetNearestItemIndex()
		{
			return GetIndexByHeight(GetScrollValue());
		}
		#endregion
	}
}