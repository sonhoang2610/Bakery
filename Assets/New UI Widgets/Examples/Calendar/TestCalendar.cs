﻿namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test Calendar.
	/// </summary>
	public class TestCalendar : MonoBehaviour
	{
		/// <summary>
		/// Calendart.
		/// </summary>
		[SerializeField]
		protected UIWidgets.Calendar Calendar;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			LocaleEn();
		}

		/// <summary>
		/// Set monday as first the day of week.
		/// </summary>
		public void FirstDayOfWeekMonday()
		{
			Calendar.FirstDayOfWeek = System.DayOfWeek.Monday;
		}

		/// <summary>
		/// Set sunday as first the day of week.
		/// </summary>
		public void FirstDayOfWeekSunday()
		{
			Calendar.FirstDayOfWeek = System.DayOfWeek.Sunday;
		}

		/// <summary>
		/// Set en-US culture.
		/// </summary>
		public void LocaleEn()
		{
			Calendar.Culture = new System.Globalization.CultureInfo("en-US");
		}

		/// <summary>
		/// Set ja-JP culture.
		/// </summary>
		public void LocaleJp()
		{
			Calendar.Culture = new System.Globalization.CultureInfo("ja-JP");
		}

		/// <summary>
		/// Set fr-FR culture.
		/// </summary>
		public void LocaleFr()
		{
			Calendar.Culture = new System.Globalization.CultureInfo("fr-FR");
		}

		/// <summary>
		/// Set de-DE culture.
		/// </summary>
		public void LocaleDe()
		{
			Calendar.Culture = new System.Globalization.CultureInfo("de-DE");
		}

		/// <summary>
		/// Set zh-CN culture.
		/// </summary>
		public void LocaleCh()
		{
			Calendar.Culture = new System.Globalization.CultureInfo("zh-CN");
		}

		/// <summary>
		/// Set ru-RU culture.
		/// </summary>
		public void LocaleRu()
		{
			Calendar.Culture = new System.Globalization.CultureInfo("ru-RU");
		}

		void SetCalendar(System.Globalization.Calendar calendar)
		{
			Calendar.Culture.DateTimeFormat.Calendar = calendar;
			Calendar.UpdateCalendar();
		}

		/// <summary>
		/// Set gregorian calendar.
		/// </summary>
		public void GregorianCalendar()
		{
			SetCalendar(new System.Globalization.GregorianCalendar());
		}

		/// <summary>
		/// Set hebrew calendar.
		/// </summary>
		public void HebrewCalendar()
		{
			SetCalendar(new System.Globalization.HebrewCalendar());
		}

		/// <summary>
		/// Set korean calendar.
		/// </summary>
		public void KoreanCalendar()
		{
			SetCalendar(new System.Globalization.KoreanCalendar());
		}

		/// <summary>
		/// Set japanese calendar.
		/// </summary>
		public void JapaneseCalendar()
		{
			SetCalendar(new System.Globalization.JapaneseCalendar());
		}

		/// <summary>
		/// Set hijri calendar.
		/// </summary>
		public void HijriCalendar()
		{
			SetCalendar(new System.Globalization.HijriCalendar());
		}

		/// <summary>
		/// Set julian calendar.
		/// </summary>
		public void JulianCalendar()
		{
			SetCalendar(new System.Globalization.JulianCalendar());
		}

		/// <summary>
		/// Set persian calendar.
		/// </summary>
		public void PersianCalendar()
		{
			SetCalendar(new System.Globalization.PersianCalendar());
		}

		/// <summary>
		/// Set taiwan calendar.
		/// </summary>
		public void TaiwanCalendar()
		{
			SetCalendar(new System.Globalization.TaiwanCalendar());
		}
	}
}