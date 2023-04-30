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

    private Universe _universe;

    public RenderingSystem(GameRoot root, World world) : base(world)
    {
        _spriteBatch = new SpriteBatch(root.GraphicsDevice);
        _testModel = root.Content.Load<Model>("Models/hexagon");
        _graphics = root.GraphicsDevice;
        _universe = root.Universe;
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
        
        var a = (int) Math.Ceiling(cameraTarget.X / MapBuilder.TileHorizontalSpacing);
        var b = (int) Math.Ceiling(cameraTarget.Z / MapBuilder.TileVerticalSpacing);

        const int offset = 32;

        var lowestCorner = (
            Math.Clamp(a - offset, 0, _universe.Map.SizeX),
            Math.Clamp(0, 0, _universe.Map.SizeY),
            Math.Clamp(b - offset, 0, _universe.Map.SizeZ)
        );
        var highestCorner = (
            Math.Clamp(a + offset, 0, _universe.Map.SizeX),
            Math.Clamp(1, 0, _universe.Map.SizeY),
            Math.Clamp(b + offset, 0, _universe.Map.SizeZ)
        );

        for (var x = lowestCorner.Item1; x < highestCorner.Item1; x++)
        {
            for (var y = lowestCorner.Item2; y < highestCorner.Item2; y++)
            {
                for (var z = lowestCorner.Item3; z < highestCorner.Item3; z++)
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
                                effect.FogStart = 45f;
                                effect.FogEnd = 50f;
                                //effect.DiffuseColor = new Vector3((x % 20) / 20f, 1, (z % 20) / 20f);
                                effect.DiffuseColor = entity.Get<Appearance>().ModelColor.ToVector3();
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