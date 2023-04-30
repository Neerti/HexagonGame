using System;
using HexagonGame.ECS.EntityFactories;
using HexagonGame.ECS.Systems;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arch.Core;
using HexagonGame.Universes;
using JetBrains.Annotations;

namespace HexagonGame;

public class GameRoot : Game
{
	public GraphicsDeviceManager Graphics;

	

	[CanBeNull] public Universe Universe;

	public OldWorld OldWorld;

	public InputSystem InputSystem;
	public OldRenderingSystem OldRenderingSystem;
	public OldCameraSystem OldCameraSystem;
	public OldUISystem OldUISystem;
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
		Graphics.PreferredBackBufferWidth = 1024;
		Graphics.PreferredBackBufferHeight = 768;
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		IsFixedTimeStep = false;
	}

	protected override void Initialize()
	{
		Window.AllowUserResizing = true;
		Window.Title = "Hexagon";
		Window.ClientSizeChanged += OnResize;

		Universe = new Universe();
		Universe.World = World.Create();
		
		Universe.Initialize(this);
		

		/*// Systems init.
		OldRenderingSystem = new OldRenderingSystem();
		OldCameraSystem = new OldCameraSystem();
		OldUISystem = new OldUISystem();
		TimeSystem = new TimeSystem();
		TextureSystem = new TextureSystem();
		TextureSystem.LoadContent(this);
		LifecycleSystem = new LifecycleSystem();

		EntityFactory = new EntityFactory();
		
		// Create the oldWorld.
		var worldBuilder = new WorldBuilder();
		
		var mapSize = (int) Math.Pow(2, 6);
		OldWorld = worldBuilder.NewWorld(this, mapSize, mapSize);*/


		base.Initialize();
	}

	protected override void LoadContent()
	{
	//	OldRenderingSystem.LoadContent(this);
	//	OldUISystem.Initialize(this);
	}

	protected override void Update(GameTime gameTime)
	{
		float deltaTime = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
		if (Universe != null)
		{
			Universe.UpdateSystems.BeforeUpdate(in gameTime);
			Universe.UpdateSystems.Update(in gameTime);
			Universe.UpdateSystems.AfterUpdate(in gameTime);
		}
		

		/*if (!Paused)
		{
			var oldCalendar = OldWorld.Calendar;
			// Every 'tick' advances the in-game time by an hour.
			TimeSystem.Tick(this, gameTime);
			if (OldWorld.Calendar.Day != oldCalendar.Day)
			{
				// Daily things go here.
				Console.WriteLine("New day!");
				LifecycleSystem.Process(this, OldWorld);
			}

			if (OldWorld.Calendar.Month != oldCalendar.Month)
			{
				// Monthly things go here.
				Console.WriteLine("New month!");
			}

			if (OldWorld.Calendar.Year != oldCalendar.Year)
			{
				// Annual things go here.
				Console.WriteLine("New year!");
			}
		}*/

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
		
		if (Universe != null)
		{
			Universe.DrawSystems.BeforeUpdate(in gameTime);
			Universe.DrawSystems.Update(in gameTime);
			Universe.DrawSystems.AfterUpdate(in gameTime);
		}
		
		
		
		
		
		
		/*
		 *            GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));
		   
		   // Call BeforeLayout first to set things up
		   _imGuiRenderer.BeforeLayout(gameTime);
		   
		   // Draw our UI
		   ImGuiLayout();
		   
		   // Call AfterLayout now to finish up and draw all the things
		   _imGuiRenderer.AfterLayout();
		   
		   base.Draw(gameTime);
		 * 
		 */

		// Background.
	

		// Tile map.
	//	OldRenderingSystem.RenderTerrain(OldWorld, this);

		// Things on the map.

		// UI.
	//	OldUISystem.DrawDebugUI(OldWorld, this, gameTime);

		base.Draw(gameTime);
	}
	
	


	public void OnResize(object sender, EventArgs e)
	{
		Graphics.PreferredBackBufferHeight = GraphicsDevice.Viewport.Height;
		Graphics.PreferredBackBufferWidth = GraphicsDevice.Viewport.Width;
		Graphics.ApplyChanges();
	}

	protected override void EndRun()
	{
		base.EndRun();

		if (Universe == null) return;
		World.Destroy(Universe.World);
		Universe.Cleanup();
	}
}