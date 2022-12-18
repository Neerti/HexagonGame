using System;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame.ECS.Systems;

public class UISystem
{
	public SpriteFont Font;
	public SpriteBatch SpriteBatch;
	
	public void Initialize(Game1 game)
	{
		Font = game.Content.Load<SpriteFont>("Fonts/debug_font");
		SpriteBatch = new SpriteBatch(game.GraphicsDevice);
	}
	
	public void DrawDebugUI(World world, Game1 game, GameTime gameTime)
	{
		SpriteBatch.Begin();

		// Graphics.
		var frameRate = Math.Round(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
		SpriteBatch.DrawString(Font, $"FPS: {frameRate}, Viewport Size: {game.GraphicsDevice.Viewport.Width}x{game.GraphicsDevice.Viewport.Height}", new Vector2(10, 10), Color.Black);
		
		// Camera.
		var cameraPos = world.PositionComponents.Get(world.CameraEntity).Position;
		SpriteBatch.DrawString(Font, $"Camera Pos: ({cameraPos.X}, {cameraPos.Y})", new Vector2(10, 25), Color.Black);
		
		// ECS.
		SpriteBatch.DrawString(Font, $"Entities: {world.EntityTally}", new Vector2(10, 40), Color.Black);
		
		// Map.
		SpriteBatch.DrawString(Font, $"Map Size: {world.Grid.SizeX}x{world.Grid.SizeY}  ({world.Grid.SizeX*world.Grid.SizeY} tiles) {world.Calendar}", new Vector2(10, 55), Color.Black);

		SpriteBatch.End();
	}
}