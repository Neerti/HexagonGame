using System;
using Arch.Core;
using Arch.System;
using HexagonGame.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame.ECS.Systems;

public class UISystem : BaseSystem<World, float>
{
	public SpriteFont Font;
	public SpriteBatch SpriteBatch;
	public GraphicsDevice GraphicsDevice;
	public UISystem(GameRoot root, World world) : base(world)
	{
		Font = root.Content.Load<SpriteFont>("Fonts/debug_font");
		SpriteBatch = new SpriteBatch(root.GraphicsDevice);
		GraphicsDevice = root.GraphicsDevice;
	}

	public override void Update(in float deltaTime)
	{
		SpriteBatch.Begin();
		
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


		SpriteBatch.End();
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