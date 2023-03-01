using System;
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
	public LifecycleSystem LifecycleSystem;

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
		LifecycleSystem = new LifecycleSystem();

		EntityFactory = new EntityFactory();
		
		// Create the world.
		var worldBuilder = new WorldBuilder();
		
		var mapSize = (int) Math.Pow(2, 6);
		World = worldBuilder.NewWorld(this, mapSize, mapSize);

		
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
				LifecycleSystem.Process(this, World);
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