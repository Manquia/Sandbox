
//
// The Artful (Micah Rust) C++ Library
//

namespace TA
{
	typedef int Type;

	struct vector
	{
		size_t size();
		void reserve();
		void push_back(Type item);
		void pop_back();
		Type back();
		Type front();
		Type operator[](size_t index);
		Type* data();
		void clear();
		void erase();
		//void emplace_back(...);
		
		// Helpers
		private:
		void Resize();
		
		// Data
		size_t count;
		Type* data;
	}
}