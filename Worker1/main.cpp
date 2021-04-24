
extern "C"  int tester(int a, int b);

extern "C" __declspec(dllexport)
int some_operation(int a, int b)
{
	int z = tester(a, b);
	return z;
}