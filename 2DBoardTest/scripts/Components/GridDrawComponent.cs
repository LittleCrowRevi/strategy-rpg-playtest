using Godot;
using System;
using System.Collections.Generic;

public partial class GridDrawComponent : Node2D
{
	[ExportGroup("Data Fields")]
	[Export] public BoardData BoardData { get; set; }
	[Export] public TileMapLayer TileMap { get; set; }

	public HashSet<Vector2I> TerrainSet = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var cell in TileMap.GetUsedCells())
		{
			var data = TileMap.GetCellTileData(cell);
			var terrainType = data.GetCustomData("Terrain").AsInt32();
			if (terrainType == (int)TerrainType.Floor)
			{
				TerrainSet.Add(cell);
			}
		}
	}

	public override void _Draw()
	{
		foreach (var coord in TerrainSet)
		{
			var worldCoord = coord * BoardData.CellSize;
			DrawRect(new Rect2(worldCoord, new Vector2(BoardData.CellSize, BoardData.CellSize)), Colors.AliceBlue, false);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		QueueRedraw();
	}
}
