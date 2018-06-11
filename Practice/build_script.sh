# ####################################################################
#
# This file is for compile the test demo of common interview question.
# author : sheng zhang
#
# ###################################################################

echo "Now Compile........."

#gcc CheckFourPoints.cpp -o CheckFourPoints -lstdc++
#chmod 777 CheckFourPoints
#./CheckFourPoints

function compileExec()
{
	file=$1
	gcc $file.cpp -o $file -lstdc++
	chmod 777 $file
	./$file
}

compileExec CheckFourPoints
