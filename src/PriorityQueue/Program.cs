using PriorityQueue;
using Distance = int;

var sanFrancisco = new City("San Francisco");
var losAngeles = new City("Los Angeles");
var dallas = new City("Dallas");
var newYork = new City("New York");
var chicago = new City("Chicago");

var cityGraph = BuildCityGraph();
PrintShortestPath(cityGraph, sanFrancisco, newYork);
return;

void PrintShortestPath(IEnumerable<City> graph, City startNode, City endNode)
{
    var route = CalculateShortestPath(graph, startNode, endNode);

    if (route is null)
    {
        Console.WriteLine($"No route could be found between {startNode.Name} and {endNode.Name}");
        return;
    }

    Console.WriteLine($"Shortest route between {startNode.Name} and {endNode.Name}");
    foreach (var (node, distance) in route)
    {
        Console.WriteLine($"{node.Name}: {distance}");
    }
}

IEnumerable<City> BuildCityGraph()
{
    sanFrancisco.Edges = 
    [
        new Edge(losAngeles, 347),
        new Edge(dallas, 1_480),
        new Edge(chicago, 1_853),
    ];
    losAngeles.Edges = 
    [
        new Edge(dallas, 1_237),
        new Edge(sanFrancisco, 347),
    ];
    dallas.Edges = 
    [
        new Edge(chicago, 802),
        new Edge(newYork, 1_370),
        new Edge(sanFrancisco, 1_480),
        new Edge(losAngeles, 1_237),
    ];
    chicago.Edges = 
    [
        new Edge(newYork, 712),
        new Edge(sanFrancisco, 1_853),
        new Edge(dallas, 802),
    ];
    newYork.Edges = 
    [
        new Edge(dallas, 1_370),
        new Edge(chicago, 712),
    ];
    
    return [sanFrancisco, losAngeles, dallas, newYork, chicago];
}

IEnumerable<(City, Distance)>? CalculateShortestPath(IEnumerable<City> cities, City startNode, City endNode)
{
    // create dictionary from graph; set previous city to null and distance to infinity (or huge number)
    // | City | Previous city, Distance |
    // | SF   | null, 2147483647 |
    // | LA   | null, 2147483647 |
    // | DA   | null, 2147483647 |  
    var distances = cities
        .Select(city => (city, details: (Previous: (City?)null, Distance: Distance.MaxValue)))
        .ToDictionary(x => x.city, x=> x.details);

    var queue = new PriorityQueue<City, Distance>();

    // set start node with distance as zero
    distances[startNode] = (null, 0);
    // enqueue the start node
    queue.Enqueue(startNode, 0);

    while(queue.Count > 0)
    {
        var current = queue.Dequeue();
        if (current == endNode)
        {
            // destination has been reached, exit
            return BuildRoute(distances, endNode); 
        }
    
        // get current distance from dictionary
        // defaults to 2147483647 for unexplored cities, otherwise it will have the shortest distance found so far
        var currentNodeDistance = distances[current].Distance;
        
        // visit the city edges
        foreach(var edge in current.Edges)
        {
            // get last recorded distance for the edge 
            var distance = distances[edge.TargetCity].Distance;
            // calculate distance going to the edge AND passing through the current NODE
            var newDistance = currentNodeDistance + edge.Distance;
            
            if(newDistance < distance)
            {
                // the new distance is shorter than the one from the dictionary
                // update dictionary with new distance
                distances[edge.TargetCity] = (current, newDistance);
                // also update the queue (upsert) to later visit that city
                queue.Remove(edge.TargetCity, out _, out _);
                queue.Enqueue(edge.TargetCity, newDistance);
            }
        }
    }

    return null;
}

IEnumerable<(City, Distance)> BuildRoute(Dictionary<City, (City? previous, Distance Distance)> distances, City endNode)
{
    var route = new List<(City, Distance)>();
    var previous = endNode;

    while (previous is not null)
    {
        var current = previous;
        (previous, var distance) = distances[current];
        route.Add((current, distance));
    }

    route.Reverse();
    return route;
}