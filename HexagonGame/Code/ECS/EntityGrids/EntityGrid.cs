using System;
using HexagonGame.ECS.Worlds;
using HexagonGame.VectorHexes;
using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.EntityGrids;

/// <summary>
/// A container for entities arranged in a grid pattern, intended to store map data.
/// Addressing specific entities can be done spatially with the <see cref="VectorHex"/> object.
/// </summary>
public struct EntityGrid
{
	public readonly int SizeX;
	public readonly int SizeY;
	public int[,,] Grid;
	public const int TileSpriteHeight = 32;
	public const int TileSpriteWidth = 49;
	public const int TerrainLayer = 0;
	public const int ObjectLayer = 1;
	public const int MaxLayers = 2;

	public EntityGrid(int newSizeX, int newSizeY)
	{
		SizeX = newSizeX;
		SizeY = newSizeY;
		Grid = new int[SizeX, SizeY, MaxLayers];
	}

	public void PopulateGrid(World world)
	{
		for (var x = 0; x < SizeX; x++)
		{
			for (var y = 0; y < SizeY; y++)
			{
				Grid[x, y, TerrainLayer] = world.NewEntity();
			}
		}
	}

	public bool IsValidCoordinate(int x, int y)
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
	
	public int GetEntity(VectorHex hex, int layer = TerrainLayer)
	{
		return GetEntity(hex.X, hex.Y, layer);
	}

	public int GetEntity(int x, int y, int layer = TerrainLayer)
	{
		if (!IsValidCoordinate(x, y))
		{
			throw new ArgumentOutOfRangeException();
		}
		return Grid[x, y, layer];
	}

	public Vector2 TileCoordinateToVector(int x, int y)
	{
		var offsetX = x * TileSpriteWidth;
		var offsetY = y * TileSpriteHeight;
		return new Vector2(offsetX, offsetY);
	}

	public Vector2 TileCoordinateToVector(VectorHex hex)
	{
		return TileCoordinateToVector(hex.X, hex.Y);
	}
	
	public (int xCoordinate, int yCoordinate) VectorToTileCoordinate(Vector2 vector)
	{
		var x = (int) Math.Ceiling(vector.X / TileSpriteWidth);
		var y = (int) Math.Ceiling(vector.Y / TileSpriteHeight);

		x = Math.Clamp(x, 0, SizeX);
		y = Math.Clamp(y, 0, SizeY);
		
		return (x, y);
	}
}