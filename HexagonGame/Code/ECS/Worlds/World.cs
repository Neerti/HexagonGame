using System.Collections.Generic;
using HexagonGame.ECS.Components;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.SparseSets;

namespace HexagonGame.ECS.Worlds;

/// <summary>
/// The World object holds every entity and component in the game that currently exists.
/// Consequently, it holds all mutable state for a particular save game, and is the starting
/// point for serialization and deserialization.
/// Multiple Worlds can exist at once, however that is intended for unit testing only, to improve isolation.
/// One World is used in normal gameplay.
/// </summary>
public class World
{
	public List<int> Entities = new List<int>();
	public int EntityTally = 1; // This starts at 1, and not 0, so that 0 will always be an empty entity.
	
	// In the future, this should be some kind of object to manage this for each component.
	public SparseSet<PositionComponent> PositionComponents;
	public SparseSet<TileAttributeComponent> TileAttributeComponents;

	// EntityGrids are used to store map data.
	// Multiple grids could be used at once, to store different maps, e.g. an underground level, or 
	// a different planet.
	public EntityGrid Grid;

	public DateTime Calendar;

	// Entity ID for the game camera.
	public int CameraEntity;
	
	public int NewEntity()
	{
		Entities.Add(EntityTally);
		return EntityTally++;
	}
}
