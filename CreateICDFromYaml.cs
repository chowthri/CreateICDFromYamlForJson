using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

class Program
{
    static void Main(string[] args)
    {
        // Load YAML data from file
        string yamlFilePath = "path/to/your/yaml/file.yaml";
        string yamlData = File.ReadAllText(yamlFilePath);

        // Deserialize YAML data
        var serializer = new XmlSerializer(typeof(ContractData));
        using (var reader = new StringReader(yamlData))
        {
            var contractData = (ContractData)serializer.Deserialize(reader);

            // Generate Word document
            using (var document = WordprocessingDocument.Create("Interface_Contract_Document.docx", WordprocessingDocumentType.Document))
            {
                var mainPart = document.AddMainDocumentPart();
                var body = new Body();

                // Create table
                var table = new Table();
                var tableProperties = new TableProperties(
                    new TableWidth { Type = TableWidthUnitValues.Auto },
                    new TableLayout { Type = TableLayoutValues.Fixed }
                );
                table.AppendChild(tableProperties);

                // Add table header row
                var headerRow = new TableRow();
                headerRow.AppendChild(CreateTableCell("ParentName", true));
                headerRow.AppendChild(CreateTableCell("Field Name", true));
                headerRow.AppendChild(CreateTableCell("Reference Name", true));
                table.AppendChild(headerRow);

                // Add content to the table
                foreach (var item in contractData.Contracts)
                {
                    foreach (var method in item.Methods)
                    {
                        var dataRow = new TableRow();
                        dataRow.AppendChild(CreateTableCell(item.ParentName));
                        dataRow.AppendChild(CreateTableCell(method.Name));
                        dataRow.AppendChild(CreateTableCell(method.Reference));
                        table.AppendChild(dataRow);
                    }
                }

                // Add table to the document body
                body.AppendChild(table);

                mainPart.Document = new Document(body);
            }
        }

        Console.WriteLine("Interface Contract Document created successfully!");
    }

    // Helper method to create table cell
    static TableCell CreateTableCell(string text, bool isHeader = false)
    {
        var cell = new TableCell(new Paragraph(new Run(new Text(text))));
        if (isHeader)
        {
            cell.TableCellProperties = new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto });
            cell.TableCellProperties.AppendChild(new TableHeader());
        }
        return cell;
    }
}

// Define classes to represent YAML structure
public class ContractData
{
    public List<Contract> Contracts { get; set; }
}

public class Contract
{
    public string ParentName { get; set; }
    public List<Method> Methods { get; set; }
}

public class Method
{
    public string Name { get; set; }
    public string Reference { get; set; }
}
