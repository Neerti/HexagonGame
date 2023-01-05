using System;
using System.Collections.Generic;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HexagonGame.ECS.Systems;

/// <summary>
/// The InputSystem takes player input, and manipulates the game in response.
/// </summary>
public class InputSystem
{
	private KeyboardState _oldKeyboardState;
	private MouseState _oldMouseState;
	private Vector2 _fixedTogglePoint;

	public void PollForInput(Game1 game, GameTime gameTime)
	{
		// Don't do anything if the window isn't focused.
		// Keyboard input isn't received, but mouse input could still come in and result in the game thinking the
		// user is trying to click on something off screen.
		if (!game.IsActive)
		{
			return;
		}

		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
		    Keyboard.GetState().IsKeyDown(Keys.Escape))
			game.Exit();

		if (IsLeftMouseButtonJustDown())
		{
			var clickedEntity = ResolveMousePositionToEntity(game.World, Mouse.GetState());
			if (clickedEntity != World.NullEntityID)
			{
				game.World.AppearanceComponents.Get(clickedEntity).SpriteColor = Color.Red;
			}
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
			var cameraPos = game.World.PositionComponents.Get(game.World.CameraEntity).Position;
			var newPos = new Vector2(
				cameraPos.X - (mousePos.X - _fixedTogglePoint.X),
				cameraPos.Y - (mousePos.Y - _fixedTogglePoint.Y)
			);
			game.CameraSystem.SetCamera(game.World, newPos);
			_fixedTogglePoint = mousePos;
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
				game.World.PositionComponents.Get(game.World.CameraEntity).Position += movementDirection * cameraSpeed *
					(float) gameTime.ElapsedGameTime.TotalSeconds;
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

		if (IsKeyJustDown(Keys.B))
		{
			game.RenderingSystem.DrawBoundingBoxes = !game.RenderingSystem.DrawBoundingBoxes;
		}

		_oldKeyboardState = Keyboard.GetState();
		_oldMouseState = Mouse.GetState();
	}

	/// <summary>
	/// Picks the entity possessing a position and appearance component, that the mouse is currently hovering over.
	/// </summary>
	/// <param name="world">The <see cref="World"/> that holds all mutable state for the game (or a particular unit test).</param>
	/// <param name="state"><see cref="MouseState"/> of the player's mouse, generally obtained with <see cref="Mouse.GetState()"/>.</param>
	/// <returns>Entity number for what the player has clicked on, or <see cref="World.NullEntityID"/> if no
	/// entities could be resolved.</returns>
	public int ResolveMousePositionToEntity(World world, MouseState state)
	{
		// Click detection is done in three stages.
		// The first stage uses bounding boxes based on each entity's texture, to approximate what could've been 
		// clicked on.
		var mousePosition = state.Position.ToVector2();
		var mouseWorldPosition = mousePosition + world.PositionComponents.Get(world.CameraEntity).Position;
		const float offset = 128f;

		var topLeftPosition = new Vector2(mouseWorldPosition.X - offset, mouseWorldPosition.Y - offset);
		var bottomRightPosition = new Vector2(mouseWorldPosition.X + offset, mouseWorldPosition.Y + offset);

		var topLeftCorner = world.Grid.VectorToTileCoordinate(topLeftPosition);
		var bottomRightCorner = world.Grid.VectorToTileCoordinate(bottomRightPosition);

		var clickCandidates = new List<int>();
		var entityLayers = new Dictionary<int, int>();

		for (var x = topLeftCorner.xCoordinate; x < bottomRightCorner.xCoordinate; x++)
		{
			for (var y = topLeftCorner.yCoordinate; y < bottomRightCorner.yCoordinate; y++)
			{
				for (var layer = 0; layer < EntityGrid.MaxLayers; layer++)
				{
					var entity = world.Grid.Grid[x, y, layer];
					if (entity == World.NullEntityID)
					{
						continue;
					}

					var boundingBox = world.AppearanceComponents.Get(entity).SpriteTexture.Bounds;
					boundingBox.Offset(world.PositionComponents.Get(entity).Position);
					//boundingBox.Offset(0, -boundingBox.Height);
					if (boundingBox.Contains(mouseWorldPosition))
					{
						clickCandidates.Add(entity);
						entityLayers[entity] = layer;
					}
				}
			}
		}

		if (clickCandidates.Count == 0)
		{
			return World.NullEntityID;
		}

		// Second stage narrows down the possible entities with pixel transparency checking, 
		// ruling out objects where the mouse was over a fully transparent pixel.
		// This makes clicking pixel perfect, but is relatively expensive.
		// Fortunately it only needs to check a few sprites due to stage one ruling out most other candidates.

		var pixelPerfectCandidates = new List<int>();

		foreach (var entity in clickCandidates)
		{
			var texture = world.AppearanceComponents.Get(entity).SpriteTexture;
			var rawData = new Color[1];

			var localClickCoordinates = mouseWorldPosition + -world.PositionComponents.Get(entity).Position;
			//	localClickCoordinates =
			//		new Vector2((float)Math.Floor(localClickCoordinates.X), (float)Math.Floor(localClickCoordinates.Y));
			var pixelRect = new Rectangle((int) localClickCoordinates.X, (int) localClickCoordinates.Y, 1, 1);
			texture.GetData(0, pixelRect, rawData, 0, 1);
			Console.WriteLine(rawData[0]);

			// If the pixel is not transparent, it's still in the running.
			if (rawData[0] != new Color())
			{
				pixelPerfectCandidates.Add(entity);
			}
		}

		if (pixelPerfectCandidates.Count == 0)
		{
			Console.WriteLine("No entities in pixel perfect list.");
			return World.NullEntityID;
		}

		// The third stage deals with overlapping sprites, based on a layer priority system.
		// This prevents clicking on a tile if a tree occludes where it was clicked.

		var thing = new Dictionary<int, float>();
		// The rules for priority are;
		//	* UI elements have priority over any map element.
		//	* Entity on a higher entity grid layer have priority over those on a lower layer.
		//		E.g. trees are clicked before tiles if they overlap.
		//	* Entities that are closer towards the bottom of the screen have priority over those towards the top.
		foreach (var entity in pixelPerfectCandidates)
		{
			var entityPosition = world.PositionComponents.Get(entity).Position;
			var entityScore = entityPosition.Y;

			entityScore *= entityLayers[entity] + 1;

			thing[entity] = entityScore;
		}

		var winner = World.NullEntityID;
		var highestScore = float.NegativeInfinity;

		foreach (var pair in thing)
		{
			if (!(pair.Value > highestScore)) continue;
			winner = pair.Key;
			highestScore = pair.Value;
		}

		return winner;
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