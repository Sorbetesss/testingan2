﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using Newtonsoft.Json;


using System.Windows.Forms;


using BizHawk.Client.EmuHawk.ToolExtensions;

using BizHawk.Emulation.Common;
using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class GyroscopeBot : ToolFormBase , IToolFormAutoConfig
	{
		private const string DialogTitle = "Gyroscope Bot";

		private string _currentFileName = "";

		private string CurrentFileName
		{
			get { return _currentFileName; }
			set
			{
				_currentFileName = value;

				if (!string.IsNullOrWhiteSpace(_currentFileName))
				{
					Text = DialogTitle + " - " + Path.GetFileNameWithoutExtension(_currentFileName);
				}
				else
				{
					Text = DialogTitle;
				}
			}

		}

		private bool _isBotting = false;
		private long _attempts = 1;
		private long _frames = 0;
		private int _targetFrame = 0;
		private bool _oldCountingSetting = false;
		private BotAttempt _currentBotAttempt = null;
		private BotAttempt _bestBotAttempt = null;
		private BotAttempt _comparisonBotAttempt = null;
		private bool _replayMode = false;
		private int _startFrame = 0;
		private string _lastRom = "";

		private bool _dontUpdateValues = false;

		private MemoryDomain _currentDomain;
		private bool _bigEndian;
		private int _dataSize;

		private Dictionary<string, double> _cachedControlProbabilities;
		private ILogEntryGenerator _logGenerator;
		private TcpClient client;
		private string IP;
		private int port;
		
		#region Services and Settings

		[RequiredService]
		private IEmulator Emulator { get; set; }

		// Unused, due to the use of MainForm to loadstate, but this needs to be kept here in order to establish an IStatable dependency
		[RequiredService]
		private IStatable StatableCore { get; set; }

		[RequiredService]
		private IMemoryDomains MemoryDomains { get; set; }

		[ConfigPersist]
		public GyroscopeBotSettings Settings { get; set; }

		public class GyroscopeBotSettings
		{
			public GyroscopeBotSettings()
			{
				RecentBotFiles = new RecentFiles();
				TurboWhenBotting = true;
			}

			public RecentFiles RecentBotFiles { get; set; }
			public bool TurboWhenBotting { get; set; }
		}

		#endregion

		#region sockethandling
		private TcpClient CreateTCPClient(string IP, int port)
		{
			return new TcpClient(IP, port);
		}
		
		private ControllerCommand SendEmulatorGameStateToController()
		{
			ControllerCommand cc = new ControllerCommand();
			try
			{

				
				NetworkStream stream = this.client.GetStream();
				byte[] bytes = new byte[1024];
				// Encode the data string into a byte array. 
				GameState gs = GetCurrentState();
				string data = JsonConvert.SerializeObject(gs);

				byte[] msg = Encoding.ASCII.GetBytes(data);
				stream.Write(msg, 0, msg.Length);


				StringBuilder myCompleteMessage = new StringBuilder();
				if (stream.CanRead)
				{
					byte[] myReadBuffer = new byte[1024];
					int numberOfBytesRead = 0;
					// Incoming message may be larger than the buffer size.
					do
					{
						numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
						myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
					}
					while (stream.DataAvailable);
				}
				cc = JsonConvert.DeserializeObject<ControllerCommand>(myCompleteMessage.ToString());
			}
			catch (ArgumentNullException ane)
			{
				cc.type = "__err__" + ane.ToString();
			}
			catch (SocketException se)
			{
				cc.type = "__err__" + se.ToString();
			}
			catch (Exception e)
			{
				cc.type = "__err__" + e.ToString();
			}
			return cc;
		}
		/*
		public static string ConnectToSocketAndSend(string IP, int port, string data)
		{
			try
			{


				TcpClient client = new TcpClient(IP, port);
				// Create a TCP/IP  socket.  
				//this.sender = new Socket(AddressFamily.InterNetwork,
				//	SocketType.Stream, ProtocolType.Tcp);
				//this.sender.Connect(remoteEP);
				//
				byte[] bytes = new byte[1024];
				// Encode the data string into a byte array.  
				byte[] msg = Encoding.ASCII.GetBytes(data);
				NetworkStream stream = client.GetStream();
				stream.Write(msg, 0, msg.Length);
				StringBuilder myCompleteMessage = new StringBuilder();

				if (stream.CanRead)
				{
					byte[] myReadBuffer = new byte[1024];
					int numberOfBytesRead = 0;
					// Incoming message may be larger than the buffer size.
					do
					{
						numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

						myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

					}
					while (stream.DataAvailable);

				}

				return myCompleteMessage.ToString();
			}
			catch (ArgumentNullException ane)
			{
				//return "__err__" + ane.ToString();
			}
			catch (SocketException se)
			{
				//return "__err__" + se.ToString();
			}
			catch (Exception e)
			{
				//return "__err__" + e.ToString();
			}
		}
		*/
		#endregion

		#region Initialize

		public GyroscopeBot()
		{
			InitializeComponent();
			Text = DialogTitle;
			Settings = new GyroscopeBotSettings();
			this.IP = "127.0.0.1";
			this.port = 9999;

			_comparisonBotAttempt = new BotAttempt();
			
		}

		private void GyroscopeBot_Load(object sender, EventArgs e)
		{
			StartBot();
		}

		#endregion

		#region streetfighter

		public int get_framecount()
		{
			return Emulator.Frame;
		}

		public int get_p1_health()
		{
			return _currentDomain.PeekByte(0x000530);
		}

		public int get_p2_health()
		{
			return _currentDomain.PeekByte(0x000730);
		}

		public int get_p1_character()
		{
			return _currentDomain.PeekByte(0x0005D1);
		}

		public int get_p2_character()
		{
			return _currentDomain.PeekByte(0x0007D1);
		}

		public int get_p1_x()
		{
			// make sure we are little endian
			return _currentDomain.PeekUshort(0x000022, _bigEndian);
		}

		public int get_p2_x()
		{
			return _currentDomain.PeekUshort(0x000026, _bigEndian);
		}

		public int get_p1_y()
		{
			return _currentDomain.PeekByte(0x00050A);
		}

		public int get_p2_y()
		{
			return _currentDomain.PeekByte(0x00070A);
		}

		public bool is_p1_jumping()
		{
			return _currentDomain.PeekByte(0x0005EA) == 1;
		}

		public bool is_p2_jumping()
		{
			return _currentDomain.PeekByte(0x0007EA) == 1;
		}

		public int p_height_delta()
		{
			return _currentDomain.PeekByte(0x0005ED);
		}

		public bool is_p1_crouching()
		{
			return _currentDomain.PeekByte(0x000544) == 1;
		}

		public bool is_p2_crouching()
		{
			return _currentDomain.PeekByte(0x00744) == 1;
		}

		public int get_timer()
		{
			return _currentDomain.PeekByte(0x0018F3);
		}

		public bool is_round_started()
		{
			return get_timer() > 0 && get_timer() <= 152;
		}

		private bool is_round_over()
		{
			return get_p1_health() == 255 || get_p2_health() == 255 || get_timer() <= 0;
		}

		private string get_round_result()
		{
			if (get_p1_health() == 255)
			{
				return "P2";
			}
			else if (get_p2_health() == 255)
			{
				return "P1";
			}
			else
			{
				return "NOT_OVER";
			}
		}

		public Dictionary<string, bool> GetJoypadButtons(int? controller = null)
		{
			var buttons = new Dictionary<string, bool>();
			var adaptor = Global.AutofireStickyXORAdapter;
			foreach (var button in adaptor.Source.Definition.BoolButtons)
			{
				if (!controller.HasValue)
				{
					buttons[button] = adaptor.IsPressed(button);
				}
				else if (button.Length >= 3 && button.Substring(0, 2) == "P" + controller)
				{
					buttons[button.Substring(3)] = adaptor.IsPressed("P" + controller + " " + button.Substring(3));
				}
			}
			return buttons;
		}


		public void SetJoypadButtons(Dictionary<string,bool> buttons, int? controller = null)
		{
			try
			{
				foreach (var button in buttons.Keys)
				{
					var invert = false;
					bool? theValue;
					var theValueStr = buttons[button].ToString();

					if (!string.IsNullOrWhiteSpace(theValueStr))
					{
						if (theValueStr.ToLower() == "false")
						{
							theValue = false;
						}
						else if (theValueStr.ToLower() == "true")
						{
							theValue = true;
						}
						else
						{
							invert = true;
							theValue = null;
						}
					}
					else
					{
						theValue = null;
					}

					var toPress = button.ToString();
					if (controller.HasValue)
					{
						toPress = "P" + controller + " " + button;
					}

					if (!invert)
					{
						if (theValue.HasValue) // Force
						{
							Global.LuaAndAdaptor.SetButton(toPress, theValue.Value);
							Global.ActiveController.Overrides(Global.LuaAndAdaptor);
						}
						else // Unset
						{
							Global.LuaAndAdaptor.UnSet(toPress);
							Global.ActiveController.Overrides(Global.LuaAndAdaptor);
						}
					}
					else // Inverse
					{
						Global.LuaAndAdaptor.SetInverse(toPress);
						Global.ActiveController.Overrides(Global.LuaAndAdaptor);
					}
				}
			}
			catch
			{
				/*Eat it*/
			}
		}
		private class PlayerState
		{
			public PlayerState()
			{
			}
			public int character { get; set; }
			public int health { get; set; }
			public int x { get; set; }
			public int y { get; set; }
			public bool jumping { get; set; }
			public bool crouching { get; set; }
			public Dictionary<string, bool> buttons { get; set; }


		}
		private class GameState
		{
			public GameState()
			{
			}
			public PlayerState p1 { get; set; }
			public PlayerState p2 { get; set; }
			public int frame { get; set; }
			public int timer { get; set; }
			public string result { get; set; }
			public bool round_started { get; set; }
			public bool round_over { get; set; }
			public int height_delta { get; set; }
		}

		private GameState GetCurrentState()
		{
			PlayerState p1 = new PlayerState();
			PlayerState p2 = new PlayerState();
			GameState gs = new GameState();
			p1.health = get_p1_health();
			p1.x = get_p1_x();
			p1.y = get_p1_y();
			p1.jumping = is_p1_jumping();
			p1.crouching = is_p1_crouching();
			p1.character = get_p1_character();
			p1.buttons = GetJoypadButtons(1);


			p2.health = get_p2_health();
			p2.x = get_p2_x();
			p2.y = get_p2_y();
			p2.jumping = is_p2_jumping();
			p2.crouching = is_p2_crouching();
			p2.character = get_p2_character();
			p2.buttons = GetJoypadButtons(2);

			gs.p1 = p1;
			gs.p2 = p2;
			gs.result = get_round_result();
			gs.frame = Emulator.Frame;
			gs.timer = get_timer();
			gs.round_started = is_round_started();
			gs.round_over = is_round_over();
			gs.height_delta = p_height_delta();

			return gs;
		}
		#endregion

		#region UI Bindings

		private Dictionary<string, double> ControlProbabilities
		{
			get
			{
				return ControlProbabilityPanel.Controls
					.OfType<GyroscopeBotControlsRow>()
					.ToDictionary(tkey => tkey.ButtonName, tvalue => tvalue.Probability);
			}
		}

		private string SelectedSlot
		{
			get
			{
				char num = StartFromSlotBox.SelectedItem
					.ToString()
					.Last();

				return "QuickSave" + num;
			}
		}

		private long Attempts
		{
			get { return _attempts; }
			set
			{
				_attempts = value;
				AttemptsLabel.Text = _attempts.ToString();
			}
		}

		private long Frames
		{
			get { return _frames; }
			set
			{
				_frames = value;
				FramesLabel.Text = _frames.ToString();
			}
		}

		private int FrameLength
		{
			get { return (int)FrameLengthNumeric.Value; }
			set { FrameLengthNumeric.Value = value; }
		}

		public int MaximizeAddress
		{
			get
			{
				int? addr = MaximizeAddressBox.ToRawInt();
				if (addr.HasValue)
				{
					return addr.Value;
				}

				return 0;
			}

			set
			{
				MaximizeAddressBox.SetFromRawInt(value);
			}
		}

		public int MaximizeValue
		{
			get
			{
				int? addr = MaximizeAddressBox.ToRawInt();
				if (addr.HasValue)
				{
					return GetRamvalue(addr.Value);
				}

				return 0;
			}
		}

		
		public byte MainComparisonType
		{
			get
			{
				return (byte)MainOperator.SelectedIndex;
			}
			set
			{
				if (value < 5) MainOperator.SelectedIndex = value;
				else MainOperator.SelectedIndex = 0;
			}
		}


		public string FromSlot
		{
			get
			{
				return StartFromSlotBox.SelectedItem != null 
					? StartFromSlotBox.SelectedItem.ToString()
					: "";
			}

			set
			{
				var item = StartFromSlotBox.Items.
					OfType<object>()
					.FirstOrDefault(o => o.ToString() == value);

				if (item != null)
				{
					StartFromSlotBox.SelectedItem = item;
				}
				else
				{
					StartFromSlotBox.SelectedItem = null;
				}
			}
		}

		#endregion

		#region IToolForm Implementation

		public bool UpdateBefore { get { return true; } }

		public void NewUpdate(ToolFormUpdateType type) { }

		public void UpdateValues()
		{
			Update(fast: false);
		}

		public void FastUpdate()
		{
			Update(fast: true);
		}

		public void Restart()
		{
			if (_currentDomain == null ||
				MemoryDomains.Contains(_currentDomain))
			{
				_currentDomain = MemoryDomains.MainMemory;
				_bigEndian = _currentDomain.EndianType == MemoryDomain.Endian.Big;
				_dataSize = 1;
			}

			if (_isBotting)
			{
				StopBot();
			}
			else if (_replayMode)
			{
				FinishReplay();
			}


			if (_lastRom != GlobalWin.MainForm.CurrentlyOpenRom)
			{
				_lastRom = GlobalWin.MainForm.CurrentlyOpenRom;
				SetupControlsAndProperties();
			}
		}

		public bool AskSaveChanges()
		{
			return true;
		}

		#endregion

		#region Control Events

		#region FileMenu

		private void FileSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			SaveMenuItem.Enabled = !string.IsNullOrWhiteSpace(CurrentFileName);
		}

		private void RecentSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			RecentSubMenu.DropDownItems.Clear();
			RecentSubMenu.DropDownItems.AddRange(
				Settings.RecentBotFiles.RecentMenu(LoadFileFromRecent, true));
		}

		private void NewMenuItem_Click(object sender, EventArgs e)
		{
			CurrentFileName = "";
			_bestBotAttempt = null;

			ControlProbabilityPanel.Controls
				.OfType<GyroscopeBotControlsRow>()
				.ToList()
				.ForEach(cp => cp.Probability = 0);

			FrameLength = 0;
			MaximizeAddress = 0;
			
			StartFromSlotBox.SelectedIndex = 0;
			MainOperator.SelectedIndex = 0;
			
			MainBestRadio.Checked = true;
			MainValueNumeric.Value = 0;
		

			UpdateBestAttempt();
			UpdateComparisonBotAttempt();
		}

		private void OpenMenuItem_Click(object sender, EventArgs e)
		{
			var file = OpenFileDialog(
					CurrentFileName,
					PathManager.MakeAbsolutePath(Global.Config.PathEntries.ToolsPathFragment, null),
					"Bot files",
					"bot"
				);

			if (file != null)
			{
				LoadBotFile(file.FullName);
			}
		}

		private void SaveMenuItem_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(CurrentFileName))
			{
				SaveBotFile(CurrentFileName);
			}
		}

		private void SaveAsMenuItem_Click(object sender, EventArgs e)
		{
			var file = SaveFileDialog(
					CurrentFileName,
					PathManager.MakeAbsolutePath(Global.Config.PathEntries.ToolsPathFragment, null),
					"Bot files",
					"bot"
				);

			if (file != null)
			{
				SaveBotFile(file.FullName);
				_currentFileName = file.FullName;
			}
		}

		private void ExitMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion

		#region Options Menu

		private void OptionsSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			TurboWhileBottingMenuItem.Checked = Settings.TurboWhenBotting;
			BigEndianMenuItem.Checked = _bigEndian;
		}

		private void MemoryDomainsMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			MemoryDomainsMenuItem.DropDownItems.Clear();
			MemoryDomainsMenuItem.DropDownItems.AddRange(
				MemoryDomains.MenuItems(SetMemoryDomain, _currentDomain.Name)
				.ToArray());
		}

		private void BigEndianMenuItem_Click(object sender, EventArgs e)
		{
			_bigEndian ^= true;
		}

		private void DataSizeMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			_1ByteMenuItem.Checked = _dataSize == 1;
			_2ByteMenuItem.Checked = _dataSize == 2;
			_4ByteMenuItem.Checked = _dataSize == 4;
		}

		private void _1ByteMenuItem_Click(object sender, EventArgs e)
		{
			_dataSize = 1;
		}

		private void _2ByteMenuItem_Click(object sender, EventArgs e)
		{
			_dataSize = 2;
		}

		private void _4ByteMenuItem_Click(object sender, EventArgs e)
		{
			_dataSize = 4;
		}

		private void TurboWhileBottingMenuItem_Click(object sender, EventArgs e)
		{
			Settings.TurboWhenBotting ^= true;
		}

		#endregion

		private void RunBtn_Click(object sender, EventArgs e)
		{
			StartBot();
		}

		private void StopBtn_Click(object sender, EventArgs e)
		{
			StopBot();
		}

		private void ClearBestButton_Click(object sender, EventArgs e)
		{
			_bestBotAttempt = null;
			Attempts = 0;
			Frames = 0;
			UpdateBestAttempt();
			UpdateComparisonBotAttempt();
		}

		private void PlayBestButton_Click(object sender, EventArgs e)
		{
			StopBot();
			_replayMode = true;
			_dontUpdateValues = true;
			GlobalWin.MainForm.LoadQuickSave(SelectedSlot, false, true); // Triggers an UpdateValues call
			_dontUpdateValues = false;
			_startFrame = Emulator.Frame;
			SetNormalSpeed();
			UpdateBotStatusIcon();
			MessageLabel.Text = "Replaying";
			GlobalWin.MainForm.UnpauseEmulator();
		}

		private void FrameLengthNumeric_ValueChanged(object sender, EventArgs e)
		{
			AssessRunButtonStatus();
		}

		private void ClearStatsContextMenuItem_Click(object sender, EventArgs e)
		{
			Attempts = 0;
			Frames = 0;
		}

		#endregion

		#region Classes

		private class ControllerCommand
		{
			public ControllerCommand() { }
			public string type { get; set; }
			public Dictionary<string, bool> p1 { get; set; }
			public Dictionary<string, bool> p2 { get; set; }
			public int player_count { get; set; }
			public string savegamepath { get; set; }

		}

		private class BotAttempt
		{
			public BotAttempt()
			{
				Log = new List<string>();
			}

			public long Attempt { get; set; }
			public int Maximize { get; set; }
			public int TieBreak1 { get; set; }
			public int TieBreak2 { get; set; }
			public int TieBreak3 { get; set; }
			public byte ComparisonTypeMain { get; set; }
			public byte ComparisonTypeTie1 { get; set; }
			public byte ComparisonTypeTie2 { get; set; }
			public byte ComparisonTypeTie3 { get; set; }

			public List<string> Log { get; set; }
		}

		private class BotData
		{
			public BotData()
			{
				MainCompareToBest = true;
				TieBreaker1CompareToBest = true;
				TieBreaker2CompareToBest = true;
				TieBreaker3CompareToBest = true;
			}

			public BotAttempt Best { get; set; }
			public Dictionary<string, double> ControlProbabilities { get; set; }
			public int Maximize { get; set; }
			public int TieBreaker1 { get; set; }
			public int TieBreaker2 { get; set; }
			public int TieBreaker3 { get; set; }
			public byte ComparisonTypeMain { get; set; }
			public byte ComparisonTypeTie1 { get; set; }
			public byte ComparisonTypeTie2 { get; set; }
			public byte ComparisonTypeTie3 { get; set; }
			public bool MainCompareToBest { get; set; }
			public bool TieBreaker1CompareToBest { get; set; }
			public bool TieBreaker2CompareToBest { get; set; }
			public bool TieBreaker3CompareToBest { get; set; }
			public int MainCompareToValue { get; set; }
			public int TieBreaker1CompareToValue { get; set; }
			public int TieBreaker2CompareToValue { get; set; }
			public int TieBreaker3CompareToValue { get; set; }
			public int FrameLength { get; set; }
			public string FromSlot { get; set; }
			public long Attempts { get; set; }
			public long Frames { get; set; }

			public string MemoryDomain { get; set; }
			public bool BigEndian { get; set; }
			public int DataSize { get; set; }
		}

		#endregion

		#region File Handling

		private void LoadFileFromRecent(string path)
		{
			var result = LoadBotFile(path);
			if (!result)
			{
				Settings.RecentBotFiles.HandleLoadError(path);
			}
		}

		private bool LoadBotFile(string path)
		{
			var file = new FileInfo(path);
			if (!file.Exists)
			{
				return false;
			}

			var json = File.ReadAllText(path);
			var botData = (BotData)ConfigService.LoadWithType(json);

			_bestBotAttempt = botData.Best;

			var probabilityControls = ControlProbabilityPanel.Controls
					.OfType<GyroscopeBotControlsRow>()
					.ToList();

			foreach (var kvp in botData.ControlProbabilities)
			{
				var control = probabilityControls.Single(c => c.ButtonName == kvp.Key);
				control.Probability = kvp.Value;
			}

			MaximizeAddress = botData.Maximize;
			
			try
			{
				MainComparisonType = botData.ComparisonTypeMain;
				

				MainBestRadio.Checked = botData.MainCompareToBest;
				
				MainValueRadio.Checked = !botData.MainCompareToBest;
				

				MainValueNumeric.Value = botData.MainCompareToValue;
				
			}
			catch
			{
				MainComparisonType = 0;
				

				MainBestRadio.Checked = true;
				
				MainBestRadio.Checked = false;
			

				MainValueNumeric.Value = 0;
			
			}
			FrameLength = botData.FrameLength;
			FromSlot = botData.FromSlot;
			Attempts = botData.Attempts;
			Frames = botData.Frames;

			_currentDomain = !string.IsNullOrWhiteSpace(botData.MemoryDomain)
					? MemoryDomains[botData.MemoryDomain]
					: MemoryDomains.MainMemory;

			_bigEndian = botData.BigEndian;
			_dataSize = botData.DataSize > 0 ? botData.DataSize : 1;

			UpdateBestAttempt();
			UpdateComparisonBotAttempt();

			if (_bestBotAttempt != null)
			{
				PlayBestButton.Enabled = true;
			}

			CurrentFileName = path;
			Settings.RecentBotFiles.Add(CurrentFileName);
			MessageLabel.Text = Path.GetFileNameWithoutExtension(path) + " loaded";

			AssessRunButtonStatus();
			return true;
		}

		private void SaveBotFile(string path)
		{
			var data = new BotData
			{
				Best = _bestBotAttempt,
				ControlProbabilities = ControlProbabilities,
				Maximize = MaximizeAddress,
				
				ComparisonTypeMain = MainComparisonType,
				
				MainCompareToBest = MainBestRadio.Checked,
			
				MainCompareToValue = (int)MainValueNumeric.Value,
	
				FromSlot = FromSlot,
				FrameLength = FrameLength,
				Attempts = Attempts,
				Frames = Frames,
				MemoryDomain = _currentDomain.Name,
				BigEndian = _bigEndian,
				DataSize = _dataSize
			};

			var json = ConfigService.SaveWithType(data);

			File.WriteAllText(path, json);
			CurrentFileName = path;
			Settings.RecentBotFiles.Add(CurrentFileName);
			MessageLabel.Text = Path.GetFileName(CurrentFileName) + " saved";
		}

		#endregion

		private void SetupControlsAndProperties()
		{
			MaximizeAddressBox.SetHexProperties(MemoryDomains.MainMemory.Size);


			StartFromSlotBox.SelectedIndex = 0;

			int starty = 0;
			int accumulatedy = 0;
			int lineHeight = 30;
			int marginLeft = 15;
			int count = 0;

			ControlProbabilityPanel.Controls.Clear();

			foreach (var button in Emulator.ControllerDefinition.BoolButtons)
			{
				var control = new GyroscopeBotControlsRow
				{
					ButtonName = button,
					Probability = 0.0,
					Location = new Point(marginLeft, starty + accumulatedy),
					TabIndex = count + 1,
					ProbabilityChangedCallback = AssessRunButtonStatus
				};

				ControlProbabilityPanel.Controls.Add(control);
				accumulatedy += lineHeight;
				count++;
			}

			if (Settings.RecentBotFiles.AutoLoad)
			{
				LoadFileFromRecent(Settings.RecentBotFiles.MostRecent);
			}

			UpdateBotStatusIcon();
		}

		private void SetMemoryDomain(string name)
		{
			_currentDomain = MemoryDomains[name];
			_bigEndian = MemoryDomains[name].EndianType == MemoryDomain.Endian.Big;
		}

		private int GetRamvalue(int addr)
		{
			int val;
			switch (_dataSize)
			{
				default:
				case 1:
					val = _currentDomain.PeekByte(addr);
					break;
				case 2:
					val = _currentDomain.PeekUshort(addr, _bigEndian);
					break;
				case 4:
					val = (int)_currentDomain.PeekUint(addr, _bigEndian);
					break;
			}

			return val;
		}

		private void Update(bool fast)
		{
			if (_dontUpdateValues)
			{
				return;
			}

			if (_isBotting)
			{
				string command_type = "";
				do
				{
					// send over the current game state
					ControllerCommand command = SendEmulatorGameStateToController();
					command_type = command.type;
					// get a command back
					// act on the command
					if (command_type == "reset")
					{
						GlobalWin.MainForm.LoadState(command.savegamepath, Path.GetFileName(command.savegamepath));
					}
					else if (command_type == "processing")
					{
						// just do nothing, we're waiting for feedback from the controller.
						// XXX how do we tell the emulator to not advance the frame?

					}
					else
					{
						SetJoypadButtons(command.p1, 1);
						if (command.player_count == 2)
						{
							SetJoypadButtons(command.p2, 2);
						}
					}
				} while (command_type == "processing");

				// press the buttons if need be
				//PressButtons();
			}
		}

		private void FinishReplay()
		{
			GlobalWin.MainForm.PauseEmulator();
			_startFrame = 0;
			_replayMode = false;
			UpdateBotStatusIcon();
			MessageLabel.Text = "Replay stopped";
		}

		private bool IsBetter(BotAttempt comparison, BotAttempt current)
		{
			
			return true;
		}

		private bool TestValue(byte operation, int currentValue, int bestValue)
		{
			switch (operation)
			{
				case 0:
					return currentValue > bestValue;
				case 1:
					return currentValue >= bestValue;
				case 2:
					return currentValue == bestValue;
				case 3:
					return currentValue <= bestValue;
				case 4:
					return currentValue < bestValue;
			}
			return false;
		}

		private void UpdateBestAttempt()
		{
			if (_bestBotAttempt != null)
			{
				ClearBestButton.Enabled = true;
				BestAttemptNumberLabel.Text = _bestBotAttempt.Attempt.ToString();
				BestMaximizeBox.Text = _bestBotAttempt.Maximize.ToString();
	

				var sb = new StringBuilder();
				foreach (var logEntry in _bestBotAttempt.Log)
				{
					sb.AppendLine(logEntry);
				}
				BestAttemptLogLabel.Text = sb.ToString();
				PlayBestButton.Enabled = true;
			}
			else
			{
				ClearBestButton.Enabled = false;
				BestAttemptNumberLabel.Text = "";
				BestMaximizeBox.Text = "";
		
				BestAttemptLogLabel.Text = "";
				PlayBestButton.Enabled = false;
			}
		}

		private void PressButtons()
		{
			var rand = new Random((int)DateTime.Now.Ticks);

			var buttonLog = new Dictionary<string, bool>();

			foreach (var button in Emulator.ControllerDefinition.BoolButtons)
			{
				double probability = _cachedControlProbabilities[button];
				bool pressed = !(rand.Next(100) < probability);

				buttonLog.Add(button, pressed);
				Global.ClickyVirtualPadController.SetBool(button, pressed);
			}

			_currentBotAttempt.Log.Add(_logGenerator.GenerateLogEntry());
		}

		private void StartBot()
		{
			if (!CanStart())
			{
				MessageBox.Show("Unable to run with current settings");
				return;
			}

			_isBotting = true;
			ControlsBox.Enabled = false;
			StartFromSlotBox.Enabled = false;
			RunBtn.Visible = false;
			StopBtn.Visible = true;
			GoalGroupBox.Enabled = false;
			_currentBotAttempt = new BotAttempt { Attempt = Attempts };
			this.client = CreateTCPClient(this.IP, this.port);

			if (Global.MovieSession.Movie.IsRecording)
			{
				_oldCountingSetting = Global.MovieSession.Movie.IsCountingRerecords;
				Global.MovieSession.Movie.IsCountingRerecords = false;
			}

			_dontUpdateValues = true;
			GlobalWin.MainForm.LoadQuickSave(SelectedSlot, false, true); // Triggers an UpdateValues call
			_dontUpdateValues = false;

			_targetFrame = Emulator.Frame + (int)FrameLengthNumeric.Value;
			Global.Config.SoundEnabled = false;
			GlobalWin.MainForm.UnpauseEmulator();
			SetMaxSpeed();
			GlobalWin.MainForm.ClickSpeedItem(6399);
			//if (Settings.TurboWhenBotting)
			//{
			//	SetMaxSpeed();
			//}

			UpdateBotStatusIcon();
			MessageLabel.Text = "Running...";
			_cachedControlProbabilities = ControlProbabilities;
			_logGenerator = Global.MovieSession.LogGeneratorInstance();
			_logGenerator.SetSource(Global.ClickyVirtualPadController);
		}

		private bool CanStart()
		{
		

			return true;
		}

		private void StopBot()
		{
			RunBtn.Visible = true;
			StopBtn.Visible = false;
			_isBotting = false;
			_targetFrame = 0;
			ControlsBox.Enabled = true;
			StartFromSlotBox.Enabled = true;
			_targetFrame = 0;
			_currentBotAttempt = null;
			GoalGroupBox.Enabled = true;

			if (Global.MovieSession.Movie.IsRecording)
			{
				Global.MovieSession.Movie.IsCountingRerecords = _oldCountingSetting;
			}

			GlobalWin.MainForm.PauseEmulator();
			SetNormalSpeed();
			UpdateBotStatusIcon();
			MessageLabel.Text = "Bot stopped";
		}

		private void UpdateBotStatusIcon()
		{
			if (_replayMode)
			{
				BotStatusButton.Image = Properties.Resources.Play;
				BotStatusButton.ToolTipText = "Replaying best result";
			}
			else if (_isBotting)
			{
				BotStatusButton.Image = Properties.Resources.RecordHS;
				BotStatusButton.ToolTipText = "Botting in progress";
			}
			else
			{
				BotStatusButton.Image = Properties.Resources.Pause;
				BotStatusButton.ToolTipText = "Bot is currently not running";
			}
		}

		private void SetMaxSpeed()
		{
			GlobalWin.MainForm.Unthrottle();
		}

		private void SetNormalSpeed()
		{
			GlobalWin.MainForm.Throttle();
		}

		private void AssessRunButtonStatus()
		{
			RunBtn.Enabled =
				FrameLength > 0
				&& !string.IsNullOrWhiteSpace(MaximizeAddressBox.Text)
				&& ControlProbabilities.Any(kvp => kvp.Value > 0);
		}

		/// <summary>
		/// Updates comparison bot attempt with current best bot attempt values for values where the "best" radio button is selected
		/// </summary>
		private void UpdateComparisonBotAttempt()
		{
			if(_bestBotAttempt == null)
			{
				if (MainBestRadio.Checked)
				{
					_comparisonBotAttempt.Maximize = 0;
				}

			}
			else
			{
				if (MainBestRadio.Checked && _bestBotAttempt.Maximize != _comparisonBotAttempt.Maximize)
				{
					_comparisonBotAttempt.Maximize = _bestBotAttempt.Maximize;
				}
				
			}
		}

		private void MainBestRadio_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton radioButton = (RadioButton)sender;
			if (radioButton.Checked)
			{
				this.MainValueNumeric.Enabled = false;
				_comparisonBotAttempt.Maximize = _bestBotAttempt == null ? 0 : _bestBotAttempt.Maximize;
			}
		}
		

		private void MainValueRadio_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton radioButton = (RadioButton)sender;
			if (radioButton.Checked)
			{
				this.MainValueNumeric.Enabled = true;
				_comparisonBotAttempt.Maximize = (int)this.MainValueNumeric.Value;
			}
		}
		
		private void MainValueNumeric_ValueChanged(object sender, EventArgs e)
		{
			NumericUpDown numericUpDown = (NumericUpDown)sender;
			this._comparisonBotAttempt.Maximize = (int)numericUpDown.Value;
		}

		private void TieBreak1Numeric_ValueChanged(object sender, EventArgs e)
		{
			NumericUpDown numericUpDown = (NumericUpDown)sender;
			this._comparisonBotAttempt.TieBreak1 = (int)numericUpDown.Value;
		}

		private void TieBreak2Numeric_ValueChanged(object sender, EventArgs e)
		{
			NumericUpDown numericUpDown = (NumericUpDown)sender;
			this._comparisonBotAttempt.TieBreak2 = (int)numericUpDown.Value;
		}

		private void TieBreak3Numeric_ValueChanged(object sender, EventArgs e)
		{
			NumericUpDown numericUpDown = (NumericUpDown)sender;
			this._comparisonBotAttempt.TieBreak3 = (int)numericUpDown.Value;
		}

	}
}
