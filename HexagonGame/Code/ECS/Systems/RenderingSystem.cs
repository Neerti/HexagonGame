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

	public void RenderTerrain(World world, Game1 game)
	{
		var viewportBounds = game.GraphicsDevice.Viewport.Bounds;
		var cameraPos = world.PositionComponents.Get(world.CameraEntity).Position;
		
		// Round the camera's position to avoid subpixeling.
		cameraPos = new Vector2(
			(float)Math.Round(cameraPos.X, MidpointRounding.ToZero),
			(float)Math.Round(cameraPos.Y, MidpointRounding.ToZero)
		);

		var topLeftCorner = new Vector2(
			cameraPos.X + -200, 
			cameraPos.Y + -200
			);
		var bottomRightCorner = new Vector2(
			cameraPos.X + viewportBounds.Width + 200,
			cameraPos.Y + viewportBounds.Height + 200
			);

		var topLeftCornerTile = world.Grid.VectorToTileCoordinate(topLeftCorner);
		var bottomRightCornerTile = world.Grid.VectorToTileCoordinate(bottomRightCorner);

		for (var z = 0; z < EntityGrid.MaxLayers; z++)
		{
			SpriteBatch.Begin(SpriteSortMode.BackToFront);
            for (var x = topLeftCornerTile.xCoordinate; x < bottomRightCornerTile.xCoordinate; x++)
            {
            	for (var y = topLeftCornerTile.yCoordinate; y < bottomRightCornerTile.yCoordinate; y++)
            	{
            		// Get the position component.
                    // This adds a branch inside a loop so it probably hurts performance.
                    if (!world.PositionComponents.Contains(world.Grid.Grid[x, y, z]))
                    {
	                    continue;
                    }
            		var texturePos = world.PositionComponents.Get(world.Grid.Grid[x, y, z]).Position;
    
            		// Offset from the camera.
            		texturePos -= cameraPos;
    
            		// Debug coloring.
            		var textureColor = Color.White;
            		if (x % 10 == 0)
            		{
            			textureColor = Color.Blue;
            			if (y % 10 == 0)
            			{
            				textureColor = Color.Yellow;
            			}
            		}
            		else if (y % 10 == 0)
            		{
            			textureColor = Color.Green;
            		}
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


}