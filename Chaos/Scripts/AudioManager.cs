using Godot;
using System;

public class AudioManager : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    AudioEffectFilter audioEffect;
    float minHz = 2000;
    float maxHz = 20000;
    bool cut = true;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        audioEffect = (AudioEffectFilter)AudioServer.GetBusEffect(1, 0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    float timer = 0;
    public override void _Process(float delta)
    {
        delta *= 0.3f;

        if (cut) {
            timer -= delta;
        } else {
            timer += delta;
        }

        timer = Mathf.Clamp(timer, 0, 1);

        audioEffect.CutoffHz = maxHz * timer + minHz * (1f - timer);
    }

    public void FadeIn () {
        cut = false;
    }

    public void FadeOut () {
        cut = true;
    }
}
