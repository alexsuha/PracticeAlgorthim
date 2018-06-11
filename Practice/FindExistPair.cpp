/*****************************************************************************
 *
 * Pair with given product | Set 1 (Find if any pair exists)
 *
 * ***************************************************************************/

#include <iostream>
#include <set>
#include <vector>

using namespace std;

bool HasPairOfInput(vector<int>& array, int& target)
{
      set<int> nums;

      for(auto n : array)
	      nums.insert(n);

      for(auto n : array)
      {
	  if (n == 0) {
	      if(target == 0)
	         return true;
	      else
		 continue;
	  }
          
          int i = target % n;
          if (i == 0)
	  {
              if (nums.find(target / n) != nums.end())
		      return true;
	  }	  
      }
      return false;
}

int main()
{
	vector<int> arr = {10, 20, 9, 40};
	int x = 400;
	HasPairOfInput(arr, x) ? cout << "Yes"
		: cout << "No";

	cout << endl;
	x = 190;
	HasPairOfInput(arr, x) ? cout << "Yes"
		: cout << "No";

	return 0;
}
