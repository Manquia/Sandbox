//
// Author: Micah Rust
// Class: CS 330
// Assignment: knapsack_dynprog
//


#include "knapsack-dp.h"
#include <iostream>
#include <numeric>
#include <algorithm> // std::max


//
// ------------------------------------------------------------------------------------------
// Forward declarations

typedef std::vector< std::vector<int> > Table; //2-dimensional table

int knapsackRecMemAux(std::vector<Item> const&, int const, int, Table&);
static void FillBag(std::vector<Item> const& items, const Table& table,
	std::vector<int>& bag, int const& W, int num_items);
static void PrintTable(std::vector<Item> const& items, const Table& table,
	int const& W, int num_items);
int valueBag(std::vector<Item> const& items, std::vector<int> const& bag);


//
// ------------------------------------------------------------------------------------------
// Helper functions
Item::Item( int const& weight, int const& value ) 
	: weight(weight), value(value) 
{
}
Item::Item( Item const& original ) 
	: weight(original.weight), value(original.value)
{
}

std::ostream& operator<< (std::ostream& os, Item const& item) {
	os << "(" << item.weight << " , " << item.value << ") ";
	return os;
}
std::istream& operator>> (std::istream& os, Item & item) {
	os >> item.weight >> item.value;
	return os;
}


//
// Takes the memoised table and returns the optimal items.
//
static void FillBag(std::vector<Item> const& items, const Table& table,
	std::vector<int>& bag, int const& W, int num_items)
{
	bag.clear();
	int weight = W;
	int itemIndex = num_items;

	while (itemIndex > 0)
	{
		while (itemIndex > 1 && table[weight][itemIndex] == table[weight][itemIndex - 1])
		{
			--itemIndex;
		}

		if (weight >= items[itemIndex - 1].weight)
			bag.push_back(itemIndex - 1);

		weight -= items[itemIndex - 1].weight;
		--itemIndex;
	}
}

//
// The given print code factored out to make things a bit cleaner
//
static void PrintTable(std::vector<Item> const& items, const Table& table,
	int const& W, int num_items)
{
	//print table - debugging?
    //do not delete this code
    if ( num_items + W < 50 ) { //print only if table is not too big
        std::cout << "   ";
        for ( int n=0; n<=num_items; ++n) { std::cout << n << "     "; }
        std::cout << "  items\n        ";
        for ( int n=0; n<num_items; ++n) { 
            std::cout << items[n].weight << "," << items[n].value<<"   "; 
        }
        std::cout << "\n   ";
        for ( int n=0; n<=num_items; ++n) { std::cout << "------"; }
        std::cout << std::endl;

        for ( int w=0; w<=W; ++w) {
            std::cout << w << "| ";
            for ( int n=0; n<=num_items; ++n) {
                std::cout << table[w][n] << "     ";
            }
            std::cout << std::endl;
        }
    }
    //end do not delete this code 
}


//
// Prints out the value of the bag with a set of corralting items
//
int valueBag(std::vector<Item> const& items, std::vector<int> const& bag)
{
	std::vector<int>::const_iterator it = bag.begin(),
		it_e = bag.end();

	int accum = 0;
	//std::cout << "Bag ";
	for (; it != it_e; ++it) {
		accum += items[*it].value;
		//std::cout << *it << " ";
	}
	//std::cout << std::endl;
	return accum;
}


//
// ------------------------------------------------------------------------------------------
// Interface


//
// the returned value is a vector of indices by using a bottom-up approach
// of dynamic programming. This is the itterative solution
//
std::vector<int> knapsackDP( std::vector<Item> const& items, int const& W ) 
{
	int num_items = items.size();

	// Initialize table
	Table table(W + 1);
	for (int i = 0; i <= W; ++i)
	{
		table[i].reserve(num_items + 1);
		for (int j = 0; j <= num_items; ++j)
			table[i].push_back(0);
	}

	// build the solution table
	for (int weight = 1; weight <= W; ++weight)
	{
		for (int i = 1; i <= num_items; ++i)
		{
			// Solution if we do not grab this item, and instead grab the last best solution for this weight
			const int& notGrabValue = table[weight][i - 1];

			// Can grab item for given sub-solution
			if (items[i-1].weight <= weight)
			{
				const int& weightOfItem = items[i-1].weight;
				const int& valueOfItem = items[i-1].value;

				// Solution if we grab item + optimal solution with remaining weight
				const int& grabValue = valueOfItem + table[std::max(weight - weightOfItem, 0)][i - 1];

				// Store solution into table
				table[weight][i] = std::max(grabValue, notGrabValue);
			}
			else // repeat optimal solution from last item
			{
				table[weight][i] = notGrabValue;
			}
		}
	}

	// Moved out to make this a bit simpler
	PrintTable(items, table, W, num_items);

	std::vector<int> bag;
	FillBag(items, table, bag, W, num_items);
	
	return bag;
}

//
// Solves the knapsack problem for a given set of items using the recursive
// method and a table for lookups
//
std::vector<int> knapsackRecMem( std::vector<Item> const& items, int const& W ) {
	int num_items = items.size();

	// Create table
	Table table(W + 1);
	// Set first row to all 0 ,0, 0, ...
	table[0].reserve(num_items + 1);
	for (int i = 0; i <= num_items; ++i)
		table[0].push_back(0);

	// Set all other rows to 0 ,-1, -1 ...
	for (int i = 1; i <= W; ++i)
	{
		table[i].reserve(num_items + 1);
		table[i].push_back(0);
		for (int j = 1; j <= num_items; ++j)
			table[i].push_back(-1);
	}

	// Fill table recursivly
	knapsackRecMemAux(items, W, num_items, table);

	// Moved out to make this a bit simpler
	PrintTable(items, table, W, num_items);

	//figure out which items are in the bag based on the table
	std::vector<int> bag;
	FillBag(items, table, bag, W, num_items);

	return bag;
}

//
// Recursive function which fills all of the nessesary fields to solve the knapsack problem.
// notice that auxiliary function returns value of the vector of items
// the actual vector is determined later from the table (similar to DP solution)
//
int knapsackRecMemAux( std::vector<Item> const& items, int const weight, int index, Table & table ) 
{
	// returned result
	int result = 0;
	
	// Base case, lookup value, if present return value
	if (table[weight][index] != -1) // valid lookup
	{
		result = table[weight][index];
	}
	else if (items[index - 1].weight <= weight) // Can Grab item
	{
		const int weightRemainingAfterGrab = weight - items[index - 1].weight;
		const int grabValue = items[index - 1].value + knapsackRecMemAux(items, weightRemainingAfterGrab, index - 1, table);
		const int notGrabValue = knapsackRecMemAux(items, weight, index - 1, table);

		// Store solution into table, and return it
		result = table[weight][index] = std::max(grabValue, notGrabValue);
	}
	else // Get Result from left
	{
		const int notGrabValue = knapsackRecMemAux(items, weight, index - 1, table);

		// Store solution into table, and return it
		result = table[weight][index] = notGrabValue;
	}

	return result;
}
