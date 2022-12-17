using System;
using HexagonGame.ECS.EntityGrids;
using HexagonGame.ECS.Worlds;
using Libraries.Noise;

namespace HexagonGame.MapGeneration;

public class MapGenerator
{
	private FastNoiseLite _heightNoise;

	public MapGenerator(int newSeed)
	{
		_heightNoise = new FastNoiseLite();
		_heightNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
		_heightNoise.SetFractalOctaves(6);
		_heightNoise.SetSeed(newSeed);
		_heightNoise.SetFrequency(0.02f);
	}

	public void ApplyNoise(World world, EntityGrid grid)
	{
		// Larger numbers make the map look bigger. Smaller ones have the opposite effect.
		var noiseSamplingScale = 1f;
		
		for (var x = 0; x < grid.SizeX; x++)
		{
			for (var y = 0; y < grid.SizeY; y++)
			{
				// Sample the noise at smaller intervals.
				var y1 = (float) y;
				var x1 = (float) x / grid.SizeX;
				
				// Three dimensional coordinates, to sample the noise in the shape of a cylinder.
				// This allows for the map to wrap on the sides.
				var noiseX = (float)(Math.Sin(x1 * Math.Tau) / Math.Tau);
				var noiseY = (float)(Math.Cos(x1 * Math.Tau) / Math.Tau);
				var noiseZ = y1;

				// [Cos/Sin]/Tau gives a range of [0, 1]. We scale this by grid.SizeX to get the range of the map.
				noiseX *= grid.SizeX;
				noiseY *= grid.SizeX;
				
				// Then divide by noise_sampling_scale to fine-tune the apparent map size.
				noiseX /= noiseSamplingScale;
				noiseY /= noiseSamplingScale;
				noiseZ /= noiseSamplingScale;

				// Apply height.
				var heightValue = _heightNoise.GetNoise(noiseX, noiseY, noiseZ);
				world.TileAttributeComponents.Get(grid.Grid[x, y]).Height = heightValue;
				Console.WriteLine(heightValue);
			}
		}
	}
	
	
}