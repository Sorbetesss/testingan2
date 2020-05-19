﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Diagnostics;

using BizHawk.Common.NumberExtensions;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common
{
	/// <summary>
	/// This class holds a watch i.e. something inside a <see cref="MemoryDomain"/> identified by an address
	/// with a specific size (8, 16 or 32bits).
	/// This is an abstract class
	/// </summary>
	[DebuggerDisplay("Note={Notes}, Value={ValueString}")]
	public abstract class Watch
		: IEquatable<Watch>,
		IEquatable<Cheat>,
		IComparable<Watch>
	{
		private MemoryDomain _domain;
		private DisplayType _type;

		/// <summary>
		/// Initializes a new instance of the <see cref="Watch"/> class
		/// </summary>
		/// <param name="domain"><see cref="MemoryDomain"/> where you want to track</param>
		/// <param name="address">The address you want to track</param>
		/// <param name="size">A <see cref="WatchSize"/> (byte, word, double word)</param>
		/// <param name="type">How you you want to display the value See <see cref="DisplayType"/></param>
		/// <param name="bigEndian">Specify the endianess. true for big endian</param>
		/// <param name="note">A custom note about the <see cref="Watch"/></param>
		/// <exception cref="ArgumentException">Occurs when a <see cref="DisplayType"/> is incompatible with the <see cref="WatchSize"/></exception>
		protected Watch(MemoryDomain domain, long address, WatchSize size, DisplayType type, bool bigEndian, string note)
		{
			if (IsDisplayTypeAvailable(type))
			{
				_domain = domain;
				Address = address;
				Size = size;
				_type = type;
				BigEndian = bigEndian;
				Notes = note;
			}
			else
			{
				throw new ArgumentException($"{nameof(DisplayType)} {type} is invalid for this type of {nameof(Watch)}", nameof(type));
			}
		}

		/// <summary>
		/// Generate sa <see cref="Watch"/> from a given string
		/// String is tab separate
		/// </summary>
		/// <param name="line">Entire string, tab separated for each value Order is:
		/// <list type="number">
		/// <item>
		/// <term>0x00</term>
		/// <description>Address in hexadecimal</description>
		/// </item>
		/// <item>
		/// <term>b,w or d</term>
		/// <description>The <see cref="WatchSize"/>, byte, word or double word</description>
		/// <term>s, u, h, b, 1, 2, 3, f</term>
		/// <description>The <see cref="DisplayType"/> signed, unsigned,etc...</description>
		/// </item>
		/// <item>
		/// <term>0 or 1</term>
		/// <description>Big endian or not</description>
		/// </item>
		/// <item>
		/// <term>RDRAM,ROM,...</term>
		/// <description>The <see cref="IMemoryDomains"/></description>
		/// </item>
		/// <item>
		/// <term>Plain text</term>
		/// <description>Notes</description>
		/// </item>
		/// </list>
		/// </param>
		/// <param name="domains"><see cref="Watch"/>'s memory domain</param>
		/// <returns>A brand new <see cref="Watch"/></returns>
		public static Watch FromString(string line, IMemoryDomains domains)
		{
			string[] parts = line.Split(new[] { '\t' }, 6);

			if (parts.Length < 6)
			{
				if (parts.Length >= 3 && parts[2] == "_")
				{
					return SeparatorWatch.Instance;
				}

				return null;
			}

			if (long.TryParse(parts[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var address))
			{
				WatchSize size = SizeFromChar(parts[1][0]);
				DisplayType type = DisplayTypeFromChar(parts[2][0]);
				bool bigEndian = parts[3] != "0";
				MemoryDomain domain = domains[parts[4]];
				string notes = parts[5].Trim('\r', '\n');

				return GenerateWatch(
					domain,
					address,
					size,
					type,
					bigEndian,
					notes);
			}

			return null;
		}

		/// <summary>
		/// Generates a new <see cref="Watch"/> instance
		/// Can be either <see cref="ByteWatch"/>, <see cref="WordWatch"/>, <see cref="DWordWatch"/> or <see cref="SeparatorWatch"/>
		/// </summary>
		/// <param name="domain">The <see cref="MemoryDomain"/> where you want to watch</param>
		/// <param name="address">The address into the <see cref="MemoryDomain"/></param>
		/// <param name="size">The size</param>
		/// <param name="type">How the watch will be displayed</param>
		/// <param name="bigEndian">Endianess (true for big endian)</param>
		/// <param name="note">A custom note about the <see cref="Watch"/></param>
		/// <param name="value">The current watch value</param>
		/// <param name="prev">Previous value</param>
		/// <param name="changeCount">Number of changes occurs in current <see cref="Watch"/></param>
		/// <returns>New <see cref="Watch"/> instance. True type is depending of size parameter</returns>
		public static Watch GenerateWatch(MemoryDomain domain, long address, WatchSize size, DisplayType type, bool bigEndian, string note = "", long value = 0, long prev = 0, int changeCount = 0)
		{
			return size switch
			{
				WatchSize.Separator => SeparatorWatch.NewSeparatorWatch(note),
				WatchSize.Byte => new ByteWatch(domain, address, type, bigEndian, note, (byte) value, (byte) prev, changeCount),
				WatchSize.Word => new WordWatch(domain, address, type, bigEndian, note, (ushort) value, (ushort) prev, changeCount),
				WatchSize.DWord => new DWordWatch(domain, address, type, bigEndian, note, (uint) value, (uint) prev, changeCount),
				_ => SeparatorWatch.NewSeparatorWatch(note)
			};
		}

		/// <summary>
		/// Equality operator between two <see cref="Watch"/>
		/// </summary>
		/// <param name="a">First watch</param>
		/// <param name="b">Second watch</param>
		/// <returns>True if both watch are equals; otherwise, false</returns>
		public static bool operator ==(Watch a, Watch b)
		{
			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
			{
				return false;
			}

			if (ReferenceEquals(a, b))
			{
				return true;
			}

			return a.Equals(b);
		}

		/// <summary>
		/// Equality operator between a <see cref="Watch"/> and a <see cref="Cheat"/>
		/// </summary>
		/// <param name="a">The watch</param>
		/// <param name="b">The cheat</param>
		/// <returns>True if they are equals; otherwise, false</returns>
		public static bool operator ==(Watch a, Cheat b)
		{
			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
			{
				return false;
			}

			return a.Equals(b);
		}

		/// <summary>
		/// Inequality operator between two <see cref="Watch"/>
		/// </summary>
		/// <param name="a">First watch</param>
		/// <param name="b">Second watch</param>
		/// <returns>True if both watch are different; otherwise, false</returns>
		public static bool operator !=(Watch a, Watch b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Inequality operator between a <see cref="Watch"/> and a <see cref="Cheat"/>
		/// </summary>
		/// <param name="a">The watch</param>
		/// <param name="b">The cheat</param>
		/// <returns>True if they are different; otherwise, false</returns>
		public static bool operator !=(Watch a, Cheat b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Compare two <see cref="Watch"/> together
		/// </summary>
		/// <param name="a">First <see cref="Watch"/></param>
		/// <param name="b">Second <see cref="Watch"/></param>
		/// <returns>True if first is lesser than b; otherwise, false</returns>
		/// <exception cref="InvalidOperationException">Occurs when you try to compare two <see cref="Watch"/> throughout different <see cref="MemoryDomain"/></exception>
		public static bool operator <(Watch a, Watch b)
		{
			return a.CompareTo(b) < 0;
		}

		/// <summary>
		/// Compare two <see cref="Watch"/> together
		/// </summary>
		/// <param name="a">First <see cref="Watch"/></param>
		/// <param name="b">Second <see cref="Watch"/></param>
		/// <returns>True if first is greater than b; otherwise, false</returns>
		/// <exception cref="InvalidOperationException">Occurs when you try to compare two <see cref="Watch"/> throughout different <see cref="MemoryDomain"/></exception>
		public static bool operator >(Watch a, Watch b)
		{
			return a.CompareTo(b) > 0;
		}

		/// <summary>
		/// Compare two <see cref="Watch"/> together
		/// </summary>
		/// <param name="a">First <see cref="Watch"/></param>
		/// <param name="b">Second <see cref="Watch"/></param>
		/// <returns>True if first is lesser or equals to b; otherwise, false</returns>
		/// <exception cref="InvalidOperationException">Occurs when you try to compare two <see cref="Watch"/> throughout different <see cref="MemoryDomain"/></exception>
		public static bool operator <=(Watch a, Watch b)
		{
			return a.CompareTo(b) <= 0;
		}

		/// <summary>
		/// Compare two <see cref="Watch"/> together
		/// </summary>
		/// <param name="a">First <see cref="Watch"/></param>
		/// <param name="b">Second <see cref="Watch"/></param>
		/// <returns>True if first is greater or equals to b; otherwise, false</returns>
		/// <exception cref="InvalidOperationException">Occurs when you try to compare two <see cref="Watch"/> throughout different <see cref="MemoryDomain"/></exception>
		public static bool operator >=(Watch a, Watch b)
		{
			return a.CompareTo(b) >= 0;
		}

		/// <summary>
		/// Gets a list a <see cref="DisplayType"/> that can be used for this <see cref="Watch"/>
		/// </summary>
		/// <returns>An enumeration that contains all valid <see cref="DisplayType"/></returns>
		public abstract IEnumerable<DisplayType> AvailableTypes();

		/// <summary>
		/// Resets the previous value; set it to the current one
		/// </summary>
		public abstract void ResetPrevious();

		/// <summary>
		/// Updates the Watch (read it from <see cref="MemoryDomain"/>
		/// </summary>
		public abstract void Update();

		protected byte GetByte()
		{
			if (_domain.Size == 0)
			{
				return _domain.PeekByte(Address);
			}

			return _domain.PeekByte(Address % _domain.Size);
		}

		protected ushort GetWord()
		{
			if (_domain.Size == 0)
			{
				return _domain.PeekUshort(Address, BigEndian);
			}

			return _domain.PeekUshort(Address % _domain.Size, BigEndian); // TODO: % size still isn't correct since it could be the last byte of the domain
		}

		protected uint GetDWord()
		{
			if (_domain.Size == 0)
			{
				return _domain.PeekUint(Address, BigEndian); // TODO: % size still isn't correct since it could be the last byte of the domain
			}

			return _domain.PeekUint(Address % _domain.Size, BigEndian); // TODO: % size still isn't correct since it could be the last byte of the domain
		}

		protected void PokeByte(byte val)
		{
			if (_domain.Size == 0)
			{
				_domain.PokeByte(Address, val);
			}
			else
			{
				_domain.PokeByte(Address % _domain.Size, val);
			}
		}

		protected void PokeWord(ushort val)
		{
			if (_domain.Size == 0)
			{
				_domain.PokeUshort(Address, val, BigEndian); // TODO: % size still isn't correct since it could be the last byte of the domain
			}
			else
			{
				_domain.PokeUshort(Address % _domain.Size, val, BigEndian); // TODO: % size still isn't correct since it could be the last byte of the domain
			}
		}

		protected void PokeDWord(uint val)
		{
			if (_domain.Size == 0)
			{
				_domain.PokeUint(Address, val, BigEndian); // TODO: % size still isn't correct since it could be the last byte of the domain
			}
			else
			{
				_domain.PokeUint(Address % _domain.Size, val, BigEndian); // TODO: % size still isn't correct since it could be the last byte of the domain
			}
		}

		/// <summary>
		/// Sets the number of changes to 0
		/// </summary>
		public void ClearChangeCount()
		{
			ChangeCount = 0;
		}

		/// <summary>
		/// Determines if this <see cref="Watch"/> is equals to another
		/// </summary>
		/// <param name="other">The <see cref="Watch"/> to compare</param>
		/// <returns>True if both object are equals; otherwise, false</returns>
		public bool Equals(Watch other)
		{
			if (ReferenceEquals(other, null))
			{
				return false;
			}

			return _domain == other._domain &&
				Address == other.Address &&
				Size == other.Size;
		}

		/// <summary>
		/// Determines if this <see cref="Watch"/> is equals to an instance of <see cref="Cheat"/>
		/// </summary>
		/// <param name="other">The <see cref="Cheat"/> to compare</param>
		/// <returns>True if both object are equals; otherwise, false</returns>
		public bool Equals(Cheat other)
		{
			return !ReferenceEquals(other, null)
				&& _domain == other.Domain
				&& Address == other.Address
				&& Size == other.Size;
		}

		/// <summary>
		/// Compares two <see cref="Watch"/> together and determine which one comes first.
		/// First we look the address and then the size
		/// </summary>
		/// <param name="other">The other <see cref="Watch"/> to compare to</param>
		/// <returns>0 if they are equals, 1 if the other is greater, -1 if the other is lesser</returns>
		/// <exception cref="InvalidOperationException">Occurs when you try to compare two <see cref="Watch"/> throughout different <see cref="MemoryDomain"/></exception>
		public int CompareTo(Watch other)
		{
			if (_domain != other._domain)
			{
				throw new InvalidOperationException("Watch cannot be compared through different domain");
			}

			if (Equals(other))
			{
				return 0;
			}

			if (Address.Equals(other.Address))
			{
				return ((int)Size).CompareTo((int)other.Size);
			}

			return Address.CompareTo(other.Address);
		}

		/// <summary>
		/// Determines if this object is Equals to another
		/// </summary>
		/// <param name="obj">The object to compare</param>
		/// <returns>True if both object are equals; otherwise, false</returns>
		public override bool Equals(object obj)
		{
			if (obj is Watch watch)
			{
				return Equals(watch);
			}

			if (obj is Cheat cheat)
			{
				return Equals(cheat);
			}

			return base.Equals(obj);
		}

		/// <summary>
		/// Hash the current watch and gets a unique value
		/// </summary>
		/// <returns><see cref="int"/> that can serves as a unique representation of current Watch</returns>
		public override int GetHashCode()
		{
			return Domain.GetHashCode() + (int)Address;
		}

		/// <summary>
		/// Determines if the specified <see cref="DisplayType"/> can be
		/// used for the current <see cref="Watch"/>
		/// </summary>
		/// <param name="type"><see cref="DisplayType"/> you want to check</param>
		public bool IsDisplayTypeAvailable(DisplayType type)
		{
			return AvailableTypes().Any(d => d == type);
		}

		/// <summary>
		/// Transforms the current instance into a string
		/// </summary>
		/// <returns>A <see cref="string"/> representation of the current <see cref="Watch"/></returns>
		public override string ToString()
		{
			return $"{(Domain == null && Address == 0 ? "0" : Address.ToHexString((Domain?.Size ?? 0xFF - 1).NumHexDigits()))}\t{SizeAsChar}\t{TypeAsChar}\t{Convert.ToInt32(BigEndian)}\t{Domain?.Name}\t{Notes.Trim('\r', '\n')}";
		}

		/// <summary>
		/// Transform the current instance into a displayable (short representation) string
		/// It's used by the "Display on screen" option in the RamWatch window
		/// </summary>
		/// <returns>A well formatted string representation</returns>
		public virtual string ToDisplayString() => $"{Notes}: {ValueString}";

		/// <summary>
		/// Gets a string representation of difference
		/// between current value and the previous one
		/// </summary>
		public abstract string Diff { get; }

		/// <summary>
		/// Gets the maximum possible value
		/// </summary>
		public abstract uint MaxValue { get; }

		/// <summary>
		/// Gets the current value
		/// </summary>
		public abstract int Value { get; }

		/// <summary>
		/// Gets a string representation of the current value
		/// </summary>
		public abstract string ValueString { get; }

		/// <summary>
		/// Try to sets the value into the <see cref="MemoryDomain"/>
		/// at the current <see cref="Watch"/> address
		/// </summary>
		/// <param name="value">Value to set</param>
		/// <returns>True if value successfully sets; otherwise, false</returns>
		public abstract bool Poke(string value);

		/// <summary>
		/// Gets the previous value
		/// </summary>
		public abstract int Previous { get; }

		/// <summary>
		/// Gets a string representation of the previous value
		/// </summary>
		public abstract string PreviousStr { get; }

		/// <summary>
		/// Gets the address in the <see cref="MemoryDomain"/>
		/// </summary>
		public long Address { get; }

		private string AddressFormatStr => _domain != null
			? $"X{(_domain.Size - 1).NumHexDigits()}"
			: "";

		/// <summary>
		/// Gets the address in the <see cref="MemoryDomain"/> formatted as string
		/// </summary>
		public string AddressString => Address.ToString(AddressFormatStr);

		/// <summary>
		/// Gets or sets a value indicating the endianess of current <see cref="Watch"/>
		/// True for big endian, false for little endian
		/// </summary>
		public bool BigEndian { get; set; }

		/// <summary>
		/// Gets or sets the number of times that value of current <see cref="Watch"/> value has changed
		/// </summary>
		public int ChangeCount { get; protected set; }

		/// <summary>
		/// Gets or sets the way current <see cref="Watch"/> is displayed
		/// </summary>
		/// <exception cref="ArgumentException">Occurs when a <see cref="DisplayType"/> is incompatible with the <see cref="WatchSize"/></exception>
		public DisplayType Type
		{
			get => _type;
			set
			{
				if (IsDisplayTypeAvailable(value))
				{
					_type = value;
				}
				else
				{
					throw new ArgumentException($"DisplayType {value} is invalid for this type of Watch");
				}
			}
		}

		/// <value>the domain of <see cref="Address"/></value>
		/// <exception cref="InvalidOperationException">(from setter) <paramref name="value"/> does not have the same name as this property's value</exception>
		public MemoryDomain Domain
		{
			get => _domain;
			internal set
			{
				if (value != null && _domain.Name == value.Name)
				{
					_domain = value;
				}
				else
				{
					throw new InvalidOperationException("You cannot set a different domain to a watch on the fly");
				}
			}
		}

		/// <summary>
		/// Gets a value that defined if the current <see cref="Watch"/> is actually a <see cref="SeparatorWatch"/>
		/// </summary>
		public bool IsSeparator => this is SeparatorWatch;

		/// <summary>
		/// Gets or sets notes for current <see cref="Watch"/>
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// Gets the current size of the watch
		/// </summary>
		public WatchSize Size { get; }

		// TODO: Replace all the following stuff by implementing ISerializable
		public static string DisplayTypeToString(DisplayType type)
		{
			return type switch
			{
				DisplayType.FixedPoint_12_4 => "Fixed Point 12.4",
				DisplayType.FixedPoint_20_12 => "Fixed Point 20.12",
				DisplayType.FixedPoint_16_16 => "Fixed Point 16.16",
				_ => type.ToString()
			};
		}

		public static DisplayType StringToDisplayType(string name)
		{
			return name switch
			{
				"Fixed Point 12.4" => DisplayType.FixedPoint_12_4,
				"Fixed Point 20.12" => DisplayType.FixedPoint_20_12,
				"Fixed Point 16.16" => DisplayType.FixedPoint_16_16,
				_ => (DisplayType) Enum.Parse(typeof(DisplayType), name)
			};
		}

		public char SizeAsChar
		{
			get
			{
				return Size switch
				{
					WatchSize.Separator => 'S',
					WatchSize.Byte => 'b',
					WatchSize.Word => 'w',
					WatchSize.DWord => 'd',
					_ => 'S'
				};
			}
		}

		public static WatchSize SizeFromChar(char c)
		{
			return c switch
			{
				'S' => WatchSize.Separator,
				'b' => WatchSize.Byte,
				'w' => WatchSize.Word,
				'd' => WatchSize.DWord,
				_ => WatchSize.Separator
			};
		}

		public char TypeAsChar
		{
			get
			{
				return Type switch
				{
					DisplayType.Separator => '_',
					DisplayType.Unsigned => 'u',
					DisplayType.Signed => 's',
					DisplayType.Hex => 'h',
					DisplayType.Binary => 'b',
					DisplayType.FixedPoint_12_4 => '1',
					DisplayType.FixedPoint_20_12 => '2',
					DisplayType.FixedPoint_16_16 => '3',
					DisplayType.Float => 'f',
					_ => '_'
				};
			}
		}

		public static DisplayType DisplayTypeFromChar(char c)
		{
			return c switch
			{
				'_' => DisplayType.Separator,
				'u' => DisplayType.Unsigned,
				's' => DisplayType.Signed,
				'h' => DisplayType.Hex,
				'b' => DisplayType.Binary,
				'1' => DisplayType.FixedPoint_12_4,
				'2' => DisplayType.FixedPoint_20_12,
				'3' => DisplayType.FixedPoint_16_16,
				'f' => DisplayType.Float,
				_ => DisplayType.Separator
			};
		}
	}
}
