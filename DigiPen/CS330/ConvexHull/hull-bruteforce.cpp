#include "hull-bruteforce.h"
#include <algorithm>
#include <iostream>
#include <cmath> /*sqrt*/

/*
//
// @Author: Micah Rust
// Class: CS 330
// Assignment: 3 Convex Hull
//
*/

bool Point::operator==( Point const& arg2 ) const {
    return ( (x==arg2.x) && (y==arg2.y) );
}

std::ostream& operator<< (std::ostream& os, Point const& p) {
	os << "(" << p.x << " , " << p.y << ") ";
	return os;
}

std::istream& operator>> (std::istream& os, Point & p) {
	os >> p.x >> p.y;
	return os;
}

//return value is (on left, on right)
std::pair<bool,bool> get_location(
		float const& p1x, //check which side of the line (p1x,p1y)-->(p2x,p2y) 
		float const& p1y, //point (qx,qy) is on
		float const& p2x,
		float const& p2y,
		float const& qx,
		float const& qy
		) 
{
	Point dir   = {p2x-p1x,p2y-p1y};
	Point norm  = {dir.y, -dir.x};
	Point point = {qx-p1x,qy-p1y};
	//scalar product is positive if on right side
	float scal_prod = norm.x*point.x + norm.y*point.y;
	return std::make_pair( (scal_prod<0), (scal_prod>0));
}

// Math helper functions
float cross_product(Point& a, Point& b)
{
    return a.x * b.y - b.x * a.y;
}
float Length(Point& vec)
{
	return std::sqrt(vec.x * vec.x + vec.y * vec.y);
}
Point Mul(Point& vec, float f)
{
	Point pt;
	pt.x = vec.x * f;
	pt.y = vec.y * f;
	return pt;
}
Point Div(Point& vec, float f)
{
	Point pt;
	pt.x = vec.x / f;
	pt.y = vec.y / f;
	return pt;
}
Point Normalize(Point& vec)
{
	return Div(vec, Length(vec));
}


//returns a set of indices of points that form convex hull
std::set<int> hullBruteForce ( std::vector< Point > const& points ) {
	size_t num_points = points.size();
	//std::cout << "number of points " << num_points << std::endl;
	if ( num_points < 3 ) throw "bad number of points";

	std::set<int> hull_indices;
	// For all possible lines made between the points
	for (size_t i = 0; i < num_points; ++i)
	{
		for (size_t j = i; j < num_points + 1; ++j)
		{
			bool OnLeft = false;
			bool OnRight = false;

			size_t j_moded = j % num_points;

			if (i == j_moded) continue;

			// Are there any points not one exactly 1 side?
			for (size_t k = 0; k < num_points; ++k)
			{
				std::pair<bool, bool> cmpRes = get_location(
					points[i].x, points[i].y,
					points[j_moded].x, points[j_moded].y,
					points[k].x, points[k].y);

				OnLeft |= cmpRes.first;
				OnRight |= cmpRes.second;

				if (!(OnLeft & OnRight) == false)
					break;

			}

			// Add item if all points were on one side or the other
			if (!(OnLeft & OnRight))
			{
				hull_indices.insert(i);
				hull_indices.insert(j_moded);
			}
		}
	}

	return hull_indices;
}

// Helper struct to hold onto index before sort
struct PointIndexPair
{
	Point pt;
	int index;
};

// Debug function to print out information about indexes relative to my point holder
void print(std::vector<int>& hull_indices, std::vector<PointIndexPair> const& points)
{
	// Print state
	for (unsigned i = 0; i < hull_indices.size(); ++i)
	{
		std::cout << "Index " << hull_indices[i] << " " <<
			points[hull_indices[i]].pt << std::endl;
	}
}

// Sorting PointIndexPairs by x value
struct SortByX
{
	bool operator()(const PointIndexPair& left, const PointIndexPair& right)
	{
		return left.pt.x < right.pt.x;
	}
};

