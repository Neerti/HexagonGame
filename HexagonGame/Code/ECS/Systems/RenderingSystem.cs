using System;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame.ECS.Systems;

public class RenderingSystem
{
	public Texture2D HexagonTexture;
	public SpriteBatch SpriteBatch;

	public void LoadContent(Game1 game)
	{
		HexagonTexture = game.Content.Load<Texture2D>("Images/generic_hexagon_64");
		SpriteBatch = new SpriteBatch(game.GraphicsDevice);
	}
	
	public void RenderMap(Game1 game, World world)
	{
		// Mostly a demo. Should be moved into a tile map render system later on.
		SpriteBatch.Begin();
		for (var y = 0; y < world.Grid.SizeX; y++)
		{
			// This is so even and odd rows get drawn separately, to layer properly.
			// Later on it should be rewritten to use the overloaded Draw parameter instead.
			for (var offset = 0; offset < 2; offset++)
			{
				for (var x = offset; x < world.Grid.SizeY; x = x + 2)
				{
					var texturePos = world.PositionComponents.Get(world.Grid.Grid[x, y]).Position;
					// Offset from the camera.
					// Frustum culling could be added later with some math.
					texturePos += world.PositionComponents.Get(world.CameraEntity).Position;
					
					// Round the camera to avoid subpixeling.
					texturePos = new Vector2(
						(float)Math.Round(texturePos.X, MidpointRounding.ToZero),
						(float)Math.Round(texturePos.Y, MidpointRounding.ToZero)
						);
					
					// To add contrast between tiles in the demo.
					// Later on it'll check an AppearanceComponent to determine the color and sprite to use.
					var color = new Color(x * 10 % 255, y * 10 % 255, 255);
				
					SpriteBatch.Draw(HexagonTexture, texturePos, color);
				}
			}
		}

		SpriteBatch.End();
	}
}