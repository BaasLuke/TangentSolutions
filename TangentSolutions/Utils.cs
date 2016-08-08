using System;
using Android.Content;

namespace TangentSolutions
{
	public class Utils
	{
		public ISharedPreferences pref { get; set; }
		public ISharedPreferencesEditor edit { get; set; }

		public Utils ()
		{
		}
	}
}

