using System;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame.ECS.Systems;

public class RenderingSystem
{
	public Texture2D HexagonTexture;
	public SpriteBatch SpriteBatch;

	public float HighestPosition;
	public float LowestPosition;

	public void LoadContent(Game1 game)
	{
		HexagonTexture = game.Content.Load<Texture2D>("Images/generic_hexagon_64");
		SpriteBatch = new SpriteBatch(game.GraphicsDevice);
	}

	public void RectangleToGrid(World world, Rectangle rect)
	{
		rect.Inflate(rect.Width * 0.2f, rect.Height * 0.2f);

		var cameraPos = world.PositionComponents.Get(world.CameraEntity).Position;

		var gridRect = new Rectangle(
			(int) cameraPos.X, (int) cameraPos.Y,
			world.Grid.SizeX * EntityGrid.TileSpriteWidth,
			world.Grid.SizeY * EntityGrid.TileSpriteHeight
		);
		
		Console.WriteLine(cameraPos);
		var foo = world.Grid.VectorToTileCoordinate(cameraPos);
		Console.WriteLine(foo);
		
		var intersectRect = Rectangle.Intersect(gridRect, rect);
		Console.WriteLine(intersectRect);

		var topLeftCorner = new Vector2(intersectRect.Top, intersectRect.Left);
		var bottomRightCorner = new Vector2(intersectRect.Bottom, intersectRect.Right);
		Console.WriteLine(topLeftCorner);
		

		var gridWidth = rect.Width / EntityGrid.TileSpriteWidth;
		var gridHeight = rect.Height / EntityGrid.TileSpriteHeight;

		var gridStartX = Math.Abs(rect.X) / EntityGrid.TileSpriteWidth;
		var gridStartY = Math.Abs(rect.Y) / EntityGrid.TileSpriteWidth;
	}

	public void RenderTerrain(World world, Game1 game)
	{
		var viewportBounds = game.GraphicsDevice.Viewport.Bounds;
		RectangleToGrid(world, viewportBounds);
		
		
		SpriteBatch.Begin(SpriteSortMode.BackToFront);
		// Later on, it will be better to iterate over a rectangular 'slice' of the grid instead of the whole grid. 
		for (var x = 0; x < world.Grid.SizeX; x++)
		{
			for (var y = 0; y < world.Grid.SizeY; y++)
			{
				// Get the position component.
				var texturePos = world.PositionComponents.Get(world.Grid.Grid[x, y]).Position;
				
				// Offset from the camera.
				// Frustum culling could be added later with some math.
				var cameraPos = world.PositionComponents.Get(world.CameraEntity).Position;
				
				// Round the camera's position to avoid subpixeling.
				cameraPos = new Vector2(
					(float)Math.Round(cameraPos.X, MidpointRounding.ToZero),
					(float)Math.Round(cameraPos.Y, MidpointRounding.ToZero)
				);
				
				texturePos -= cameraPos;

				var textureColor = Color.White;
				if (x == 0 && y == 0)
				{
					textureColor = Color.Red;
				}

				var step = 1f / world.Grid.SizeY;
				
				var spriteLayer = step * y;
				if ((x & 1) == 1) // Odd tiles are moved down half a step.
				{
					spriteLayer += step / 2;
				}
				
				spriteLayer = 1 - spriteLayer;
				
				SpriteBatch.Draw(
					HexagonTexture, 
					texturePos,
					null,
					textureColor,
					0f,
					Vector2.Zero,
					Vector2.One,
					SpriteEffects.None,
					spriteLayer
					);
				
			}
		}
		SpriteBatch.End();
	}


}