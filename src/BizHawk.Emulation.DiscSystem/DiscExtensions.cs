﻿using System;

namespace BizHawk.Emulation.DiscSystem
{
	public static class DiscExtensions
	{
		public static Disc CreateAnyType(string path, Action<string> errorCallback)
		{
			return CreateImpl(null, path, errorCallback);
		}
		public static Disc Create(this DiscType type, string path, Action<string> errorCallback)
		{
			return CreateImpl(type, path, errorCallback);
		}

		private static Disc CreateImpl(DiscType? type, string path, Action<string> errorCallback)
		{
			//--- load the disc in a context which will let us abort if it's going to take too long
			var discMountJob = new DiscMountJob(fromPath: path, slowLoadAbortThreshold: 8);
			discMountJob.Run();
			
			if (discMountJob.OUT_SlowLoadAborted)
			{
				errorCallback("This disc would take too long to load. Run it through DiscoHawk first, or find a new rip because this one is probably junk");
				return null;
			}

			if (discMountJob.OUT_ErrorLevel)
			{
				throw new InvalidOperationException($"\r\n{discMountJob.OUT_Log}");
			}

			var disc = discMountJob.OUT_Disc;

			var discType = new DiscIdentifier(disc).DetectDiscType();

			if (type.HasValue && discType != type)
			{
				errorCallback($"Not a {type} disc");
				return null;
			}

			return disc;
		}
	}
}
