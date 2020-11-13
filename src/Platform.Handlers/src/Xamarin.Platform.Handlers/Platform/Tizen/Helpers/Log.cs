using System;
using System.Runtime.CompilerServices;

namespace Xamarin.Platform.Tizen
{
	public static class Log
	{
		public static string Tag { get; set; } = "Xamarin";
		public static ILogger Logger { get; set; } = IsTizen() ? (ILogger)new DlogLogger() : (ILogger)new ConsoleLogger();

		public static void Debug(string message,
								 Guardian _ = default(Guardian),
								 [CallerFilePath] string file = "",
								 [CallerMemberName] string func = "",
								 [CallerLineNumber] int line = 0)
		{
			Logger.Debug(Tag, message, file, func, line);
		}

		public static void Debug<T0>(string message,
									 T0 arg0,
									 Guardian _ = default(Guardian),
									 [CallerFilePath] string file = "",
									 [CallerMemberName] string func = "",
									 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0), _, file, func, line);
		}

		public static void Debug<T0, T1>(string message,
										 T0 arg0,
										 T1 arg1,
										 Guardian _ = default(Guardian),
										 [CallerFilePath] string file = "",
										 [CallerMemberName] string func = "",
										 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0, arg1), _, file, func, line);
		}

		public static void Debug<T0, T1, T2>(string message,
											 T0 arg0,
											 T1 arg1,
											 T2 arg2,
											 Guardian _ = default(Guardian),
											 [CallerFilePath] string file = "",
											 [CallerMemberName] string func = "",
											 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0, arg1, arg2), _, file, func, line);
		}

		public static void Debug<T0, T1, T2, T3>(string message,
												 T0 arg0,
												 T1 arg1,
												 T2 arg2,
												 T3 arg3,
												 Guardian _ = default(Guardian),
												 [CallerFilePath] string file = "",
												 [CallerMemberName] string func = "",
												 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0, arg1, arg2, arg3), _, file, func, line);
		}

		public static void Debug<T0, T1, T2, T3, T4>(string message,
													 T0 arg0,
													 T1 arg1,
													 T2 arg2,
													 T3 arg3,
													 T4 arg4,
													 Guardian _ = default(Guardian),
													 [CallerFilePath] string file = "",
													 [CallerMemberName] string func = "",
													 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0, arg1, arg2, arg3, arg4), _, file, func, line);
		}

		public static void Debug<T0, T1, T2, T3, T4, T5>(string message,
														 T0 arg0,
														 T1 arg1,
														 T2 arg2,
														 T3 arg3,
														 T4 arg4,
														 T5 arg5,
														 Guardian _ = default(Guardian),
														 [CallerFilePath] string file = "",
														 [CallerMemberName] string func = "",
														 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5), _, file, func, line);
		}

		public static void Debug<T0, T1, T2, T3, T4, T5, T6>(string message,
															 T0 arg0,
															 T1 arg1,
															 T2 arg2,
															 T3 arg3,
															 T4 arg4,
															 T5 arg5,
															 T6 arg6,
															 Guardian _ = default(Guardian),
															 [CallerFilePath] string file = "",
															 [CallerMemberName] string func = "",
															 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6), _, file, func, line);
		}

		public static void Debug<T0, T1, T2, T3, T4, T5, T6, T7>(string message,
																 T0 arg0,
																 T1 arg1,
																 T2 arg2,
																 T3 arg3,
																 T4 arg4,
																 T5 arg5,
																 T6 arg6,
																 T7 arg7,
																 Guardian _ = default(Guardian),
																 [CallerFilePath] string file = "",
																 [CallerMemberName] string func = "",
																 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7), _, file, func, line);
		}

		public static void Debug<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string message,
																	 T0 arg0,
																	 T1 arg1,
																	 T2 arg2,
																	 T3 arg3,
																	 T4 arg4,
																	 T5 arg5,
																	 T6 arg6,
																	 T7 arg7,
																	 T8 arg8,
																	 Guardian _ = default(Guardian),
																	 [CallerFilePath] string file = "",
																	 [CallerMemberName] string func = "",
																	 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), _, file, func, line);
		}

		public static void Debug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string message,
																		 T0 arg0,
																		 T1 arg1,
																		 T2 arg2,
																		 T3 arg3,
																		 T4 arg4,
																		 T5 arg5,
																		 T6 arg6,
																		 T7 arg7,
																		 T8 arg8,
																		 T9 arg9,
																		 Guardian _ = default(Guardian),
																		 [CallerFilePath] string file = "",
																		 [CallerMemberName] string func = "",
																		 [CallerLineNumber] int line = 0)
		{
			Debug(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), _, file, func, line);
		}

		public static void Verbose(string message,
								   Guardian _ = default(Guardian),
								   [CallerFilePath] string file = "",
								   [CallerMemberName] string func = "",
								   [CallerLineNumber] int line = 0)
		{
			Logger.Verbose(Tag, message, file, func, line);
		}

		public static void Verbose<T0>(string message,
									   T0 arg0,
									   Guardian _ = default(Guardian),
									   [CallerFilePath] string file = "",
									   [CallerMemberName] string func = "",
									   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0), _, file, func, line);
		}

		public static void Verbose<T0, T1>(string message,
										   T0 arg0,
										   T1 arg1,
										   Guardian _ = default(Guardian),
										   [CallerFilePath] string file = "",
										   [CallerMemberName] string func = "",
										   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0, arg1), _, file, func, line);
		}

		public static void Verbose<T0, T1, T2>(string message,
											   T0 arg0,
											   T1 arg1,
											   T2 arg2,
											   Guardian _ = default(Guardian),
											   [CallerFilePath] string file = "",
											   [CallerMemberName] string func = "",
											   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0, arg1, arg2), _, file, func, line);
		}

		public static void Verbose<T0, T1, T2, T3>(string message,
												   T0 arg0,
												   T1 arg1,
												   T2 arg2,
												   T3 arg3,
												   Guardian _ = default(Guardian),
												   [CallerFilePath] string file = "",
												   [CallerMemberName] string func = "",
												   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0, arg1, arg2, arg3), _, file, func, line);
		}

		public static void Verbose<T0, T1, T2, T3, T4>(string message,
													   T0 arg0,
													   T1 arg1,
													   T2 arg2,
													   T3 arg3,
													   T4 arg4,
													   Guardian _ = default(Guardian),
													   [CallerFilePath] string file = "",
													   [CallerMemberName] string func = "",
													   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0, arg1, arg2, arg3, arg4), _, file, func, line);
		}

		public static void Verbose<T0, T1, T2, T3, T4, T5>(string message,
														   T0 arg0,
														   T1 arg1,
														   T2 arg2,
														   T3 arg3,
														   T4 arg4,
														   T5 arg5,
														   Guardian _ = default(Guardian),
														   [CallerFilePath] string file = "",
														   [CallerMemberName] string func = "",
														   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5), _, file, func, line);
		}

		public static void Verbose<T0, T1, T2, T3, T4, T5, T6>(string message,
															   T0 arg0,
															   T1 arg1,
															   T2 arg2,
															   T3 arg3,
															   T4 arg4,
															   T5 arg5,
															   T6 arg6,
															   Guardian _ = default(Guardian),
															   [CallerFilePath] string file = "",
															   [CallerMemberName] string func = "",
															   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6), _, file, func, line);
		}

		public static void Verbose<T0, T1, T2, T3, T4, T5, T6, T7>(string message,
																   T0 arg0,
																   T1 arg1,
																   T2 arg2,
																   T3 arg3,
																   T4 arg4,
																   T5 arg5,
																   T6 arg6,
																   T7 arg7,
																   Guardian _ = default(Guardian),
																   [CallerFilePath] string file = "",
																   [CallerMemberName] string func = "",
																   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7), _, file, func, line);
		}

		public static void Verbose<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string message,
																	   T0 arg0,
																	   T1 arg1,
																	   T2 arg2,
																	   T3 arg3,
																	   T4 arg4,
																	   T5 arg5,
																	   T6 arg6,
																	   T7 arg7,
																	   T8 arg8,
																	   Guardian _ = default(Guardian),
																	   [CallerFilePath] string file = "",
																	   [CallerMemberName] string func = "",
																	   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), _, file, func, line);
		}

		public static void Verbose<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string message,
																		   T0 arg0,
																		   T1 arg1,
																		   T2 arg2,
																		   T3 arg3,
																		   T4 arg4,
																		   T5 arg5,
																		   T6 arg6,
																		   T7 arg7,
																		   T8 arg8,
																		   T9 arg9,
																		   Guardian _ = default(Guardian),
																		   [CallerFilePath] string file = "",
																		   [CallerMemberName] string func = "",
																		   [CallerLineNumber] int line = 0)
		{
			Verbose(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), _, file, func, line);
		}

		public static void Info(string message,
								Guardian _ = default(Guardian),
								[CallerFilePath] string file = "",
								[CallerMemberName] string func = "",
								[CallerLineNumber] int line = 0)
		{
			Logger.Info(Tag, message, file, func, line);
		}

		public static void Info<T0>(string message,
									T0 arg0,
									Guardian _ = default(Guardian),
									[CallerFilePath] string file = "",
									[CallerMemberName] string func = "",
									[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0), _, file, func, line);
		}

		public static void Info<T0, T1>(string message,
										T0 arg0,
										T1 arg1,
										Guardian _ = default(Guardian),
										[CallerFilePath] string file = "",
										[CallerMemberName] string func = "",
										[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0, arg1), _, file, func, line);
		}

		public static void Info<T0, T1, T2>(string message,
											T0 arg0,
											T1 arg1,
											T2 arg2,
											Guardian _ = default(Guardian),
											[CallerFilePath] string file = "",
											[CallerMemberName] string func = "",
											[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0, arg1, arg2), _, file, func, line);
		}

		public static void Info<T0, T1, T2, T3>(string message,
												T0 arg0,
												T1 arg1,
												T2 arg2,
												T3 arg3,
												Guardian _ = default(Guardian),
												[CallerFilePath] string file = "",
												[CallerMemberName] string func = "",
												[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0, arg1, arg2, arg3), _, file, func, line);
		}

		public static void Info<T0, T1, T2, T3, T4>(string message,
													T0 arg0,
													T1 arg1,
													T2 arg2,
													T3 arg3,
													T4 arg4,
													Guardian _ = default(Guardian),
													[CallerFilePath] string file = "",
													[CallerMemberName] string func = "",
													[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0, arg1, arg2, arg3, arg4), _, file, func, line);
		}

		public static void Info<T0, T1, T2, T3, T4, T5>(string message,
														T0 arg0,
														T1 arg1,
														T2 arg2,
														T3 arg3,
														T4 arg4,
														T5 arg5,
														Guardian _ = default(Guardian),
														[CallerFilePath] string file = "",
														[CallerMemberName] string func = "",
														[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5), _, file, func, line);
		}

		public static void Info<T0, T1, T2, T3, T4, T5, T6>(string message,
															T0 arg0,
															T1 arg1,
															T2 arg2,
															T3 arg3,
															T4 arg4,
															T5 arg5,
															T6 arg6,
															Guardian _ = default(Guardian),
															[CallerFilePath] string file = "",
															[CallerMemberName] string func = "",
															[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6), _, file, func, line);
		}

		public static void Info<T0, T1, T2, T3, T4, T5, T6, T7>(string message,
																T0 arg0,
																T1 arg1,
																T2 arg2,
																T3 arg3,
																T4 arg4,
																T5 arg5,
																T6 arg6,
																T7 arg7,
																Guardian _ = default(Guardian),
																[CallerFilePath] string file = "",
																[CallerMemberName] string func = "",
																[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7), _, file, func, line);
		}

		public static void Info<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string message,
																	T0 arg0,
																	T1 arg1,
																	T2 arg2,
																	T3 arg3,
																	T4 arg4,
																	T5 arg5,
																	T6 arg6,
																	T7 arg7,
																	T8 arg8,
																	Guardian _ = default(Guardian),
																	[CallerFilePath] string file = "",
																	[CallerMemberName] string func = "",
																	[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), _, file, func, line);
		}

		public static void Info<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string message,
																		T0 arg0,
																		T1 arg1,
																		T2 arg2,
																		T3 arg3,
																		T4 arg4,
																		T5 arg5,
																		T6 arg6,
																		T7 arg7,
																		T8 arg8,
																		T9 arg9,
																		Guardian _ = default(Guardian),
																		[CallerFilePath] string file = "",
																		[CallerMemberName] string func = "",
																		[CallerLineNumber] int line = 0)
		{
			Info(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), _, file, func, line);
		}

		public static void Warn(string message,
								Guardian _ = default(Guardian),
								[CallerFilePath] string file = "",
								[CallerMemberName] string func = "",
								[CallerLineNumber] int line = 0)
		{
			Logger.Warn(Tag, message, file, func, line);
		}

		public static void Warn<T0>(string message,
									T0 arg0,
									Guardian _ = default(Guardian),
									[CallerFilePath] string file = "",
									[CallerMemberName] string func = "",
									[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0), _, file, func, line);
		}

		public static void Warn<T0, T1>(string message,
										T0 arg0,
										T1 arg1,
										Guardian _ = default(Guardian),
										[CallerFilePath] string file = "",
										[CallerMemberName] string func = "",
										[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0, arg1), _, file, func, line);
		}

		public static void Warn<T0, T1, T2>(string message,
											T0 arg0,
											T1 arg1,
											T2 arg2,
											Guardian _ = default(Guardian),
											[CallerFilePath] string file = "",
											[CallerMemberName] string func = "",
											[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0, arg1, arg2), _, file, func, line);
		}

		public static void Warn<T0, T1, T2, T3>(string message,
												T0 arg0,
												T1 arg1,
												T2 arg2,
												T3 arg3,
												Guardian _ = default(Guardian),
												[CallerFilePath] string file = "",
												[CallerMemberName] string func = "",
												[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0, arg1, arg2, arg3), _, file, func, line);
		}

		public static void Warn<T0, T1, T2, T3, T4>(string message,
													T0 arg0,
													T1 arg1,
													T2 arg2,
													T3 arg3,
													T4 arg4,
													Guardian _ = default(Guardian),
													[CallerFilePath] string file = "",
													[CallerMemberName] string func = "",
													[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0, arg1, arg2, arg3, arg4), _, file, func, line);
		}

		public static void Warn<T0, T1, T2, T3, T4, T5>(string message,
														T0 arg0,
														T1 arg1,
														T2 arg2,
														T3 arg3,
														T4 arg4,
														T5 arg5,
														Guardian _ = default(Guardian),
														[CallerFilePath] string file = "",
														[CallerMemberName] string func = "",
														[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5), _, file, func, line);
		}

		public static void Warn<T0, T1, T2, T3, T4, T5, T6>(string message,
															T0 arg0,
															T1 arg1,
															T2 arg2,
															T3 arg3,
															T4 arg4,
															T5 arg5,
															T6 arg6,
															Guardian _ = default(Guardian),
															[CallerFilePath] string file = "",
															[CallerMemberName] string func = "",
															[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6), _, file, func, line);
		}

		public static void Warn<T0, T1, T2, T3, T4, T5, T6, T7>(string message,
																T0 arg0,
																T1 arg1,
																T2 arg2,
																T3 arg3,
																T4 arg4,
																T5 arg5,
																T6 arg6,
																T7 arg7,
																Guardian _ = default(Guardian),
																[CallerFilePath] string file = "",
																[CallerMemberName] string func = "",
																[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7), _, file, func, line);
		}

		public static void Warn<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string message,
																	T0 arg0,
																	T1 arg1,
																	T2 arg2,
																	T3 arg3,
																	T4 arg4,
																	T5 arg5,
																	T6 arg6,
																	T7 arg7,
																	T8 arg8,
																	Guardian _ = default(Guardian),
																	[CallerFilePath] string file = "",
																	[CallerMemberName] string func = "",
																	[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), _, file, func, line);
		}

		public static void Warn<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string message,
																		T0 arg0,
																		T1 arg1,
																		T2 arg2,
																		T3 arg3,
																		T4 arg4,
																		T5 arg5,
																		T6 arg6,
																		T7 arg7,
																		T8 arg8,
																		T9 arg9,
																		Guardian _ = default(Guardian),
																		[CallerFilePath] string file = "",
																		[CallerMemberName] string func = "",
																		[CallerLineNumber] int line = 0)
		{
			Warn(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), _, file, func, line);
		}

		public static void Error(string message,
								 Guardian _ = default(Guardian),
								 [CallerFilePath] string file = "",
								 [CallerMemberName] string func = "",
								 [CallerLineNumber] int line = 0)
		{
			Logger.Error(Tag, message, file, func, line);
		}

		public static void Error<T0>(string message,
									 T0 arg0,
									 Guardian _ = default(Guardian),
									 [CallerFilePath] string file = "",
									 [CallerMemberName] string func = "",
									 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0), _, file, func, line);
		}

		public static void Error<T0, T1>(string message,
										 T0 arg0,
										 T1 arg1,
										 Guardian _ = default(Guardian),
										 [CallerFilePath] string file = "",
										 [CallerMemberName] string func = "",
										 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0, arg1), _, file, func, line);
		}

		public static void Error<T0, T1, T2>(string message,
											 T0 arg0,
											 T1 arg1,
											 T2 arg2,
											 Guardian _ = default(Guardian),
											 [CallerFilePath] string file = "",
											 [CallerMemberName] string func = "",
											 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0, arg1, arg2), _, file, func, line);
		}

		public static void Error<T0, T1, T2, T3>(string message,
												 T0 arg0,
												 T1 arg1,
												 T2 arg2,
												 T3 arg3,
												 Guardian _ = default(Guardian),
												 [CallerFilePath] string file = "",
												 [CallerMemberName] string func = "",
												 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0, arg1, arg2, arg3), _, file, func, line);
		}

		public static void Error<T0, T1, T2, T3, T4>(string message,
													 T0 arg0,
													 T1 arg1,
													 T2 arg2,
													 T3 arg3,
													 T4 arg4,
													 Guardian _ = default(Guardian),
													 [CallerFilePath] string file = "",
													 [CallerMemberName] string func = "",
													 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0, arg1, arg2, arg3, arg4), _, file, func, line);
		}

		public static void Error<T0, T1, T2, T3, T4, T5>(string message,
														 T0 arg0,
														 T1 arg1,
														 T2 arg2,
														 T3 arg3,
														 T4 arg4,
														 T5 arg5,
														 Guardian _ = default(Guardian),
														 [CallerFilePath] string file = "",
														 [CallerMemberName] string func = "",
														 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5), _, file, func, line);
		}

		public static void Error<T0, T1, T2, T3, T4, T5, T6>(string message,
															 T0 arg0,
															 T1 arg1,
															 T2 arg2,
															 T3 arg3,
															 T4 arg4,
															 T5 arg5,
															 T6 arg6,
															 Guardian _ = default(Guardian),
															 [CallerFilePath] string file = "",
															 [CallerMemberName] string func = "",
															 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6), _, file, func, line);
		}

		public static void Error<T0, T1, T2, T3, T4, T5, T6, T7>(string message,
																 T0 arg0,
																 T1 arg1,
																 T2 arg2,
																 T3 arg3,
																 T4 arg4,
																 T5 arg5,
																 T6 arg6,
																 T7 arg7,
																 Guardian _ = default(Guardian),
																 [CallerFilePath] string file = "",
																 [CallerMemberName] string func = "",
																 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7), _, file, func, line);
		}

		public static void Error<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string message,
																	 T0 arg0,
																	 T1 arg1,
																	 T2 arg2,
																	 T3 arg3,
																	 T4 arg4,
																	 T5 arg5,
																	 T6 arg6,
																	 T7 arg7,
																	 T8 arg8,
																	 Guardian _ = default(Guardian),
																	 [CallerFilePath] string file = "",
																	 [CallerMemberName] string func = "",
																	 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), _, file, func, line);
		}

		public static void Error<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string message,
																		 T0 arg0,
																		 T1 arg1,
																		 T2 arg2,
																		 T3 arg3,
																		 T4 arg4,
																		 T5 arg5,
																		 T6 arg6,
																		 T7 arg7,
																		 T8 arg8,
																		 T9 arg9,
																		 Guardian _ = default(Guardian),
																		 [CallerFilePath] string file = "",
																		 [CallerMemberName] string func = "",
																		 [CallerLineNumber] int line = 0)
		{
			Error(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), _, file, func, line);
		}

		public static void Fatal(string message,
								 Guardian _ = default(Guardian),
								 [CallerFilePath] string file = "",
								 [CallerMemberName] string func = "",
								 [CallerLineNumber] int line = 0)
		{
			Logger.Fatal(Tag, message, file, func, line);
		}

		public static void Fatal<T0>(string message,
									 T0 arg0,
									 Guardian _ = default(Guardian),
									 [CallerFilePath] string file = "",
									 [CallerMemberName] string func = "",
									 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0), _, file, func, line);
		}

		public static void Fatal<T0, T1>(string message,
										 T0 arg0,
										 T1 arg1,
										 Guardian _ = default(Guardian),
										 [CallerFilePath] string file = "",
										 [CallerMemberName] string func = "",
										 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0, arg1), _, file, func, line);
		}

		public static void Fatal<T0, T1, T2>(string message,
											 T0 arg0,
											 T1 arg1,
											 T2 arg2,
											 Guardian _ = default(Guardian),
											 [CallerFilePath] string file = "",
											 [CallerMemberName] string func = "",
											 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0, arg1, arg2), _, file, func, line);
		}

		public static void Fatal<T0, T1, T2, T3>(string message,
												 T0 arg0,
												 T1 arg1,
												 T2 arg2,
												 T3 arg3,
												 Guardian _ = default(Guardian),
												 [CallerFilePath] string file = "",
												 [CallerMemberName] string func = "",
												 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0, arg1, arg2, arg3), _, file, func, line);
		}

		public static void Fatal<T0, T1, T2, T3, T4>(string message,
													 T0 arg0,
													 T1 arg1,
													 T2 arg2,
													 T3 arg3,
													 T4 arg4,
													 Guardian _ = default(Guardian),
													 [CallerFilePath] string file = "",
													 [CallerMemberName] string func = "",
													 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0, arg1, arg2, arg3, arg4), _, file, func, line);
		}

		public static void Fatal<T0, T1, T2, T3, T4, T5>(string message,
														 T0 arg0,
														 T1 arg1,
														 T2 arg2,
														 T3 arg3,
														 T4 arg4,
														 T5 arg5,
														 Guardian _ = default(Guardian),
														 [CallerFilePath] string file = "",
														 [CallerMemberName] string func = "",
														 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5), _, file, func, line);
		}

		public static void Fatal<T0, T1, T2, T3, T4, T5, T6>(string message,
															 T0 arg0,
															 T1 arg1,
															 T2 arg2,
															 T3 arg3,
															 T4 arg4,
															 T5 arg5,
															 T6 arg6,
															 Guardian _ = default(Guardian),
															 [CallerFilePath] string file = "",
															 [CallerMemberName] string func = "",
															 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6), _, file, func, line);
		}

		public static void Fatal<T0, T1, T2, T3, T4, T5, T6, T7>(string message,
																 T0 arg0,
																 T1 arg1,
																 T2 arg2,
																 T3 arg3,
																 T4 arg4,
																 T5 arg5,
																 T6 arg6,
																 T7 arg7,
																 Guardian _ = default(Guardian),
																 [CallerFilePath] string file = "",
																 [CallerMemberName] string func = "",
																 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7), _, file, func, line);
		}

		public static void Fatal<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string message,
																	 T0 arg0,
																	 T1 arg1,
																	 T2 arg2,
																	 T3 arg3,
																	 T4 arg4,
																	 T5 arg5,
																	 T6 arg6,
																	 T7 arg7,
																	 T8 arg8,
																	 Guardian _ = default(Guardian),
																	 [CallerFilePath] string file = "",
																	 [CallerMemberName] string func = "",
																	 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), _, file, func, line);
		}

		public static void Fatal<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string message,
																		 T0 arg0,
																		 T1 arg1,
																		 T2 arg2,
																		 T3 arg3,
																		 T4 arg4,
																		 T5 arg5,
																		 T6 arg6,
																		 T7 arg7,
																		 T8 arg8,
																		 T9 arg9,
																		 Guardian _ = default(Guardian),
																		 [CallerFilePath] string file = "",
																		 [CallerMemberName] string func = "",
																		 [CallerLineNumber] int line = 0)
		{
			Fatal(String.Format(message, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), _, file, func, line);
		}

		static bool IsTizen()
		{
			return System.IO.File.Exists("/etc/tizen-release");
		}

		public struct Guardian
		{
		}
	}
}

