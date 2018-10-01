/*
 * Problem:
 *
 * Given a string, determine if a permutation of the string could from a palindrome.
 *
 * Example 1:
 *   Input : "code"
 *   Output : false
 *
 * Example 2:
 *   Input : "aab"
 *   Output : true
 *
 * Example 3: 
 *   Input : "carerac"
 *   Output : true
 *
 */

#include <iostream>
#include <unordered_map>

using namespace std;

class Solution {
public:
	bool canPermutePalindrome(string s) {
		unordered_map<char, int> marks;

		for(auto &c : s)
		{
                    if(marks.find(c) != marks.end())
			    marks[c]+= 1;
		    else
			    marks[c] = 1;
		}

                int count = 0;

		for(auto &m : marks)
			if (m.second % 2 != 0)
			{
				if (count > 0)
					return false;
				count++;
			}
		return true;
	}
};

int main() {
	Solution s;
	bool res = false;
	res = s.canPermutePalindrome("aab");
	cout << res << endl;
	res = s.canPermutePalindrome("code");
	cout << res << endl;
	res = s.canPermutePalindrome("carerac");
	cout << res << endl;
	return 0;
}
