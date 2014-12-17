using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PartituraMagica
{
	public class TraceEntry : IDisposable
	{
		private string path;
		private int startTime;
		private int endTime;

		public TraceEntry(string path) {
			this.path = path;
			this.startTime = Environment.TickCount;
			this.endTime = -1;
		}

		public void Stop() {
			this.endTime = Environment.TickCount;
		}

		//public void StopIfNecessary() {
		//    if (this.endTime == -1) {
		//        this.Stop();
		//    }
		//}
		public override string ToString() {
			Debug.Assert(this.endTime != -1);
			//return String.Format("startTime = {0}, endTime = {1}",
			return String.Format("{0} {1} ({2})",
				this.path,
				this.startTime,
				this.endTime - this.startTime);
		}

		public void Dispose() {
			Stop();
		}
	}
	public static class SimpleTracer
	{
		private static List<TraceEntry> entries = new List<TraceEntry>();

		/** should be used as follows:
		 * using (TraceEntry e = t.Start("...")) {
		 *  ... perform action to measure
		 * }
		 */
		public static TraceEntry Start(string path) {
			TraceEntry newTraceEntry = new TraceEntry(path);
			entries.Add(newTraceEntry);
			return newTraceEntry;
		}

		public static string GetData() {			
			return String.Format("{0}",
				entries.Select(s => s.ToString()).Aggregate((x, y) => x + "\n" + y));
		}
	}
}
