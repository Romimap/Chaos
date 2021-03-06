using Godot;
using System;

public class BulletSpawner : Node
{
    private Random _rng = new Random();
    private PackedScene _bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/Bullet.tscn");
    private PackedScene _laserScene = (PackedScene)ResourceLoader.Load("res://Scenes/Laser.tscn");
    private PackedScene _usedBulletScene;
    private Vector2 _arenaMin = new Vector2();
    private Vector2 _arenaMax = new Vector2();
    [Export] private NodePath _arenaMinNode;
    [Export] private NodePath _arenaMaxNode;
    private AnimationPlayer _animationPlayer;
    [Export] private NodePath _animationPlayerNode;
    [Export] private NodePath _levelRTL;
    private RichTextLabel _levelRichTextLabel;

    public static BulletSpawner Singleton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        _arenaMin = (GetNode(_arenaMinNode) as Node2D).GlobalPosition;
        _arenaMax = (GetNode(_arenaMaxNode) as Node2D).GlobalPosition;
        _animationPlayer = (AnimationPlayer)GetNode(_animationPlayerNode);
        _levelRichTextLabel = (RichTextLabel)GetNode(_levelRTL);
        Singleton = this;
    }

    float timer = 1;
    float timerValue = 3;
    float levelTimer = 20;
    bool stop = true;
    int level = 1;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (stop) return;

        timer -= delta;
        levelTimer -= delta;
        if (levelTimer < 0) {
            _animationPlayer.Play("EndLevelAnimation");
            _levelRichTextLabel.BbcodeText = "[center]Level " + level;
            stop = true;
            Player.Singleton._bubbleAnimationPlayer.Play("OMG");

        } else if (timer < 0) {
            timer = timerValue;

            if (_rng.NextDouble() > 2f / (float)level) {
                _usedBulletScene = _laserScene;
            } else {
                _usedBulletScene = _bulletScene;
            }

            switch(_rng.Next(0, 3)) {
                case 0:
                    RadialSpawn();
                    break;
                case 1:
                    WallSpawn();
                    break;
                case 2:
                    CornerSpawn();
                    break;
            }
        }
    }

    public void Init () {
        timer = 1;
        timerValue = 3;
        levelTimer = 20;
        stop = true;
        level = 1;
        _levelRichTextLabel.BbcodeText = "[center]Level " + level;
        _animationPlayer.Play("StartGameAnimation");
        Player.Singleton._bubbleAnimationPlayer.Play("OMG");

    }

    public void StartLevel () {
        levelTimer = 20;
        timerValue *= 0.66f;
        timer = 0.5f;
        stop = false;
        level++;

    }

    public void Stop () {
        stop = true;
        _animationPlayer.Play("GameOverAnimation");
    }

    public void Hit () {
        if (_animationPlayer.IsPlaying()) return;
        _animationPlayer.Play("Hit");
    }

    private void RadialSpawn() {
        int bullets = 3 + _rng.Next(0, 4);
        float globalOffsetAngle = (float)_rng.NextDouble()* Mathf.Pi * 2;
        float offsetAngle = 0;
        if (_rng.Next(0, 3) != 0) { //1 out of 3 will have 0 offset angle
            offsetAngle = ((float)_rng.NextDouble() - 0.5f) * Mathf.Pi * 0.5f;
        }

        float hipo = Mathf.Sqrt(Mathf.Pow(_arenaMax.x - _arenaMin.x, 2) + Mathf.Pow(_arenaMax.y - _arenaMin.y, 2)) / 2;
        Vector2 arenaMid = (_arenaMin + _arenaMax) / 2;
        for (int i = 0; i < bullets; i++) {
            Bullet bullet = (Bullet)_usedBulletScene.Instance();
            float angle = (float)i / (float)bullets * Mathf.Pi * 2;
            angle += globalOffsetAngle;
            Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
            bullet.GlobalPosition = arenaMid + (hipo * -direction);

            angle += offsetAngle;
            bullet._direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));

            AddChild(bullet);
        }
    }

    private void WallSpawn() {
        int wall = _rng.Next(0, 4);
        if (wall == 0) { //L or R
            int bullets = 2 + _rng.Next(0, 2);
            for (int i = 0; i < bullets; i++) {
                Bullet bullet = (Bullet)_usedBulletScene.Instance();
                bullet._direction = new Vector2(-1, 0);
                float t = (float)(i + 1) / (float)(bullets + 1);
                float y = _arenaMin.y * t + _arenaMax.y * (1 - t);
                bullet.GlobalPosition = new Vector2(_arenaMax.x, y);

                AddChild(bullet);
            }
        } else if (wall == 1) {
            int bullets = 2 + _rng.Next(0, 2);
            for (int i = 0; i < bullets; i++) {
                Bullet bullet = (Bullet)_usedBulletScene.Instance();
                bullet._direction = new Vector2(1, 0);
                float t = (float)(i + 1) / (float)(bullets + 1);
                float y = _arenaMin.y * t + _arenaMax.y * (1 - t);
                bullet.GlobalPosition = new Vector2(_arenaMin.x, y);

                AddChild(bullet);
            }
        } else if (wall == 2) {
            int bullets = 3 + _rng.Next(0, 3);
            for (int i = 0; i < bullets; i++) {
                Bullet bullet = (Bullet)_usedBulletScene.Instance();
                bullet._direction = new Vector2(0, 1);
                float t = (float)(i + 1) / (float)(bullets + 1);
                float x = _arenaMin.x * t + _arenaMax.x * (1 - t);
                bullet.GlobalPosition = new Vector2(x, _arenaMin.y);

                AddChild(bullet);
            }
        } else {
            int bullets = 3 + _rng.Next(0, 3);
            for (int i = 0; i < bullets; i++) {
                Bullet bullet = (Bullet)_usedBulletScene.Instance();
                bullet._direction = new Vector2(0, -1);
                float t = (float)(i + 1) / (float)(bullets + 1);
                float x = _arenaMin.x * t + _arenaMax.x * (1 - t);
                bullet.GlobalPosition = new Vector2(x, _arenaMax.y);

                AddChild(bullet);
            }
        }
    }

    private void CornerSpawn() {
        int bullets = 1 + _rng.Next(0, 2);
        for (int i = 0; i < bullets; i++) {
            Bullet bullet = (Bullet)_usedBulletScene.Instance();
            float angle = .0f * Mathf.Pi + ((Mathf.Pi / 2) / (bullets + 1) * (i + 1));
            bullet._direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            bullet.GlobalPosition = new Vector2(_arenaMin.x, _arenaMin.y);
            AddChild(bullet);
        }
        for (int i = 0; i < bullets; i++) {
            Bullet bullet = (Bullet)_usedBulletScene.Instance();
            float angle = .5f * Mathf.Pi + ((Mathf.Pi / 2) / (bullets + 1) * (i + 1));
            bullet._direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            bullet.GlobalPosition = new Vector2(_arenaMax.x, _arenaMin.y);
            AddChild(bullet);
        }
        for (int i = 0; i < bullets; i++) {
            Bullet bullet = (Bullet)_usedBulletScene.Instance();
            float angle = 1f * Mathf.Pi + ((Mathf.Pi / 2) / (bullets + 1) * (i + 1));
            bullet._direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            bullet.GlobalPosition = new Vector2(_arenaMax.x, _arenaMax.y);
            AddChild(bullet);
        }
        for (int i = 0; i < bullets; i++) {
            Bullet bullet = (Bullet)_usedBulletScene.Instance();
            float angle = 1.5f * Mathf.Pi + ((Mathf.Pi / 2) / (bullets + 1) * (i + 1));
            bullet._direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            bullet.GlobalPosition = new Vector2(_arenaMin.x, _arenaMax.y);
            AddChild(bullet);
        }

    }
}
