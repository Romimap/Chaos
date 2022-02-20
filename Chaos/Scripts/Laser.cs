using Godot;
using System;

public class Laser : Bullet
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    AnimatedSprite animatedSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
           LookAt(GlobalPosition + _direction);
           Scale = new Vector2(100, 1);
           animatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
           animatedSprite.Animation = "Warmup";
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    float timer = 1;
    bool animSet = false;
    public override void _Process(float delta)
    {
        timer -= delta;
        if (!animSet && timer < 0) {
            animatedSprite.Animation = "Shoot";
            animSet = true;
            if (IsColliding()) Player.Singleton.Hit();
        } else if (timer < -0.4) {
            QueueFree();
        }
    }

    bool IsColliding () {
        Vector2 O = GlobalPosition;
        Vector2 D = _direction;
        for (int i = 0; i < 10; i++) {
            float d = (O - Player.Singleton.GlobalPosition).Length();
            if (d <= 9) return true;
            O += D * d;
        }
        return false;
    }
}
