using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

public enum TerrainType
{
	Void, Wall, Floor
}

public enum BattleStates
{
	Setup,
	EnvironmentTurn,
	EntityTurn,
	Win,
	Loss,
}

public partial class Board : Node2D
{
	[ExportCategory("References")]
	[Export] public BoardData BoardData { get; set; }
	[Export] public TileMapLayer TileMap { get; set; }
	[Export] public PlayerController Player { get; set; }
	[Export] public PeekUi PeekUi { get; set; }
	[Export] public Entity[] PlayerEntites { get; set; }
	[Export] public Entity[] BattleMembers { get; set; }

	[ExportCategory("Battle")]
	[Export] public BattleStates State = BattleStates.Setup;
	public List<Entity> TurnOrder = new();

	public override void _Ready()
	{
		var pointer = Pointer.PointerFactory(BoardData);
		Player = PlayerController.PlayerControllerFactory(this, pointer, BoardData);
		Player.ControlledEntites.AddRange(PlayerEntites);

		Player.EntitySelected += PeekUi.OnEntitySelected;
		Player.EntityDeselected += PeekUi.OnEntityDeselected;

		var turnOrder = BattleMembers.ToList();
		turnOrder.Sort();
		GD.Print("BattleMembers sorted by Speed: ");
		for (int i = 0; i < turnOrder.Count; i++)
        {
            GD.Print($"{i + 1}: {turnOrder[i].Name}");
        }
	}


}
