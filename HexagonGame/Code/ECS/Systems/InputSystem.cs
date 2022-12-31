using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HexagonGame.ECS.Systems;

/// <summary>
/// The InputSystem takes player input, and manipulates the game in response.
/// </summary>
public class InputSystem
{
	public KeyboardState OldKeyboardState = new KeyboardState();
	public MouseState OldMouseState;
	public Vector2 FixedTogglePoint;
	
	public void PollForInput(Game1 game, GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
		    Keyboard.GetState().IsKeyDown(Keys.Escape))
			game.Exit();

		// Camera control.
		// This should get spun off into a camera system later on so it can do things like easing.
		
		// Mouse camera control.
		// Click-drag camera movement code adapted from code written by "AlienBuchner" at https://godotengine.org/qa/46892/would-map-navigation-camera-with-middle-mouse#a52576.
		if (IsMiddleMouseButtonJustDown())
		{
			Mouse.SetCursor(MouseCursor.SizeAll);
			FixedTogglePoint = Mouse.GetState().Position.ToVector2();
		}

		if (IsMiddleMouseButtonJustUp())
		{
			// Reset courser to normal shape.
			Mouse.SetCursor(MouseCursor.Arrow);
		}

		if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
		{
			var mousePos = Mouse.GetState().Position.ToVector2();
			var cameraPos = game.World.PositionComponents.Get(game.World.CameraEntity).Position;
			var newPos = new Vector2(
				cameraPos.X - (mousePos.X - FixedTogglePoint.X),
				cameraPos.Y - (mousePos.Y - FixedTogglePoint.Y)
			);
			game.CameraSystem.SetCamera(game.World, newPos);
			FixedTogglePoint = mousePos;
		}

		else
		{
			// Keyboard camera control.
			var movementDirection = Vector2.Zero;
			var cameraSpeed = 500f;
		
			// TODO: Keybinding system so people can rebind keys.
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				movementDirection += -Vector2.UnitY;
			}

			else if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				movementDirection += Vector2.UnitY;
			}
		
			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				movementDirection += -Vector2.UnitX;
			}
		
			else if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				movementDirection += Vector2.UnitX;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
			{
				cameraSpeed *= 2;
			}
		
		

			if (movementDirection != Vector2.Zero)
			{
				game.CameraSystem.MoveCamera(game.World, movementDirection, cameraSpeed, gameTime);
				game.World.PositionComponents.Get(game.World.CameraEntity).Position += movementDirection * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
		}


		// Other keyboard input.
		if (IsKeyJustDown(Keys.Space))
		{
			game.Paused = !game.Paused;
		}

		if (IsKeyJustDown(Keys.OemPlus))
		{
			game.TickSpeedIndex = Math.Min(++game.TickSpeedIndex, game.TickSpeedOptions.Length - 1);
			game.TickDelay = game.TickSpeedOptions[game.TickSpeedIndex];
		}
		
		if (IsKeyJustDown(Keys.OemMinus))
		{
			game.TickSpeedIndex = Math.Max(--game.TickSpeedIndex, 0);
			game.TickDelay = game.TickSpeedOptions[game.TickSpeedIndex];
		}

		OldKeyboardState = Keyboard.GetState();
		OldMouseState = Mouse.GetState();

	}

	/// <summary>
	/// Tests if a key was just pressed. Will only be true once per key press, even if held down.
	/// </summary>
	/// <param name="key"><see cref="Keys"/> enum corresponding to the key to test for.</param>
	/// <returns>True if the key is held down this frame, and it was not held down on the previous frame.</returns>
	public bool IsKeyJustDown(Keys key)
	{
		return Keyboard.GetState().IsKeyDown(key) && OldKeyboardState.IsKeyUp(key);
	}

	/// <summary>
	/// Tests if a key was just released. Will only be true once per key press.
	/// </summary>
	/// <param name="key"><see cref="Keys"/> enum corresponding to the key to test for.</param>
	/// <returns>True if the key is currently up this frame, and it was not up on the previous frame.</returns>
	public bool IsKeyJustUp(Keys key)
	{
		return Keyboard.GetState().IsKeyUp(key) && OldKeyboardState.IsKeyDown(key);
	}
	
	// Sadly the Mouse class doesn't have a generic `IsMouseButton[Down|Up]` like the Keyboard class does.
	// Thus methods for left, middle, and right mouse buttons each.
	
	/// <summary>
	/// Tests if the left mouse button was just clicked. Will only be true once per click, even if held down.
	/// </summary>
	/// <returns>True if the left mouse button is held down this frame,
	/// and it was not held down on the previous frame.</returns>
	public bool IsLeftMouseButtonJustDown()
	{
		return Mouse.GetState().LeftButton == ButtonState.Pressed && OldMouseState.LeftButton == ButtonState.Released;
	}
	
	public bool IsLeftMouseButtonJustUp()
	{
		return Mouse.GetState().LeftButton == ButtonState.Released && OldMouseState.LeftButton == ButtonState.Pressed;
	}
	
	public bool IsMiddleMouseButtonJustDown()
	{
		return Mouse.GetState().MiddleButton == ButtonState.Pressed && OldMouseState.MiddleButton == ButtonState.Released;
	}
	
	public bool IsMiddleMouseButtonJustUp()
	{
		return Mouse.GetState().MiddleButton == ButtonState.Released && OldMouseState.MiddleButton == ButtonState.Pressed;
	}
	
	public bool IsRightMouseButtonJustDown()
	{
		return Mouse.GetState().RightButton == ButtonState.Pressed && OldMouseState.RightButton == ButtonState.Released;
	}
	
	public bool IsRightMouseButtonJustUp()
	{
		return Mouse.GetState().RightButton == ButtonState.Released && OldMouseState.RightButton == ButtonState.Pressed;
	}
	

}