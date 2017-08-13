/*
*	 Description: Dijkstra two stack algorithm for calculator.
*	 Propose: I change a little for any input with variety organizion for calculate.
*	1. char to num
*	2. opers in stack
*	3. *, / ,next char is value, then send value
*	4. loop for '('
*	5. check the rest of values in stack, put all sum.
*	6. loop to remember, two stack pop together, only first value pop first.
*/


#include "stdafx.h"

#include <iostream>
#include <stack>

using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	char a[256];

	cin >> a;

	stack<char> opers;
	stack<int> vals;

	int count = 0;
	char* cur = a;
	int sum = 0;
	
	while (*cur != '\0')
	{
		if (*cur >= '0' && *cur <= '9')
			vals.push(atoi(cur));
		else if (*cur == '+')
			opers.push(*cur);
		else if (*cur == '-')
			opers.push(*cur);
		else if (*cur == '*' || *cur == '/')
		{
			char oper = *cur;
			if (*(cur + 1) != '(')
			{
				int val = vals.top();
				vals.pop();
				int val1 = atoi(++cur);
				if (oper == '*') val *= val1;
				else if (oper == '/') val /= val1;
				vals.push(val);
			}
			else
				opers.push(*cur);
		}
		/*else if (*cur == '/')
			opers.push(*cur);*/
		else if (*cur == '(')
			opers.push(*cur);
		else if (*cur == ')')
		{
			int val = 0, val1 = 0;
			val = vals.top();
			vals.pop();
			while (opers.top() != '(')
			{
				char oper = opers.top();
				opers.pop();
				
				val1 = vals.top();
				vals.pop();
				if (oper == '+') val1 += val;
				if (oper == '-') val1 -= val;
				if (oper == '*') val1 *= val;
				if (oper == '/') val1 /= val;
				val = val1;
			}
			vals.push(val);
			opers.pop();
		}
		cur++;
		count++;
	}

	sum = vals.top();
	vals.pop();
	int val = 0;
	while (!opers.empty())
	{
		val = vals.top();
		vals.pop();

		char oper = opers.top();
		opers.pop();

		if (oper == '+') sum = val + sum;
		if (oper == '-') sum = val - sum;
	}

	cout <<  ' ' << endl;
	
	cout << "sum = " << sum << endl;
	cout << "vals size is " << vals.size() << endl;

	if (count > 257)
		cout << "you got wrong loop." << endl;
	else
		cout << count << endl;

	return 0;
}

