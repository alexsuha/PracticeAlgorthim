/*
 * Problem:
 *
 * Let's call an array A a mountain if the following properties hold:
 *
 *    1. A.length >= 3
 *    2. There exists some 0 < i < A.length - 1 such that A[0] < A[1] < ... A[i-1] < A[i] > A[i+1] > ... > A[A.length - 1]
 *
 * Given an array that is definitely a mountain, return any i such that A[0] < A[1] < ... < A[i] > A[i+1] > ... > A[A.length -1].
 *
 * Example 1:
 *  Input : [0, 1, 0]
 *  Output : 1
 *
 * Example 2:
 *  Input : [ 0, 2, 1, 0]
 *  Output : 1
 *
 * Note:
 *  1. 3 <= A.length <= 10000
 *  2. 0 <= A[i] <= 10^6
 *  3. A is a mountain, as defined above.
 *
 */

#include <iostream>
#include <vector>

using namespace std;

class Solution {
public:
	int peakIndexInMountainArray(vector<int>& A) {
		int i = 0, j = A.size() - 1, mid = 0;
		while(i < j)
		{
			mid = (i + j) >> 1;
                        if (A[mid - 1] < A[mid] && A[mid] > A[mid+1])
				break;
			else if(A[mid - 1] < A[mid] && A[mid] < A[mid+1])
				i = mid;
			else if(A[mid - 1] > A[mid] && A[mid] > A[mid+1])
				j = mid;
		}
		return mid;
	}
};

int main()
{
	vector<int> A = {0, 2, 1, 0};
	Solution s;
	int res = s.peakIndexInMountainArray(A);
	cout << res << endl;
	return 0;
}
