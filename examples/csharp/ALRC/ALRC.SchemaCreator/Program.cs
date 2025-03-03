using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using ALRC.Abstraction;

JsonSerializerOptions options = JsonSerializerOptions.Default;
JsonNode schema = options.GetJsonSchemaAsNode(typeof(ALRCFile));

File.WriteAllText("schema.json",schema.ToJsonString());