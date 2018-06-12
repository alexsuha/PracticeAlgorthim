/******************************************************************************************************************
 *
 * Minimum of Depth of a binary tree
 *
 * ***************************************************************************************************************/


#include <iostream>
#include <vector>

using namespace std;

struct TreeNode
{
	int val;
	TreeNode* left;
	TreeNode* right;

	TreeNode(int x): val(x), left(NULL), right(NULL) {}
};


TreeNode* CreateBT(vector<int> &arrays, int index)
{
	/*  0
	 *  |\
	 *  1 2
	 *  |\ |\
	 *  3 4 5 6
	 */
	if (index > arrays.size() - 1 || index < 0)
		return NULL;

	if (arrays[index] < 0)
		return NULL;

	TreeNode* root = new TreeNode(arrays[index]);

	TreeNode* left = CreateBT(arrays, 2 * index + 1);
	TreeNode* right = CreateBT(arrays, 2 * index + 2);

	if (root)
	{
		root->left = left;
		root->right = right;
	}
	return root;
}

void InOrderTree(TreeNode* root)
{
	if (root == NULL)
		return;

	cout << root->val << ", ";
	InOrderTree(root->left);
	InOrderTree(root->right);
}

void MinDepthOfTree(TreeNode* root, int &depth, int &minDepth)
{
	if (root == NULL)
		return;

	if (!root->left && !root->right) {
		//cout << depth << "-" << minDepth << "|";
		minDepth = min(depth, minDepth); 
		//cout << depth << "-" << minDepth << endl;
	        return;
	}

	if (root->left)
	{
		depth++;
		MinDepthOfTree(root->left, depth, minDepth);
		depth--;
	}
	if (root->right)
	{
		depth++;
		MinDepthOfTree(root->right, depth, minDepth);
		depth--;
	}
}

int main() {
	vector<int> tree = { 0, 2, 1, -1, 6, 7, -1, -1, -1, 9, 6 };
	TreeNode* root = CreateBT(tree, 0);
	if (root==NULL)
		cout << "root is empty" << endl;
	InOrderTree(root);
	int minDepth = 100, depth = 0;
	MinDepthOfTree(root, depth, minDepth);
        cout << endl;
        cout << "min depth: " << minDepth << endl;	
	return 0;
}


