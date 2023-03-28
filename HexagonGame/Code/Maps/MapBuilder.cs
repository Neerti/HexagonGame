using System;
using Arch.Core;
using HexagonGame.ECS.Components;
using Microsoft.Xna.Framework;

namespace HexagonGame.Maps;

public class MapBuilder
{
	private const float TileSize = .5f;
	private float TileHeight => (float) Math.Sqrt(3) * TileSize;
	private float TileWidth => 2 * TileSize;
	private float TileHorizontalSpacing => (3f / 4f) * TileWidth;
	private float TileVerticalSpacing => TileHeight;
	
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
							new Position{WorldPosition = new Vector3(
								x * TileHorizontalSpacing,
								y * 2,
								(z * TileVerticalSpacing) + (x % 2 == 0 ? TileVerticalSpacing * 0.5f: 0)
								)},
							new Appearance());
						map.Grid[x, y, z, l] = entity;
					}
				}
			}
		}
		
		return map;
	}
	
}