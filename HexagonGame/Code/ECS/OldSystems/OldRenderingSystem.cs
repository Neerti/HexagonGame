using System;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HexagonGame.ECS.Systems;

public class OldRenderingSystem
{
	public SpriteBatch SpriteBatch;
	public Texture2D BoundingBoxTexture;

	public bool DrawBoundingBoxes = true;

	public void LoadContent(GameRoot game)
	{
		SpriteBatch = new SpriteBatch(game.GraphicsDevice);
		BoundingBoxTexture = new Texture2D(game.GraphicsDevice, 1, 1);
		BoundingBoxTexture.SetData(new[] {Color.White});
	}

	public void RenderTerrain(OldWorld oldWorld, GameRoot game)
	{
		var viewportBounds = game.GraphicsDevice.Viewport.Bounds;
		var cameraPos = oldWorld.PositionComponents.Get(oldWorld.CameraEntity).Position;

		// Round the camera's position to avoid subpixeling.
		cameraPos = new Vector2(
			(float) Math.Round(cameraPos.X, MidpointRounding.ToZero),
			(float) Math.Round(cameraPos.Y, MidpointRounding.ToZero)
		);

		var topLeftCorner = new Vector2(
			cameraPos.X + -200,
			cameraPos.Y + -200
		);
		var bottomRightCorner = new Vector2(
			cameraPos.X + viewportBounds.Width + 200,
			cameraPos.Y + viewportBounds.Height + 200
		);

		var topLeftCornerTile = oldWorld.Grid.VectorToTileCoordinate(topLeftCorner);
		var bottomRightCornerTile = oldWorld.Grid.VectorToTileCoordinate(bottomRightCorner);

		for (var z = 0; z < EntityGrid.MaxLayers; z++)
		{
			SpriteBatch.Begin(SpriteSortMode.BackToFront);
			for (var x = topLeftCornerTile.xCoordinate; x < bottomRightCornerTile.xCoordinate; x++)
			{
				for (var y = topLeftCornerTile.yCoordinate; y < bottomRightCornerTile.yCoordinate; y++)
				{
					// Get the position component.
					// This adds a branch inside a loop so it probably hurts performance.
					if (!oldWorld.PositionComponents.Contains(oldWorld.Grid.Grid[x, y, z]))
					{
						continue;
					}

					var texturePos = oldWorld.PositionComponents.Get(oldWorld.Grid.Grid[x, y, z]).Position;

					// Offset from the camera.
					texturePos -= cameraPos;

					var entity = oldWorld.Grid.Grid[x, y, z];
					var appearanceComponent = oldWorld.AppearanceComponents.Get(oldWorld.Grid.Grid[x, y, z]);

					// Move the texture down to pretend that the sprite origin is at the bottom left, instead of the top left.
					// This is done to support sprites taller than the tile size.
					//texturePos = new Vector2(texturePos.X, texturePos.Y - appearanceComponent.SpriteTexture.Height);

					var step = 1f / oldWorld.Grid.SizeY;

					var spriteLayer = step * y;
					if ((x & 1) == 1) // Odd tiles are moved down half a step.
					{
						spriteLayer += step / 2;
					}

					spriteLayer = 1 - spriteLayer;

					SpriteBatch.Draw(
						//appearanceComponent.SpriteTexture,
						game.TextureSystem.Textures[appearanceComponent.TextureName],
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
					// Code adapted from https://stackoverflow.com/a/13894313.
					//var rect = appearanceComponent.SpriteTexture.Bounds;
					var rect = game.TextureSystem.Textures[appearanceComponent.TextureName].Bounds;
					var boundingColor = Color.Red;
					var boundingLineSize = 1;
					rect.Offset(texturePos);
					if (rect.Contains(Mouse.GetState().Position))
					{
						boundingColor = Color.Green;
						boundingLineSize++;
					}

					// Left line.
					SpriteBatch.Draw(BoundingBoxTexture, new Rectangle(
							rect.X,
							rect.Y,
							boundingLineSize,
							rect.Height + boundingLineSize),
						boundingColor
					);

					// Top line.
					SpriteBatch.Draw(BoundingBoxTexture, new Rectangle(
							rect.X,
							rect.Y,
							rect.Width + boundingLineSize,
							boundingLineSize),
						boundingColor
					);

					// Right line.
					SpriteBatch.Draw(BoundingBoxTexture, new Rectangle(
							rect.X + rect.Width,
							rect.Y,
							boundingLineSize,
							rect.Height + boundingLineSize),
						boundingColor
					);

					// Bottom line.
					SpriteBatch.Draw(BoundingBoxTexture, new Rectangle(
							rect.X,
							rect.Y + rect.Height,
							rect.Width + boundingLineSize,
							boundingLineSize),
						boundingColor
					);
				}
			}

			SpriteBatch.End();
		}
	}
}