using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string yamlContent = @"
json:
  - rigid
  - better for data interchange
yaml: 
  - slim and flexible
  - better for configuration
object:
  key: value
  array:
    - null_value:
    - boolean: true
    - integer: 1
    - alias: &example aliases are like variables
    - alias: *example
paragraph: >
   Blank lines denote

   paragraph breaks
content: |-
   Or we
   can auto
   convert line breaks
   to save space
alias: &foo
  bar: baz
alias_reuse: *foo";

        var deserializer = new DeserializerBuilder().Build();
        var yamlObject = deserializer.Deserialize<Dictionary<string, object>>(new StringReader(yamlContent));

        Console.WriteLine("<html>");
        Console.WriteLine("<head><title>YAML to HTML Table</title></head>");
        Console.WriteLine("<body>");
        Console.WriteLine("<table border='1'>");
        Console.WriteLine("<tr><th>ParentName</th><th>FieldName</th><th>Datatype</th></tr>");

        ParseYaml(yamlObject);

        Console.WriteLine("</table>");
        Console.WriteLine("</body>");
        Console.WriteLine("</html>");
    }

    static void ParseYaml(object obj, string parentName = "")
    {
        if (obj is Dictionary<object, object> dictionary)
        {
            foreach (var entry in dictionary)
            {
                if (entry.Value is Dictionary<object, object>)
                {
                    ParseYaml(entry.Value, parentName + entry.Key + ".");
                }
                else if (entry.Value is List<object>)
                {
                    int index = 0;
                    foreach (var item in (List<object>)entry.Value)
                    {
                        ParseYaml(item, parentName + entry.Key + "[" + index + "].");
                        index++;
                    }
                }
                else
                {
                    Console.WriteLine("<tr>");
                    Console.WriteLine($"<td>{parentName}</td>");
                    Console.WriteLine($"<td>{entry.Key}</td>");
                    Console.WriteLine($"<td>{GetDatatype(entry.Value)}</td>");
                    Console.WriteLine("</tr>");
                }
            }
        }
    }

    static string GetDatatype(object value)
    {
        if (value == null)
            return "null";
        else if (value is bool)
            return "boolean";
        else if (value is int)
            return "integer";
        else if (value is string)
            return "string";
        else
            return "unknown";
    }
}
