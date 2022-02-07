﻿using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using BizHawk.Common;
using BizHawk.Common.IOExtensions;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores;
using BizHawk.Emulation.Cores.Nintendo.SNES;

namespace BizHawk.Client.Common.movie.import
{
	// ReSharper disable once UnusedMember.Global
	/// <summary>For lsnes' <see href="https://tasvideos.org/Lsnes/Movieformat"><c>.lsmv</c> format</see></summary>
	[ImporterFor("LSNES", ".lsmv")]
	internal class LsmvImport : MovieImporter
	{
		private static readonly byte[] Zipheader = { 0x50, 0x4b, 0x03, 0x04 };
		private LibsnesControllerDeck _deck;
		protected override void RunImport()
		{
			Result.Movie.HeaderEntries[HeaderKeys.Core] = CoreNames.Bsnes;

			// .LSMV movies are .zip files containing data files.
			using var fs = new FileStream(SourceFile.FullName, FileMode.Open, FileAccess.Read);
			{
				byte[] data = new byte[4];
				fs.Read(data, 0, 4);
				if (!data.SequenceEqual(Zipheader))
				{
					Result.Errors.Add("This is not a zip file.");
					return;
				}
				fs.Position = 0;
			}

			using var zip = new ZipArchive(fs, ZipArchiveMode.Read, true);

			var ss = new LibsnesCore.SnesSyncSettings
			{
				LeftPort = LibsnesControllerDeck.ControllerType.Gamepad,
				RightPort = LibsnesControllerDeck.ControllerType.Gamepad
			};
			_deck = new LibsnesControllerDeck(ss);

			string platform = VSystemID.Raw.SNES;

			foreach (var item in zip.Entries)
			{
				if (item.FullName == "authors")
				{
					using var stream = item.Open();
					string authors = Encoding.UTF8.GetString(stream.ReadAllBytes());
					string authorList = "";
					string authorLast = "";
					using (var reader = new StringReader(authors))
					{
						string line;

						// Each author is on a different line.
						while ((line = reader.ReadLine()) != null)
						{
							string author = line.Trim();
							if (author != "")
							{
								if (authorLast != "")
								{
									authorList += $"{authorLast}, ";
								}

								authorLast = author;
							}
						}
					}

					if (authorList != "")
					{
						authorList += "and ";
					}

					if (authorLast != "")
					{
						authorList += authorLast;
					}

					Result.Movie.HeaderEntries[HeaderKeys.Author] = authorList;
				}
				else if (item.FullName == "coreversion")
				{
					using var stream = item.Open();
					string coreVersion = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					Result.Movie.Comments.Add($"CoreOrigin {coreVersion}");
				}
				else if (item.FullName == "gamename")
				{
					using var stream = item.Open();
					string gameName = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					Result.Movie.HeaderEntries[HeaderKeys.GameName] = gameName;
				}
				else if (item.FullName == "gametype")
				{
					using var stream = item.Open();
					string gametype = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();

					// TODO: Handle the other types.
					switch (gametype)
					{
						case "gdmg":
							platform = VSystemID.Raw.GB;
							break;
						case "ggbc":
						case "ggbca":
							platform = VSystemID.Raw.GBC;
							break;
						case "sgb_ntsc":
						case "sgb_pal":
							platform = VSystemID.Raw.SNES;
							Config.GbAsSgb = true;
							break;
					}

					bool pal = gametype == "snes_pal" || gametype == "sgb_pal";
					Result.Movie.HeaderEntries[HeaderKeys.Pal] = pal.ToString();
				}
				else if (item.FullName == "input")
				{
					using var stream = item.Open();
					string input = Encoding.UTF8.GetString(stream.ReadAllBytes());

					int lineNum = 0;
					using (var reader = new StringReader(input))
					{
						lineNum++;
						string line;
						while ((line = reader.ReadLine()) != null)
						{
							if (line == "")
							{
								continue;
							}

							// Insert an empty frame in lsmv snes movies
							// https://github.com/TASEmulators/BizHawk/issues/721
							// Both emulators send the input to bsnes core at the same V interval, but:
							// lsnes' frame boundary occurs at V = 241, after which the input is read;
							// BizHawk's frame boundary is just before automatic polling;
							// This isn't a great place to add this logic but this code is a mess
							if (lineNum == 1 && platform == VSystemID.Raw.SNES)
							{
								// Note that this logic assumes the first non-empty log entry is a valid input log entry
								// and that it is NOT a subframe input entry.  It seems safe to assume subframe input would not be on the first line
								Result.Movie.AppendFrame(EmptyLmsvFrame());
							}

							ImportTextFrame(line, platform);
						}
					}
				}
				else if (item.FullName.StartsWith("moviesram."))
				{
					using var stream = item.Open();
					byte[] movieSram = stream.ReadAllBytes();
					if (movieSram.Length != 0)
					{
						// TODO:  Why don't we support this?
						Result.Errors.Add("Movies that begin with SRAM are not supported.");
						return;
					}
				}
				else if (item.FullName == "port1")
				{
					using var stream = item.Open();
					string port1 = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					Result.Movie.HeaderEntries["port1"] = port1;
					ss.LeftPort = LibsnesControllerDeck.ControllerType.Gamepad;
					_deck = new LibsnesControllerDeck(ss);
				}
				else if (item.FullName == "port2")
				{
					using var stream = item.Open();
					string port2 = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					Result.Movie.HeaderEntries["port2"] = port2;
					ss.RightPort = LibsnesControllerDeck.ControllerType.Gamepad;
					_deck = new LibsnesControllerDeck(ss);
				}
				else if (item.FullName == "projectid")
				{
					using var stream = item.Open();
					string projectId = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					Result.Movie.HeaderEntries["ProjectID"] = projectId;
				}
				else if (item.FullName == "rerecords")
				{
					using var stream = item.Open();
					string rerecords = Encoding.UTF8.GetString(stream.ReadAllBytes());
					int rerecordCount;

					// Try to parse the re-record count as an integer, defaulting to 0 if it fails.
					try
					{
						rerecordCount = int.Parse(rerecords);
					}
					catch
					{
						rerecordCount = 0;
					}

					Result.Movie.Rerecords = (ulong)rerecordCount;
				}
				else if (item.FullName.EndsWith(".sha256"))
				{
					using var stream = item.Open();
					string rom = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					int pos = item.FullName.LastIndexOf(".sha256");
					string name = item.FullName.Substring(0, pos);
					Result.Movie.HeaderEntries[$"SHA256_{name}"] = rom;
				}
				else if (item.FullName == "savestate")
				{
					Result.Errors.Add("Movies that begin with a savestate are not supported.");
					return;
				}
				else if (item.FullName == "subtitles")
				{
					using var stream = item.Open();
					string subtitles = Encoding.UTF8.GetString(stream.ReadAllBytes());
					using (var reader = new StringReader(subtitles))
					{
						string line;
						while ((line = reader.ReadLine()) != null)
						{
							var subtitle = ImportTextSubtitle(line);
							if (!string.IsNullOrEmpty(subtitle))
							{
								Result.Movie.Subtitles.AddFromString(subtitle);
							}
						}
					}
				}
				else if (item.FullName == "starttime.second")
				{
					using var stream = item.Open();
					string startSecond = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					Result.Movie.HeaderEntries["StartSecond"] = startSecond;
				}
				else if (item.FullName == "starttime.subsecond")
				{
					using var stream = item.Open();
					string startSubSecond = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					Result.Movie.HeaderEntries["StartSubSecond"] = startSubSecond;
				}
				else if (item.FullName == "systemid")
				{
					using var stream = item.Open();
					string systemId = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					Result.Movie.Comments.Add($"{EmulationOrigin} {systemId}");
				}
			}

			Result.Movie.HeaderEntries[HeaderKeys.Platform] = platform;
			Result.Movie.SyncSettingsJson = ConfigService.SaveWithType(ss);
			MaybeSetCorePreference(VSystemID.Raw.SNES, CoreNames.Bsnes, fileExt: ".lsmv");
		}

