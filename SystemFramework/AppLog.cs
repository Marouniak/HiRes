
namespace HiRes.SystemFramework.Logging {

	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Threading;
    
	/// <summary>
	///     Logging class to provide tracing and logging support. 
	///     <remarks>
	///         There are four different logging levels (error, warning, info, trace) 
	///         that produce output to either the system event log or a tracing 
	///         file as specified in the current ApplicationConfiguration settings.
	///     </remarks>
	/// </summary>
	public class AppLog {
    

		private static TraceLevel eventLogTraceLevel;

		private static TraceSwitch debugSwitch;

		private static StreamWriter debugWriter;
        
		public static TraceLevel traceLevel {
			get { return eventLogTraceLevel; }
			set { eventLogTraceLevel = value; }
		}

		public static void LogTrace(String msg) {
			//Defer to the helper function to log the message.
			LogEvent(TraceLevel.Verbose, msg);
		}
                
		public static void LogInfo(String msg) {
			LogEvent(TraceLevel.Info, msg);
		}

		public static void LogWarning(String msg) {
			LogEvent(TraceLevel.Warning, msg);
		}
        
		public static void LogError(String msg) {
			LogEvent(TraceLevel.Error, msg);
		}
        
		public static void LogError(String msg, Exception e) {
			LogEvent(TraceLevel.Error, FormatException(e,msg));

		}

		/// <summary>
		///     Determine where a string needs to be written based on the
		///     configuration settings and the error level.
		///     <param name="level">The severity of the information to be logged.</param>
		///     <param name="messageText">The string to be logged.</param>
		/// </summary>
		private static void LogEvent(TraceLevel level, String messageText) {

			try {
				if (debugWriter != null) {
					if (level <= debugSwitch.Level) {
						lock(debugWriter) {
							Debug.WriteLine(messageText);
							debugWriter.Flush();
						}
					}
				}

				
        
				#region OLD
				if (level <= eventLogTraceLevel)
				{

					EventLogEntryType LogEntryType;
					switch (level)
					{
						case TraceLevel.Error:
							LogEntryType = EventLogEntryType.Error;
							break;
						case TraceLevel.Warning:
							LogEntryType = EventLogEntryType.Warning;
							break;
						case TraceLevel.Info:
							LogEntryType = EventLogEntryType.Information;
							break;
						case TraceLevel.Verbose:
							LogEntryType = EventLogEntryType.SuccessAudit;
							break;
						default:
							LogEntryType = EventLogEntryType.SuccessAudit;
							break;
					}
					
/*					if (!EventLog.SourceExists(LogConfiguration.EventLogSourceName)) {
						EventLog.CreateEventSource(LogConfiguration.EventLogSourceName,"Application");
					}

					EventLog eventLog = new EventLog();
					eventLog.Source = LogConfiguration.EventLogSourceName;
					//Write the entry to the event log
					eventLog.WriteEntry(messageText);*/

					/*EventLog eventLog = new EventLog("Application", LogConfiguration.EventLogMachineName, LogConfiguration.EventLogSourceName );
					//Write the entry to the event log
					eventLog.WriteEntry(messageText, LogEntryType);*/

					// TODO: change it to work with the source that is set in web.config
					EventLog eventLog = new EventLog();
					
					eventLog.Source = "Application";
					eventLog.MachineName = LogConfiguration.EventLogMachineName;
					//Write the entry to the event log
					eventLog.WriteEntry(messageText, LogEntryType);

				}
		#endregion
			}
			catch {
			} //Ignore any exceptions.
		}

		/// <summary>
		///     Format exception for logging purposes.
		///     <param name="ex">The Exception object to format</param>
		///     <param name="catchInfo">The string to prepend to the exception information.</param>
		///     <retvalue>
		///         <para>A nicely format exception string, including message and StackTrace information.</para>
		///     </retvalue>
		/// </summary>
		public static String FormatException(Exception ex, String catchInfo) {

			StringBuilder strBuilder = new StringBuilder();
			
			if (catchInfo != String.Empty) 
			{
				strBuilder.Append(catchInfo).Append("\r\n");
			}

			strBuilder.Append(ex.Message).Append("\r\n").Append(ex.StackTrace);

			return strBuilder.ToString();
		}

        
		/// <summary>
		///     Constructor for ApplicationLog.  
		///     <remarks>Initialize all shared variables based on the ApplicationConfigurationsettings. 
		///         Called when this class is first loaded.
		///     </remarks>
		/// </summary>
		static AppLog()
		{
			//
			// Read the current settings from the configuration file to determine
			//   whether we need trace file support, event logging, or both.
			//
        
			//Get the class object in order to take the initialization lock
			Type myType = typeof(AppLog);
        
			//Protect thread locks with Try/Catch to guarantee that we let go of the lock.
			try
			{
				//See if anyone else is using the lock, grab it if they//re not
				if (!Monitor.TryEnter(myType))
				{
					//Just wait until the other thread finishes processing, then leave if
					//  the lock was already in use.
					Monitor.Enter(myType);
					return;
				}
        
				//See if there is a debug configuration file specified and set up the
				//  tracing variables.
				bool clearSettings = true;
				try
				{
					//Check if we're enabled
        
					if (LogConfiguration.TracingEnabled)
					{
						//Make sure we have a TraceSettings file.
						String tracingFile = LogConfiguration.TracingTraceFile;
						if (tracingFile != String.Empty)
						{
        
							//Read in the tracing switch name and create the switch.
							String switchName = LogConfiguration.TracingSwitchName;
        
							//Create the new switch
							if (switchName != String.Empty)
							{
								//Create a debug listener and add it as a debug listener
								FileInfo file = new FileInfo(tracingFile);
								debugWriter = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
								Debug.Listeners.Add(new TextWriterTraceListener(debugWriter));
            
								debugSwitch = new TraceSwitch(switchName, LogConfiguration.TracingSwitchDescription);
								debugSwitch.Level = LogConfiguration.TracingTraceLevel;
							}
							clearSettings = false;
						}						
					}
				}
				catch
				{
					//Ignore the error
				}
            
				//Use default (empty) values if something went wrong
				if (clearSettings)
				{
					//Tracing information is off or not properly specified, clear it
					debugSwitch = null;
					debugWriter = null;
				}

				if(LogConfiguration.EventLogEnabled)
					eventLogTraceLevel = LogConfiguration.EventLogTraceLevel;
				else 
					eventLogTraceLevel = TraceLevel.Off;
			}
			finally
			{
				//Remove the lock from the class object
				Monitor.Exit(myType);
			}
		}
    
	} //class ApplicationLog

}
