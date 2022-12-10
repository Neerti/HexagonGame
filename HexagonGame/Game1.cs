using System;
using HexagonGame.ECS.Components;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.SparseSets;
using HexagonGame.ECS.Systems;
using HexagonGame.ECS.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame;

public class Game1 : Game
{
	private Texture2D _hexagonTexture;

	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	public World World;
	public int CameraEntity;
	private InputSystem _inputSystem;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		IsFixedTimeStep = false;
	}

	protected override void Initialize()
	{
		// TODO: Add your initialization logic here
		Window.AllowUserResizing = true;

		// Create the world.
		// Later on this should be part of starting a new game or loading a save file.
		World = new World();
		
		// Set up the component holders.
		World.PositionComponents = new SparseSet<PositionComponent>(500000);

		// The map.
		World.Grid = new EntityGrid(256, 256);
		World.Grid.PopulateGrid(World);
		
		// These should go elsewhere later.
		var SpriteWidth = 49;
		var SpriteHeight = 32;

		for (var i = 0; i < World.Grid.SizeX; i++)
		{
			for (var j = 0; j < World.Grid.SizeY; j++)
			{
				var tileEntity = World.Grid.Grid[i, j];
				var newX = i * SpriteWidth;
				var newY = j * SpriteHeight;
				if ((i & 1) == 1) // Odd numbers are moved down by half.
				{
					newY += SpriteHeight / 2;
				}
				World.PositionComponents.Add(tileEntity, new PositionComponent(newX, newY));
			}
		}

		// Systems init.
		_inputSystem = new InputSystem();
		
		// Set up a basic camera out of a few components.
		CameraEntity = World.NewEntity();
		var posComponent = new PositionComponent(position: Vector2.Zero);
		World.PositionComponents.Add(CameraEntity, posComponent);

		base.Initialize();
	}

	protected override void LoadContent()
	{
		// Create a new SpriteBatch, which can be used to draw textures.
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		// TODO: use this.Content to load your game content here
		_hexagonTexture = Content.Load<Texture2D>("Images/generic_hexagon_64");
	}

	protected override void Update(GameTime gameTime)
	{
		_inputSystem.PollForInput(this, gameTime);
		

		// TODO: Add your update logic here
		
		//float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
		//Console.WriteLine(frameRate);

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		_graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

		// TODO: Add your drawing code here

		// Mostly a demo. Should be moved into a tile map render system later on.
		_spriteBatch.Begin();
		for (var y = 0; y < World.Grid.SizeX; y++)
		{
			// This is so even and odd rows get drawn separately, to layer properly.
			// Later on it should be rewritten to use the overloaded Draw parameter instead.
			for (var offset = 0; offset < 2; offset++)
			{
				for (var x = offset; x < World.Grid.SizeY; x = x + 2)
				{
					var texturePos = World.PositionComponents.Get(World.Grid.Grid[x, y]).Position;
					// Offset from the camera.
					// Frustum culling could be added later with some math.
					texturePos += World.PositionComponents.Get(CameraEntity).Position;
					texturePos = new Vector2((float)Math.Round(texturePos.X, MidpointRounding.ToZero), (float)Math.Round(texturePos.Y, MidpointRounding.ToZero));
					
					// To add contrast between tiles in the demo.
					var color = new Color(x * 10 % 255, y * 10 % 255, 255);
				
					_spriteBatch.Draw(_hexagonTexture, texturePos, color);
				}
			}
		}

		_spriteBatch.End();

		base.Draw(gameTime);
	}
}