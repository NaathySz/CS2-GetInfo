using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;

namespace GetInfo;

public class GetInfo : BasePlugin
{
    public override string ModuleName => "Get Info";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Nathy";
    public override string ModuleDescription => "Get information about players. Github: https://github.com/NaathySz";

    public override void Load(bool hotReload)
    {
        
    }


    [ConsoleCommand("css_getinfo", "Get player info")]
    [CommandHelper(minArgs: 1, usage: "[target]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/generic")]
    public void OnCommand(CCSPlayerController? caller, CommandInfo command)
    {
        var targetResult = command.GetArgTargetResult(1);
        
        foreach (var player in targetResult.Players)
        {
            if (player.IsValid && !player.IsBot && !player.IsHLTV)
            {
                string IPAddress = player.IpAddress != null ? player.IpAddress.Split(":")[0] : "??";

                string Country = GetPlayerCountry(player);
                string City = GetPlayerCity(player);

                if (caller == null)
                {
                    command.ReplyToCommand($" Name: {player.PlayerName}");
                    command.ReplyToCommand($" SteamID64: {player.SteamID}");
                    command.ReplyToCommand($" Profile: https://steamcommunity.com/profiles/{player.SteamID}");
                    command.ReplyToCommand($" IP: {IPAddress}");
                    command.ReplyToCommand($" Location: {Country} - {City}");

                    return;
                }
                else
                {
                    caller.PrintToChat($" {ChatColors.Red}Name: {ChatColors.Blue}{player.PlayerName}");
                    caller.PrintToChat($" {ChatColors.Red}SteamID64: {ChatColors.Blue}{player.SteamID}");
                    caller.PrintToChat($" {ChatColors.Red}Profile: {ChatColors.Blue}https://steamcommunity.com/profiles/{player.SteamID}");
                    caller.PrintToChat($" {ChatColors.Red}IP {ChatColors.Blue}{IPAddress}");
                    caller.PrintToChat($" {ChatColors.Red}Location: {ChatColors.Blue}{Country} {ChatColors.White}- {ChatColors.Blue}{City}");

                    return;
                }
            }
        }
    }

    public string GetPlayerCountry(CCSPlayerController player)
	{
		string IPAddress = player.IpAddress != null ? player.IpAddress.Split(":")[0] : "??";

		using DatabaseReader reader = new DatabaseReader(Path.Combine(ModuleDirectory, "GeoLite2-Country.mmdb"));
		{
			try
			{
				MaxMind.GeoIP2.Responses.CountryResponse response = reader.Country(IPAddress);
				return response.Country.Name ?? "??";
			}
			catch (AddressNotFoundException)
			{
				return "??";
			}
			catch (GeoIP2Exception)
			{
				return "??";
			}
		}
	}

    public string GetPlayerCity(CCSPlayerController player)
	{
		string IPAddress = player.IpAddress != null ? player.IpAddress.Split(":")[0] : "??";

		using DatabaseReader reader = new DatabaseReader(Path.Combine(ModuleDirectory, "GeoLite2-City.mmdb"));
		{
			try
			{
				MaxMind.GeoIP2.Responses.CityResponse response = reader.City(IPAddress);
				return response.City.Name ?? "??";
			}
			catch (AddressNotFoundException)
			{
				return "??";
			}
			catch (GeoIP2Exception)
			{
				return "??";
			}
		}
	}
}