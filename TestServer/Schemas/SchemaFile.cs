using System.Text;
using Newtonsoft.Json.Schema;

namespace TestServer.Schemas
{
    internal class SchemaFile
    {
        public static JSchema Read(string filename)
        {
            var streams = typeof(SchemaFile).Assembly.GetManifestResourceNames();
            var streamName = streams.FirstOrDefault(x => x.EndsWith($"{filename}.json"));

            if (streamName is not null)
            {
                using var resource = typeof(SchemaFile).Assembly.GetManifestResourceStream(streamName);
                using var reader = new StreamReader(resource, Encoding.UTF8);

                var schemaJson = reader.ReadToEnd();
                var schemaParse = JSchema.Parse(schemaJson);

                return schemaParse;
            }
            else
            {
                throw new FileNotFoundException("Json schema can't be found.", $"{filename}.json");
            }           
        }
    }
}
