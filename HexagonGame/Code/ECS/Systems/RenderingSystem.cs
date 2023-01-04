using System;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame.ECS.Systems;

public class RenderingSystem
{
	public SpriteBatch SpriteBatch;
	public Texture2D BoundingBoxTexture;

	public bool DrawBoundingBoxes = true;

	public void LoadContent(Game1 game)
	{
		SpriteBatch = new SpriteBatch(game.GraphicsDevice);
		BoundingBoxTexture = new Texture2D(game.GraphicsDevice, 1, 1);
		BoundingBoxTexture.SetData(new[]{Color.White});
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

                    var appearanceComponent = world.AppearanceComponents.Get(world.Grid.Grid[x, y, z]);

                    // Move the texture down to pretend that the sprite origin is at the bottom left, instead of the top left.
                    // This is done to support sprites taller than the tile size.
                    texturePos = new Vector2(texturePos.X, texturePos.Y - appearanceComponent.SpriteTexture.Height);

                    var step = 1f / world.Grid.SizeY;
            		
            		var spriteLayer = step * y;
            		if ((x & 1) == 1) // Odd tiles are moved down half a step.
            		{
            			spriteLayer += step / 2;
            		}
            		
            		spriteLayer = 1 - spriteLayer;
            		
            		SpriteBatch.Draw(
	                    appearanceComponent.SpriteTexture, 
            			texturePos,
            			null,
	                    appearanceComponent.SpriteColor,
            			0f,
            			Vector2.Zero,
            			Vector2.One,
            			SpriteEffects.None,
            			spriteLayer
            			);

                    if (!DrawBoundingBoxes)
                    {
	                    continue;
                    }

                    // Bounding box drawing.
                    var rect = appearanceComponent.SpriteTexture.Bounds;
                    
                    // Left line.
                    SpriteBatch.Draw(BoundingBoxTexture, new Rectangle(
	                    rect.X + (int)texturePos.X,
	                    rect.Y + (int)texturePos.Y,
	                    1,
	                    rect.Height + 1),
	                    Color.Red
	                    );
                    
                    // Top line.
                    SpriteBatch.Draw(BoundingBoxTexture, new Rectangle(
	                    rect.X + (int)texturePos.X,
	                    rect.Y + (int)texturePos.Y,
	                    rect.Width + 1,
	                    1),
	                    Color.Red
	                    );
                    
                    // Right line.
                    SpriteBatch.Draw(BoundingBoxTexture, new Rectangle(
		                    rect.X + (int)texturePos.X + rect.Width,
		                    rect.Y + (int)texturePos.Y,
		                    1,
		                    rect.Height + 1),
	                    Color.Red
                    );
                    
                    // Bottom line.
                    SpriteBatch.Draw(BoundingBoxTexture, new Rectangle(
		                    rect.X + (int)texturePos.X,
		                    rect.Y + (int)texturePos.Y + rect.Height,
		                    rect.Width + 1,
		                    1),
	                    Color.Red
                    );
            		
            	}
            }
            SpriteBatch.End();
		}
		

	}


}


/*
class RectangleSprite
{
    static Texture2D _pointTexture;
    public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
    {
        if (_pointTexture == null)
        {
            _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pointTexture.SetData<Color>(new Color[]{Color.White});
        }

        spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
        spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
        spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
        spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
    }     
}

*/