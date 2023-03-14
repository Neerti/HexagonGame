using System;
using Arch.Core;
using Arch.System;
using HexagonGame.ECS.Components;
using ImGuiNET;
using ImGuiNET.SampleProgram.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Num = System.Numerics;

namespace HexagonGame.ECS.Systems;

public class UISystem : BaseSystem<World, GameTime>
{
	public SpriteFont Font;
	public SpriteBatch SpriteBatch;
	public GraphicsDevice GraphicsDevice;
	public ImGuiRenderer ImGuiRenderer;
	public UISystem(GameRoot root, World world) : base(world)
	{
		Font = root.Content.Load<SpriteFont>("Fonts/debug_font");
		SpriteBatch = new SpriteBatch(root.GraphicsDevice);
		GraphicsDevice = root.GraphicsDevice;
		ImGuiRenderer = new ImGuiRenderer(root);
		ImGuiRenderer.RebuildFontAtlas();
	}

	public override void Update(in GameTime gameTime)
	{
		ImGuiRenderer.BeforeLayout(gameTime);
		
		ImGuiLayout();
		
		ImGuiRenderer.AfterLayout();
		
		
		/*SpriteBatch.Begin();
		
		var lineY = 10;

		// Graphics.
		var frameRate = Math.Round(1 / deltaTime);
		SpriteBatch.DrawString(Font,
			$"FPS: {frameRate}, Viewport Size: {GraphicsDevice.Viewport.Width}x{GraphicsDevice.Viewport.Height}",
			new Vector2(10, lineY), Color.Black);
		
		// Camera.
		var cameraDesc = new QueryDescription().WithExclusive<Position, Camera>();
		World.Query(in cameraDesc, (ref Position pos, ref Camera cam) =>
			{
				lineY += 15;
				SpriteBatch.DrawString(Font, $"  - Azimuth angle: ({MathHelper.ToDegrees(cam.AzimuthAngle)} degrees)", new Vector2(10, lineY), Color.Black);
				lineY += 15;
				SpriteBatch.DrawString(Font, $"  - Polar angle: ({MathHelper.ToDegrees(cam.PolarAngle)} degrees)", new Vector2(10, lineY), Color.Black);
				lineY += 15;
				SpriteBatch.DrawString(Font, $"  - Radius: ({cam.Radius})", new Vector2(10, lineY), Color.Black);
				
				
				lineY += 15;
				SpriteBatch.DrawString(Font, $"Camera Point: ({pos.WorldPosition.X}, {pos.WorldPosition.Y}, {pos.WorldPosition.Z})", new Vector2(10, lineY),
					Color.Black);
				lineY += 15;
				SpriteBatch.DrawString(Font, $"View Position: ({cam.ViewPosition.X}, {cam.ViewPosition.Y}, {cam.ViewPosition.Z})", new Vector2(10, lineY),
					Color.Black);
			}
		);


		SpriteBatch.End();*/
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
}
/*
	public SpriteFont Font;
	public SpriteBatch SpriteBatch;

	public void Initialize(GameRoot game)
	{
		Font = game.Content.Load<SpriteFont>("Fonts/debug_font");
		SpriteBatch = new SpriteBatch(game.GraphicsDevice);
	}

	public void DrawDebugUI(OldWorld oldWorld, GameRoot game, GameTime gameTime)
	{
		SpriteBatch.Begin();
		var lineY = 10;

		// Graphics.
		var frameRate = Math.Round(1 / (float) gameTime.ElapsedGameTime.TotalSeconds);
		SpriteBatch.DrawString(Font,
			$"FPS: {frameRate}, Viewport Size: {game.GraphicsDevice.Viewport.Width}x{game.GraphicsDevice.Viewport.Height}",
			new Vector2(10, lineY), Color.Black);

		// Camera.
		var cameraPos = oldWorld.PositionComponents.Get(oldWorld.CameraEntity).Position;
		lineY += 15;
		SpriteBatch.DrawString(Font, $"Camera Pos: ({cameraPos.X}, {cameraPos.Y})", new Vector2(10, lineY),
			Color.Black);

		// ECS.
		lineY += 15;
		SpriteBatch.DrawString(Font, $"Entities: {oldWorld.EntityTally}", new Vector2(10, lineY), Color.Black);

		// Map.
		lineY += 15;
		SpriteBatch.DrawString(Font,
			$"Map Size: {oldWorld.Grid.SizeX}x{oldWorld.Grid.SizeY}  ({oldWorld.Grid.SizeX * oldWorld.Grid.SizeY} tiles) ",
			new Vector2(10, lineY),
			Color.Black
		);

		// Mouse control.
		lineY += 15;
		SpriteBatch.DrawString(Font,
			$"Mouse Screen Position: {Mouse.GetState().Position}, Mouse OldWorld Position: {cameraPos + Mouse.GetState().Position.ToVector2()}",
			new Vector2(10, lineY),
			Color.Black);

		// Time.
		lineY += 15;
		SpriteBatch.DrawString(Font,
			$"Paused: {game.Paused} TickDelay: {game.TickDelay}/s  Calendar: {oldWorld.Calendar}",
			new Vector2(10, lineY),
			Color.Black);

		SpriteBatch.End();
	}
*/