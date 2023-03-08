using PowerCollections;

PowerCollections.Stack<int> stack = new (10);

for(int i = 0; i < 10; i++)
{
    stack.Push(i);
}

foreach(int i in stack)
{
    Console.WriteLine(i);
}