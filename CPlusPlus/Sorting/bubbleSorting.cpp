#include <iostream>

using namespace std;

void BubbleSorting(int[] arr, int size)
{
     int count = size;

     for (int i = size - 1; i > 0; i--)
     {
	bool swap = false;
	for (int j = 0; j < i; j++)
	{
		if (arr[j] < arr[j+1])
		{
			int tmp = arr[i];
			arr[i] = arr[j];
			arr[j] = tmp;
			swap = true;
		}
	}

	if (!swap)
	{
		break;
	}

     }
}

int main()
{
	int arr[] = {14, 33, 27, 35, 10};
	BubbleSorting(arr, 5);
	for (int i = 0; i < 5; i++)
	{
		cout<< arr[i] << ",";
	}
	cout << endl;
	return 0;
}
