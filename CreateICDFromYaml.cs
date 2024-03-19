using System;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

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

        var input = new StringReader(yamlContent);
        var yamlStream = new YamlStream();
        yamlStream.Load(input);

        var rootMappingNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        Console.WriteLine("<html>");
        Console.WriteLine("<head><title>YAML to HTML Table</title></head>");
        Console.WriteLine("<body>");
        Console.WriteLine("<table border='1'>");
        Console.WriteLine("<tr><th>ParentName</th><th>FieldName</th><th>Datatype</th></tr>");

        ParseYaml(rootMappingNode);

        Console.WriteLine("</table>");
        Console.WriteLine("</body>");
        Console.WriteLine("</html>");
    }

    static void ParseYaml(YamlNode node, string parentName = "")
    {
        if (node is YamlMappingNode mappingNode)
        {
            foreach (var entry in mappingNode.Children)
            {
                var fieldName = ((YamlScalarNode)entry.Key).Value;
                var fieldValue = entry.Value;

                Console.WriteLine("<tr>");
                Console.WriteLine($"<td>{parentName}</td>");
                Console.WriteLine($"<td>{fieldName}</td>");
                Console.WriteLine($"<td>{GetDatatype(fieldValue)}</td>");
                Console.WriteLine("</tr>");

                ParseYaml(fieldValue, parentName + fieldName + ".");
            }
        }
        else if (node is YamlSequenceNode sequenceNode)
        {
            for (int i = 0; i < sequenceNode.Children.Count; i++)
            {
                ParseYaml(sequenceNode.Children[i], parentName + "[" + i + "].");
            }
        }
    }

    static string GetDatatype(YamlNode node)
{
    if (node is YamlScalarNode scalarNode)
    {
        string tag = scalarNode.Tag.ToString();
        switch (tag)
        {
            case "tag:yaml.org,2002:null":
                return "null";
            case "tag:yaml.org,2002:bool":
                return "boolean";
            case "tag:yaml.org,2002:int":
                return "integer";
            case "tag:yaml.org,2002:str":
                return "string";
            default:
                return "unknown";
        }
    }
    else if (node is YamlSequenceNode)
    {
        return "sequence";
    }
    else if (node is YamlMappingNode)
    {
        return "object";
    }
    else
    {
        return "unknown";
    }
}


}
