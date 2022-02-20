using Godot;
using System;

public class Player : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Vector2 _velocity = new Vector2();
    private Vector2 _desiredVelocity = new Vector2();
    [Export] private float _speed = 32;
    [Export] private float _acceleration = 0.1f;
    private Vector2 _arenaMin = new Vector2();
    private Vector2 _arenaMax = new Vector2();
    [Export] private NodePath _arenaMinNode;
    [Export] private NodePath _arenaMaxNode;
    private RichTextLabel _richTextLabelScore;
    [Export] private NodePath _richTextLabelScorePath;
    [Export] private NodePath _skinPath;
    private AnimatedSprite _skin;

    private float HPTimer = 0;
    private int HP = 3;
            int anim = 0;


    private bool _alive = false;
    bool Alive {get{return _alive;} set{
        if (value) {
            timer = 0;
        }
        _alive = value;
    }}

    public static Player Singleton = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        _arenaMin = (GetNode(_arenaMinNode) as Node2D).GlobalPosition;
        _arenaMax = (GetNode(_arenaMaxNode) as Node2D).GlobalPosition;
        _richTextLabelScore = (RichTextLabel)GetNode(_richTextLabelScorePath);
        _skin = (AnimatedSprite)GetNode(_skinPath);
        Singleton = this;
    }

    public void StartGame () {
        BulletSpawner.Singleton.Init();
        Alive = true;
        HP = 3;
    }

    float timer = 0;
    bool started = false;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!Alive) {
            _desiredVelocity = new Vector2();
            return;
        }

        timer += delta;
        Vector2 desiredDirection = new Vector2();
        if (Input.IsKeyPressed((int)Godot.KeyList.W) || Input.IsKeyPressed((int)Godot.KeyList.Up)) {
            desiredDirection.y -= 1;
        }
        if (Input.IsKeyPressed((int)Godot.KeyList.S) || Input.IsKeyPressed((int)Godot.KeyList.Down)) {
            desiredDirection.y += 1;
        }
        if (Input.IsKeyPressed((int)Godot.KeyList.A) || Input.IsKeyPressed((int)Godot.KeyList.Left)) {
            desiredDirection.x -= 1;
        }
        if (Input.IsKeyPressed((int)Godot.KeyList.D) || Input.IsKeyPressed((int)Godot.KeyList.Right)) {
            desiredDirection.x += 1;
        }

        desiredDirection = desiredDirection.Normalized();

        _desiredVelocity = desiredDirection * _speed;

        int minutes = (int)timer / 60;
        int seconds = (int)timer % 60;

        _richTextLabelScore.BbcodeText =  "[center]" + minutes + ":" + seconds;

        float currentSpeed = _velocity.Length() / _speed;
        _skin.Scale = new Vector2(1, (Mathf.Cos(timer * 20) * 0.1f) * currentSpeed + 1);

    }

    public override void _PhysicsProcess(float delta) {
        base._PhysicsProcess(delta);

        _velocity = _velocity * (1f - _acceleration) + _desiredVelocity * _acceleration;
        GlobalPosition += _velocity * delta;
        if (GlobalPosition.x < _arenaMin.x) {
            GlobalPosition = new Vector2(_arenaMin.x, GlobalPosition.y);
            if (_velocity.x < 0) _velocity.x = 0;
        } else if (GlobalPosition.x > _arenaMax.x) {
            GlobalPosition = new Vector2(_arenaMax.x, GlobalPosition.y);
            if (_velocity.x > 0) _velocity.x = 0;
        }

        if (GlobalPosition.y < _arenaMin.y) {
            GlobalPosition = new Vector2(GlobalPosition.x, _arenaMin.y);
            if (_velocity.y < 0) _velocity.y = 0;
        } else if (GlobalPosition.y > _arenaMax.y) {
            GlobalPosition = new Vector2(GlobalPosition.x, _arenaMax.y);
            if (_velocity.y > 0) _velocity.y = 0;
        }

        if (_velocity.x > 1) {
            anim = anim | 0b01;
        } else if (_velocity.x < -1) {
            anim = anim & 0b10;
        }


        if (_velocity.y > 1) {
            anim = anim | 0b10;
        } else if (_velocity.y < -1) {
            anim = anim & 0b01;
        }

        _skin.Frame = anim;
    }

    public void Hit () {
        HP--;
        if (HP <= 0) {
            Die();
        }

        HPTimer = 0.5f;
    }

    public void Die () {
        BulletSpawner.Singleton.Stop();
        Alive = false;
    }
}
