#include <iostream>

using namespace std;

struct Point{
	int x, y;
	Point(int _x, int _y) : x(_x), y(_y) {}

	friend Point operator+(const Point& a, const Point& b) {
	    return Point(a.x + b.x, a.y + b.y);
	}

	Point& operator+=(const Point& rhs){

      this->x += rhs.x;
	  this->y += rhs.y;
      return *this;
	}

	Point& operator/=(const int& rhs){

      this->x /= rhs;
	  this->y /= rhs;
      return *this;
	}
};

// A utility function to find square of distance
// from point 'p' to poitn 'q'
int distSq(Point& p, Point& q)
{
    return (p.x - q.x)*(p.x - q.x) +
           (p.y - q.y)*(p.y - q.y);
}

// This function returns true if (p1, p2, p3, p4) form a
// square, otherwise false
bool isSquare(Point& p1, Point& p2, Point& p3, Point& p4)
{
    int d2 = distSq(p1, p2);  // from p1 to p2
    int d3 = distSq(p1, p3);  // from p1 to p3
    int d4 = distSq(p1, p4);  // from p1 to p4
 
    // If lengths if (p1, p2) and (p1, p3) are same, then
    // following conditions must met to form a square.
    // 1) Square of length of (p1, p4) is same as twice
    //    the square of (p1, p2)
    // 2) p4 is at same distance from p2 and p3
    if (d2 == d3 && 2*d2 == d4)
    {
        int d = distSq(p2, p4);
        return (d == distSq(p3, p4) && d == d2);
    }
 
    // The below two cases are similar to above case
    if (d3 == d4 && 2*d3 == d2)
    {
        int d = distSq(p2, p3);
        return (d == distSq(p2, p4) && d == d3);
    }
    if (d2 == d4 && 2*d2 == d3)
    {
        int d = distSq(p2, p3);
        return (d == distSq(p3, p4) && d == d2);
    }
 
    return false;
}

int main()
{
    cout << "...........Checking Four Points in a square........" << endl;
	Point p1 = {20, 10}, p2 = {10, 20},
          p3 = {20, 20}, p4 = {10, 10};
    isSquare(p1, p2, p3, p4)? (cout << "Yes" << endl) : (cout << "No" << endl);
	return 0;
}