using ALRC.Abstraction;
using NJsonSchema;

var schema = JsonSchema.FromType<ALRCFile>();

File.WriteAllText("schema.json",schema.ToJson());