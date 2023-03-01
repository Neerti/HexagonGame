﻿using System;
using HexagonGame.ECS.Components;
using HexagonGame.ECS.EntityFactories;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.SparseSets;
using HexagonGame.ECS.Systems;
using HexagonGame.ECS.Worlds;
using HexagonGame.MapGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame;

public class GameRoot : Game
{
	public GraphicsDeviceManager Graphics;

	public World World;

	public InputSystem InputSystem;
	public RenderingSystem RenderingSystem;
	public CameraSystem CameraSystem;
	public UISystem UISystem;
	public TimeSystem TimeSystem;
	public TextureSystem TextureSystem;

	public EntityFactory EntityFactory;

	public bool Paused = false;
	public TimeSpan FractionalTick;
	public float TickDelay = 1f;
	public float[] TickSpeedOptions = {2f, 1f, 0.5f, 0.1f, 0.01f};
	public int TickSpeedIndex = 1;

	public GameRoot()
	{
		Graphics = new GraphicsDeviceManager(this);
		Graphics.GraphicsProfile = GraphicsProfile.HiDef;
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		IsFixedTimeStep = false;
	}

	protected override void Initialize()
	{
		Window.AllowUserResizing = true;
		Window.Title = "Hexagon";

		// Systems init.
		RenderingSystem = new RenderingSystem();
		CameraSystem = new CameraSystem();
		InputSystem = new InputSystem();
		UISystem = new UISystem();
		TimeSystem = new TimeSystem();
		TextureSystem = new TextureSystem();
		TextureSystem.LoadContent(this);


		// Create the world.
		// Later on this should be part of starting a new game or loading a save file.
		World = new World();

		EntityFactory = new EntityFactory();

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
		
		base.Initialize();
	}

	protected override void LoadContent()
	{
		RenderingSystem.LoadContent(this);
		UISystem.Initialize(this);
	}

	protected override void Update(GameTime gameTime)
	{
		InputSystem.PollForInput(this, gameTime);

		if (!Paused)
		{
			var oldCalendar = World.Calendar;
			// Every 'tick' advances the in-game time by an hour.
			TimeSystem.Tick(this, gameTime);
			if (World.Calendar.Day != oldCalendar.Day)
			{
				// Daily things go here.
				Console.WriteLine("New day!");
			}

			if (World.Calendar.Month != oldCalendar.Month)
			{
				// Monthly things go here.
				Console.WriteLine("New month!");
			}

			if (World.Calendar.Year != oldCalendar.Year)
			{
				// Annual things go here.
				Console.WriteLine("New year!");
			}
		}

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		// Background.
		Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

		// Tile map.
		RenderingSystem.RenderTerrain(World, this);

		// Things on the map.

		// UI.
		UISystem.DrawDebugUI(World, this, gameTime);

		base.Draw(gameTime);
	}
}