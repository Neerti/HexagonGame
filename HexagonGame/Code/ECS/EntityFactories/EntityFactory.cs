using System;
using HexagonGame.ECS.Components;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.EntityFactories;

public class EntityFactory
{
	public PrefabCollection PrefabCollection;

	public EntityFactory()
	{
		SetUp();
	}

	public void SetUp()
	{
		PrefabCollection = new PrefabCollection();
	}
	
	public int EntityFromPrefab(OldWorld oldWorld, string prefabName)
	{
		// This might be slow if a lot of prefabs get generated.
		if (!PrefabCollection.Prefabs.ContainsKey(prefabName))
		{
			throw new ArgumentException("Prefab name does not exist.");
		}
		var prefab = PrefabCollection.Prefabs[prefabName];

		var entity = oldWorld.NewEntity();

		// This will get unmanageable as more components are added.
		if (prefab.PositionComponent.HasValue)
		{
			var component = new PositionComponent(prefab.PositionComponent.Value);
			oldWorld.PositionComponents.Add(entity, component);
		}

		if (prefab.AppearanceComponent.HasValue)
		{
			var component = new AppearanceComponent(prefab.AppearanceComponent.Value);
			oldWorld.AppearanceComponents.Add(entity, component);
		}
		
		if (prefab.TileAttributeComponent.HasValue)
		{
			var component = new TileAttributeComponent(prefab.TileAttributeComponent.Value);
			oldWorld.TileAttributeComponents.Add(entity, component);
		}

		return entity;

	}
}