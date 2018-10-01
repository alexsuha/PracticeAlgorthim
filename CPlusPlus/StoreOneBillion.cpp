/***************************************************************************
 *
 * Test Whether is crashed when stored using vector
 *
 * *************************************************************************/

#include <vector>
#include <iostream>
#include <ctime>

using namespace std;

//#define ARRAY_NUM 100000000000000000000000000000000000000000 // too large for integer to initial the vector size
#define ARRAY_NUM 10000000

int main()
{
	srand(time(NULL));

	//int array[ARRAY_NUM]; // crash using an "automatic" variable, which is allocated on the stack
	// method1: allocate ARRAY_NUM first, then the memory is in the heap, won't crash, but need a protection
	// method2: vector usage, declare size of vector first.
	//int *array = new int[ARRAY_NUM];
	vector<int> array(ARRAY_NUM); // worked.
	//vector<int> array(); // if only declare, can't use index directly, because the memory is read-only.
	//vector<int> array();  // failed.
	//array.reserve(ARRAY_NUM); // failed.
	for(int i = 0; i < ARRAY_NUM; i++)
        //	array.push_back((rand() % ARRAY_NUM) + 1); // only class type
		array[i] = (rand() % ARRAY_NUM) + 1;
	for(int i = 99; i < 3444; i++)
		if (i % 2556 == 0)
			cout << i;
	cout << "finish" << endl;
	//delete[] array;
	return 0;
}
