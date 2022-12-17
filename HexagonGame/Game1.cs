using HexagonGame.ECS.Components;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.SparseSets;
using HexagonGame.ECS.Systems;
using HexagonGame.ECS.Worlds;
using HexagonGame.MapGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame;

public class Game1 : Game
{
	public GraphicsDeviceManager Graphics;
	public SpriteBatch SpriteBatch;

	public World World;
	public InputSystem InputSystem;
	public RenderingSystem RenderingSystem;
	public CameraSystem CameraSystem;

	public Game1()
	{
		Graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		IsFixedTimeStep = false;
	}

	protected override void Initialize()
	{
		// TODO: Add your initialization logic here
		Window.AllowUserResizing = true;

		// Systems init.
		RenderingSystem = new RenderingSystem();
		CameraSystem = new CameraSystem();
		InputSystem = new InputSystem();

		// Create the world.
		// Later on this should be part of starting a new game or loading a save file.
		World = new World();
		
		// Set up the component holders.
		World.PositionComponents = new SparseSet<PositionComponent>(500000);
		World.TileAttributeComponents = new SparseSet<TileAttributeComponent>(500000);

		// The map.
		World.Grid = new EntityGrid(64, 64);
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
				
				World.TileAttributeComponents.Add(tileEntity, new TileAttributeComponent());
			}
		}
		
		// Generate the map.
		var mapGenerator = new MapGenerator(0);
		mapGenerator.ApplyNoise(World, World.Grid);

		// Set up a basic camera out of a few components.
		World.CameraEntity = World.NewEntity();
		var posComponent = new PositionComponent(position: Vector2.Zero);
		World.PositionComponents.Add(World.CameraEntity, posComponent);

		base.Initialize();
	}

	protected override void LoadContent()
	{
		RenderingSystem.LoadContent(this);
	}

	protected override void Update(GameTime gameTime)
	{
		InputSystem.PollForInput(this, gameTime);
		
		RenderingSystem.CalculateBounds(World);

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		// Background.
		Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

		// Tile map.
		RenderingSystem.RenderTerrain(World);
		
		// Things on the map.
		
		// UI.
		

		base.Draw(gameTime);
	}
}