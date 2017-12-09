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

void printMap(MAP map)
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

	std::vector<int> path(numCities + 1);
	MAP map(numDistances);
	for (int i = 0; i < map.size(); ++i) map.reserve(numDistances - i); // resurve space

	std::cout << "Num Distances: " << numDistances << std::endl;

	// Load map
	for (int i = 0; i < numDistances; ++i)
	{
		for (int j = i; j < numDistances; ++j)
		{
			inFile >> strBuffer; // read distance
			int distance = std::stoi(strBuffer);

			map[i].push_back(distance);
		}
	}

	printMap(map);


	// indexer
	// int& number = map[i][j-i]




	
	return path;
}