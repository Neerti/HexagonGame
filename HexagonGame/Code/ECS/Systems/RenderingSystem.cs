using System;
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

	public void RenderTerrain(World world)
	{
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
				// TODO: Move this to camera movement code so it doesn't need to constantly do this every iteration.
				cameraPos = new Vector2(
					(float)Math.Round(cameraPos.X, MidpointRounding.ToZero),
					(float)Math.Round(cameraPos.Y, MidpointRounding.ToZero)
				);
				
				texturePos += cameraPos;
				
				// To add contrast between tiles in the demo.
				// Later on it'll check an AppearanceComponent to determine the color and sprite to use.
				var textureColor = new Color(x * 10 % 255, y * 10 % 255, 255);
				

				// Textures are layered based on their Y position.
				// Things towards the bottom of the screen need to layer over things towards the top.
				
				// Calculate how far a given position is from the lowest Y value used.
				var distanceFromTop = Math.Abs(texturePos.Y - LowestPosition);
				
				// Convert the distance to a number between 0.0f and 1.0f.
				var foo = distanceFromTop / HighestPosition;

				// In MonoGame, 0.0f is the front and 1.0f is the back.
				// So it needs to be inverted for it to layer properly.
				var spriteLayer = 1f - foo;
				
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

	/// <summary>
	/// Finds the highest and lowest position coordinates in use.
	/// </summary>
	/// <param name="world">The ECS World object in use.</param>
	public void CalculateBounds(World world)
	{
		HighestPosition = 0f;
		LowestPosition = 0f;
		
		// For now this only handles Y coordinate things, as that's all that is needed.
		// If needed, X coordinates can also be added.
		for (var i = 0; i < world.PositionComponents.Count; i++)
		{
			var component = world.PositionComponents.Elements[i];
			if (component.Position.Y > HighestPosition)
			{
				HighestPosition = component.Position.Y;
			}
			else if (component.Position.Y < LowestPosition)
			{
				LowestPosition = component.Position.Y;
			}
		}
	}
	
	
}