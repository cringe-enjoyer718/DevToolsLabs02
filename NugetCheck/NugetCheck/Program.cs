using PowerCollections;

PowerCollections.Stack<int> stack = new();

for(int i = 0; i < stack.Capacity; i++)
{
    stack.Push(i);
}

foreach(int i in stack)
{
    Console.WriteLine(i);
}