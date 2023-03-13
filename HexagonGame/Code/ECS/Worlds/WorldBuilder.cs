using System;
using HexagonGame.ECS.Components;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.SparseSets;
using HexagonGame.MapGeneration;
using Microsoft.Xna.Framework;

namespace HexagonGame.ECS.Worlds;

/// <summary>
/// Creates a <see cref="OldWorld"/> instance.
/// </summary>
public class WorldBuilder
{
	public OldWorld NewWorld(GameRoot root, int mapSizeX, int mapSizeY)
	{
		var world = new OldWorld();

		// Set up the component holders.
		// Might be good to add auto-resizing to the sparse sets.
		var maxEntities = mapSizeX * mapSizeY + 1000;
		world.PositionComponents = new SparseSet<PositionComponent>(maxEntities);
		world.TileAttributeComponents = new SparseSet<TileAttributeComponent>(maxEntities);
		world.AppearanceComponents = new SparseSet<AppearanceComponent>(maxEntities);
		world.LifecycleComponents = new SparseSet<LifecycleComponent>(1000, maxEntities);
	//	world.NeedsComponents = new SparseSet<NeedsComponent>(1000, maxEntities);
	//	world.ResidentComponents = new SparseSet<ResidentComponent>(1000, maxEntities);
	//	world.IdentityComponents = new SparseSet<IdentityComponent>(maxEntities);
		
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
		
		// Make the first settlement.
		var settlement = world.NewEntity();
		var residentComponent = new ResidentComponent();
	//	world.ResidentComponents.Add(settlement, residentComponent);
		
		// Some starting items.
		for (int i = 0; i < 20; i++)
		{
			var item = world.NewEntity();
			var itemIdentity = new IdentityComponent
			{
				Name = "Food",
				Description = "This is a placeholder."
			};
			//world.IdentityComponents.Add(item, itemIdentity);
		}
		

		// Spawn in the starting population.
		for (var i = 0; i < 10; i++)
		{
			var pop = world.NewEntity();
			var lifecycleComponent = new LifecycleComponent(world.Calendar.AddYears(-20));
			world.LifecycleComponents.Add(pop, lifecycleComponent);
			
			// and their needs.
			var needsComponent = new NeedsComponent();
			
			var foodNeed = new Need();
			foodNeed.RequiredDaily = 2000;
			foodNeed.MaxDeficiency = 21 * foodNeed.RequiredDaily;
			needsComponent.Needs.Add(NeedsTag.Calories, foodNeed);

			var waterNeed = new Need();
			waterNeed.RequiredDaily = 2000;
			waterNeed.MaxDeficiency = 3 * waterNeed.RequiredDaily;
			needsComponent.Needs.Add(NeedsTag.Hydration, waterNeed);
			
			residentComponent.Residents.Add(pop);
		//	world.NeedsComponents.Add(pop, needsComponent);
		}


		return world;
	}
}