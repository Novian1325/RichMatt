using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using SickscoreGames;

namespace SickscoreGames.HUDNavigationSystem
{
	public static class HNS
	{
		#region Static Variables
		public const string PublisherName = "Sickscore Games";
		public const string Name = "HUD Navigation System";
		public const string Version = "v2.0.1";
		#endregion


		#region Static Methods
		#if (UNITY_EDITOR)
		[MenuItem("Window/" + HNS.PublisherName + "/" + HNS.Name + "/Help/Support Forum", false, 12)]
		public static void ShowSupportForum ()
		{
			Application.OpenURL ("https://forum.unity.com/threads/hud-navigation-system-radar-compass-bar-indicators-minimap.528165/");
		}
		#endif
		#endregion
	}
}
