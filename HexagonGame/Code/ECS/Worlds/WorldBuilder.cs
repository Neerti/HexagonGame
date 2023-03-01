using System;
using HexagonGame.ECS.Components;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.SparseSets;
using HexagonGame.MapGeneration;
using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Worlds;

/// <summary>
/// Creates a <see cref="World"/> instance.
/// </summary>
public class WorldBuilder
{
	public World NewWorld(GameRoot root, int mapSizeX, int mapSizeY)
	{
		var world = new World();

		// Set up the component holders.
		// Might be good to add auto-resizing to the sparse sets.
		var maxEntities = mapSizeX * mapSizeY + 1000;
		world.PositionComponents = new SparseSet<PositionComponent>(maxEntities);
		world.TileAttributeComponents = new SparseSet<TileAttributeComponent>(maxEntities);
		world.AppearanceComponents = new SparseSet<AppearanceComponent>(maxEntities);
		world.LifecycleComponents = new SparseSet<LifecycleComponent>(1000, maxEntities);
		
		// The map.
		world.Grid = new EntityGrid(mapSizeX, mapSizeY);
		world.Grid.PopulateGrid(world);
		
		// This needs to be torched.
		for (var i = 0; i < world.Grid.SizeX; i++)
		{
			for (var j = 0; j < world.Grid.SizeY; j++)
			{
				var tileEntity = world.Grid.Grid[i, j, EntityGrid.TerrainLayer];
				var newX = i * EntityGrid.TileSpriteWidth;
				var newY = j * EntityGrid.TileSpriteHeight;
				if ((i & 1) == 1) // Odd numbers are moved down by half.
				{
					newY += EntityGrid.TileSpriteHeight / 2;
				}

				world.PositionComponents.Add(tileEntity, new PositionComponent(newX, newY));

				world.TileAttributeComponents.Add(tileEntity, new TileAttributeComponent());

				// Debug coloring.
				var textureColor = Color.White;
				if (i % 10 == 0)
				{
					textureColor = Color.Blue;
					if (j % 10 == 0)
					{
						textureColor = Color.Yellow;
					}
				}
				else if (j % 10 == 0)
				{
					textureColor = Color.Green;
				}

				if (i == 0 && j == 0)
				{
					textureColor = Color.Red;
				}

				world.AppearanceComponents.Add(tileEntity, new AppearanceComponent("hexagon", textureColor));
			}
		}
		
		// Generate the map.
		var mapGenerator = new MapGenerator(0);
		mapGenerator.ApplyNoise(world, world.Grid);
		mapGenerator.AddTrees(root, world);

		// Set up a basic camera out of a few components.
		world.CameraEntity = world.NewEntity();
		var posComponent = new PositionComponent(position: Vector2.Zero);
		world.PositionComponents.Add(world.CameraEntity, posComponent);

		// Make time exist.
		world.Calendar = new DateTime();
		world.Calendar = world.Calendar.AddYears(100);

		// Spawn in the starting population.
		for (var i = 0; i < 10; i++)
		{
			var pop = world.NewEntity();
			var lifecycleComponent = new LifecycleComponent(world.Calendar.AddYears(-20));
			world.LifecycleComponents.Add(pop, lifecycleComponent);
		}
		
		
		return world;
	}
}

/*
		// Create the world.
		// Later on this should be part of starting a new game or loading a save file.
		World = new World();

		var mapSize = (int) Math.Pow(2, 6);

		// Set up the component holders.
		// Might be good to add auto-resizing to the sparse sets.
		World.PositionComponents = new SparseSet<PositionComponent>(mapSize * mapSize + 1000);
		World.TileAttributeComponents = new SparseSet<TileAttributeComponent>(mapSize * mapSize + 1000);
		World.AppearanceComponents = new SparseSet<AppearanceComponent>(mapSize * mapSize + 1000);
		World.LifecycleComponents = new SparseSet<LifecycleComponent>(1000);

		// The map.
		World.Grid = new EntityGrid(mapSize, mapSize);
		World.Grid.PopulateGrid(World);

		for (var i = 0; i < World.Grid.SizeX; i++)
		{
			for (var j = 0; j < World.Grid.SizeY; j++)
			{
				var tileEntity = World.Grid.Grid[i, j, EntityGrid.TerrainLayer];
				var newX = i * EntityGrid.TileSpriteWidth;
				var newY = j * EntityGrid.TileSpriteHeight;
				if ((i & 1) == 1) // Odd numbers are moved down by half.
				{
					newY += EntityGrid.TileSpriteHeight / 2;
				}

				World.PositionComponents.Add(tileEntity, new PositionComponent(newX, newY));

				World.TileAttributeComponents.Add(tileEntity, new TileAttributeComponent());

				// Debug coloring.
				var textureColor = Color.White;
				if (i % 10 == 0)
				{
					textureColor = Color.Blue;
					if (j % 10 == 0)
					{
						textureColor = Color.Yellow;
					}
				}
				else if (j % 10 == 0)
				{
					textureColor = Color.Green;
				}

				if (i == 0 && j == 0)
				{
					textureColor = Color.Red;
				}

				World.AppearanceComponents.Add(tileEntity, new AppearanceComponent("hexagon", textureColor));
			}
		}

		// Generate the map.
		var mapGenerator = new MapGenerator(0);
		mapGenerator.ApplyNoise(World, World.Grid);
		mapGenerator.AddTrees(this, World);

		// Set up a basic camera out of a few components.
		World.CameraEntity = World.NewEntity();
		var posComponent = new PositionComponent(position: Vector2.Zero);
		World.PositionComponents.Add(World.CameraEntity, posComponent);

		// Make time exist.
		World.Calendar = new DateTime();
		World.Calendar = World.Calendar.AddYears(100);

		// Spawn in the starting population.
		for (int i = 0; i < 10; i++)
		{
			var pop = World.NewEntity();
			var lifecycleComponent = new LifecycleComponent(World.Calendar.AddYears(-20));
			World.LifecycleComponents.Add(pop, lifecycleComponent);
		}
*/