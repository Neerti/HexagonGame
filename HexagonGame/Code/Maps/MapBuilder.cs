using System;
using Arch.Core;
using HexagonGame.ECS.Components;
using Microsoft.Xna.Framework;

namespace HexagonGame.Maps;

public class MapBuilder
{
	private const float TileSize = .5f;
	public static float TileHeight => (float) Math.Sqrt(3) * TileSize;
	public static float TileWidth => 2 * TileSize;
	public static float TileHorizontalSpacing => (3f / 4f) * TileWidth;
	public static float TileVerticalSpacing => TileHeight;
	
	public LogicalMap NewMap(World world, int mapSizeX, int mapSizeY, int mapSizeZ)
	{
		var map = new LogicalMap(mapSizeX, mapSizeY, mapSizeZ);

		for (var x = 0; x < map.SizeX; x++)
		{
			for (var y = 0; y < map.SizeY; y++)
			{
				for (var z = 0; z < map.SizeZ; z++)
				{
					for (var l = 0; l < LogicalMap.ObjectLayer; l++)
					{
						var entity = world.Create(
							new Position{WorldPosition = CoordinateToPosition(x, y, z)},
							new Appearance{ModelColor = Color.White});
						map.Grid[x, y, z, l] = entity;
					}
				}
			}
		}
		
		return map;
	}

	public Vector3 CoordinateToPosition(int x, int y, int z)
	{
		return new Vector3(
			x * TileHorizontalSpacing,
			y * 2,
			z * TileVerticalSpacing + (x % 2 == 0 ? TileVerticalSpacing * 0.5f : 0)
		);
	}
	
}