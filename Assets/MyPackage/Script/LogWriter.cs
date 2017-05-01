using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogWriter {

	public static int logCapacity = 200;

	public static List<string> loglist = new List<string> ();

	public static List<string> getLogList(){
		return loglist;
	}

	public static void Log(string lines){
		if (loglist.Count > logCapacity) {
			loglist.RemoveAt (0);
		}

		loglist.Add (string.Format ("::INFO:: {0}\n", lines));

	}

	public static void Error(string lines){
		if (loglist.Count > logCapacity) {
			loglist.RemoveAt (0);
		}
		loglist.Add(string.Format ("::ERROR:: {0}\n", lines));
	}
}