		private IController EmptyLmsvFrame()
		{
			SimpleController emptyController = new(_deck.Definition);

			foreach (var button in emptyController.Definition.BoolButtons)
			{
				emptyController[button] = false;
			}

			return emptyController;
		}

		private void ImportTextFrame(string line, string platform)
		{
			SimpleController controllers = new(_deck.Definition);

			var buttons = new[]
			{
				"B", "Y", "Select", "Start", "Up", "Down", "Left", "Right", "A", "X", "L", "R"
			};

			if (platform == VSystemID.Raw.GB || platform == VSystemID.Raw.GBC)
			{
				buttons = new[] { "A", "B", "Select", "Start", "Right", "Left", "Up", "Down" };
			}

			// Split up the sections of the frame.
			string[] sections = line.Split('|');

			if (sections.Length != 0)
			{
				string flags = sections[0];
				char[] off = { '.', ' ', '\t', '\n', '\r' };
				if (flags.Length == 0 || off.Contains(flags[0]))
				{
					Result.Warnings.Add("Unable to import subframe.");

				}

				bool reset = flags.Length >= 2 && !off.Contains(flags[1]);
				flags = SingleSpaces(flags.Substring(2));
				if (reset && ((flags.Length >= 2 && flags[1] != '0') || (flags.Length >= 4 && flags[3] != '0')))
				{
					Result.Warnings.Add("Unable to import delayed reset.");
				}

				controllers["Reset"] = reset;
			}

			// LSNES frames don't start or end with a |.
			int end = sections.Length;

			for (int player = 1; player < end; player++)
			{
				string prefix = $"P{player} ";
				
				// Gameboy doesn't currently have a prefix saying which player the input is for.
				if (controllers.Definition.Name == "Gameboy Controller")
				{
					prefix = "";
				}

				// Only count lines with that have the right number of buttons and are for valid players.
				if (
					sections[player].Length == buttons.Length)
				{
					for (int button = 0; button < buttons.Length; button++)
					{
						// Consider the button pressed so long as its spot is not occupied by a ".".
						controllers[prefix + buttons[button]] = sections[player][button] != '.';
					}
				}
			}

			// Convert the data for the controllers to a mnemonic and add it as a frame.
			Result.Movie.AppendFrame(controllers);
		}

		private static string ImportTextSubtitle(string line)
		{
			line = SingleSpaces(line);

			// The header name, frame, and message are separated by whitespace.
			int first = line.IndexOf(' ');
			int second = line.IndexOf(' ', first + 1);
			if (first != -1 && second != -1)
			{
				// Concatenate the frame and message with default values for the additional fields.
				string frame = line.Substring(0, first);
				string length = line.Substring(first + 1, second - first - 1);
				string message = line.Substring(second + 1).Trim();

				return $"subtitle {frame} 0 0 {length} FFFFFFFF {message}";
			}

			return null;
		}
	}
}
