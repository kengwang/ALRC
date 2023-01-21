using System.Reflection;
using ALRC.Abstraction;
using NJsonSchema;

var schema = JsonSchema.FromType<ALRCFile>();

Console.WriteLine(schema.ToJson());