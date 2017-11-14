#include "closestpair.h"
#include <algorithm>
#include <limits>
#include <cmath>
#include <iostream>
#include <utility>

std::ostream& operator<< (std::ostream& os, Point const& p) {
	os << "(" << p.x << " , " << p.y << ") ";
	return os;
}

std::istream& operator>> (std::istream& os, Point & p) {
	os >> p.x >> p.y;
	return os;
}

float distanceBetweenSq(const Point& p1, const Point& p2)
{
	Point vec(p1.x - p2.x, p1.y - p2.y);
	return vec.x * vec.x + vec.y + vec.y
}

struct SortByX
{
	bool operator()(const Point& left, const Point& right)
	{
		return left.x < right.x;
	}
}
struct SortByY
{
	bool operator()(const Point& left, const Point& right)
	{
		return left.y < right.y;
	}
	
}

////////////////////////////////////////////////////////////////////////////////
float closestPair_Split (std::vector<Point>::iterator begin, std::vector::iterator end, bool verticalDivide, float& closestPairSq);

float closestPair_Brute(std::
////////////////////////////////////////////////////////////////////////////////
float closestPair ( std::vector< Point > const& points ) {
	int size = points.size();

	// copy points so that we can sort and modify them
	std::vector<Point> givenPoints = points;
	
	//std::cerr << "closestPair_Split " << size << " points:";
	//for(int i=0;i<size;++i) { std::cerr << points[i] << " "; } std::cerr << std::endl;

	if (size==0) throw "zero size subset";
	if (size==1) return std::numeric_limits<float>::max();

	float distance = std::numeric_limits<float>::max();
	std::sort(givenPoints.beging(), givenPoints.end(), SortByX());
	
	return closestPair_Split(givenPoints.begin(), givenPoints.end(),  , points );
}

////////////////////////////////////////////////////////////////////////////////
float closestPair_Split (std::vector<Point>::iterator begin, std::vector::iterator end, bool verticalDivide)
{
	int size = indices.size();

	//std::cerr << "closestPair_Split ";
	//for(int i=0;i<size;++i) { std::cerr << points[ indices[i] ] << " "; } std::cerr << std::endl;

	if (size==0) throw "zero size subset";
	if (size==1) return std::numeric_limits<float>::max();


	return min_dist;
}

