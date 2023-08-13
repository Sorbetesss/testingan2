﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using BizHawk.Common.IOExtensions;
using BizHawk.Common.StringExtensions;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores;
using BizHawk.Emulation.Cores.Nintendo.BSNES;

namespace BizHawk.Client.Common.movie.import
{
	// ReSharper disable once UnusedMember.Global
	/// <summary>For lsnes' <see href="https://tasvideos.org/Lsnes/Movieformat"><c>.lsmv</c> format</see></summary>
	[ImporterFor("LSNES", ".lsmv")]
	internal class LsmvImport : MovieImporter
	{
		private static readonly byte[] Zipheader = { 0x50, 0x4b, 0x03, 0x04 };
		private BsnesControllers _controllers;
		private int _playerCount;
		// hacky variable; just exists because if subframe input is used, the previous frame needs to be marked subframe aware
		private SimpleController _previousControllers;

		protected override void RunImport()
		{
			Result.Movie.HeaderEntries[HeaderKeys.Core] = CoreNames.SubBsnes115;

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

			var ss = new BsnesCore.SnesSyncSettings();

			string platform = VSystemID.Raw.SNES;

			// need to handle ports first to ensure controller types are known
			ZipArchiveEntry portEntry;
			if ((portEntry = zip.GetEntry("port1")) != null)
			{
				using var stream = portEntry.Open();
				string port1 = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
				Result.Movie.HeaderEntries["port1"] = port1;
				ss.LeftPort = port1 switch
				{
					"none" => BsnesApi.BSNES_PORT1_INPUT_DEVICE.None,
					"gamepad16" => BsnesApi.BSNES_PORT1_INPUT_DEVICE.ExtendedGamepad,
					"multitap" => BsnesApi.BSNES_PORT1_INPUT_DEVICE.SuperMultitap,
					"multitap16" => BsnesApi.BSNES_PORT1_INPUT_DEVICE.Payload,
					_ => BsnesApi.BSNES_PORT1_INPUT_DEVICE.Gamepad
				};
			}
			if ((portEntry = zip.GetEntry("port2")) != null)
			{
				using var stream = portEntry.Open();
				string port2 = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
				Result.Movie.HeaderEntries["port2"] = port2;
				ss.RightPort = port2 switch
				{
					"none" => BsnesApi.BSNES_INPUT_DEVICE.None,
					"gamepad16" => BsnesApi.BSNES_INPUT_DEVICE.ExtendedGamepad,
					"multitap" => BsnesApi.BSNES_INPUT_DEVICE.SuperMultitap,
					"multitap16" => BsnesApi.BSNES_INPUT_DEVICE.Payload,
					// will these even work lol
					"superscope" => BsnesApi.BSNES_INPUT_DEVICE.SuperScope,
					"justifier" => BsnesApi.BSNES_INPUT_DEVICE.Justifier,
					"justifiers" => BsnesApi.BSNES_INPUT_DEVICE.Justifiers,
					_ => BsnesApi.BSNES_INPUT_DEVICE.Gamepad
				};
			}
			_controllers = new BsnesControllers(ss, true);
			Result.Movie.LogKey = new Bk2LogEntryGenerator("SNES", new Bk2Controller(_controllers.Definition)).GenerateLogKey();
			_playerCount = _controllers.Definition.PlayerCount;

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
						// Each author is on a different line.
						while (reader.ReadLine() is string line)
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

					// Insert an empty frame in lsmv snes movies
					// see https://github.com/TASEmulators/BizHawk/issues/721
					Result.Movie.AppendFrame(EmptyLmsvFrame());
					using (var reader = new StringReader(input))
					{
						while(reader.ReadLine() is string line)
						{
							if (line == "") continue;

							ImportTextFrame(line);
						}
					}
					Result.Movie.AppendFrame(_previousControllers);
				}
				else if (item.FullName.StartsWithOrdinal("moviesram."))
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
					ulong rerecordCount;

					// Try to parse the re-record count as an integer, defaulting to 0 if it fails.
					try
					{
						rerecordCount = ulong.Parse(rerecords);
					}
					catch
					{
						rerecordCount = 0;
					}

					Result.Movie.Rerecords = rerecordCount;
				}
				else if (item.FullName.EndsWithOrdinal(".sha256"))
				{
					using var stream = item.Open();
					string sha256Hash = Encoding.UTF8.GetString(stream.ReadAllBytes()).Trim();
					string name = item.FullName.RemoveSuffix(".sha256");
					Result.Movie.HeaderEntries[name is "rom" ? HeaderKeys.Sha256 : $"SHA256_{name}"] = sha256Hash;
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
						while (reader.ReadLine() is string line)
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
		}

		private IController EmptyLmsvFrame()
		{
			SimpleController emptyController = new(_controllers.Definition);

			foreach (var button in emptyController.Definition.BoolButtons)
			{
				emptyController[button] = false;
			}

			return emptyController;
		}

		private void ImportTextFrame(string line)
		{
			SimpleController controllers = new(_controllers.Definition);

			// Split up the sections of the frame.
			string[] sections = line.Split('|');

			bool reset = false;
			if (sections.Length != 0)
			{
				string flags = sections[0];
				if (flags[0] != 'F' && _previousControllers != null) _previousControllers["Subframe"] = true;
				reset = flags[1] != '.';
				flags = SingleSpaces(flags.Substring(2));
				string[] splitFlags = flags.Split(' ');
				int delay;
				try
				{
					delay = int.Parse(splitFlags[1]) * 10000 + int.Parse(splitFlags[2]);
				}
				catch
				{
					delay = 0;
				}

				if (delay != 0)
				{
					controllers.AcceptNewAxis("Reset Instruction", delay);
					Result.Warnings.Add("Delayed reset may be mistimed."); // lsnes doesn't count some instructions that our bsnes version does
				}

				controllers["Reset"] = reset;
			}

			// LSNES frames don't start or end with a |.
			int end = sections.Length;

			for (int player = 1; player < end; player++)
			{
				if (player > _playerCount) break;

				IReadOnlyList<string> buttons = controllers.Definition.ControlsOrdered[player];
				if (buttons[0].EndsWithOrdinal("Up")) // hack to identify gamepad / multitap which have a different button order in bizhawk compared to lsnes
				{
					buttons = new[] { "B", "Y", "Select", "Start", "Up", "Down", "Left", "Right", "A", "X", "L", "R" }
						.Select(button => $"P{player} {button}")
						.ToList();
				}
				// Only consider lines that have the right number of buttons
				if (sections[player].Length == buttons.Count)
				{
					for (int button = 0; button < buttons.Count; button++)
					{
						// Consider the button pressed so long as its spot is not occupied by a ".".
						controllers[buttons[button]] = sections[player][button] != '.';
					}
				}
			}

			// Convert the data for the controllers to a mnemonic and add it as a frame.
			if (_previousControllers != null)
				Result.Movie.AppendFrame(_previousControllers);

			if (reset) Result.Movie.AppendFrame(EmptyLmsvFrame());

			_previousControllers = controllers;
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
