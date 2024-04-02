using Godot;
using System;
using System.Xml.Serialization;

public partial class Laser : Line2D
{
    private GpuParticles2D startParticles, endParticles, bodyParticles;

    public override void _Ready()
    {
        // Get References
        startParticles = GetNode<GpuParticles2D>("StartParticles");
        endParticles = GetNode<GpuParticles2D>("EndParticles");
        bodyParticles = GetNode<GpuParticles2D>("BodyParticles");

        // Set positions of the particles
        startParticles.Position = Points[0];
        endParticles.Position = Points[1];
        bodyParticles.Position = (Points[0] + Points[1]) / 2; // Use midpoint of the 2 points

        float distance = Points[0].DistanceTo(Points[1]);
        bodyParticles.ProcessMaterial.Set("emission_box_extents", new Vector3(10, distance/2, 0));
        
        // Set rotation of particles
        Vector2 direction = Points[1] - Points[0];
        float angle = Mathf.Atan2(direction.Y, direction.X);

        // Set rotation of particles
        startParticles.Rotation = angle;
        endParticles.Rotation = angle + Mathf.Pi;
        bodyParticles.Rotation = angle - Mathf.Pi/2;
    
    }
}

