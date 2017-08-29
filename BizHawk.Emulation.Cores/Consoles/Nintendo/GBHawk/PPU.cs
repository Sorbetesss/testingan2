﻿using System;
using BizHawk.Emulation.Common;
using BizHawk.Common.NumberExtensions;
using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.GBHawk
{
	public class PPU
	{
		public GBHawk Core { get; set; }

		//public byte BGP_l;

		// register variables
		public byte LCDC;
		public byte STAT;
		public byte scroll_y;
		public byte scroll_x;
		public byte LY;
		public byte LY_inc;
		public byte LYC;
		public byte DMA_addr;
		public byte BGP;
		public byte obj_pal_0;
		public byte obj_pal_1;
		public byte window_y;
		public byte window_x;
		public bool DMA_start;
		public int DMA_clock;
		public int DMA_inc;
		public byte DMA_byte;

		// state variables
		public int cycle;
		public bool LYC_INT;
		public bool HBL_INT;
		public bool VBL_INT;
		public bool OAM_INT;
		public bool LCD_was_off;
		public bool stat_line;
		public bool stat_line_old;
		public bool hbl_set_once;
		// OAM scan
		public bool OAM_access;
		public int OAM_scan_index;
		public int SL_sprites_index;
		public int[] SL_sprites = new int[40];
		public int write_sprite;
		// render
		public bool VRAM_access;
		public int read_case;
		public int internal_cycle;
		public int y_tile;
		public int y_scroll_offset;
		public int x_tile;
		public int x_scroll_offset;
		public int tile_byte;
		public int sprite_fetch_cycles;
		public bool fetch_sprite;
		public int temp_fetch;
		public int tile_inc;
		public bool pre_render;
		public byte[] tile_data = new byte[2];
		public byte[] tile_data_latch = new byte[2];
		public int latch_counter;
		public bool latch_new_data;
		public int render_counter;
		public int render_offset;
		public int pixel_counter;
		public int pixel;
		public byte[] sprite_data = new byte[2];
		public byte[] sprite_sel = new byte[2];
		public int sl_use_index;
		public bool no_sprites;
		public int sprite_fetch_index;
		public int[] SL_sprites_ordered = new int[40]; // (x_end, data_low, data_high, attr)
		public int index_used;
		public int sprite_ordered_index;
		public int bottom_index;

		public byte ReadReg(int addr)
		{
			byte ret = 0;

			switch (addr)
			{		
				case 0xFF40: ret = LCDC;					break; // LCDC
				case 0xFF41: ret = STAT;					break; // STAT
				case 0xFF42: ret = scroll_y;				break; // SCY
				case 0xFF43: ret = scroll_x;				break; // SCX
				case 0xFF44: ret = LY;						break; // LY
				case 0xFF45: ret = LYC;						break; // LYC
				case 0xFF46: /*ret = DMA_addr; */			break; // DMA (not readable?)
				case 0xFF47: ret = BGP;						break; // BGP
				case 0xFF48: ret = obj_pal_0;				break; // OBP0
				case 0xFF49: ret = obj_pal_1;				break; // OBP1
				case 0xFF4A: ret = window_y;				break; // WY
				case 0xFF4B: ret = window_x;				break; // WX
			}

			return ret;
		}

		public void WriteReg(int addr, byte value)
		{
			switch (addr)
			{
				case 0xFF40: // LCDC
					LCDC = value;
					break; 
				case 0xFF41: // STAT
					STAT = (byte)((value & 0xF8) | (STAT & 7) | 0x80);
					break; 
				case 0xFF42: // SCY
					scroll_y = value;
					break; 
				case 0xFF43: // SCX
					scroll_x = value;
					// calculate the column number of the tile to start with
					x_tile = (int)Math.Floor((float)(scroll_x) / 8);
					break; 
				case 0xFF44: // LY
					LY = 0; /*reset*/
					break;
				case 0xFF45:  // LYC
					LYC = value;
					if (LY != LYC) { STAT &= 0xFB; }
					break;
				case 0xFF46: // DMA 
					DMA_addr = value;
					DMA_start = true;
					DMA_clock = 0;
					DMA_inc = 0;
					break; 
				case 0xFF47: // BGP
					BGP = value;
					break; 
				case 0xFF48: // OBP0
					obj_pal_0 = value;
					break; 
				case 0xFF49: // OBP1
					obj_pal_1 = value;
					break;
				case 0xFF4A: // WY
					window_y = value;
					break;
				case 0xFF4B: // WX
					window_x = value;
					break;
			}
		}

		public void tick()
		{
			// tick DMA
			if (DMA_start)
			{
				if (DMA_clock >= 4)
				{
					OAM_access = false;
					if ((DMA_clock % 4) == 1)
					{
						// the cpu can't access memory during this time, but we still need the ppu to be able to.
						DMA_start = false;
						DMA_byte = Core.ReadMemory((ushort)((DMA_addr << 8) + DMA_inc));
						DMA_start = true;
					}
					else if ((DMA_clock % 4) == 3)
					{
						if ((DMA_inc % 4) == 3)
						{
							Core.OAM[DMA_inc] = DMA_byte;
						}
						else
						{
							Core.OAM[DMA_inc] = DMA_byte;
						}

						if (DMA_inc < (0xA0 - 1)) { DMA_inc++; }
					}
				}

				DMA_clock++;

				if (DMA_clock==648)
				{
					DMA_start = false;
					OAM_access = true;
				}
			}
			
			// the ppu only does anything if it is turned on via bit 7 of LCDC
			if (LCDC.Bit(7))
			{
				// exit vblank if LCD went from off to on
				if (LCD_was_off)
				{
					//VBL_INT = false;
					Core.in_vblank = false;
					LCD_was_off = false;

					// we exit vblank into mode 0 for 4 cycles 
					// but no hblank interrupt, presumably this only happens transitioning from mode 3 to 0
					STAT &= 0xFC;
				}

				// the VBL stat is continuously asserted
				if ((LY >= 144))
				{
					if (STAT.Bit(4))
					{
						if ((cycle >= 4) && (LY == 144))
						{
							VBL_INT = true;
						}
						else if (LY > 144)
						{
							VBL_INT = true;
						}
					}

					if ((cycle == 4) && (LY == 144)) {

						HBL_INT = false;

						// set STAT mode to 1 (VBlank) and interrupt flag if it is enabled
						STAT &= 0xFC;
						STAT |= 0x01;

						if (Core.REG_FFFF.Bit(0)) { Core.cpu.FlagI = true; }
						Core.REG_FF0F |= 0x01;
					}

					if ((LY >= 144) && (cycle == 4))
					{
						// a special case of OAM mode 2 IRQ assertion, even though PPU Mode still is 1
						if (STAT.Bit(5)) { OAM_INT = true; }
					}

					if ((LY == 153) && (cycle == 8))
					{
						LY = 0;
						LY_inc = 0;
						Core.cpu.LY = LY;
					}
				}

				if (!Core.in_vblank)
				{
					if (cycle == 4)
					{
						// here mode 2 will be set to true and interrupts fired if enabled
						STAT &= 0xFC;
						STAT |= 0x2;
						if (STAT.Bit(5)) { OAM_INT = true; }
						
						HBL_INT = false;
					}

					if (cycle >= 4 && cycle < 84)
					{
						// here OAM scanning is performed
						OAM_scan(cycle - 4);
					}
					else if (cycle >= 84 && LY < 144)
					{
						// render the screen and handle hblank
						render(cycle - 84);
					}
				}


				if ((LY_inc == 0))
				{
					if (cycle == 12)
					{
						LYC_INT = false;
						STAT &= 0xFB;

						// Special case of LY = LYC
						if (LY == LYC)
						{
							// set STAT coincidence FLAG and interrupt flag if it is enabled
							STAT |= 0x04;
							if (STAT.Bit(6)) { LYC_INT = true; }
						}

						// also a special case of OAM mode 2 IRQ assertion, even though PPU Mode still is 1
						if (STAT.Bit(5)) { OAM_INT = true; }
					}

					if (cycle == 92) { OAM_INT = false; }
				}

				// here LY=LYC will be asserted
				if ((cycle == 4) && (LY != 0))
				{
					LYC_INT = false;
					STAT &= 0xFB;

					if (LY == LYC)
					{
						// set STAT coincidence FLAG and interrupt flag if it is enabled
						STAT |= 0x04;
						if (STAT.Bit(6)) { LYC_INT = true; }
					}
				}

				cycle++;

				if (cycle==456)
				{
					cycle = 0;
					LY+=LY_inc;

					if (LY==0 && LY_inc == 0)
					{
						LY_inc = 1;
						Core.in_vblank = false;
						VBL_INT = false;
					}

					Core.cpu.LY = LY;

					if (LY==144)
					{
						Core.in_vblank = true;
					}
				}
			}
			else
			{
				// screen disable sets STAT as though it were vblank, but there is no Stat IRQ asserted
				STAT &= 0xFC;
				STAT |= 0x01;

				VBL_INT = LYC_INT = HBL_INT = OAM_INT = false;

				Core.in_vblank = true;

				LCD_was_off = true;

				LY = 0;
				Core.cpu.LY = LY;

				cycle = 0;
			}

			// assert the STAT IRQ line if the line went from zero to 1
			stat_line = VBL_INT | LYC_INT | HBL_INT | OAM_INT;

			if (stat_line && !stat_line_old)
			{
				if (Core.REG_FFFF.Bit(1)) { Core.cpu.FlagI = true; }
				Core.REG_FF0F |= 0x02;
			}

			stat_line_old = stat_line;

			// process latch delays
			//latch_delay();

		}

		// might be needed, not sure yet
		public void latch_delay()
		{
			//BGP_l = BGP;
		}

		public void OAM_scan(int OAM_cycle)
		{
			// we are now in STAT mode 2
			// TODO: maybe stat mode 2 flags are set at cycle 0 on visible scanlines?
			if (OAM_cycle == 0)
			{
				OAM_access = false;
				OAM_scan_index = 0;
				SL_sprites_index = 0;
				write_sprite = 0;
			}

			// the gameboy has 80 cycles to scan through 40 sprites, picking out the first 10 it finds to draw
			// the following is a guessed at implmenentation based on how NES does it, it's probably pretty close
			if (OAM_cycle < 10)
			{
				// start by clearing the sprite table (probably just clears X on hardware, but let's be safe here.)
				SL_sprites[OAM_cycle * 4] = 0;
				SL_sprites[OAM_cycle * 4 + 1] = 0;
				SL_sprites[OAM_cycle * 4 + 2] = 0;
				SL_sprites[OAM_cycle * 4 + 3] = 0;
			}
			else
			{
				if (write_sprite == 0)
				{
					if (OAM_scan_index < 40)
					{
						// (sprite Y - 16) equals LY, we have a sprite
						if ((Core.OAM[OAM_scan_index * 4] - 16) <= LY &&
							((Core.OAM[OAM_scan_index * 4] - 16) + 8 + (LCDC.Bit(2) ? 8 : 0)) > LY)
						{
							// always pick the first 10 in range sprites
							if (SL_sprites_index < 10)
							{
								SL_sprites[SL_sprites_index * 4] = Core.OAM[OAM_scan_index * 4];

								write_sprite = 1;
							}
							else
							{
								// if we already have 10 sprites, there's nothing to do, increment the index
								OAM_scan_index++;
							}
						}
						else
						{
							OAM_scan_index++;
						}
					}
				}
				else
				{
					SL_sprites[SL_sprites_index * 4 + write_sprite] = Core.OAM[OAM_scan_index * 4 + write_sprite];
					write_sprite++;

					if (write_sprite == 4)
					{
						write_sprite = 0;
						SL_sprites_index++;
						OAM_scan_index++;
					}
				}
			}
		}

		public void render(int render_cycle)
		{
			// we are now in STAT mode 3
			// NOTE: presumably the first necessary sprite is fetched at sprite evaulation
			// i.e. just keeping track of the lowest x-value sprite
			if (render_cycle == 0)
			{
				STAT &= 0xFC;
				STAT |= 0x03;
				OAM_INT = false;

				OAM_access = false;
				VRAM_access = false;
				OAM_scan_index = 0;
				read_case = 0;
				internal_cycle = 0;
				pre_render = true;
				tile_inc = 0;
				pixel_counter = 0;
				sl_use_index = 0;
				index_used = 0;
				bottom_index = 0;
				sprite_ordered_index = 0;
				fetch_sprite = false;
				no_sprites = false;

				// calculate the row number of the tiles to be fetched
				y_tile = ((int)Math.Floor((float)(scroll_y + LY) / 8)) % 32;

				if (SL_sprites_index == 0)
				{
					no_sprites = true;
				}
			}

			if (!pre_render && !fetch_sprite)
			{
				// start by fetching all the sprites that need to be fetched
				if (!no_sprites)
				{
					for (int i = 0; i < SL_sprites_index; i++)
					{
						if ((pixel_counter >= (SL_sprites[i * 4 + 1] - 8)) &&
							(pixel_counter < SL_sprites[i * 4 + 1]) &&
							!index_used.Bit(i))
						{
							fetch_sprite = true;
							sprite_fetch_index = 0;
						}
					}
				}

				if (!fetch_sprite)
				{
					// start shifting data into the LCD
					if (render_counter >= (render_offset + 8))
					{
						pixel = tile_data_latch[0].Bit(7 - (render_counter % 8)) ? 1 : 0;
						pixel |= tile_data_latch[1].Bit(7 - (render_counter % 8)) ? 2 : 0;
						pixel = (BGP >> (pixel * 2)) & 3;
						// now we have the BG pixel, we next need the sprite pixel
						if (!no_sprites)
						{
							bool have_sprite = false;
							int i = bottom_index;
							int s_pixel = 0;
							int sprite_attr = 0;

							while (i < sprite_ordered_index)
							{
								if (SL_sprites_ordered[i * 4] == pixel_counter)
								{
									bottom_index++;
									if (bottom_index == SL_sprites_index) { no_sprites = true; }
								}
								else if (!have_sprite)
								{
									// we can use the current sprite, so pick out a pixel for it
									int t_index = pixel_counter - (SL_sprites_ordered[i * 4] - 8);

									t_index = 7 - t_index;

									sprite_data[0] = (byte)((SL_sprites_ordered[i * 4 + 1] >> t_index) & 1);
									sprite_data[1] = (byte)(((SL_sprites_ordered[i * 4 + 2] >> t_index) & 1) << 1);

									s_pixel = sprite_data[0] + sprite_data[1];
									sprite_attr = SL_sprites_ordered[i * 4 + 3];

									// pixel color of 0 is transparent, so if this is the case we dont have a pixel
									if (s_pixel != 0)
									{
										have_sprite = true;
									}
								}
								i++;
							}

							if (have_sprite)
							{
								bool use_sprite = false;
								if (LCDC.Bit(1))
								{
									if (!sprite_attr.Bit(7))
									{
										if (s_pixel != 0) { use_sprite = true; }
									}
									else if (pixel == 0)
									{
										use_sprite = true;
									}

									if (!LCDC.Bit(0))
									{
										use_sprite = true;
									}
								}

								if (use_sprite)
								{
									if (sprite_attr.Bit(4))
									{
										pixel = (obj_pal_1 >> (s_pixel * 2)) & 3;
									}
									else
									{
										pixel = (obj_pal_0 >> (s_pixel * 2)) & 3;
									}
								}
							}
						}

						// based on sprite priority and pixel values, pick a final pixel color
						Core._vidbuffer[LY * 160 + pixel_counter] = (int)GBHawk.color_palette[pixel];
						pixel_counter++;

						if (pixel_counter == 160)
						{
							read_case = 8;
							hbl_set_once = true;
						}
					}
					render_counter++;
				}
			}

			if (!fetch_sprite)
			{
				if (latch_new_data)
				{
					latch_new_data = false;
					tile_data_latch[0] = tile_data[0];
					tile_data_latch[1] = tile_data[1];
				}

				switch (read_case)
				{
					case 0: // read a background tile
						if ((internal_cycle % 2) == 0)
						{

							temp_fetch = y_tile * 32 + (x_tile + tile_inc) % 32;
							tile_byte = LCDC.Bit(3) ? Core.BG_map_2[temp_fetch] : Core.BG_map_1[temp_fetch];

						}
						else
						{
							if (!pre_render)
							{
								tile_inc++;
							}
							read_case = 1;
						}
						break;

					case 1: // read from tile graphics (0)
						if ((internal_cycle % 2) == 0)
						{
							y_scroll_offset = (scroll_y + LY) % 8;

							if (LCDC.Bit(4))
							{
								tile_data[0] = Core.CHR_RAM[tile_byte * 16 + y_scroll_offset * 2];
							}
							else
							{
								// same as before except now tile byte represents a signed byte
								if (tile_byte.Bit(7))
								{
									tile_byte -= 256;
								}
								tile_data[0] = Core.CHR_RAM[0x1000 + tile_byte * 16 + y_scroll_offset * 2];
							}

						}
						else
						{
							read_case = 2;
						}
						break;

					case 2: // read from tile graphics (1)
						if ((internal_cycle % 2) == 0)
						{
							y_scroll_offset = (scroll_y + LY) % 8;

							if (LCDC.Bit(4))
							{
								// if LCDC somehow changed between the two reads, make sure we have a positive number
								if (tile_byte < 0)
								{
									tile_byte += 256;
								}

								tile_data[1] = Core.CHR_RAM[tile_byte * 16 + y_scroll_offset * 2 + 1];
							}
							else
							{
								// same as before except now tile byte represents a signed byte
								if (tile_byte.Bit(7) && tile_byte > 0)
								{
									tile_byte -= 256;
								}

								tile_data[1] = Core.CHR_RAM[0x1000 + tile_byte * 16 + y_scroll_offset * 2 + 1];
							}

						}
						else
						{
							if (pre_render)
							{
								// here we set up rendering
								pre_render = false;
								render_offset = scroll_x % 8;
								render_counter = -1;
								latch_counter = 0;
								read_case = 0;
							}
							else
							{
								read_case = 3;
							}

						}
						break;

					case 3: // read from sprite data
						if ((internal_cycle % 2) == 0)
						{
							// nothing to do if not fetching
						}
						else
						{
							read_case = 0;
							latch_new_data = true;
						}
						break;

					case 4: // read from window data
						break;

					case 6: // read from tile graphics (for the window)
						break;

					case 7: // read from tile graphics (for the window)
						break;

					case 8: // done reading, we are now in phase 0
						
						OAM_access = true;
						VRAM_access = true;

						STAT &= 0xFC;
						STAT |= 0x00;
						pre_render = true;
						if (hbl_set_once)
						{
							if (STAT.Bit(3)) { HBL_INT = true; }
							hbl_set_once = false;
						}
						
						break;
				}

				internal_cycle++;
			}

			if (fetch_sprite)
			{
				if (sprite_fetch_index < SL_sprites_index)
				{
					if (pixel_counter != 0) { 
						if ((pixel_counter == (SL_sprites[sprite_fetch_index * 4 + 1] - 8)) &&
								//(pixel_counter < SL_sprites[sprite_fetch_index * 4 + 1]) &&
								!index_used.Bit(sprite_fetch_index))
						{
							sl_use_index = sprite_fetch_index;
							process_sprite();
							SL_sprites_ordered[sprite_ordered_index * 4] = SL_sprites[sprite_fetch_index * 4 + 1];
							SL_sprites_ordered[sprite_ordered_index * 4 + 1] = sprite_sel[0];
							SL_sprites_ordered[sprite_ordered_index * 4 + 2] = sprite_sel[1];
							SL_sprites_ordered[sprite_ordered_index * 4 + 3] = SL_sprites[sprite_fetch_index * 4 + 3];
							sprite_ordered_index++;
							index_used |= (1 << sl_use_index);
						}
						sprite_fetch_index++;
						if (sprite_fetch_index == SL_sprites_index) { fetch_sprite = false; }
					}
					else
					{
						// whan pixel counter is 0, we want to scan all the points before 0 as well
						// certainly non-physical but good enough for now
						for (int j = -7; j < 1; j++)
						{
							for (int i = 0; i < SL_sprites_index; i++)
							{	
								if ((j == (SL_sprites[i * 4 + 1] - 8)) &&
								!index_used.Bit(i))
								{
									sl_use_index = i;
									process_sprite();
									SL_sprites_ordered[sprite_ordered_index * 4] = SL_sprites[i * 4 + 1];
									SL_sprites_ordered[sprite_ordered_index * 4 + 1] = sprite_sel[0];
									SL_sprites_ordered[sprite_ordered_index * 4 + 2] = sprite_sel[1];
									SL_sprites_ordered[sprite_ordered_index * 4 + 3] = SL_sprites[i * 4 + 3];
									sprite_ordered_index++;
									index_used |= (1 << sl_use_index);
								}
							}
						}
						fetch_sprite = false;
					}
				}
			}
		}

		public void Reset()
		{
			LCDC = 0;
			STAT = 0x80;
			scroll_y = 0;
			scroll_x = 0;
			LY = 0;
			LYC = 0;
			DMA_addr = 0;
			BGP = 0;
			obj_pal_0 = 0;
			obj_pal_1 = 0;
			window_y = 0;
			window_x = 0;
			LY_inc = 1;

			cycle = 0;
			LYC_INT = false;
			HBL_INT = false;
			VBL_INT = false;
			OAM_INT = false;

			stat_line = false;
			stat_line_old = false;
		}

		public void process_sprite()
		{
			int y;

			if (SL_sprites[sl_use_index * 4 + 3].Bit(6))
			{
				if (LCDC.Bit(2))
				{
					y = LY - (SL_sprites[sl_use_index * 4] - 16);
					y = 15 - y;
					sprite_sel[0] = Core.CHR_RAM[(SL_sprites[sl_use_index * 4 + 2] & 0xFE) * 16 + y * 2];
					sprite_sel[1] = Core.CHR_RAM[(SL_sprites[sl_use_index * 4 + 2] & 0xFE) * 16 + y * 2 + 1];
				}
				else
				{
					y = LY - (SL_sprites[sl_use_index * 4] - 16);
					y = 7 - y;
					sprite_sel[0] = Core.CHR_RAM[SL_sprites[sl_use_index * 4 + 2] * 16 + y * 2];
					sprite_sel[1] = Core.CHR_RAM[SL_sprites[sl_use_index * 4 + 2] * 16 + y * 2 + 1];
				}
			}
			else
			{
				if (LCDC.Bit(2))
				{
					y = LY - (SL_sprites[sl_use_index * 4] - 16);
					sprite_sel[0] = Core.CHR_RAM[(SL_sprites[sl_use_index * 4 + 2] & 0xFE) * 16 + y * 2];
					sprite_sel[1] = Core.CHR_RAM[(SL_sprites[sl_use_index * 4 + 2] & 0xFE) * 16 + y * 2 + 1];
				}
				else
				{
					y = LY - (SL_sprites[sl_use_index * 4] - 16);
					sprite_sel[0] = Core.CHR_RAM[SL_sprites[sl_use_index * 4 + 2] * 16 + y * 2];
					sprite_sel[1] = Core.CHR_RAM[SL_sprites[sl_use_index * 4 + 2] * 16 + y * 2 + 1];
				}
			}

			if (SL_sprites[sl_use_index * 4 + 3].Bit(5))
			{
				int b0, b1, b2, b3, b4, b5, b6, b7 = 0;
				for (int i = 0; i < 2; i++)
				{
					b0 = (sprite_sel[i] & 0x01) << 7;
					b1 = (sprite_sel[i] & 0x02) << 5;
					b2 = (sprite_sel[i] & 0x04) << 3;
					b3 = (sprite_sel[i] & 0x08) << 1;
					b4 = (sprite_sel[i] & 0x10) >> 1;
					b5 = (sprite_sel[i] & 0x20) >> 3;
					b6 = (sprite_sel[i] & 0x40) >> 5;
					b7 = (sprite_sel[i] & 0x80) >> 7;

					sprite_sel[i] = (byte)(b0 | b1 | b2 | b3 | b4 | b5 | b6 | b7);
				}
			}
		}

		public void SyncState(Serializer ser)
		{
			ser.Sync("LCDC", ref LCDC);
			ser.Sync("STAT", ref STAT);
			ser.Sync("scroll_y", ref scroll_y);
			ser.Sync("scroll_x", ref scroll_x);
			ser.Sync("LY", ref LY);
			ser.Sync("LYinc", ref LY_inc);
			ser.Sync("LYC", ref LYC);
			ser.Sync("DMA_addr", ref DMA_addr);
			ser.Sync("BGP", ref BGP);
			ser.Sync("obj_pal_0", ref obj_pal_0);
			ser.Sync("obj_pal_1", ref obj_pal_1);
			ser.Sync("window_y", ref window_y);
			ser.Sync("window_x", ref window_x);
			ser.Sync("DMA_start", ref DMA_start);
			ser.Sync("DMA_clock", ref DMA_clock);
			ser.Sync("DMA_inc", ref DMA_inc);
			ser.Sync("DMA_byte", ref DMA_byte);

			ser.Sync("LYC_INT", ref LYC_INT);
			ser.Sync("HBL_INT", ref HBL_INT);
			ser.Sync("VBL_INT", ref VBL_INT);
			ser.Sync("OAM_INT", ref OAM_INT);
			ser.Sync("stat_line", ref stat_line);
			ser.Sync("stat_line_old", ref stat_line_old);
			ser.Sync("hbl_set_once", ref hbl_set_once);
			ser.Sync("LCD_was_off", ref LCD_was_off);
			ser.Sync("OAM_access", ref OAM_access);
			ser.Sync("OAM_scan_index", ref OAM_scan_index);
			ser.Sync("SL_sprites_index", ref SL_sprites_index);
			ser.Sync("SL_sprites", ref SL_sprites, false);
			ser.Sync("write_sprite", ref write_sprite);

			ser.Sync("VRAM_access", ref VRAM_access);
			ser.Sync("read_case", ref read_case);
			ser.Sync("internal_cycle", ref internal_cycle);
			ser.Sync("y_tile", ref y_tile);
			ser.Sync("y_scroll_offset", ref y_scroll_offset);
			ser.Sync("x_tile", ref x_tile);
			ser.Sync("x_scroll_offset", ref x_scroll_offset);
			ser.Sync("tile_byte", ref tile_byte);
			ser.Sync("sprite_fetch_cycles", ref sprite_fetch_cycles);
			ser.Sync("fetch_sprite", ref fetch_sprite);
			ser.Sync("temp_fetch", ref temp_fetch);
			ser.Sync("tile_inc", ref tile_inc);
			ser.Sync("pre_render", ref pre_render);
			ser.Sync("tile_data", ref tile_data, false);
			ser.Sync("tile_data_latch", ref tile_data_latch, false);
			ser.Sync("latch_counter", ref latch_counter);
			ser.Sync("latch_new_data", ref latch_new_data);
			ser.Sync("render_counter", ref render_counter);
			ser.Sync("render_offset", ref render_offset);
			ser.Sync("pixel_counter", ref pixel_counter);
			ser.Sync("pixel", ref pixel);
			ser.Sync("sprite_data", ref sprite_data, false);
			ser.Sync("sl_use_index", ref sl_use_index);
			ser.Sync("sprite_sel", ref sprite_sel, false);
			ser.Sync("no_sprites", ref no_sprites);
			ser.Sync("sprite_fetch_index", ref sprite_fetch_index);
			ser.Sync("SL_sprites_ordered", ref SL_sprites_ordered, false);
			ser.Sync("index_used", ref index_used);
			ser.Sync("sprite_ordered_index", ref sprite_ordered_index);
			ser.Sync("bottom_index", ref bottom_index);

		}
	}
}
