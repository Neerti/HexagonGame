using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HexagonGame.ECS.Systems;

/// <summary>
/// The InputSystem takes player input, and manipulates the game in response.
/// </summary>
public class InputSystem
{
	public void PollForInput(Game1 game, GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
		    Keyboard.GetState().IsKeyDown(Keys.Escape))
			game.Exit();

		// Camera control.
		// This should get spun off into a camera system later on so it can do things like easing.
		var movementDirection = Vector2.Zero;
		var cameraSpeed = 500f;
		
		// TODO: Keybinding system so people can rebind keys.
		if (Keyboard.GetState().IsKeyDown(Keys.W))
		{
			movementDirection += Vector2.UnitY;
		}

		else if (Keyboard.GetState().IsKeyDown(Keys.S))
		{
			movementDirection += -Vector2.UnitY;
		}
		
		if (Keyboard.GetState().IsKeyDown(Keys.A))
		{
			movementDirection += Vector2.UnitX;
		}
		
		else if (Keyboard.GetState().IsKeyDown(Keys.D))
		{
			movementDirection += -Vector2.UnitX;
		}
		
		

		if (movementDirection != Vector2.Zero)
		{
			game.CameraSystem.MoveCamera(game.World, movementDirection, cameraSpeed, gameTime);
			game.World.PositionComponents.Get(game.World.CameraEntity).Position += movementDirection * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

	}
}