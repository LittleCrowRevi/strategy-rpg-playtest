using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Entity : Area2D, IComparable<Entity>
{
	[ExportCategory("References")]
	[Export] public Sprite2D Sprite { get; set; }

	[ExportCategory("Game Properties")]
	[Export] public int MoveRange = 5;
	[Export] public int Speed = 10;
	[Export] public FactionComponent FactionComponent { get; set; }

	[Signal] public delegate void MoveFinishedEventHandler(Vector2 origin, Vector2 target);
	[Signal] public delegate void MoveEventHandler(Vector2 origin, Vector2 target);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Move += OnMove;
	}

	public async void OnMove(Vector2 origin, Vector2 target)
	{
		List<Vector2> path = new();

		HorizontalPath(path, origin, target);
		VerticalPath(path, new Vector2(target.X, origin.Y), target);

		Sprite.FlipH = target.X < origin.X;

		for (int i = 0; i < path.Count; i++)
		{
			var node = path[i];
			Position += node;
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		}

		Position = target;
		EmitSignal(SignalName.MoveFinished);
	}

	[Export] public int Step = 1;

	//TODO: if step is too high or irregular the loop can go endlessly with the current condition as the != could be skipped
	// best would be to change it to account for that
	public void HorizontalPath(List<Vector2> path, Vector2 from, Vector2 to)
	{
		int hstart = (int)from.X;
		int hend = (int)to.X;

		var step = hstart < hend ? Step : -Step;
		for (int i = hstart; i != hend; i += step)
		{
			var node = new Vector2(step, 0);
			path.Add(node);
		}
	}

	public void VerticalPath(List<Vector2> path, Vector2 from, Vector2 to)
	{
		int vstart = (int)from.Y;
		int vend = (int)to.Y;

		var step = vstart < vend ? Step : -Step;
		for (int i = vstart; i != vend; i += step)
		{
			var node = new Vector2(0, step);
			path.Add(node);
		}
	}

	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event.IsActionPressed("mouse_left"))
		{
			GD.Print(GetGlobalMousePosition());
		}
	}

    public int CompareTo(Entity other)
    {
		if (this.Speed < other.Speed) return 1;
		if (this.Speed > other.Speed) return -1;
		return 0;
    }

}
