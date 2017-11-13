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

////////////////////////////////////////////////////////////////////////////////
float closestPair_aux ( .................... );

////////////////////////////////////////////////////////////////////////////////
float closestPair ( std::vector< Point > const& points ) {
	int size = points.size();

	//std::cerr << "closestPair_aux " << size << " points:";
	//for(int i=0;i<size;++i) { std::cerr << points[i] << " "; } std::cerr << std::endl;

	if (size==0) throw "zero size subset";
	if (size==1) return std::numeric_limits<float>::max();

	return closestPair_aux( indices, points );
}

////////////////////////////////////////////////////////////////////////////////
float closestPair_aux ( ..................... ) {
	int size = indices.size();

	//std::cerr << "closestPair_aux ";
	//for(int i=0;i<size;++i) { std::cerr << points[ indices[i] ] << " "; } std::cerr << std::endl;

	if (size==0) throw "zero size subset";
	if (size==1) return std::numeric_limits<float>::max();

	.................

	return min_dist;
}

