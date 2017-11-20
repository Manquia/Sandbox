#include "knapsack-dp.h"
#include <iostream>
#include <numeric>
#include <algorithm> // std::max

////////////////////////////////////////////////////////////
Item::Item( int const& weight, int const& value ) 
	: weight(weight), value(value) 
{
}

////////////////////////////////////////////////////////////
Item::Item( Item const& original ) 
	: weight(original.weight), value(original.value)
{
}

////////////////////////////////////////////////////////////
std::ostream& operator<< (std::ostream& os, Item const& item) {
	os << "(" << item.weight << " , " << item.value << ") ";
	return os;
}

////////////////////////////////////////////////////////////
std::istream& operator>> (std::istream& os, Item & item) {
	os >> item.weight >> item.value;
	return os;
}

////////////////////////////////////////////////////////////
typedef std::vector< std::vector<int> > Table; //2-dimensional table

////////////////////////////////////////////////////////////
//the returned value is a vector of indices
std::vector<int> knapsackDP( std::vector<Item> const& items, int const& W ) 
{
	int num_items = items.size();

	// Create table
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
				int grabValue = valueOfItem + table[std::max(weight - weightOfItem, 0)][i - 1];

				// Store solution into table
				table[weight][i] = std::max(grabValue, notGrabValue);
			}
			else // repeat optimal solution from last item
			{
				table[weight][i] = notGrabValue;
			}
		}
	}

	//print final table - for debugging?
    //do not delete this code
    if ( num_items + W < 50 ) { //print only if table is not too big
        std::cout << "   ";
        for ( int n=0; n<=num_items; ++n) { std::cout << n << "     "; }
        std::cout << "  items\n        ";
        for ( int n=0; n<num_items; ++n) { std::cout << items[n].weight << "," << items[n].value<<"   "; }
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

	//figure out which items are in the bag based on the table
	std::vector<int> bag;


	int weight = W;
	int itemIndex = num_items;

	while(table[weight][itemIndex - 1] > 0)
	{
		while (itemIndex > 1 && table[weight][itemIndex] == table[weight][itemIndex - 1])
		{
			--itemIndex;
		}

		bag.push_back(itemIndex -1);
		weight -= items[itemIndex - 1].weight;
	}

	
	
	return bag;
}

////////////////////////////////////////////////////////////
int valueBag( std::vector<Item> const& items, std::vector<int> const& bag ) 
{
	std::vector<int>::const_iterator it   = bag.begin(),
		                             it_e = bag.end();

	int accum = 0;
	//std::cout << "Bag ";
	for ( ; it != it_e; ++it) { 
		accum += items[ *it ].value; 
		//std::cout << *it << " ";
	}
	//std::cout << std::endl;
	return accum;
}

////////////////////////////////////////////////////////////
//prototype
//notice that auxiliary function returns value of the vector of items
//the actual vector is determined later from the table (similar to DP solution)
int knapsackRecMemAux( std::vector<Item> const&, int const&, int, Table& );

////////////////////////////////////////////////////////////
//function to kick start
std::vector<int> knapsackRecMem( std::vector<Item> const& items, int const& W ) {
	int num_items = items.size();

	// Create table
	Table table(W + 1);
	for (int i = 0; i < W + 1; ++i)
	{
		table[i].reserve(num_items);
		for (int i = 0; i < num_items + 1; ++i)
			table[i].push_back(0);
	}

	knapsackRecMemAux(items, W, 0, table);

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

	//figure out which items are in the bag based on the table
	std::vector<int> bag;
    /* ........... */
	return bag;
}

////////////////////////////////////////////////////////////
//the real recursive function
int
knapsackRecMemAux( std::vector<Item> const& items, int const& W, int index, Table & table ) {



    /* ........... */
	return 0;
}
////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////
