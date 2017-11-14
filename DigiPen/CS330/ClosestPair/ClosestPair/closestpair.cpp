#include "closestpair.h"
#include <algorithm> // std::sort
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

// ---------------------------------------------- //
// Forward Declarations
// ---------------------------------------------- //
typedef size_t offsetPtr;

float distanceBetweenSq(const Point& p1, const Point& p2);
float closestPairSq_Split(std::vector<Point>& points, const bool verticalDivide);
float closestPairSq_Brute(std::vector<Point>const& points);
float closestPairSq_Brute(std::vector<Point>::const_iterator begin, std::vector<Point>::const_iterator end);

// ---------------------------------------------- //
// Sorging Functors
// ---------------------------------------------- //
struct SortByX
{
	bool operator()(const Point& left, const Point& right)
	{
		return left.x < right.x;
	}
};
struct SortByY
{
	bool operator()(const Point& left, const Point& right)
	{
		return left.y < right.y;
	}
};

// ---------------------------------------------- //
// Main implimentation
// ---------------------------------------------- //
float closestPair ( std::vector< Point > const& points ) {
	int size = points.size();

	// copy points so that we can sort and modify them
	std::vector<Point> givenPoints = points;
	
	//std::cerr << "closestPair_Split " << size << " points:";
	//for(int i=0;i<size;++i) { std::cerr << points[i] << " "; } std::cerr << std::endl;

	//if (size < 1000)
	//	return std::sqrt(closestPairSq_Brute(givenPoints));

	if (size==0) throw "zero size subset";
	if (size==1) return std::numeric_limits<float>::max();
	
	return std::sqrt(closestPairSq_Split(givenPoints, true));
}


// ---------------------------------------------- //
// Helper Functions
// ---------------------------------------------- //
float closestPairSq_Brute(std::vector<Point> const& points)
{
	return closestPairSq_Brute(points.begin(), points.end());
}
float closestPairSq_Brute(std::vector<Point>::const_iterator begin, std::vector<Point>::const_iterator end)
{
	float minDist = std::numeric_limits<float>::max();
	for (std::vector<Point>::const_iterator it1 = begin; it1 != end; ++it1)
	{
		for (std::vector<Point>::const_iterator it2 = it1 + 1; it2 != end; ++it2)
		{
			float dist = distanceBetweenSq(*it1, *it2);
			minDist = std::min(minDist, dist);
		}
	}
	return minDist;
}
float closestPairSq_Split (std::vector<Point>& points, const bool verticalDivide)
{
	int pointCount = points.size();
	float distSq = std::numeric_limits<float>::max();
	
	if (pointCount == 0) throw "zero size subset";
	if (pointCount == 1) return distSq;


	if (pointCount < 4) // do brute force with 3 or less points
	{
		return closestPairSq_Brute(points);
	}

	float centerLineXY = 0.0f;
	offsetPtr vectorOffset = 0;
	if (verticalDivide)
	{
		std::sort(points.begin(), points.end(), SortByX());
		centerLineXY = points[(points.size() / 2) - 1].x;
		vectorOffset = reinterpret_cast<offsetPtr>(&(static_cast<Point*>(0))->x);
	}
	else // horizontal
	{
		std::sort(points.begin(), points.end(), SortByY());
		centerLineXY = points[(points.size() / 2) - 1].y;
		vectorOffset = reinterpret_cast<offsetPtr>(&(static_cast<Point*>(0))->y);
	}

	// divide points
	std::vector<Point> lower(points.begin(), points.begin() + (points.size() / 2));
	std::vector<Point> upper(points.begin() + (points.size() / 2), points.end());
	float dLowerSq = closestPairSq_Split(lower, !verticalDivide);
	float dUpperSq = closestPairSq_Split(upper, !verticalDivide);
	distSq = std::min(dLowerSq, dUpperSq);



	// Check points along axis of division
	std::vector<Point>::const_iterator lowBound = points.begin();
	std::vector<Point>::const_iterator highBound = points.end() - 1;
	{
		float dist = std::sqrt(distSq); // + epsilon
		for (std::vector<Point>::const_iterator it = points.begin(); it != points.end(); ++it)
		{
			float pointPosXY = *reinterpret_cast<const float*>(reinterpret_cast<char const*> (&(*it)) + vectorOffset);
			if (std::abs(pointPosXY - centerLineXY) < dist) // inside section
			{
				highBound = it;
			}
			else
			{
				// reached other end
				if (it == highBound + 1)
					break;

				lowBound = it;
			}
		}
	}

	// The algorithm given on the notes does this by only comparing y values which is a bit
	// faster.
	return std::min(distSq, closestPairSq_Brute(lowBound, highBound + 1));
}
float distanceBetweenSq(const Point& p1, const Point& p2)
{
	Point vec;
	vec.x = p1.x - p2.x;
	vec.y = p1.y - p2.y;
	return (vec.x * vec.x) + (vec.y * vec.y);
}