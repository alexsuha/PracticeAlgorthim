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

CPP_FILE=$1

function compileExec()
{
	file=$1
	gcc $file.cpp -o $file -lstdc++
	chmod 777 $file
	./$file
}

compileExec $CPP_FILE

find . -name "*.exe" -type f -delete
