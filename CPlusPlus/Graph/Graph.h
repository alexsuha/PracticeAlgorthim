// prototype of the Graph.
#ifndef DEMO_GRAPH_H_
#define DEMO_GRAPH_H_

#include <iostream>
#include <list>

using namespace std;

class Graph {
    int V; // number of vertices
    
    // Pointer to an array containing adjacency lists
    list<int> *adj;
    
public:
    Graph(int V); // Constructor
    
    // function to add an edge to graph
    void addEdge(int v, int w);
    
    // prints BFS traversal from a given source s
    void BFS(int s);
    
    // prints DFS traversal from a given source s
    void DFS(int s);
};

#endif 