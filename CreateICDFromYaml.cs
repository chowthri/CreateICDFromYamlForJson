using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

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
        var yamlObject = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);

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
        if (obj is Dictionary<string, object> dictionary)
        {
            foreach (var entry in dictionary)
            {
                // Check if the value is another nested dictionary
                if (entry.Value is Dictionary<string, object> nestedDictionary)
                {
                    // If nested dictionary found, call ParseYaml recursively with updated parentName
                    ParseYaml(nestedDictionary, parentName + entry.Key + ".");
                }
                // Check if the value is a list
                else if (entry.Value is List<object> list)
                {
                    // If list found, iterate through its elements
                    for (int i = 0; i < list.Count; i++)
                    {
                        // Call ParseYaml recursively for each element of the list
                        ParseYaml(list[i], parentName + entry.Key + "[" + i + "].");
                    }
                }
                // If the value is not a nested dictionary or a list, it's a leaf node (scalar)
                else
                {
                    // Print the information in HTML table row format
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
