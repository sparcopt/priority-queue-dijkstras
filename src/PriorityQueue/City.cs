namespace PriorityQueue;

using Distance = int;

public record City(string Name)
{
    public Edge[] Edges { get; set; } = [];
}

public record Edge(City TargetCity, Distance Distance);