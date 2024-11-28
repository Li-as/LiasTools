using System.Collections.Generic;
using UnityEngine.Events;

public static class PlayersRegistry
{
    private static readonly List<IPlayer> _players = new List<IPlayer>();
    private const string _debugStart = "[" + nameof(PlayersRegistry) + "] ";

    public static IReadOnlyList<IPlayer> Players => _players.AsReadOnly();
    
    public static event UnityAction<IPlayer> Registered;
    public static event UnityAction<IPlayer> Unregistered;

    public static void Register(IPlayer player)
    {
        if (_players.Contains(player))
        {
            Logger.LogWarning(_debugStart + $"Can't register! Registry already contains player with ID {player.Id}");
            return;
        }

        _players.Add(player);
        Registered?.Invoke(player);
        Logger.Log(_debugStart + $"Player {player.Id} succesfully registered");
    }

    public static void Unregister(IPlayer player)
    {
        if (_players.Contains(player) == false)
        {
            Logger.LogWarning(_debugStart + $"Can't unregister! Registry doesn't contain player with ID {player.Id}");
            return;
        }

        _players.Remove(player);
        Unregistered?.Invoke(player);
        Logger.Log(_debugStart + $"Player {player.Id} succesfully unregistered");
    }
}
