//
// Author: Micah Rust
// Class: CS 330, Extra Credit TSP problem
//

#include "tsp.h"
#include <vector>
#include <cstdio>
#include <iostream>
#include <fstream>
#include <string>

struct Edge
{
	int distance;
	int parentRow;
	int parentCol;
};
typedef std::vector< std::vector<Edge> > DirectedMap;

void printMap(MAP& map)
{
	for(auto& it1 : map)
	{
		for(auto& it2 : it1)
		{
			std::cout << it2 << " ";
		}
		std::cout << "\n";
	}
	std::cout << std::flush;
}


int FindPathRec(std::vector<int>& path, int currentCity, const DirectedMap& map)
{
	int minDistIndex = 0;
	int minDistValue = 0;

	int minBound = 0;

	// Found end of path
	if (currentCity == 0 && path.size() == map.size())
		return 0;


	int distances_tried = 0;
}


std::vector<int> SolveTSP(char const* filename)
{
	std::ifstream inFile(filename);
	std::string strBuffer;

	if(inFile.is_open() == false)
	{
		std::cout << "Error: Failed to open file named: " << filename << std::endl;
		return std::vector<int>();
	}

	inFile >> strBuffer; // read num cities
	const int numCities = std::stoi(strBuffer);
	const int numDistances = numCities - 1;

	DirectedMap map(numDistances);
	for (int i = 0; i < map.size(); ++i) map.reserve(numDistances - i); // resurve space

	std::cout << "Num Distances: " << numDistances << std::endl;

	// Load map
	for (int i = 0; i < numDistances; ++i)
	{
		for (int j = i; j < numDistances; ++j)
		{
			inFile >> strBuffer; // read distance
			int distance = std::stoi(strBuffer) << 1; //shifted right 1 so we have 1 bit reserved, all distances are doubled

			map[i].push_back(distance);
		}
	}

	printMap(map);


	


	std::vector<int> path;

	return path;
}


