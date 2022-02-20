using Godot;
using System;

public class Bullet : Node2D {
    public Vector2 _direction = new Vector2(1, 0);
    [Export] public float _speed = 50;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        GlobalPosition += _direction * _speed * delta;

        if ((Player.Singleton.GlobalPosition - GlobalPosition).LengthSquared() < 81f) {
            Player.Singleton.Hit();
            QueueFree();
        }


    }
}
