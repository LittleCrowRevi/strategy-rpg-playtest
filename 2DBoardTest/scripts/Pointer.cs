using Godot;
using System;
using System.Net;

public partial class Pointer : Node2D
{
	[ExportCategory("References")]
	[Export] public BoardData BoardData { get; set; }

	[Signal] public delegate void TileClickedEventHandler(Vector2 position);
	[Signal] public delegate void DeselectTileEventHandler(Vector2 position);

	public bool IsMoving = false;

	public Vector2 PointerPos = Vector2.Zero;

	public static Pointer PointerFactory(BoardData boardData)
	{
		return new Pointer() { BoardData = boardData };
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SnapToGrid();
		QueueRedraw();
	}
	public override void _Draw()
	{
		DrawRect(new Rect2(Vector2.Zero, new Vector2(BoardData.CellSize, BoardData.CellSize)), new Color(0.9f, 0.9f, 0.9f, 0.6f));
	}
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(InputRef.MOUSE_RIGHT)) EmitSignal(SignalName.DeselectTile, PointerPos);

		if (@event.IsActionPressed(InputRef.MOUSE_LEFT))
		{
			InteractClick();
		}

		if (@event.IsActionPressed(InputRef.UP))
		{
			Engine.MaxFps += 10;
		}
		if (@event.IsActionPressed(InputRef.DOWN))
		{
			Engine.MaxFps -= 10;
		}
	}

	public void InteractClick()
	{
		EmitSignal(SignalName.TileClicked, PointerPos);
	}

	private void SnapToGrid()
	{
		var mousePos = GetGlobalMousePosition();

		PointerPos = BoardData.SnapWorldToGridCoords(mousePos);

		GlobalPosition = PointerPos;
	}
}
