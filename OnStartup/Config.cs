using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OnStartup
{
	public class Config
	{
		public List<string> StartupCommands = new List<string>(); 

		public void Write(string path)
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		public static Config Read(string path)
		{
			return !File.Exists(path)
				? new Config()
				: JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
		}
	}
}
