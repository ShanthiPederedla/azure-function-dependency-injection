namespace DependencyInjection
{
    public class AGreeter : IGreeter
    {
        public string Greet()
        {
            return "Hello World with!";
        }
    }

    public class CountUpGreeter : IGreeter
    {
        private int count = 0;
        public string Greet()
        {
            return $"Hello World with {count++}!";
        }
    }
}
