#include "common.h"
#pragma once

struct PathFinderNode
{
public:
	short X, Y;
	int H;
	bool IsNull = true;

	PathFinderNode(short x, short y, int h)
		: X(x), Y(y), H(h)
	{
		IsNull = false;
	}

	PathFinderNode *LastNode;
	PathFinderNode *NextNode;
};

struct Point
{
public:
	short X;
	short Y;
};
