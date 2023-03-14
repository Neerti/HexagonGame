using System;
using HexagonGame.ECS.EntityFactories;
using HexagonGame.ECS.Systems;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arch.Core;
using HexagonGame.Universes;
using ImGuiNET;
using ImGuiNET.SampleProgram.XNA;
using Num = System.Numerics;
using JetBrains.Annotations;

namespace HexagonGame;

public class GameRoot : Game
{
	public GraphicsDeviceManager Graphics;

	public ImGuiRenderer ImGuiRenderer;

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

		ImGuiRenderer = new ImGuiRenderer(this);
		ImGuiRenderer.RebuildFontAtlas();

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
			Universe.UpdateSystems.BeforeUpdate(in deltaTime);
			Universe.UpdateSystems.Update(in deltaTime);
			Universe.UpdateSystems.AfterUpdate(in deltaTime);
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
		ImGuiRenderer.BeforeLayout(gameTime);
		float deltaTime = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

		Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
		
		if (Universe != null)
		{
			Universe.DrawSystems.BeforeUpdate(in deltaTime);
			Universe.DrawSystems.Update(in deltaTime);
			Universe.DrawSystems.AfterUpdate(in deltaTime);
		}
		
		
		ImGuiLayout();
		
		ImGuiRenderer.AfterLayout();
		
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
	
	
	// Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
	private float f = 0.0f;

	private bool show_test_window = false;
	private bool show_another_window = false;
	private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
	private byte[] _textBuffer = new byte[100];
	
	protected virtual void ImGuiLayout()
	{
		// 1. Show a simple window
		// Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
		{
			ImGui.Text("Hello, world!");
			ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
			ImGui.ColorEdit3("clear color", ref clear_color);
			if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
			if (ImGui.Button("Another Window")) show_another_window = !show_another_window;
			ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

			ImGui.InputText("Text input", _textBuffer, 100);

			ImGui.Text("Texture sample");
			//ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
		}

		// 2. Show another simple window, this time using an explicit Begin/End pair
		if (show_another_window)
		{
			ImGui.SetNextWindowSize(new Num.Vector2(200, 100), ImGuiCond.FirstUseEver);
			ImGui.Begin("Another Window", ref show_another_window);
			ImGui.Text("Hello");
			ImGui.End();
		}

		// 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
		if (show_test_window)
		{
			ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
			ImGui.ShowDemoWindow(ref show_test_window);
		}
	}

	public void OnResize(object sender, EventArgs e)
	{
		Graphics.PreferredBackBufferHeight = GraphicsDevice.Viewport.Height;
		Graphics.PreferredBackBufferWidth = GraphicsDevice.Viewport.Width;
		Graphics.ApplyChanges();
	}
}