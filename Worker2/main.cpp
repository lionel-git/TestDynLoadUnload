
extern "C"  int tester(int a, int b);


extern "C" __declspec(dllexport)
int some_operation(int a, int b)
{
	int w = tester(a, b);
	return a - b;
}