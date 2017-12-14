// Author: Micah Rust
// Course: CS 330, assignment Johnson Trotter
// Date: 2017/12/13

#include "perm-jt.h"	/* header */
#include <assert.h>		/* assert */
#include <stdlib.h>		/* abs */

// Initialize the number of elements to use in the Johnson Trotter algorithm
PermJohnsonTrotter::PermJohnsonTrotter(int size)
	:data(size), direction(size)
{
	// setup number seq
	for (int i = 0; i < size; ++i)
		data[i] = (i + 1);

	// setup directions
	for (int i = 0; i < size; ++i)
		direction[i] = -(i + 1);
}

// Computes the next permutation of the given elements
bool PermJohnsonTrotter::Next()
{
	int index = FindLargestMobileIndex();
	
	if (index == -1) // no mobile index found
		return false;
	
	// get neighbor index
	int neighborIndex = index;
	if (direction[index] > 0) neighborIndex += 1;
	else neighborIndex -= 1;

	// swap with neighbor
	std::swap(direction[index], direction[neighborIndex]);
	std::swap(data[index], data[neighborIndex]);

	// reverse all elements larger than our swapped element
	int swappedValue = direction[neighborIndex];
	for (unsigned i = 0; i < direction.size(); ++i)
	{
		if (abs(direction[i]) > abs(swappedValue))
		{
			direction[i] = -direction[i];
		}
	}
	return true;
}

// returns index of largest mobile index
// returns -1 when there are no mobile indexes
int PermJohnsonTrotter::FindLargestMobileIndex()
{
	int largestMobileIndex = -1;
	int largestMobileVector = 0;

	assert(direction.size() == data.size());
	assert(direction.size() > 0);

	for (unsigned i = 0; i < direction.size(); ++i)
	{
		if (abs(direction[i]) > abs(largestMobileVector) && IsMobile(i))
		{
			largestMobileIndex = i;
			largestMobileVector = direction[i];
		}
	}


	return largestMobileIndex;
}

// Helper function to determine if an index is mobile.
// Does index bound checking on neighboring index so any
// valid index into the direction vector is valid
bool PermJohnsonTrotter::IsMobile(int index)
{
	assert(index > -1);				// must havea a valid index
	assert(direction[index] != 0);	// must always have a direction

	// get direction
	int dirVec;
	if (direction[index] > 0)
		dirVec = 1;		// right
	else if(direction[index] < 0)
		dirVec = -1;	// left

	int neighborIndex = dirVec + index;
	if (neighborIndex >= static_cast<int>(direction.size()) ||
		neighborIndex < 0)
		return false; // direction points out of bounds

	// IsMobile means greater than its immediate neighbor in the direction
	// it is pointing to
	return abs(direction[index]) > abs(direction[neighborIndex]);
}

// returns a copy of the current permutation of the elements
std::vector<int> const & PermJohnsonTrotter::Get() const
{
	return data;
}