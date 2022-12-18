using System;
using HexagonGame.ECS.Worlds;
using HexagonGame.VectorHexes;
using JetBrains.Annotations;
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
	public int[,] Grid;
	public const int TileSpriteHeight = 32;
	public const int TileSpriteWidth = 49;

	public EntityGrid(int newSizeX, int newSizeY)
	{
		SizeX = newSizeX;
		SizeY = newSizeY;
		Grid = new int[SizeX, SizeY];
	}

	public void PopulateGrid(World world)
	{
		for (var x = 0; x < SizeX; x++)
		{
			for (var y = 0; y < SizeY; y++)
			{
				Grid[x, y] = world.NewEntity();
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
	
	public int GetEntity(VectorHex hex)
	{
		return GetEntity(hex.X, hex.Y);
	}

	public int GetEntity(int x, int y)
	{
		if (!IsValidCoordinate(x, y))
		{
			throw new ArgumentOutOfRangeException();
		}
		return Grid[x, y];
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

	[CanBeNull]
	public (int xCoordinate, int yCoordinate) VectorToTileCoordinate(Vector2 vector)
	{
		var x = (int) Math.Ceiling(TileSpriteWidth * vector.X);
		var y = (int) Math.Ceiling(TileSpriteHeight * vector.Y);
		if (IsValidCoordinate(x, y))
		{
			return (x, y);
		}
		
		return (-1, -1); // I hate this.
	}
}