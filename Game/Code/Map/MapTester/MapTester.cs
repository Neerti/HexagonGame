using Godot;
using System;
namespace Hexagon
{
	public class MapTester : Node
	{

		public override void _Ready()
		{
			var MapGen = new MapGenerator(2);
			var grid = MapGen.GenerateNewMap(1024, 1024);
			var img = grid.GenerateMapImage();
			img.SavePng("res://height_test.png");
			img = grid.GenerateHeightNoiseImage();
			img.SavePng("res://height_noise.png");
			img = grid.GenerateHeatMap();
			img.SavePng("res://heat_map.png");
		}
		

	}
}

