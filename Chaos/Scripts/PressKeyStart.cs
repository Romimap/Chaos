using Godot;
using System;

public class PressKeyStart : RichTextLabel {

    bool listen = true;
    public static PressKeyStart Singleton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Singleton = this;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    public void Start () {
        Visible = false;
        listen = false;
        Player.Singleton.StartGame();
    }

    public void End () {
        Visible = true;
        listen = true;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!listen) return;

        if (@event is InputEventKey) {
            InputEventKey inputEventKey = (InputEventKey)@event;
            if (inputEventKey.Pressed) {
                Start();
            }
        }
    }
}
