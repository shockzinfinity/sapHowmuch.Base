using NLog;
using NLog.Config;
using NLog.Targets;

namespace sapHowmuch.Base.Helpers
{
	public static class sapHowmuchLogger
	{
		private static readonly Logger _classLogger;

		static sapHowmuchLogger()
		{
#if DEBUG
			var sentinalTarget = new NLogViewerTarget()
			{
				Name = "sentinal",
				Address = "udp://127.0.0.1:9999",
				IncludeNLogData = false
			};

			var sentinalRule = new LoggingRule("*", LogLevel.Trace, sentinalTarget);
			LogManager.Configuration.AddTarget(sentinalTarget.Name, sentinalTarget);
			LogManager.Configuration.LoggingRules.Add(sentinalRule);
#endif
			LogManager.ReconfigExistingLoggers();

			_classLogger = LogManager.GetCurrentClassLogger();
		}

		/// <summary>
		/// This method writes the Debug information to trace file
		/// </summary>
		/// <param name="message">the message</param>
		public static void Debug(string message)
		{
			if (!_classLogger.IsDebugEnabled) return;

			_classLogger.Debug(message);
		}

		/// <summary>
		/// This method writes the information to trace file
		/// </summary>
		/// <param name="message">the message</param>
		public static void Info(string message)
		{
			if (!_classLogger.IsInfoEnabled) return;

			_classLogger.Info(message);
		}

		/// <summary>
		/// This method writes the warning to trace file
		/// </summary>
		/// <param name="message">the message</param>
		public static void Warn(string message)
		{
			if (!_classLogger.IsWarnEnabled) return;

			_classLogger.Warn(message);
		}

		/// <summary>
		/// This method writes the error to trace file
		/// </summary>
		/// <param name="message">the message</param>
		public static void Error(string message)
		{
			if (!_classLogger.IsErrorEnabled) return;

			_classLogger.Error(message);
		}

		/// <summary>
		/// This method writes the fatal exception to trace file
		/// </summary>
		/// <param name="message">the message</param>
		public static void Fatal(string message)
		{
			if (!_classLogger.IsFatalEnabled) return;

			_classLogger.Fatal(message);
		}

		/// <summary>
		/// This method writes the trace information to trace file
		/// </summary>
		/// <param name="message">the message</param>
		public static void Trace(string message)
		{
			if (!_classLogger.IsTraceEnabled) return;

			_classLogger.Trace(message);
		}
	}
}