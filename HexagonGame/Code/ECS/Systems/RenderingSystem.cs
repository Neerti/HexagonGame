using System;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using HexagonGame.ECS.Components;
using HexagonGame.Maps;
using HexagonGame.Universes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame.ECS.Systems;

public class RenderingSystem : BaseSystem<World, GameTime>
{
    private SpriteBatch _spriteBatch;
    private Model _testModel;
    private Matrix _world = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateRotationZ(MathHelper.ToRadians(30));
    private Matrix _view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.Up);
    private Matrix _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 400f, 0.1f, 100f);
    private GraphicsDevice _graphics;


    private float _tileSize = .5f;
    private float _tileHeight;
    private float _tileWidth;
    private float _tileHorizontalSpacing;
    private float _tileVerticalSpacing;

    private Universe _universe;

    public RenderingSystem(GameRoot root, World world) : base(world)
    {
        _spriteBatch = new SpriteBatch(root.GraphicsDevice);
        _testModel = root.Content.Load<Model>("Models/hexagon");
        _graphics = root.GraphicsDevice;
        _universe = root.Universe;
    }
    private QueryDescription _cameraDescription = new QueryDescription().WithAll<Position, Camera>();

    public override void Initialize()
    {
        _tileHeight = (float) (Math.Sqrt(3) * _tileSize);
        _tileWidth = 2 * _tileSize;
        _tileHorizontalSpacing = (3f / 4f) * _tileWidth;
        _tileVerticalSpacing = _tileHeight;
    }

    public override void Update(in GameTime gameTime)
    {
        Vector3 cameraPosition = default;
        Vector3 cameraTarget = default;
        
        var cameraDesc = new QueryDescription().WithExclusive<Position, Camera>();
        World.Query(in cameraDesc, (ref Position pos, ref Camera cam) =>
            {
                cameraPosition = cam.ViewPosition;
                cameraTarget = pos.WorldPosition;
            }
        );

        _graphics.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
        for (var x = 0; x < _universe.Map.SizeX; x++)
        {
            for (var y = 0; y < _universe.Map.SizeY; y++)
            {
                for (var z = 0; z < _universe.Map.SizeZ; z++)
                {
                    for (var l = 0; l < LogicalMap.MaxLayers; l++)
                    {
                        var entity = _universe.Map.Grid[x, y, z, l];
                        if (!entity.Has<Position, Appearance>())
                        {
                            continue;
                        }
                        foreach (var mesh in _testModel.Meshes)
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                effect.World = Matrix.CreateTranslation(World.Get<Position>(entity).WorldPosition);
                                effect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
                                effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                                    (float) _graphics.Viewport.Width / _graphics.Viewport.Height,
                                    0.1f,
                                    500f);
                                effect.EnableDefaultLighting();
                                effect.FogEnabled = true;
                                effect.FogColor = Color.CornflowerBlue.ToVector3();
                                effect.FogStart = 30f;
                                effect.FogEnd = 50f;
                            }
                            mesh.Draw();
                        }
                        
                    }
                }
            }
        }
        
        
        /*for (var i = 0; i < 20; i++)
        {
            for (var j = 0; j < 20; j++)
            {
                foreach (var mesh in _testModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        var newX = i * _tileHorizontalSpacing;
                        var newZ = (j * _tileVerticalSpacing) + (i % 2 == 0 ? _tileVerticalSpacing * 0.5f : 0);
                        effect.World = Matrix.CreateTranslation(new Vector3(newX, 0, newZ));
                        effect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                            (float)_graphics.Viewport.Width / _graphics.Viewport.Height, 0.1f, 500f);
                        effect.EnableDefaultLighting();
                        effect.FogEnabled = true;
                        effect.FogColor = Color.CornflowerBlue.ToVector3();
                        effect.FogStart = 15f;
                        effect.FogEnd = 20f;
                    }
                    mesh.Draw();
                }
            }

        }*/
    }
}