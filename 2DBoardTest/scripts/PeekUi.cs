using Godot;
using System;

public partial class PeekUi : Control
{
	[Export] public RichTextLabel FactionLabel { get; set; }
	[Export] public Entity SelectedEntity { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (SelectedEntity != null)
		{
			if (SelectedEntity.FactionComponent == null)
			{
				GD.PushWarning("Missing Faction Component at: " + SelectedEntity.Name);
			}
			FactionLabel.Text = SelectedEntity.FactionComponent.Faction.ToString();
			Show();
		}
		else
		{
			FactionLabel.Text = "";
			Hide();
		}
	}

	public void OnEntitySelected(Entity entity)
	{
		SelectedEntity = entity;
	}

	internal void OnEntityDeselected()
	{
		SelectedEntity = null;
	}

}
