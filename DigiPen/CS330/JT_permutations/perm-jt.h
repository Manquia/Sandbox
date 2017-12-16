// Author: Micah Rust
// Course: CS 330, assignment Johnson Trotter
// Date: 2017/12/13

#ifndef PERM_JOHNSON_TROTTER_H
#define PERM_JOHNSON_TROTTER_H
#include <vector>

// Impliments the Johnson Trotter algorithm
class PermJohnsonTrotter {
	public:
		// Initialization of the length of elements
		PermJohnsonTrotter(int size); 
		// Get next permutation
		bool Next();
		// returns the current permutation	
		std::vector<int> const& Get() const;

		size_t GetSwitches() const;

	private:
		// helper functions
		int FindLargestMobileIndex();
		bool IsMobile(int index);

		// data
		std::vector<int> data;
		std::vector<int> direction;
		size_t switchCount;
};
#endif // PERM_JOHNSON_TROTTER_H
