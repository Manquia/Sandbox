#include "perm-jt.h"
#include <iterator> //ostream_iterator
#include <algorithm>
#include <iostream>

struct Print {
	void operator() ( std::vector<int> const& v) const {
		std::copy( v.begin(), v.end(), 
				std::ostream_iterator<int>(std::cout, " ") );
		std::cout << std::endl;
	}
};

int main() {



	for (size_t i = 1; i < 9; ++i)
	{
		PermJohnsonTrotter pjt(i);


		std::cout << "Permutation digits: " << i << std::endl << std::endl;

		float averageSwitches = 0;
		size_t maxSwitches = 0;
		size_t iterations = 0;
		do
		{
			++iterations;
			size_t countSwitches = pjt.GetSwitches();
			maxSwitches = std::max(countSwitches, maxSwitches);
			averageSwitches += countSwitches;
			//std::cout << countSwitches << "\n";
		} while (pjt.Next());
		averageSwitches /= iterations;

		//std::cout << std::endl << std::endl;

		std::cout << "Max Switches(" << i << "):" << maxSwitches << std::endl;
		std::cout << "Average Switches(" << i << "): " << averageSwitches << std::endl;
	}

	int myInput;
	std::cin >> myInput;
}
