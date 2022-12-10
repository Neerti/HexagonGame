using System;
using HexagonGame.ECS.Worlds;
using HexagonGame.VectorHexes;

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
				Grid[x, y] = world.NewEntity();;
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
}