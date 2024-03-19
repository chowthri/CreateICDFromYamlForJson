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
            // Get the field name
            string fieldName = entry.Key;

            // Get the field value
            object fieldValue = entry.Value;

            // If the field value is a string, print it directly
            if (fieldValue is string)
            {
                Console.WriteLine("<tr>");
                Console.WriteLine($"<td>{parentName}</td>");
                Console.WriteLine($"<td>{fieldName}</td>");
                Console.WriteLine($"<td>string</td>");
                Console.WriteLine("</tr>");
            }
            else if (fieldValue is Dictionary<string, object> nestedDictionary)
            {
                // If nested dictionary found, call ParseYaml recursively with updated parentName
                ParseYaml(nestedDictionary, parentName + fieldName + ".");
            }
            else if (fieldValue is List<object> list)
            {
                // If list found, iterate through its elements
                for (int i = 0; i < list.Count; i++)
                {
                    // Call ParseYaml recursively for each element of the list
                    ParseYaml(list[i], parentName + fieldName + "[" + i + "].");
                }
            }
            else
            {
                // Print the information in HTML table row format
                Console.WriteLine("<tr>");
                Console.WriteLine($"<td>{parentName}</td>");
                Console.WriteLine($"<td>{fieldName}</td>");
                Console.WriteLine($"<td>{GetDatatype(fieldValue)}</td>");
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
