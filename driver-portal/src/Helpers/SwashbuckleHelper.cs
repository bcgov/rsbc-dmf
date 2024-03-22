namespace Rsbc.Dmf.DriverPortal.Api
{
    // this will avoid conflicts with names that are the same but in different namespaces
    // by adding a numeric incrementing suffix
    // https://stackoverflow.com/questions/61881770/invalidoperationexception-cant-use-schemaid-the-same-schemaid-is-already-us
    internal static class SwashbuckleHelper
    {
        static SwashbuckleHelper() 
        {
            _schemaNameRepetition = new Dictionary<string, int>();
            _schemaNameRepetition.Clear();
        }

        private static readonly Dictionary<string, int> _schemaNameRepetition;

        public static string GetSchemaId(Type type)
        {
            string id = type.Name;

            if (!_schemaNameRepetition.ContainsKey(id))
                _schemaNameRepetition.Add(id, 0);

            int count = (_schemaNameRepetition[id] + 1);
            _schemaNameRepetition[id] = count;

            return type.Name + (count > 1 ? count.ToString() : "");
        }
    }
}
