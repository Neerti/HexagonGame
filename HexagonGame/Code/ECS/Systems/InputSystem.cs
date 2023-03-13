using System;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using HexagonGame.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HexagonGame.ECS.Systems;

public class InputSystem : BaseSystem<World, float>
{
	private GameRoot _game;
	private KeyboardState _oldKeyboardState;
	private MouseState _oldMouseState;
	private Vector2 _fixedTogglePoint;
	public InputSystem(GameRoot root, World world) : base(world)
	{
		_game = root;
	}

	public override void Update(in float deltaTime)
	{
		// Don't do anything if the window isn't focused.
		// Keyboard input isn't received, but mouse input could still come in and result in the game thinking the
		// user is trying to click on something off screen.
		if (!_game.IsActive)
		{
			return;
		}

		if (Keyboard.GetState().IsKeyDown(Keys.Escape))
		{
			_game.Exit();
		}
		
		// Camera control.
		// This should get spun off into a camera system later on so it can do things like easing.

		// Mouse camera control.
		// Click-drag camera movement code adapted from code written by "AlienBuchner" at https://godotengine.org/qa/46892/would-map-navigation-camera-with-middle-mouse#a52576.
		if (IsMiddleMouseButtonJustDown())
		{
			Mouse.SetCursor(MouseCursor.SizeAll);
			_fixedTogglePoint = Mouse.GetState().Position.ToVector2();
		}

		if (IsMiddleMouseButtonJustUp())
		{
			// Reset courser to normal shape.
			Mouse.SetCursor(MouseCursor.Arrow);
		}

		if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
		{
			var mousePos = Mouse.GetState().Position.ToVector2();
			
			var cameraDesc = new QueryDescription().WithExclusive<Position, Camera>();
			World.Query(in cameraDesc, (ref Position pos, ref Camera cam) =>
				{
					var newPos = new Vector2(
						pos.WorldPosition.X - (mousePos.X - _fixedTogglePoint.X),
						pos.WorldPosition.Z - (mousePos.Y - _fixedTogglePoint.Y)
					);
					
					pos.WorldPosition = new Vector3(newPos.X, 0, newPos.Y);
				}
			);
			_fixedTogglePoint = mousePos;
		}

		else
		{
			// Keyboard camera control.
			// Translation.
			var movementDirection = Vector3.Zero;
			var cameraSpeed = 10f;

			// TODO: Keybinding system so people can rebind keys.
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				movementDirection += -Vector3.UnitZ;
			}

			else if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				movementDirection += Vector3.UnitZ;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				movementDirection += -Vector3.UnitX;
			}

			else if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				movementDirection += Vector3.UnitX;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
			{
				cameraSpeed *= 2;
			}

			// Yaw.
			var azimuthDirection = 0;
			if (Keyboard.GetState().IsKeyDown(Keys.Q))
			{
				azimuthDirection = -1;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.E))
			{
				azimuthDirection = 1;
			}

			// Pitch.
			var polarDirection = 0;
			if (Keyboard.GetState().IsKeyDown(Keys.R))
			{
				polarDirection = 1;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.F))
			{
				polarDirection = -1;
			}
			
			// Zoom.
			var radiusDirection = 0;
			if (Keyboard.GetState().IsKeyDown(Keys.T))
			{
				radiusDirection = -1;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.G))
			{
				radiusDirection = 1;
			}

			// This might not be worth checking.
			if (movementDirection != Vector3.Zero || azimuthDirection != 0 || polarDirection != 0 || radiusDirection != 0)
			{
				var cameraDesc = new QueryDescription().WithExclusive<Position, Camera>();
				var f = deltaTime;
				World.Query(in cameraDesc, (ref Position pos, ref Camera cam) =>
					{
						pos.WorldPosition += movementDirection * cameraSpeed * f;
						cam.AzimuthAngle += azimuthDirection * f;
						cam.PolarAngle += polarDirection * f;
						cam.Radius += radiusDirection * f;
					}
				);
			}
		}
		
		_oldKeyboardState = Keyboard.GetState();
		_oldMouseState = Mouse.GetState();
			
	}
	
	
	
		/// <summary>
	/// Tests if a key was just pressed. Will only be true once per key press, even if held down.
	/// </summary>
	/// <param name="key"><see cref="Keys"/> enum corresponding to the key to test for.</param>
	/// <returns>True if the key is held down this frame, and it was not held down on the previous frame.</returns>
	public bool IsKeyJustDown(Keys key)
	{
		return Keyboard.GetState().IsKeyDown(key) && _oldKeyboardState.IsKeyUp(key);
	}

	/// <summary>
	/// Tests if a key was just released. Will only be true once per key press.
	/// </summary>
	/// <param name="key"><see cref="Keys"/> enum corresponding to the key to test for.</param>
	/// <returns>True if the key is currently up this frame, and it was not up on the previous frame.</returns>
	public bool IsKeyJustUp(Keys key)
	{
		return Keyboard.GetState().IsKeyUp(key) && _oldKeyboardState.IsKeyDown(key);
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
		return Mouse.GetState().LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released;
	}

	public bool IsLeftMouseButtonJustUp()
	{
		return Mouse.GetState().LeftButton == ButtonState.Released && _oldMouseState.LeftButton == ButtonState.Pressed;
	}

	public bool IsMiddleMouseButtonJustDown()
	{
		return Mouse.GetState().MiddleButton == ButtonState.Pressed &&
		       _oldMouseState.MiddleButton == ButtonState.Released;
	}

	public bool IsMiddleMouseButtonJustUp()
	{
		return Mouse.GetState().MiddleButton == ButtonState.Released &&
		       _oldMouseState.MiddleButton == ButtonState.Pressed;
	}

	public bool IsRightMouseButtonJustDown()
	{
		return Mouse.GetState().RightButton == ButtonState.Pressed && _oldMouseState.RightButton == ButtonState.Released;
	}

	public bool IsRightMouseButtonJustUp()
	{
		return Mouse.GetState().RightButton == ButtonState.Released && _oldMouseState.RightButton == ButtonState.Pressed;
	}
}