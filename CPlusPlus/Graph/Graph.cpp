#include "Graph.h"
#include <vector>
#include <stack>

Graph::Graph(int V)
    : V(0)
    , adj(NULL)
{
    if (V == 0) {
        cout << "must initiate a graph with at least one vertices." << endl;
        return;
    }
    
    this->V = V;
    adj = new list<int>[V];
}

void Graph::addEdge(int v, int w) {
    if (v < V && w < V)
        adj[v].push_back(w);
}

void Graph::BFS(int s) {
    // mark all the vertices as not visited.
    vector<bool> visited(V, false);
    
    // Create a queue for BFS
    list<int> queue;
    
    // mark the source node, and enqueue it.
    visited[s] = true;
    queue.push_back(s);
    
    // declare an iterator for the adjacency list of vertices
    list<int>::iterator iter;
    while(!queue.empty()) {
        // Dequeue a vertex from queue and print it.
        s = queue.front();
        cout << s << " ";
        queue.pop_front();
        
        // Get all adjacent vertices of the dequeued Vertex s. If a adjacent has not visited,
        // then mark it visited and enqueue it.
        for (iter = adj[s].begin(); iter != adj[s].end(); ++iter) {
            if (!visited[*iter]) {
                visited[*iter] = true;
                queue.push_back(*iter);
            }
        }
    }
}

void Graph::DFS(int s) {
    vector<bool> visited(V, false);
    
    stack<int> stack;
    
    visited[s] = true;
    stack.push(s);
    
    list<int>::reverse_iterator iter;
    
    while(!stack.empty()) {
        s = stack.top();
        cout << s << " ";
        stack.pop();
        
        for (iter = adj[s].rbegin(); iter != adj[s].rend(); ++iter) {
            if (!visited[*iter]) {
                visited[*iter] = true;
                stack.push(*iter);
            }                
        }
    }
}