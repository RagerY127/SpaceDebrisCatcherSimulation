using One_Sgp4;
using System;

public enum DebrisShape
{
    Cube,
    Cylinder
}

public class DebrisData
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public float OrbitFirstAxis { get; private set; }
    public float OrbitSecondAxis { get; private set; }
    public float InitialPosition { get; private set; }
    public float RevolutionsPerDay { get; private set; }
    public float Mass { get; private set; }
    public DebrisShape Shape { get; private set; }
    public float Height { get; private set; }
    public float Length { get; private set; }
    public float Width { get; private set; }

    public DebrisData(string name, float orbitFirstAxis, float orbitSecondAxis, float initialPosition,
                  float revolutionsPerDay, float mass, DebrisShape shape, float height, float length, float width)
    {
        this.Id = Guid.NewGuid().ToString();
        this.Name = name;
        this.OrbitFirstAxis = orbitFirstAxis;
        this.OrbitSecondAxis = orbitSecondAxis;
        this.InitialPosition = initialPosition;
        this.RevolutionsPerDay = revolutionsPerDay;
        this.Mass = mass;
        this.Shape = shape;
        this.Height = height;
        this.Length = length;
        this.Width = width;
    }

    public Tle ToTle()
    {
        string tleLine1 = $"1 00000U 00000ACM 00000.00000000  .00000000  00000+0  00000+0 0  000".AddTleChecksum();
        string tleLine2 = FormattableString.Invariant($"2 00000 {this.OrbitFirstAxis:000.0000} {this.OrbitSecondAxis:000.0000} 0000000 000.0000 {this.InitialPosition:000.0000} {this.RevolutionsPerDay:00.00000000}00000").AddTleChecksum();
        return ParserTLE.parseTle(tleLine1, tleLine2);
    }

    public static DebrisData TestDebrisData()
    {
        return new DebrisData(
            "Test Debris",
            0.0f,
            0.0f,
            0.0f,
            14.84061f,
            100.0f,
            DebrisShape.Cube,
            1.0f,
            1.0f,
            1.0f
        );
    }
}
