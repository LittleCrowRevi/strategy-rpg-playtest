using System;
using Godot;

public enum Faction
{
    PlayerControlled,
    Friendly,
    Neutral,
    Hostile
}
public partial class FactionComponent : Node
{

    [Export] public Faction Faction { get; private set; } = Faction.Neutral;

    public void ChangeFaction(Faction faction) {
        Faction = faction;
    }
}