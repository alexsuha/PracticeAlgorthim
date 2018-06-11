/***********************************************************************
 *
 * Get First set Bit position
 *
 * ***********************************************************************/

#include <stdio.h>
#include <math.h>

unsigned int getFirstSetBitPos(int n)
{
	return log2(n&-n) + 1;
}

int main()
{
	int n = 12;
	printf("%u", getFirstSetBitPos(n));
	return 0;
}

