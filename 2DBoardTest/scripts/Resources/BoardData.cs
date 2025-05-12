using Godot;
using System;

public partial class BoardData : Resource
{
    [Export] public int CellSize = 16;

    public Vector2 WorldToGridCell(Vector2 originPos) {
        return (originPos / CellSize).Floor();
    }

    public Vector2 SnapWorldToGridCoords(Vector2 originPos) {
        return (originPos / CellSize).Floor() * CellSize;
    }
}
