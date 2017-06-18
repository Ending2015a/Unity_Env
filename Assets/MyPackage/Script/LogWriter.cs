using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public static class LogWriter {

	public static int logCapacity = 200;

	public static List<string> loglist = new List<string> ();
	public static bool enablelog = true;

	public static List<string> getLogList(){
		List<string> list;
		lock (loglist) {
			list = new List<string> (loglist);
		}
		return list;
	}

	public static void Log(string lines){
		if (!enablelog)
			return;
		lock (loglist) {
			if (loglist.Count > logCapacity) {
				loglist.RemoveAt (0);
			}

			loglist.Add (string.Format ("::INFO:: {0}\n", lines));
		}

	}

	public static void Error(string lines){
		if (!enablelog)
			return;
		lock (loglist) {
			if (loglist.Count > logCapacity) {
				loglist.RemoveAt (0);
			}
			loglist.Add(string.Format ("::ERROR:: {0}\n", lines));
		}
	}
}
