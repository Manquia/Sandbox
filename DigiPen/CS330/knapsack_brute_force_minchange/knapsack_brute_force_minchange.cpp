#include "knapsack_brute_force_minchange.h"

#include <assert.h>
#include <iostream>
#include "definitions.h"

GreyCode::GreyCode(int s)
	:codes(1ULL << s, 0), index(1)
{
	assert(s > 0);
	size_t size = static_cast<size_t>(s);

	// base case n = 1
	codes[0] = 0;
	codes[1] = 1;

	// reflect and mask
	for (size_t i = 1; i < size; ++i)
	{
		size_t bitToMask = 1 << i;
		size_t halfwayPoint = 1 << i;
		for (size_t r = 1; r <= halfwayPoint; ++r)
		{
			size_t indexReflect = halfwayPoint * 2 - r;
			size_t indexSource = static_cast<size_t>(r - 1);

			// reflex bit and mask
			codes[indexReflect] = codes[indexSource] | bitToMask;
		}
	}
}

////////////////////////////////////////////////////////////////////////////////
std::pair< bool, std::pair< bool, int > > GreyCode::Next() 
{   
	assert(index > 0);

	int	pastCode = codes[index - 1];
	int nextCode = codes[index];

	// XOR
	int diffCode = pastCode ^ nextCode;

	// did we add or take away a bit?
	// is true if new value is 1 (add item), false otherwise 
	bool add = !!(diffCode & nextCode);

	int  pos = 0;	// which position is modified 
	
	while ((diffCode >> 1) > 0)
	{
		++pos;
		diffCode = diffCode >> 1;
	}


	++index;
	bool last = index == codes.size();	// is this the last permutation 
    return std::make_pair( !last, std::make_pair( add, pos ) );
}

void GreyCode::PrintCode()
{
	for (auto& it : codes)
	{
		std::cout << it << std::endl;
	}
}

////////////////////////////////////////////////////////////////////////////////
std::vector<bool> knapsack_brute_force( std::vector<Item> const& items, Weight const& W )
{
	GreyCode gc(items.size());

	std::vector<bool> bestItemsTaken(items.size(), false);
	Weight bestWeight;
	int bestValue = 0;


	std::vector<bool> curItemsTaken(items.size(), false);
	Weight curWeight;
	int curValue = 0;

	bool getNext = true;
	while (getNext)
	{
		std::pair< bool, std::pair< bool, int > > r = gc.Next();
		getNext = r.first;
		bool add = r.second.first;
		int  pos = r.second.second;

		int itemValue = items[pos].GetValue();
		Weight itemWeight = items[pos].GetWeight();

		// Set current items to have the correct amount
		curItemsTaken[pos] = add;
		if (add)
		{
			curWeight += itemWeight;
			curValue += itemValue;
		}
		else
		{
			curWeight -= itemWeight;
			curValue -= itemValue;
		}

		
		if (curValue > bestValue &&		// is best value
			curWeight <= W)				// can carry subset
		{
			bestValue = curValue;
			bestWeight = curWeight;
			bestItemsTaken = curItemsTaken;
		}
	}
	return bestItemsTaken;
}
