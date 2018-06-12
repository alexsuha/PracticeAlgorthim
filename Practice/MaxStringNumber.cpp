/*************************************************************************************************************************************************************
 *
 * Output the strings which shows max time
 *
 * **********************************************************************************************************************************************************/

#include <vector>
#include <string>
#include <iostream>
#include <unordered_map>
#include <algorithm>

using namespace std;

vector<string> ShowMaxniumString(vector<string> &words)
{
	vector<string> res;
	if (words.size() == 0) return res; 
	unordered_map<string, int> wmaps;
	
	int _max = 0;
	for(auto s : words) {
		//cout << s;
		transform(s.begin(), s.end(), s.begin(), ::tolower);
		//cout << "_" << s;
		wmaps[s]++;
		_max = max(wmaps[s], _max);
	}

	for(auto p : wmaps) {
		if (p.second == _max) {
			res.push_back(p.first);
		}
	}
	return res;
}

int main()
{
	vector<string> words = { "Jack", "jack", "ff", "cc", "Ffa", "CC" };
	vector<string> res = ShowMaxniumString(words);
	for(auto s : res)
		cout << s << ", ";

	char tmp ='X';
	cout << endl;
	cout << tmp;
	cout << "_" << (char)tolower(tmp) << endl;

	cout << "c compare:" << ('c' == tolower('C')?"Yes":"No") << endl;
	return 0;

}
