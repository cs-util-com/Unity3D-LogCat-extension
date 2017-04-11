﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LogCatExtension
{
	internal class LogCatFilter
	{
		public static List<LogCatLog> FilterLogList( FilterConfiguration config, List<LogCatLog> fullLogList )
		{
			List<LogCatLog> filterLogList = new List<LogCatLog> ();
			// Filter
			foreach (var logCatLog in fullLogList) {
				if (FilterLogCatLog (logCatLog, config)) {
					filterLogList.Add (logCatLog);
				}
			}
					
			return filterLogList;
		}

		private static bool FilterLogCatLog(LogCatLog logCatLog, FilterConfiguration config){
			bool shouldFilterByString = config.filterByString.Length > 1;
			if (!shouldFilterByString || FilterByString (logCatLog, config.filterByString)) {
				
			} else {
				return false;
			}
			bool shouldFilterByRegex = config.filterByRegex.Length > 1;
			if (!shouldFilterByRegex || FilterByRegex (logCatLog, config.filterByRegex)) {

			} else {
				return false;
			}
			if (!config.prefilterOnlyUnity || FilterByUnityString (logCatLog)) {

			} else {
				return false;
			}
			if (!config.filterTime || FilterByTimeSpan (logCatLog, config.filterByTimeFrom, config.filterByTimeTo)) {

			} else {
				return false;
			}
			bool filtered = false;
			if (filtered || (config.filterError && FilterByType (logCatLog, 'E'))) {
				filtered = true;
			} 
			if (filtered || (config.filterWarning && FilterByType (logCatLog, 'W'))) {
				filtered = true;
			} 
			if (filtered || (config.filterDebug && FilterByType (logCatLog, 'D'))) {
				filtered = true;
			} 
			if (filtered || (config.filterInfo && FilterByType (logCatLog, 'I'))) {
				filtered = true;
			} 
			if (filtered || (config.filterVerbose && FilterByType (logCatLog, 'V'))) {
				filtered = true;
			}

			return filtered;
		}

		private static bool FilterByString(LogCatLog log, string filterBy){
			return log.Message.ToLower ().Contains (filterBy.ToLower ());
		}

		private static bool FilterByRegex(LogCatLog log, string filterBy){
			if (string.IsNullOrEmpty (filterBy)) {
				return false;
			}

			bool filtered = false;

			try
			{
				Regex rgx = new Regex(filterBy);
				filtered = rgx.IsMatch(log.Message);
			}
			catch (ArgumentException)
			{
				return false;
			}

			return filtered;
		}

		private static bool FilterByUnityString(LogCatLog log){
			return log.Message.Contains ("Unity");
		}

		private static bool FilterByType(LogCatLog log, char type){
			return log.Type == type;
		}

		private static bool FilterByTimeSpan(LogCatLog log, string filterTimeFrom, string filterTimeTo){
			TimeSpan logTimeTamp = log.TimeSpamp.TimeOfDay;
			TimeSpan timeFrom = TimeSpan.Parse(filterTimeFrom);
			TimeSpan timeTo = TimeSpan.Parse(filterTimeTo);

			if ((logTimeTamp > timeFrom) && (logTimeTamp < timeTo))
			{
				return true;
			}

			return false;
		}
	}
}