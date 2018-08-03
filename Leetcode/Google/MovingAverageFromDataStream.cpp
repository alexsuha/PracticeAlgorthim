/*
 * Problem:
 *
 * Given a stream of integers and a window size, calculate the moving average of all integers in the sliding window.
 *
 * For example,
 *
 *    MovingAverage m = new MovingAverage(3);
 *    m.next(1) = 1;
 *    m.next(10) = (1 + 10) / 2;
 *    m.next(3) = (1 + 10 + 3) / 3;
 *    m.next(5) = (10 + 3 + 5) / 3;
 *
 */

#include <iostream>
#include <vector>

using namespace std;

class MovingAverage {
public:
	/** Initialize your data structure here. */
	MovingAverage(int size) {
		stack.reserve(size);
		for(int i = 0; i < size; i++)
			stack[i] = 0;
		//for(auto &n : stack)
		//	n = 0;
		count = 0;
		sum = 0;
		stack_size = size;
	}

	double next(int val) {
		int oldValue = stack[count % stack_size];
		stack[count % stack_size] = val;
		count++;
		int loop_num = count > stack_size ? stack_size : count;
		
		sum += (val - oldValue);
		cout << "[sum]" << oldValue << "[]" << sum << "@" << count << "@";
		return sum / loop_num;
	}

private:
	int count;
	int stack_size;
	double sum;
	vector<int> stack;
};

int main() 
{
	MovingAverage* ma = new MovingAverage(3);
	double res = ma->next(1);
	cout << res << "->";
	res = ma->next(10);
	cout << res << "->";
	res = ma->next(3);
	cout << res << "->";
	res = ma->next(5);
	cout << res << ".";
	return 0;
}
