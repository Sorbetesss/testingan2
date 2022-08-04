﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BizHawk.Common.NumberExtensions;

namespace BizHawk.Client.EmuHawk
{
	public partial class GenericDebugger
	{
		private readonly List<DisasmOp> _disassemblyLines = new List<DisasmOp>();
		private int _pcRegisterSize = 4;
		private uint _currentDisassemblerAddress;

		private class DisasmOp
		{
			public DisasmOp(uint address, int size, string mnemonic)
			{
				Address = address;
				Size = size;
				Mnemonic = mnemonic;
			}

			public uint Address { get; }
			public int Size { get; }
			public string Mnemonic { get; }
		}

		private long BusMaxValue => MemoryDomains.SystemBus.Size;

		private void UpdatePC()
		{
			if (CanDisassemble)
			{
				_currentDisassemblerAddress = (uint)PCRegister.Value;
			}
		}

		private void UpdateDisassembler()
		{
			if (CanDisassemble)
			{
				Disassemble();
				SetDisassemblerItemCount();
			}
		}
		
		private void Disassemble()
		{
			int lineCount = DisassemblerView.RowCount * 6 + 2;

			_disassemblyLines.Clear();
			uint a = _currentDisassemblerAddress;
			for (int i = 0; i <= lineCount; ++i)
			{
				string line = Disassembler.Disassemble(MemoryDomains.SystemBus, a, out var advance);
				_disassemblyLines.Add(new DisasmOp(a, advance, line));
				a += (uint)advance;
				if (a > BusMaxValue)
				{
					break;
				}
			}
		}

		private void DisassemblerView_QueryItemText(int index, RollColumn column, out string text, ref int offsetX, ref int offsetY)
		{
			text = "";

			if (index < _disassemblyLines.Count)
			{
				if (column.Name == AddressColumnName)
				{
					text = _disassemblyLines[index].Address.ToHexString(_pcRegisterSize);
				}
				else if (column.Name == InstructionColumnName)
				{
					text = _disassemblyLines[index].Mnemonic;
				}
			}
		}

		private void DisassemblerView_QueryItemBkColor(int index, RollColumn column, ref Color color)
		{
			if (_disassemblyLines.Any() && index < _disassemblyLines.Count)
			{
				if (_disassemblyLines[index].Address == _currentDisassemblerAddress)
				{
					color = Color.LightCyan;
				}
			}
		}


		private void DecrementCurrentAddress()
		{
			if (_currentDisassemblerAddress == 0)
			{
				return;
			}

			uint newaddress = _currentDisassemblerAddress;
			
			while (true)
			{
				Disassembler.Disassemble(MemoryDomains.SystemBus, newaddress, out var bytestoadvance);
				if (newaddress + bytestoadvance == _currentDisassemblerAddress)
				{
					break;
				}

				newaddress--;

				if (newaddress < 0)
				{
					newaddress = 0;
					break;
				}

				// Just in case
				if (_currentDisassemblerAddress - newaddress > 5)
				{
					newaddress = _currentDisassemblerAddress - 1;
					break;
				}
			}

			_currentDisassemblerAddress = newaddress;
		}

		private void IncrementCurrentAddress()
		{
			_currentDisassemblerAddress += (uint) _disassemblyLines[0].Size;
			if (_currentDisassemblerAddress >= BusMaxValue)
			{
				_currentDisassemblerAddress = (uint)(BusMaxValue - 1);
			}
		}

//		private bool _blockScroll;
		private void DisassemblerView_Scroll(object sender, EventArgs e)
		{
//			if (_blockScroll) { return; }

			// is this still needed?
		}

		private void SetDisassemblerItemCount()
		{
			if (DisassemblerView.VisibleRows > 0)
			{
				DisassemblerView.RowCount = DisassemblerView.VisibleRows * 6 + 2;
			}		
		}

		private void DisassemblerView_SizeChanged(object sender, EventArgs e)
		{
			SetDisassemblerItemCount();
			if (CanDisassemble)
			{
				Disassemble();
			}
		}

		private void SmallIncrement()
		{
			IncrementCurrentAddress();
			Disassemble();
			DisassemblerView.Refresh();
		}

		private void SmallDecrement()
		{
			DecrementCurrentAddress();
			Disassemble();
			DisassemblerView.Refresh();
		}

		private void DisassemblerView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.IsCtrl(Keys.C))
			{
				CopySelectedDisassembler();
			}
			else if (e.IsPressed(Keys.OemCloseBrackets))
			{
				SmallIncrement();
			}
			else if (e.IsPressed(Keys.OemOpenBrackets))
			{
				SmallDecrement();
			}
			else if (e.IsShift(Keys.OemCloseBrackets))
			{
				SmallIncrement();
				SmallIncrement();
				SmallIncrement();
			}
			else if (e.IsShift(Keys.OemOpenBrackets))
			{
				SmallDecrement();
				SmallDecrement();
				SmallDecrement();
			}
		}

		private void CopySelectedDisassembler()
		{
			if (!DisassemblerView.AnyRowsSelected) return;
			StringBuilder blob = new();
			foreach (var line in DisassemblerView.SelectedRows.Select(i => _disassemblyLines[i]))
			{
				blob.AppendFormat("{0} {1}\n", line.Address.ToHexString(_pcRegisterSize), line.Mnemonic);
			}
			Clipboard.SetDataObject(blob.ToString());
		}

		private void OnPauseChanged(bool isPaused)
		{
			if (isPaused) FullUpdate();
		}

		private void DisassemblerContextMenu_Opening(object sender, EventArgs e)
		{
			AddBreakpointContextMenuItem.Enabled = DisassemblerView.AnyRowsSelected;
		}

		private void AddBreakpointContextMenuItem_Click(object sender, EventArgs e)
		{
			if (DisassemblerView.AnyRowsSelected)
			{
				var line = _disassemblyLines[DisassemblerView.FirstSelectedRowIndex];
				BreakPointControl1.AddBreakpoint(line.Address, 0xFFFFFFFF, Emulation.Common.MemoryCallbackType.Execute);
			}
		}
	}
}
