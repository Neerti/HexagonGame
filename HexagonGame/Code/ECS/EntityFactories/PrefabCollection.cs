using System;
using System.Collections.Generic;
using System.IO;
using HexagonGame.ECS.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HexagonGame.ECS.EntityFactories;

public class PrefabCollection
{
	public Dictionary<string, Prefab> Prefabs;

	public PrefabCollection()
	{
		SetUp();
	}
	
	private void SetUp()
	{
		var JObjects = LoadJson();
		Prefabs = new Dictionary<string, Prefab>();
		JObjectsToPrefabs(JObjects);
		
	}

	private Dictionary<string, JObject> LoadJson()
	{
		var workingDirectory = Environment.CurrentDirectory;
		var projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName;
		var templateFilePaths = Directory.GetFiles($"{projectDirectory}/Data/Prefabs", "*.json");
		
		var jsonObjects = new Dictionary<string, JObject>();
		
		foreach (var filePath in templateFilePaths)
		{
			try
			{
				using var sr = new StreamReader(filePath);
				var jArray = JsonConvert.DeserializeObject<JToken>(sr.ReadToEnd());
				foreach (var thing in jArray)
				{
					if (thing["Name"] is null)
					{
						continue;
					}
					
					jsonObjects.Add(thing["Name"].ToString(), (JObject)thing);
				}
				
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		return jsonObjects;
	}
	
	private void JObjectsToPrefabs(Dictionary<string, JObject> jObjects)
	{
		foreach (var (key, jObject) in jObjects)
		{
			var prefab = MakePrefab(jObject);
			
			Prefabs.Add(key, prefab);
		}
	}

	private Prefab MakePrefab(JObject jObject)
	{
		// Another part of the prefab code that unfortunately requires updating every time a new component is done.
		// Might be worth looking into a way to avoid needing to do that.
		var prefab = new Prefab();
		if (jObject["Appearance"] != null)
		{
			prefab.AppearanceComponent = JsonConvert.DeserializeObject<AppearanceComponent>(jObject["Appearance"].ToString());
		}
		
		if (jObject["Position"] != null)
		{
			prefab.PositionComponent = JsonConvert.DeserializeObject<PositionComponent>(jObject["Position"].ToString());
		}
		
		if (jObject["TileAttribute"] != null)
		{
			prefab.TileAttributeComponent = JsonConvert.DeserializeObject<TileAttributeComponent>(jObject["TileAttribute"].ToString());
		}

		return prefab;
	}
}