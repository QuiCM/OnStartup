using System;
using System.IO;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace OnStartup
{
	[ApiVersion(1, 20)]
    public class Plugin : TerrariaPlugin
	{
		private Config _config = new Config();

		public override string Author
		{
			get { return "White"; }
		}

		public override string Description
		{
			get { return "Run commands on startup"; }
		}

		public override string Name
		{
			get { return "OnStartup"; }
		}

		public override Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public Plugin(Main game) : base(game)
		{
		}

		public override void Initialize()
		{
			var path = Path.Combine(TShock.SavePath, "StartupCommands.json");
			(_config = Config.Read(path)).Write(path);
			ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);

			Commands.ChatCommands.Add(new Command("os.startup.run", RunStartup, "runstartup"));
			Commands.ChatCommands.Add(new Command("os.startup.reload", ReloadStartup, "reloadstartup"));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
			}
			base.Dispose(disposing);
		}

		private void RunStartup(CommandArgs args)
		{
			args.Player.SendInfoMessage("Running startup commands.");
			OnPostInitialize(EventArgs.Empty);
			args.Player.SendSuccessMessage("Startup commands have been run.");
		}

		private void ReloadStartup(CommandArgs args)
		{
			var path = Path.Combine(TShock.SavePath, "StartupCommands.json");
			(_config = Config.Read(path)).Write(path);
			args.Player.SendSuccessMessage("Reloaded startup commands. Run them with {0}runstartup",
				TShock.Config.CommandSpecifier);
		}

		private void OnPostInitialize(EventArgs args)
		{
			foreach (var commandWArgs in _config.StartupCommands)
			{
				Commands.HandleCommand(TSPlayer.Server, commandWArgs);
			}
		}
    }
}