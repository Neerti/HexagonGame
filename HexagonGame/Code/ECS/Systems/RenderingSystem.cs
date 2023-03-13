using System;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using HexagonGame.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonGame.ECS.Systems;

public class RenderingSystem : BaseSystem<World, float>
{
    private SpriteBatch _spriteBatch;
    private Model _testModel;
    private Matrix _world = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateRotationZ(MathHelper.ToRadians(30));
    private Matrix _view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.Up);
    private Matrix _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 400f, 0.1f, 100f);
    private GraphicsDevice _graphics;

    public RenderingSystem(GameRoot root, World world) : base(world)
    {
        _spriteBatch = new SpriteBatch(root.GraphicsDevice);
        _testModel = root.Content.Load<Model>("Models/hexagon_flat");
        _graphics = root.GraphicsDevice;
    }
    private QueryDescription _cameraDescription = new QueryDescription().WithAll<Position, Camera>();

    public override void Update(in float deltaTime)
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

        for (var i = 0; i < 5; i++)
        {
            foreach (var mesh in _testModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateTranslation(new Vector3(i * 2, 0, 0)) * Matrix.CreateRotationX(MathHelper.ToRadians(-90));
                    //effect.World = Matrix.CreateTranslation(new Vector3(i, 0, 0));
                    effect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
                    //effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                        (float)_graphics.Viewport.Width / _graphics.Viewport.Height, 0.1f, 500f);
                }
                mesh.Draw();
            }
        }
        

        
        

        base.Update(in deltaTime);
    }
}

/*
// BaseSystem provides several usefull methods for interacting and structuring systems
public class MovementSystem : BaseSystem<World, float>{

    private QueryDescription _desc = new QueryDescription().WithAll<Position, Velocity>();
    public MovementSystem(World world) : base(world) {}
    
    // Can be called once per frame
    public override void Update(in float deltaTime)
    {
        // Run query, can also run multiple queries inside the update
        World.Query(in _desc, (ref Position pos, ref Velocity vel) => {
            pos.X += vel.X;
            pos.Y += vel.Y;
        });  
    }
}
*/