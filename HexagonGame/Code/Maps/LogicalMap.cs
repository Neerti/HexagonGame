using Arch.Core;

namespace HexagonGame.Maps;

/// <summary>
/// A container for entities arranged in a grid pattern, intended to store map data.
/// </summary>
public struct LogicalMap
{
	public readonly int SizeX;
	public readonly int SizeY;
	public readonly int SizeZ;
	public Entity[,,,] Grid;
	public const int TerrainLayer = 0;
	public const int ObjectLayer = 1;
	public const int MaxLayers = 2;

	public LogicalMap(int newSizeX, int newSizeY, int newSizeZ)
	{
		SizeX = newSizeX;
		SizeY = newSizeY;
		SizeZ = newSizeZ;
		Grid = new Entity[SizeX, SizeY, SizeZ, MaxLayers];
	}
	
}