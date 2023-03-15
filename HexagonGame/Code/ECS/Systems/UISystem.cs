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
		
		
		
		//ImGuiLayout();
		DebugUI();

		
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

	private bool showDemo;
	private void DebugUI()
	{
		
		//    ImGuiWindowFlags window_flags = ImGuiWindowFlags_NoDecoration | ImGuiWindowFlags_NoDocking | ImGuiWindowFlags_AlwaysAutoResize | ImGuiWindowFlags_NoSavedSettings | ImGuiWindowFlags_NoFocusOnAppearing | ImGuiWindowFlags_NoNav;
		var flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking |
		                         ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings |
		                         ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;
		ImGui.SetNextWindowPos(new Num.Vector2(10, 10), ImGuiCond.Always);
		ImGui.SetNextWindowBgAlpha(.35f);
		ImGui.Begin("Debug Stats", flags);
		ImGui.Text("Debug Window");
		ImGui.Separator();
		if (ImGui.Button("Show Dear ImGui demo window"))
		{
			showDemo = !showDemo;
			
		}
		if (showDemo)
		{
			ImGui.ShowDemoWindow();
		}

		if (ImGui.CollapsingHeader("Performance"))
		{
			ImGui.Text($"{ImGui.GetIO().Framerate:F1} FPS ({1000f / ImGui.GetIO().Framerate:F3} ms/frame)");
		}

		if (ImGui.CollapsingHeader("ECS"))
		{
			var entityCount = World.CountEntities(new QueryDescription().WithNone<int>());
			ImGui.Text($"{(entityCount).ToString()} entities");
		}
		
		if (ImGui.CollapsingHeader("Camera"))
		{
			var cameraDesc = new QueryDescription().WithExclusive<Position, Camera>();
			World.Query(in cameraDesc, (ref Position pos, ref Camera cam) =>
				{
					ImGui.Text("Translation");
					ImGui.Text($"Camera Position: ({cam.ViewPosition.X:F1}, {cam.ViewPosition.Y:F1}, {cam.ViewPosition.Z:F1})");
					ImGui.Text("Focus Point");
					ImGui.BeginTable("Focus Point", 3, ImGuiTableFlags.SizingStretchProp);
					ImGui.TableNextColumn();
					ImGui.DragFloat("X", ref pos.WorldPosition.X);
					ImGui.TableNextColumn();
					ImGui.DragFloat("Y", ref pos.WorldPosition.Y);
					ImGui.TableNextColumn();
					ImGui.DragFloat("Z", ref pos.WorldPosition.Z);
					ImGui.EndTable();
					
					ImGui.Text("Rotation");
					ImGui.SliderAngle("Azimuth Angle", ref cam.AzimuthAngle, 0f, 360f);
					ImGui.SliderAngle("Polar Angle", ref cam.PolarAngle, 0f, 90f);
					ImGui.Text("Zoom");
					ImGui.SliderFloat("Radius", ref cam.Radius, 0f, 30f);

				}
			);
		}





		ImGui.End();
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