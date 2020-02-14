﻿using Newtonsoft.Json;

namespace Jellyfish.Virtu
{
	public sealed class DiskIIDrive
	{
		// ReSharper disable once FieldCanBeMadeReadOnly.Local
		private IDiskIIController _diskController;

		// ReSharper disable once UnusedMember.Global
		public DiskIIDrive() { }

		public DiskIIDrive(IDiskIIController diskController)
		{
			_diskController = diskController;
			_driveArmStepDelta[0] = new[] { 0, 0, 1, 1, 0, 0, 1, 1, -1, -1, 0, 0, -1, -1, 0, 0 }; // phase 0
			_driveArmStepDelta[1] = new[] { 0, -1, 0, -1, 1, 0, 1, 0, 0, -1, 0, -1, 1, 0, 1, 0 }; // phase 1
			_driveArmStepDelta[2] = new[] { 0, 0, -1, -1, 0, 0, -1, -1, 1, 1, 0, 0, 1, 1, 0, 0 }; // phase 2
			_driveArmStepDelta[3] = new[] { 0, 1, 0, 1, -1, 0, -1, 0, 0, 1, 0, 1, -1, 0, -1, 0 }; // phase 3
		}

		// ReSharper disable once UnusedMember.Global
		public void InsertDisk(string name, byte[] data, bool isWriteProtected)
		{
			FlushTrack();
			_disk = Disk525.CreateDisk(name, data, isWriteProtected);
			_trackLoaded = false;
		}

		private static int Clamp(int value, int min, int max)
		{
			return value < min ? min : value > max ? max : value;
		}

		internal void ApplyPhaseChange(int phaseState)
		{
			// step the drive head according to stepper magnet changes
			int delta = _driveArmStepDelta[_trackNumber & 0x3][phaseState];
			if (delta != 0)
			{
				int newTrackNumber = Clamp(_trackNumber + delta, 0, TrackNumberMax);
				if (newTrackNumber != _trackNumber)
				{
					FlushTrack();
					_trackNumber = newTrackNumber;
					_trackOffset = 0;
					_trackLoaded = false;
				}
			}
		}

		internal int Read()
		{
			if (LoadTrack())
			{
				int data = _trackData[_trackOffset++];
				if (_trackOffset >= Disk525.TrackSize)
				{
					_trackOffset = 0;
				}

				_diskController.DriveLight = true;
				return data;
			}

			return 0x80;
		}

		internal void Write(int data)
		{
			if (LoadTrack())
			{
				_trackChanged = true;
				_trackData[_trackOffset++] = (byte)data;
				if (_trackOffset >= Disk525.TrackSize)
				{
					_trackOffset = 0;
				}

				_diskController.DriveLight = true;
			}
		}

		private bool LoadTrack()
		{
			if (!_trackLoaded && (_disk != null))
			{
				_disk.ReadTrack(_trackNumber, 0, _trackData);
				_trackLoaded = true;
			}

			return _trackLoaded;
		}

		internal void FlushTrack()
		{
			if (_trackChanged)
			{
				_disk.WriteTrack(_trackNumber, 0, _trackData);
				_trackChanged = false;
			}
		}

		[JsonIgnore]
		public bool IsWriteProtected => _disk.IsWriteProtected;

		private const int TrackNumberMax = 0x44;

		private const int PhaseCount = 4;

		// ReSharper disable once FieldCanBeMadeReadOnly.Local
		private int[][] _driveArmStepDelta = new int[PhaseCount][];

		private bool _trackLoaded;
		private bool _trackChanged;
		private int _trackNumber;
		private int _trackOffset;

		// ReSharper disable once FieldCanBeMadeReadOnly.Local
		private byte[] _trackData = new byte[Disk525.TrackSize];

		private Disk525 _disk;
	}
}