// Helper function which does the blulk of the work to add/remove poitns
void AddPointToBound(int index, std::vector<int>& indexes_Of_Bound,
	std::vector<PointIndexPair>& sortedPoints)
{
	Point pointToAdd = sortedPoints[index].pt;

	// Check all previous points against new point
	bool checkAllPoints = false;
	unsigned hullIndex = indexes_Of_Bound.size() - 1;
	while (hullIndex > 0)
	{
		Point curPoint = sortedPoints[indexes_Of_Bound.back()].pt;
		Point lastPoint = sortedPoints[indexes_Of_Bound[indexes_Of_Bound.size() - 2]].pt;
		Point vecCur = { curPoint.x - lastPoint.x,   curPoint.y - lastPoint.y };
		Point vecNext = { pointToAdd.x - curPoint.x, pointToAdd.y - curPoint.y };
		vecCur = Normalize(vecCur);
		vecNext = Normalize(vecNext);

		if (cross_product(vecCur, vecNext) <= 0.0f)// bad point, pop it!
		{
			checkAllPoints = true;
			while (indexes_Of_Bound.size() > hullIndex)
			{
				indexes_Of_Bound.pop_back();
			}
		}

		--hullIndex;
		if (checkAllPoints == false)
			break;
	}

	indexes_Of_Bound.push_back(index);
}

// Brute force hull implimentation 2
std::vector<int> hullBruteForce2 ( std::vector< Point > const& points ) 
{
	int num_points = points.size();
	if ( num_points < 3 ) throw "bad number of points";
	
	SortByX Sorter;

	std::vector<PointIndexPair> sortedPoints;
	{// Setup sorted points
		sortedPoints.reserve(points.size());

		// Save index values
		for (int i = 0; i < static_cast<int>(points.size()); ++i)
		{
			PointIndexPair pip;
			pip.index = i;
			pip.pt = points[i];
			sortedPoints.push_back(pip);
		}
		std::sort(sortedPoints.begin(), sortedPoints.end(), Sorter);
	}

	std::vector<int> hull_sorted_indexs_found_Lower;
	{ // get lower bounds
		hull_sorted_indexs_found_Lower.reserve(static_cast<unsigned>(sortedPoints.size() * 0.25f));
		hull_sorted_indexs_found_Lower.push_back(0);

		// Get lower bounds
		for (unsigned i = 1; i < sortedPoints.size(); ++i)
		{
			AddPointToBound(i, hull_sorted_indexs_found_Lower, sortedPoints);
		}
	}

	std::vector<int> hull_sorted_indexs_found_Upper;
	{ // get upper bounds
		hull_sorted_indexs_found_Upper.reserve(static_cast<unsigned>(sortedPoints.size() * 0.25f));
		hull_sorted_indexs_found_Upper.push_back(sortedPoints.size() - 1);

		// Get upper bounds
		for (int i = sortedPoints.size() - 2; i >= 0; --i)
		{
			AddPointToBound(i, hull_sorted_indexs_found_Upper, sortedPoints);
		}
	}

	// Remove the first + last of lower bounds
	hull_sorted_indexs_found_Upper.pop_back();
	hull_sorted_indexs_found_Upper.erase(hull_sorted_indexs_found_Upper.begin());

	// merge sorted indexes
	for(unsigned i = 0; i < hull_sorted_indexs_found_Upper.size(); ++i)
	{
		hull_sorted_indexs_found_Lower.push_back(hull_sorted_indexs_found_Upper[i]);
	}

	std::vector<int> hull_indices;
	hull_indices.reserve(static_cast<unsigned>(sortedPoints.size() * 0.5f));

	// transform back into input indexes
	for(unsigned i = 0; i < hull_sorted_indexs_found_Lower.size(); ++i)
	{
		hull_indices.push_back(sortedPoints[hull_sorted_indexs_found_Lower[i]].index);
	}
	
	return hull_indices;
}