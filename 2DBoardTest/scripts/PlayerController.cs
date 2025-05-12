using System;
using System.Collections.Generic;
using Godot;

public partial class PlayerController : Node2D
{
    [ExportCategory("References")]
    [Export] public Entity SelectedEntity { get; set; }
    [Export] public Pointer Pointer { get; set; }
    [Export] public BoardData BoardData { get; set; }

    public List<Node2D> ControlledEntites = new();

    public bool IsMoving = false;

    [Signal] public delegate void EntitySelectedEventHandler(Entity entity);
    [Signal] public delegate void EntityDeselectedEventHandler();

    public static PlayerController PlayerControllerFactory(Node2D parent, Pointer pointer, BoardData boardData)
    {
        var pc = new PlayerController() { Pointer = pointer, BoardData = boardData };
        parent.AddChild(pc);

        pc.AddChild(pointer);
        pointer.TileClicked += pc.OnTileClicked;
        pointer.DeselectTile += pc.OnDeselectTile;

        return pc;
    }

    private void OnDeselectTile(Vector2 position)
    {
        SelectedEntity = null;
        EmitSignal(SignalName.EntityDeselected);
    }

    private void OnTileClicked(Vector2 coords)
    {
        if (IsMoving) return;

        if (SelectedEntity == null)
        {
            TrySelectEntity(coords);
        }
        else
        {
            TryMoveEntity(coords);
        }
    }

    private void TrySelectEntity(Vector2 coords)
    {
        var entity = GetEntityAtTile(coords);
        if (entity != null && ControlledEntites.Contains(entity))
        {
            SelectedEntity = entity;
            GD.Print($"Player selected Entity [{SelectedEntity.Name}]");
            EmitSignal(SignalName.EntitySelected, entity);
        }
    }

    private async void TryMoveEntity(Vector2 coords)
    {
        if (SelectedEntity == null) return;

        float distance = ((coords - SelectedEntity.Position) / BoardData.CellSize).LengthSquared();
        if (distance > SelectedEntity.MoveRange * SelectedEntity.MoveRange) return;

        IsMoving = true;
        SelectedEntity.EmitSignal(Entity.SignalName.Move, SelectedEntity.Position, coords);
        await ToSignal(SelectedEntity, Entity.SignalName.MoveFinished);
        IsMoving = false;
    }

    private Entity GetEntityAtTile(Vector2 coords)
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsPointQueryParameters2D
        {
            Position = new Vector2(coords.X, coords.Y),
            CollisionMask = 1,
            CollideWithAreas = true
        };

        var result = spaceState.IntersectPoint(query, 1);
        return result.Count > 0 ? (Entity)result[0]["collider"] : null;
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {

        if (SelectedEntity == null || IsMoving) return;

        var range = SelectedEntity.MoveRange;
        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                var squared = dy * dy + dx * dx;
                if (squared > range * range) continue;
                var position = new Vector2(dx * BoardData.CellSize, dy * BoardData.CellSize) - GlobalPosition + SelectedEntity.GlobalPosition;
                DrawRect(new Rect2(position, new Vector2(BoardData.CellSize, BoardData.CellSize)), new Color(0.5f, 0.5f, 0.9f, 0.5f));
            }
        }
    }
}