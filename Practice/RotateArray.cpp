/***************************************************************************************************************************************************
 *
 * Rotate array length n with k step
 *
 * ***********************************************************************************************************************************************/

#include <iostream>
#include <vector>

using namespace std;

void RotateArrayWithSteps(vector<int> &array, int step)
{
	if (array.size() == 0) return;
	int cur = 0;
	int curval = array[0];
	do {
		cur = (cur + step) % array.size();
		int tmp = array[cur];
		array[cur] = curval;
		curval = tmp;
	} while(cur != 0);
}

int main() 
{
	vector<int> array = {1, 2, 3, 4, 5, 6, 7};
	RotateArrayWithSteps(array, 3);
	for(auto i : array)
		cout << i << ", ";
	return 0;
}
