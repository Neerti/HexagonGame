using System;
using System.Collections.Generic;
using Godot;
using JetBrains.Annotations;

namespace Hexagon
{
	/// <summary>
	/// A container for logical hexagon objects.
	/// </summary>
	public class HexGrid
	{
		public readonly int SizeX;
		public readonly int SizeY;
		private Hex[,] _grid;

		public HexGrid(int new_size_x, int new_size_y)
		{
			SizeX = new_size_x;
			SizeY = new_size_y;
			MakeEmptyGrid();
		}

		public Hex GetHex(int x, int y)
		{
			if(x < 0 || x > SizeX - 1)
			{
				throw new IndexOutOfRangeException("X coordinate was out of bounds.");
			}
			if(y < 0 || y > SizeY - 1)
			{
				throw new IndexOutOfRangeException("Y coordinate was out of bounds.");
			}
			return _grid[x,y];
		}

		public bool HasHex(int x, int y)
		{
			if(x < 0 || x > SizeX - 1)
			{
				return false;
			}
			if(y < 0 || y > SizeY - 1)
			{
				return false;
			}
			return true;
		}

		public Hex GetHexByOffset(int x, int y, int x_offset, int y_offset)
		{
			if(HasHex(x + x_offset, y + y_offset))
			{
				return GetHex(x + x_offset, y + y_offset);
			}
			return null;
		}

		public Hex GetHexByOffset(Hex tile, int x_offset, int y_offset)
		{
			return GetHexByOffset(tile.X, tile.Y, x_offset, y_offset);
		}
		
		[CanBeNull]
		public Hex GetHexCubic(int q, int r, int s)
		{
			var x = q;
			var y = r + (q - (q & 1)) / 2;
			return HasHex(x, y) ? GetHex(x, y) : null;
		}
		

		// Creates an empty grid of logical hexagons objects. 
		// This will overwrite and completely wipe out any existing map.
		private void MakeEmptyGrid()
		{
			_grid = new Hex[SizeX, SizeY];
			for (var x = 0; x < SizeX; x++)
			{
				for (var y = 0; y < SizeY; y++)
				{
					_grid[x, y] = new Hex(x, y);
				}
			}
		}

		public Image GenerateHeightNoiseImage()
		{
			var img = new Image();
			img.Create(SizeX, SizeY, false, Image.Format.Rgba8);
			img.Lock();
			for(var x = 0; x < SizeX; x++)
			{
				for(var y = 0; y < SizeY; y++)
				{
					float value = _grid[x,y].Height;

					var chosen_color = Colors.Black;
					chosen_color = chosen_color.LinearInterpolate(Colors.White, value);
					img.SetPixel(x, y, chosen_color);
				}
			}
			img.Unlock();
			return img;
		}

		public void AssignBiomesToHexes()
		{
			for(var x = 0; x < SizeX; x++)
			{
				for(var y = 0; y < SizeY; y++)
				{
					// For now it's just height based.
					// Someday it will also incorporate temperature and humidity.

					var tile = _grid[x,y];
					float value = tile.Height;

					// TESTING
					var chosen_tile_type = Hex.TileType.BASE;

					// This is super ugly and hopefully temporary.
					float sea_level_offset = 0.1f;
					if(value > (0.25f + sea_level_offset))
					{
						chosen_tile_type = Hex.TileType.SNOW;
					}
					else if(value > (0.20f + sea_level_offset))
					{
						chosen_tile_type = Hex.TileType.ROCK;
					}
					else if(value > (0.15f + sea_level_offset))
					{
						chosen_tile_type = Hex.TileType.FOREST;
					}
					else if(value > (0.05f + sea_level_offset))
					{
						chosen_tile_type = Hex.TileType.GRASS;
					}
					else if(value > (0f + sea_level_offset))
					{
						chosen_tile_type = Hex.TileType.BEACH_SAND;
					}
					else if(value > (-0.15f + sea_level_offset))
					{
						chosen_tile_type = Hex.TileType.SHALLOW_SALT_WATER;
					}
					else
					{
						chosen_tile_type = Hex.TileType.DEEP_SALT_WATER;
					}
					tile.tile_type = chosen_tile_type;
				}
			}
		}

		public Image GenerateMapImage()
		{
			var img = new Image();
			img.Create(SizeX, SizeY, false, Image.Format.Rgba8);
			img.Lock();

			for(var x = 0; x < SizeX; x++)
			{
				for(var y = 0; y < SizeY; y++)
				{
					var chosen_color = Colors.White;
					var tile = _grid[x,y];
					// TileTypes should probably be made into their own objects.
					switch (tile.tile_type)
					{
						case Hex.TileType.SNOW:
							chosen_color = Colors.LightBlue;
							break;
						case Hex.TileType.ROCK:
							chosen_color = Colors.DimGray;
							break;
						case Hex.TileType.FOREST:
							chosen_color = Colors.DarkGreen;
							break;
						case Hex.TileType.GRASS:
							chosen_color = Colors.Limegreen;
							break;
						case Hex.TileType.BEACH_SAND:
							chosen_color = Colors.PaleGoldenrod;
							break;
						case Hex.TileType.SHALLOW_SALT_WATER:
							chosen_color = Colors.MediumBlue;
							break;
						case Hex.TileType.DEEP_SALT_WATER:
							chosen_color = Colors.NavyBlue;
							break;	
						default:
							chosen_color = Colors.Black;
							break;
					}

					if(GetHexBiomeBitmask(tile) != 15)
					{
						chosen_color = chosen_color.Darkened(0.5f);
					}

					img.SetPixel(x, y, chosen_color);
				}
			}
			img.Unlock();
			return img;
		}

		public Image GenerateHeatMap()
		{
			var img = new Image();
			img.Create(SizeX, SizeY, false, Image.Format.Rgba8);
			img.Lock();

			for(var x = 0; x < SizeX; x++)
			{
				for(var y = 0; y < SizeY; y++)
				{
					var chosen_color = Colors.Blue;
					var tile = _grid[x,y];
					chosen_color = chosen_color.LinearInterpolate(Colors.Red, tile.Temperature);

					if(GetHexBiomeBitmask(tile) != 15)
					{
						chosen_color = chosen_color.Darkened(0.5f);
					}
					img.SetPixel(x, y, chosen_color);
					
				}
			}
			img.Unlock();
			return img;
		}

		public int GetHexBiomeBitmask(Hex tile)
		{
			int result = 0;
			Hex top_tile, bottom_tile, left_tile, right_tile;
			top_tile = GetHexByOffset(tile,  0, -1);
			bottom_tile = GetHexByOffset(tile,  0,  1);
			left_tile =  GetHexByOffset(tile, -1,  0);
			right_tile =  GetHexByOffset(tile,  1,  0);
			if(top_tile?.tile_type == tile.tile_type)
			{
				result += 1;
			}
			if(right_tile?.tile_type == tile.tile_type)
			{
				result += 2;
			}
			if(bottom_tile?.tile_type == tile.tile_type)
			{
				result += 4;
			}
			if(left_tile?.tile_type == tile.tile_type)
			{
				result += 8;
			}
			return result;
		}

	}
	
}


