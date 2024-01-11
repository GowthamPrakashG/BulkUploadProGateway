using System.Data;
using System.Drawing;
using System.Text;
using DBUtilityHub.Data;
using Spire.Xls;
using ExcelSyncHub.Model.DTO;
using DBUtilityHub.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ExcelSyncHub.Models.DTO;
using Newtonsoft.Json;
using ExcelSyncHub.Service.IService;

namespace ExcelSyncHub.Service
{
    public class ExcelService : IExcelService
    {

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExcelService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //Generate Excel
        public byte[] GenerateExcelFile(List<ColumnMetaDataDTO> columns, int? parentId)
        {
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            // Add the first worksheet with detailed column information
            worksheet.Name = "DataDictionary";

            // Set protection options for the first sheet (read-only)
            worksheet.Protect("your_password", SheetProtectionType.All);
            worksheet.Protect("your_password", SheetProtectionType.None);
            worksheet.DefaultRowHeight = 15;
            var headerColor = worksheet.Range["A2:O2"];
            headerColor.Style.FillPattern = ExcelPatternType.Solid;
            headerColor.Style.Font.Color = Color.White;
            headerColor.Style.KnownColor = ExcelColors.Blue;
            headerColor.Style.Font.IsBold = true;
            worksheet.DefaultColumnWidth = 15;
            // Add column headers for the first sheet
            worksheet.Range["A2"].Text = "SI.No";
            worksheet.Range["B2"].Text = "Data Item";
            worksheet.Range["C2"].Text = "Data Type";
            //worksheet.Range["D2"].Text = "Length";
            worksheet.Range["D2"].Text = "MinLength";
            worksheet.Range["E2"].Text = "MaxLength";
            worksheet.Range["F2"].Text = "MinRange";
            worksheet.Range["G2"].Text = "MaxRange";
            worksheet.Range["H2"].Text = "DateMinValue";
            worksheet.Range["I2"].Text = "DateMaxValue";
            worksheet.Range["J2"].Text = "Description";
            worksheet.Range["K2"].Text = "Blank Not Allowed";
            worksheet.Range["L2"].Text = "Default Value";
            worksheet.Range["M2"].Text = "Unique Value";
            worksheet.Range["N2"].Text = "Option1";
            worksheet.Range["O2"].Text = "Option2";

            // Populate the first sheet with column details
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                int row = i + 3; // Adjust the row index to start from 3

                if (row % 2 == 0)
                {
                    worksheet.Range[row, 1, row, 15].Style.Color = Color.LightBlue; // Set even row color
                }
                else
                {
                    worksheet.Range[row, 1, row, 15].Style.Color = Color.Azure; // Set odd row color
                }
                worksheet.Range[i + 3, 1].Value = (i + 1).ToString();
                worksheet.Range[i + 3, 2].Text = column.ColumnName;
                worksheet.Range[i + 3, 3].Text = column.Datatype;
                if (column.MinLength == null || column.MinLength == 0)
                {
                    worksheet.Range[i + 3, 4].Text = string.Empty;
                }
                else
                {
                    worksheet.Range[i + 3, 4].Text = column.MinLength.ToString();
                }
                if (column.MaxLength == 0)
                {
                    worksheet.Range[i + 3, 5].Text = string.Empty;
                }
                else
                {
                    worksheet.Range[i + 3, 5].Text = column.MaxLength.ToString();
                }
                if (column.MinRange == null || column.MinRange == 0)
                {
                    worksheet.Range[i + 3, 6].Text = string.Empty;
                }
                else
                {
                    worksheet.Range[i + 3, 6].Text = column.MinRange.ToString();
                }
                if (column.MaxRange == 0)
                {
                    worksheet.Range[i + 3, 7].Text = string.Empty;
                }
                else
                {
                    worksheet.Range[i + 3, 7].Text = column.MaxRange.ToString();
                }
                if (string.IsNullOrEmpty(column.DateMinValue) && string.IsNullOrEmpty(column.DateMaxValue))
                {
                    worksheet.Range[i + 3, 8].Text = string.Empty;
                    worksheet.Range[i + 3, 9].Text = string.Empty;
                }
                else
                {
                    worksheet.Range[i + 3, 8].Text = column.DateMinValue;
                    worksheet.Range[i + 3, 9].Text = column.DateMaxValue;
                }
                worksheet.Range[i + 3, 9].Text = column.DateMaxValue.ToString();
                worksheet.Range[i + 3, 10].Text = column.Description;
                worksheet.Range[i + 3, 11].Text = column.IsNullable.ToString();

                if (column.Datatype.ToLower() == "boolean")
                {
                    if (column.DefaultValue.ToLower() == "true")
                    {
                        worksheet.Range[i + 3, 12].Text = column.True;
                    }
                    else if (column.DefaultValue.ToLower() == "false")
                    {
                        worksheet.Range[i + 3, 12].Text = column.False;
                    }
                }
                else
                {
                    worksheet.Range[i + 3, 12].Text = column.DefaultValue.ToString();
                }

                worksheet.Range[i + 3, 13].Text = column.IsPrimaryKey.ToString();
                worksheet.Range[i + 3, 14].Text = column.True.ToString();
                worksheet.Range[i + 3, 15].Text = column.False.ToString();

                int lastRowIndex1 = worksheet.Rows.Length;

                int entityId = column.EntityId;
                worksheet.Range["A1"].Text = entityId.ToString();
            }
            worksheet.HideRow(1);

            // Add static content in the last row (vertically)
            var lastRowIndex = worksheet.Rows.Length;
            for (int i = lastRowIndex + 2; i <= lastRowIndex + 7; i++)
            {
                if (i % 2 == 0)
                {
                    worksheet.Range[i, 1, i, 5].Style.Color = Color.LightGray; // Set even row color
                }
                else
                {
                    worksheet.Range[i, 1, i, 5].Style.Color = Color.LightSkyBlue; // Set odd row color
                }
            }
            worksheet.Range[lastRowIndex + 2, 1].Text = "";
            worksheet.Range[lastRowIndex + 3, 1].Text = "Note:";
            worksheet.Range[lastRowIndex + 4, 1].Text = "1. Don't add or delete any columns";
            worksheet.Range[lastRowIndex + 5, 1].Text = "2. Don't add any extra sheets";
            worksheet.Range[lastRowIndex + 6, 1].Text = "3. Follow the length if mentioned";
            if (parentId.HasValue)
            {
                worksheet.Range[lastRowIndex + 7, 1].Text = "4. This is Exported Data ExcelFile";
            }
            var staticContentRange = worksheet.Range[lastRowIndex + 2, 1, lastRowIndex + 7, 5];
            staticContentRange.Style.FillPattern = ExcelPatternType.Solid;
            staticContentRange.Style.KnownColor = ExcelColors.Yellow;
            // Add the second worksheet for column names
            Worksheet columnNamesWorksheet;

            if (!parentId.HasValue)
            {
                columnNamesWorksheet = workbook.Worksheets.Add("Fill data");
            }
            else
            {
                columnNamesWorksheet = workbook.Worksheets.Add("Error Records");
            }

            // After adding content to the columns
            // Set a default column width for the "Fill data" worksheet
            columnNamesWorksheet.DefaultColumnWidth = 15; // Set the width in characters (adjust as needed)
            columnNamesWorksheet.DefaultRowHeight = 15;
            int lastColumnIndex = columns.Count + 1;

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                columnNamesWorksheet.Range[2, i + 1].Text = column.ColumnName;
                int entityId = column.EntityId;
                columnNamesWorksheet.Range["A1"].Text = entityId.ToString();
                columnNamesWorksheet.Range[2, 1, 2, lastColumnIndex - 1].Style.Color = Color.Blue;
                columnNamesWorksheet.Range[2, 1, 2, lastColumnIndex - 1].Style.Font.IsBold = true;
                columnNamesWorksheet.Range[2, 1, 2, lastColumnIndex - 1].Style.Font.Color = Color.White;
            }
            columnNamesWorksheet.HideRow(1);

            if (parentId.HasValue)
            {
                InsertDataIntoExcel(columnNamesWorksheet, columns, parentId);
            }

            string[] sheetsToRemove = { "Sheet2", "Sheet3" }; // Names of sheets to be removed
            foreach (var sheetName in sheetsToRemove)
            {
                Worksheet sheetToRemove = workbook.Worksheets[sheetName];
                if (sheetToRemove != null)
                {
                    workbook.Worksheets.Remove(sheetToRemove);
                }
            }
            AddDataValidation(columnNamesWorksheet, columns, parentId);


            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveToStream(memoryStream, FileFormat.Version2013);
                return memoryStream.ToArray();
            }
        }

        //HighLight Formula
        private void HighlightDuplicates(Worksheet sheet, int columnNumber, int startRow, int endRow)
        {
            string columnLetter = GetExcelColumnName(columnNumber);

            string range = $"{columnLetter}{startRow}:{columnLetter}{endRow}";
            ConditionalFormatWrapper format = sheet.Range[range].ConditionalFormats.AddCondition();
            format.FormatType = ConditionalFormatType.DuplicateValues;
            format.BackColor = Color.IndianRed;
        }

        // Validation formulas
        private void AddDataValidation(Worksheet columnNamesWorksheet, List<ColumnMetaDataDTO> columns, int? parentId)
        {
            int startRow = 2; // The first row where you want validation
            int endRow = 100000;  // Adjust the last row as needed

            if (parentId.HasValue)
            {
                int columnCount = columnNamesWorksheet.Columns.Length - 2;
                char letter = 'A';
                char lastletter = 'A';
                // Protect the worksheet with a password
                columnNamesWorksheet.Protect("password");
                for (int i = 2; i <= columnCount; i++)
                {
                    lastletter++;
                }
                for (int col = 1; col <= columnCount; col++)
                {
                    // Get the data type for the current column
                    string dataType = columns[col - 1].Datatype;
                    int length = (int)columns[col - 1].Length;
                    bool isPrimaryKey = columns[col - 1].IsPrimaryKey;
                    string truevalue = columns[col - 1].True;
                    string falsevalue = columns[col - 1].False;
                    bool isNullable = columns[col - 1].IsNullable;
                    int? nullableMinRange = columns[col - 1].MinRange;
                    int? nullableMaxRange = columns[col - 1].MaxRange;
                    int minRange = nullableMinRange.HasValue ? nullableMinRange.Value : 0; // Use 0 as a default value when MinLength is null
                    int maxRange = nullableMaxRange.HasValue ? nullableMaxRange.Value : 0; // Use 0 as a default value when MaxLength is null
                    int? nullableMinLength = columns[col - 1].MinLength;
                    int? nullableMaxLength = columns[col - 1].MaxLength;
                    int minLength = nullableMinLength.HasValue ? nullableMinLength.Value : 0; // Use 0 as a default value when MinLength is null
                    int maxLength = nullableMaxLength.HasValue ? nullableMaxLength.Value : 0; // Use 0 as a default value when MaxLength is null
                    string dateMinValue = columns[col - 1].DateMinValue;
                    string dateMaxValue = columns[col - 1].DateMaxValue;

                    // Specify the range for data validation
                    CellRange range = columnNamesWorksheet.Range[startRow, col, endRow, col];
                    Validation validation = range.DataValidation;


                    //Protect the worksheet with password
                    columnNamesWorksheet.Protect("123456", SheetProtectionType.All);
                    List<string> integerTypes = new List<string> { "int", "integer", "number", "numeric", /* add more as needed */ };
                    List<string> stringTypes = new List<string> { "string", "varchar", "nvarchar", "text", "character varying"/* add more as needed */ };
                    List<string> timestampTypes = new List<string> { "timestamp", "datetime", "timestamp without time zone", "date" /* add more as needed */ };



                    if (stringTypes.Contains(dataType, StringComparer.OrdinalIgnoreCase))
                    {
                        // Text validation with min and max length
                        validation.CompareOperator = ValidationComparisonOperator.Between;
                        if ((minLength == 0) && (maxLength == 0))
                        {
                            // Handle the case when both minimum and maximum length are 0
                            validation.Formula1 = "0";
                            validation.Formula2 = "0";
                        }
                        else if ((!string.IsNullOrEmpty(minLength.ToString()) || minLength == 0) && (string.IsNullOrEmpty(maxLength.ToString()) || maxLength == 0))
                        {
                            // Minimum length provided, no maximum length
                            validation.Formula1 = minLength.ToString();
                            validation.AllowType = CellDataType.TextLength;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Enter a value with a minimum length of {validation.Formula1} characters.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = $"The value must have a minimum length of {validation.Formula1} characters.";
                        }
                        else if ((string.IsNullOrEmpty(minLength.ToString()) || minLength == 0) && (!string.IsNullOrEmpty(maxLength.ToString()) || maxLength == 0))
                        {
                            validation.Formula2 = maxLength.ToString();
                            validation.AllowType = CellDataType.TextLength;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Type text with a maximum length of {validation.Formula2} characters.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "The entered value exceeds the allowed length.";
                        }
                        else
                        {
                            // Both minimum and maximum length provided
                            validation.Formula1 = minLength.ToString();
                            validation.Formula2 = maxLength.ToString();
                            validation.AllowType = CellDataType.TextLength;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Type text with a length between {validation.Formula1} and {validation.Formula2} characters.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "Entered value should be within the specified length range.";
                        }
                        if (isPrimaryKey)
                        {
                            HighlightDuplicates(columnNamesWorksheet, col, startRow, endRow);
                            validation.InputMessage = $"Enter the unique value.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "Entered Values must be unique";
                        }
                    }
                    else if (integerTypes.Contains(dataType, StringComparer.OrdinalIgnoreCase))
                    {
                        validation.CompareOperator = ValidationComparisonOperator.Between;
                        if (isPrimaryKey)
                        {
                            HighlightDuplicates(columnNamesWorksheet, col, startRow, endRow);

                            validation.CompareOperator = ValidationComparisonOperator.Between;
                            if ((minRange == 0) && (maxRange == 0))
                            {
                                validation.Formula1 = "1";
                                validation.Formula2 = int.MaxValue.ToString();
                                validation.AllowType = CellDataType.Integer;
                                validation.InputTitle = "Input Data";
                                validation.InputMessage = "Enter an integer from 1 ";
                                validation.ErrorTitle = "Error";
                                validation.ErrorMessage = "The value should be an greater than or equal to 1 ";
                            }
                            else if ((!string.IsNullOrEmpty(minRange.ToString()) || minRange == 0) && (string.IsNullOrEmpty(maxRange.ToString()) || maxRange == 0))
                            {
                                // Minimum value provided, no maximum value
                                validation.Formula1 = minRange.ToString();
                                validation.Formula2 = int.MaxValue.ToString();
                                validation.AllowType = CellDataType.Integer;
                                validation.InputTitle = "Input Data";
                                validation.InputMessage = $"Enter a value with a minimum value of {validation.Formula1}.";
                                validation.ErrorTitle = "Error";
                                validation.ErrorMessage = $"The value must be at least {validation.Formula1}.";
                            }
                            else if ((string.IsNullOrEmpty(minRange.ToString()) || minRange == 0) && (!string.IsNullOrEmpty(maxRange.ToString()) || maxRange == 0))
                            {
                                validation.Formula1 = "1";
                                validation.Formula2 = maxRange.ToString();
                                validation.AllowType = CellDataType.Integer;
                                validation.InputTitle = "Input Data";
                                validation.InputMessage = $"Enter an integer value between 1 to {validation.Formula2}.";
                                validation.ErrorTitle = "Error";
                                validation.ErrorMessage = "The entered value exceeds the allowed range.";
                            }
                            else
                            {
                                // Both minimum and maximum values provided
                                validation.Formula1 = minRange.ToString();
                                validation.Formula2 = maxRange.ToString();
                                validation.AllowType = CellDataType.Integer;
                                validation.InputTitle = "Input Data";
                                validation.InputMessage = $"Enter an integer between {validation.Formula1} and {validation.Formula2}.";
                                validation.ErrorTitle = "Error";
                                validation.ErrorMessage = "The value should be within the specified range.";
                            }
                        }
                        else if ((minRange == 0) && (maxRange == 0))
                        {
                            // Handle the case when both minimum and maximum length are 0
                            validation.CompareOperator = ValidationComparisonOperator.Between;
                            validation.Formula1 = int.MinValue.ToString();
                            validation.Formula2 = int.MaxValue.ToString();
                            validation.AllowType = CellDataType.Integer;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = "Enter an integer.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "The value should be an integer ";
                        }
                        else if ((!string.IsNullOrEmpty(minRange.ToString()) || minRange == 0) && (string.IsNullOrEmpty(maxRange.ToString()) || maxRange == 0))
                        {
                            // Minimum value provided, no maximum value
                            validation.Formula1 = minRange.ToString();
                            validation.Formula2 = int.MaxValue.ToString();
                            validation.AllowType = CellDataType.Integer;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Enter a value with a minimum value of {validation.Formula1}.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = $"The value must be at least {validation.Formula1}.";
                        }
                        else if ((string.IsNullOrEmpty(minRange.ToString()) || minRange == 0) && (!string.IsNullOrEmpty(maxRange.ToString()) || maxRange == 0))
                        {
                            validation.Formula1 = int.MinValue.ToString();
                            validation.Formula2 = maxRange.ToString();
                            validation.AllowType = CellDataType.Integer;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Enter an integer value less than or equal to {validation.Formula2}.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "The entered value exceeds the allowed range.";
                        }
                        else
                        {
                            // Both minimum and maximum values provided
                            validation.Formula1 = minRange.ToString();
                            validation.Formula2 = maxRange.ToString();
                            validation.AllowType = CellDataType.Integer;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Enter an integer between {validation.Formula1} and {validation.Formula2}.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "The value should be within the specified range.";
                        }

                    }
                    else if (dataType.Equals("Date", StringComparison.OrdinalIgnoreCase) || timestampTypes.Contains(dataType, StringComparer.OrdinalIgnoreCase))
                    {
                        // Date validation
                        validation.CompareOperator = ValidationComparisonOperator.Between;

                        if (string.IsNullOrEmpty(dateMinValue) && string.IsNullOrEmpty(dateMaxValue))
                        {
                            // No minimum and maximum date values provided
                            validation.Formula1 = "1757-01-01";
                            validation.Formula2 = "9999-01-01";
                        }
                        else if (!string.IsNullOrEmpty(dateMinValue) && string.IsNullOrEmpty(dateMaxValue))
                        {
                            // Minimum date value provided, no maximum date value
                            validation.Formula1 = dateMinValue;
                            validation.Formula2 = "9999-01-01";
                        }
                        else if (string.IsNullOrEmpty(dateMinValue) && !string.IsNullOrEmpty(dateMaxValue))
                        {
                            // No minimum date value, maximum date value provided
                            validation.Formula1 = "1757-01-01";
                            validation.Formula2 = dateMaxValue;
                        }
                        else
                        {
                            // Both minimum and maximum date values provided
                            validation.Formula1 = dateMinValue;
                            validation.Formula2 = dateMaxValue;
                        }

                        validation.AllowType = CellDataType.Date;
                        validation.InputTitle = "Input Data";
                        validation.InputMessage = $"Type a date between {validation.Formula1} and {validation.Formula2} in this cell.";
                        validation.ErrorTitle = "Error";
                        validation.ErrorMessage = "Enter a valid date with correct format (yyyy-MM-dd).";

                        // Ensure the date format is not avoided
                        var cellRange = range.Worksheet.Range[range.Row, range.Column];
                        cellRange.NumberFormat = "yyyy-MM-dd";
                    }
                    else if (dataType.Equals("boolean", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(truevalue) && string.IsNullOrEmpty(falsevalue))
                        {
                            // No specific values provided, allow "true" and "false"
                            validation.Values = new string[] { "true", "false" };
                            validation.ErrorTitle = "Error";
                            validation.InputTitle = "Input Data";
                            validation.ErrorMessage = "Select values from dropdown";
                            validation.InputMessage = "Select values from dropdown";
                        }
                        else
                        {
                            // Specific values provided, enforce dropdown validation
                            validation.Values = new string[] { truevalue, falsevalue };
                            validation.ErrorTitle = "Error";
                            validation.InputTitle = "Input Data";
                            validation.ErrorMessage = "Select values from dropdown";
                            validation.InputMessage = "Select values from dropdown";
                        }
                    }
                    else if (dataType.Equals("char", StringComparison.OrdinalIgnoreCase))
                    {
                        // Character validation for a single character
                        validation.CompareOperator = ValidationComparisonOperator.Between;
                        validation.Formula1 = "1";
                        validation.Formula2 = "1";
                        validation.AllowType = CellDataType.TextLength;
                        validation.InputTitle = "Input Data";
                        validation.InputMessage = "Type a single character.";
                        validation.ErrorTitle = "Error";
                        validation.ErrorMessage = "Enter a valid character.";
                    }
                    else if (dataType.Equals("bytea", StringComparison.OrdinalIgnoreCase))
                    {
                        // Byte validation
                        // Modify the validation code for bytea data
                        validation.CompareOperator = ValidationComparisonOperator.Between;
                        validation.Formula1 = "1"; // Set a minimum length of 1
                        validation.Formula2 = "1000000"; // Set a maximum length as needed
                        validation.AllowType = CellDataType.TextLength;
                        validation.InputTitle = "Input Data";
                        validation.InputMessage = "Type a byte array with a length between 1 and 1000000 characters.";
                        validation.ErrorTitle = "Error";
                        validation.ErrorMessage = "Invalid byte array length";
                        // Include byte validation
                        bool isValidByteA = IsValidByteA(columns[col - 1].DefaultValue, 1, 1000000); // Modify the length limits as needed
                        if (!isValidByteA)
                        {
                            // Data does not meet byte validation criteria
                            validation.ErrorMessage = "Invalid byte array format or length.";
                        }
                    }
                }
                for (int i = 3; i <= 65537; i++)
                {
                    string startindex = letter + i.ToString();
                    string endindex = lastletter + i.ToString();
                    CellRange lockrange = columnNamesWorksheet.Range[startindex + ":" + endindex];
                    lockrange.Style.Locked = false;
                }
            }
            else
            {
                int columnCount = columnNamesWorksheet.Columns.Length;
                char letter = 'A';
                char lastletter = 'A';
                // Protect the worksheet with a password
                columnNamesWorksheet.Protect("password");
                for (int i = 2; i <= columnCount; i++)
                {
                    lastletter++;
                }

                for (int col = 1; col <= columnCount; col++)
                {
                    // Get the data type for the current column
                    string dataType = columns[col - 1].Datatype;
                    int length = (int)columns[col - 1].Length;
                    bool isPrimaryKey = columns[col - 1].IsPrimaryKey;
                    string truevalue = columns[col - 1].True;
                    string falsevalue = columns[col - 1].False;
                    bool isNullable = columns[col - 1].IsNullable;
                    int? nullableMinRange = columns[col - 1].MinRange;
                    int? nullableMaxRange = columns[col - 1].MaxRange;
                    int minRange = nullableMinRange.HasValue ? nullableMinRange.Value : 0; // Use 0 as a default value when MinLength is null
                    int maxRange = nullableMaxRange.HasValue ? nullableMaxRange.Value : 0; // Use 0 as a default value when MaxLength is null
                    int? nullableMinLength = columns[col - 1].MinLength;
                    int? nullableMaxLength = columns[col - 1].MaxLength;
                    int minLength = nullableMinLength.HasValue ? nullableMinLength.Value : 0; // Use 0 as a default value when MinLength is null
                    int maxLength = nullableMaxLength.HasValue ? nullableMaxLength.Value : 0; // Use 0 as a default value when MaxLength is null
                    string dateMinValue = columns[col - 1].DateMinValue;
                    string dateMaxValue = columns[col - 1].DateMaxValue;
                    // Specify the range for data validation
                    CellRange range = columnNamesWorksheet.Range[startRow, col, endRow, col];
                    Validation validation = range.DataValidation;
                    //Protect the worksheet with password
                    columnNamesWorksheet.Protect("123456", SheetProtectionType.All);
                    // Check if the current column has a data type of "ListOfValues"
                    List<string> integerTypes = new List<string> { "int", "integer", "number", "numeric", /* add more as needed */ };
                    List<string> stringTypes = new List<string> { "string", "varchar", "nvarchar", "text", "character varying"/* add more as needed */ };
                    List<string> timestampTypes = new List<string> { "timestamp", "datetime", "timestamp without time zone", "date" /* add more as needed */ };

                    if (stringTypes.Contains(dataType, StringComparer.OrdinalIgnoreCase))
                    {
                        // Text validation with min and max length
                        validation.CompareOperator = ValidationComparisonOperator.Between;
                        if ((minLength == 0) && (maxLength == 0))
                        {
                            // Handle the case when both minimum and maximum length are 0
                            validation.Formula1 = "0";
                            validation.Formula2 = "0";
                        }
                        else if ((!string.IsNullOrEmpty(minLength.ToString()) || minLength == 0) && (string.IsNullOrEmpty(maxLength.ToString()) || maxLength == 0))
                        {
                            // Minimum length provided, no maximum length
                            validation.Formula1 = minLength.ToString();
                            validation.AllowType = CellDataType.TextLength;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Enter a value with a minimum length of {validation.Formula1} characters.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = $"The value must have a minimum length of {validation.Formula1} characters.";
                        }
                        else if ((string.IsNullOrEmpty(minLength.ToString()) || minLength == 0) && (!string.IsNullOrEmpty(maxLength.ToString()) || maxLength == 0))
                        {
                            validation.Formula2 = maxLength.ToString();
                            validation.AllowType = CellDataType.TextLength;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Type text with a maximum length of {validation.Formula2} characters.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "The entered value exceeds the allowed length.";
                        }
                        else
                        {
                            // Both minimum and maximum length provided
                            validation.Formula1 = minLength.ToString();
                            validation.Formula2 = maxLength.ToString();
                            validation.AllowType = CellDataType.TextLength;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Type text with a length between {validation.Formula1} and {validation.Formula2} characters.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "Entered value should be within the specified length range.";
                        }
                        if (isPrimaryKey)
                        {
                            HighlightDuplicates(columnNamesWorksheet, col, startRow, endRow);
                            validation.InputMessage = $"Enter the unique value.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "Entered Values must be unique";
                        }
                    }
                    else if (integerTypes.Contains(dataType, StringComparer.OrdinalIgnoreCase))
                    {
                        validation.CompareOperator = ValidationComparisonOperator.Between;
                        if (isPrimaryKey)
                        {
                            HighlightDuplicates(columnNamesWorksheet, col, startRow, endRow);

                            validation.CompareOperator = ValidationComparisonOperator.Between;
                            if ((minRange == 0) && (maxRange == 0))
                            {
                                validation.Formula1 = "1";
                                validation.Formula2 = int.MaxValue.ToString();
                                validation.AllowType = CellDataType.Integer;
                                validation.InputTitle = "Input Data";
                                validation.InputMessage = "Enter an integer from 1 ";
                                validation.ErrorTitle = "Error";
                                validation.ErrorMessage = "The value should be an greater than or equal to 1 ";
                            }
                            else if ((!string.IsNullOrEmpty(minRange.ToString()) || minRange == 0) && (string.IsNullOrEmpty(maxRange.ToString()) || maxRange == 0))
                            {
                                // Minimum value provided, no maximum value
                                validation.Formula1 = minRange.ToString();
                                validation.Formula2 = int.MaxValue.ToString();
                                validation.AllowType = CellDataType.Integer;
                                validation.InputTitle = "Input Data";
                                validation.InputMessage = $"Enter a value with a minimum value of {validation.Formula1}.";
                                validation.ErrorTitle = "Error";
                                validation.ErrorMessage = $"The value must be at least {validation.Formula1}.";
                            }
                            else if ((string.IsNullOrEmpty(minRange.ToString()) || minRange == 0) && (!string.IsNullOrEmpty(maxRange.ToString()) || maxRange == 0))
                            {
                                validation.Formula1 = "1";
                                validation.Formula2 = maxRange.ToString();
                                validation.AllowType = CellDataType.Integer;
                                validation.InputTitle = "Input Data";
                                validation.InputMessage = $"Enter an integer value between 1 to {validation.Formula2}.";
                                validation.ErrorTitle = "Error";
                                validation.ErrorMessage = "The entered value exceeds the allowed range.";
                            }
                            else
                            {
                                // Both minimum and maximum values provided
                                validation.Formula1 = minRange.ToString();
                                validation.Formula2 = maxRange.ToString();
                                validation.AllowType = CellDataType.Integer;
                                validation.InputTitle = "Input Data";
                                validation.InputMessage = $"Enter an integer between {validation.Formula1} and {validation.Formula2}.";
                                validation.ErrorTitle = "Error";
                                validation.ErrorMessage = "The value should be within the specified range.";
                            }
                        }
                        else if ((minRange == 0) && (maxRange == 0))
                        {
                            // Handle the case when both minimum and maximum length are 0
                            validation.CompareOperator = ValidationComparisonOperator.Between;
                            validation.Formula1 = int.MinValue.ToString();
                            validation.Formula2 = int.MaxValue.ToString();
                            validation.AllowType = CellDataType.Integer;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = "Enter an integer.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "The value should be an integer ";
                        }
                        else if ((!string.IsNullOrEmpty(minRange.ToString()) || minRange == 0) && (string.IsNullOrEmpty(maxRange.ToString()) || maxRange == 0))
                        {
                            // Minimum value provided, no maximum value
                            validation.Formula1 = minRange.ToString();
                            validation.Formula2 = int.MaxValue.ToString();
                            validation.AllowType = CellDataType.Integer;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Enter a value with a minimum value of {validation.Formula1}.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = $"The value must be at least {validation.Formula1}.";
                        }
                        else if ((string.IsNullOrEmpty(minRange.ToString()) || minRange == 0) && (!string.IsNullOrEmpty(maxRange.ToString()) || maxRange == 0))
                        {
                            validation.Formula1 = int.MinValue.ToString();
                            validation.Formula2 = maxRange.ToString();
                            validation.AllowType = CellDataType.Integer;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Enter an integer value less than or equal to {validation.Formula2}.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "The entered value exceeds the allowed range.";
                        }
                        else
                        {
                            // Both minimum and maximum values provided
                            validation.Formula1 = minRange.ToString();
                            validation.Formula2 = maxRange.ToString();
                            validation.AllowType = CellDataType.Integer;
                            validation.InputTitle = "Input Data";
                            validation.InputMessage = $"Enter an integer between {validation.Formula1} and {validation.Formula2}.";
                            validation.ErrorTitle = "Error";
                            validation.ErrorMessage = "The value should be within the specified range.";
                        }

                    }
                    else if (dataType.Equals("Date", StringComparison.OrdinalIgnoreCase) || timestampTypes.Contains(dataType, StringComparer.OrdinalIgnoreCase))
                    {
                        // Date validation
                        validation.CompareOperator = ValidationComparisonOperator.Between;

                        if (string.IsNullOrEmpty(dateMinValue) && string.IsNullOrEmpty(dateMaxValue))
                        {
                            // No minimum and maximum date values provided
                            validation.Formula1 = "1757-01-01";
                            validation.Formula2 = "9999-01-01";
                        }
                        else if (!string.IsNullOrEmpty(dateMinValue) && string.IsNullOrEmpty(dateMaxValue))
                        {
                            // Minimum date value provided, no maximum date value
                            validation.Formula1 = dateMinValue;
                            validation.Formula2 = "9999-01-01";
                        }
                        else if (string.IsNullOrEmpty(dateMinValue) && !string.IsNullOrEmpty(dateMaxValue))
                        {
                            // No minimum date value, maximum date value provided
                            validation.Formula1 = "1757-01-01";
                            validation.Formula2 = dateMaxValue;
                        }
                        else
                        {
                            // Both minimum and maximum date values provided
                            validation.Formula1 = dateMinValue;
                            validation.Formula2 = dateMaxValue;
                        }

                        validation.AllowType = CellDataType.Date;
                        validation.InputTitle = "Input Data";
                        validation.InputMessage = $"Type a date between {validation.Formula1} and {validation.Formula2} in this cell.";
                        validation.ErrorTitle = "Error";
                        validation.ErrorMessage = "Enter a valid date with correct format (yyyy-MM-dd).";

                        // Ensure the date format is not avoided
                        var cellRange = range.Worksheet.Range[range.Row, range.Column];
                        cellRange.NumberFormat = "yyyy-MM-dd";
                    }
                    else if (dataType.Equals("boolean", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(truevalue) && string.IsNullOrEmpty(falsevalue))
                        {
                            // No specific values provided, allow "true" and "false"
                            validation.Values = new string[] { "true", "false" };
                            validation.ErrorTitle = "Error";
                            validation.InputTitle = "Input Data";
                            validation.ErrorMessage = "Select values from dropdown";
                            validation.InputMessage = "Select values from dropdown";
                        }
                        else
                        {
                            // Specific values provided, enforce dropdown validation
                            validation.Values = new string[] { truevalue, falsevalue };
                            validation.ErrorTitle = "Error";
                            validation.InputTitle = "Input Data";
                            validation.ErrorMessage = "Select values from dropdown";
                            validation.InputMessage = "Select values from dropdown";
                        }
                    }
                    else if (dataType.Equals("char", StringComparison.OrdinalIgnoreCase))
                    {
                        // Character validation for a single character
                        validation.CompareOperator = ValidationComparisonOperator.Between;
                        validation.Formula1 = "1";
                        validation.Formula2 = "1";
                        validation.AllowType = CellDataType.TextLength;
                        validation.InputTitle = "Input Data";
                        validation.InputMessage = "Type a single character.";
                        validation.ErrorTitle = "Error";
                        validation.ErrorMessage = "Enter a valid character.";
                    }
                    else if (dataType.Equals("bytea", StringComparison.OrdinalIgnoreCase))
                    {
                        // Byte validation
                        // Modify the validation code for bytea data
                        validation.CompareOperator = ValidationComparisonOperator.Between;
                        validation.Formula1 = "1"; // Set a minimum length of 1
                        validation.Formula2 = "1000000"; // Set a maximum length as needed
                        validation.AllowType = CellDataType.TextLength;
                        validation.InputTitle = "Input Data";
                        validation.InputMessage = "Type a byte array with a length between 1 and 1000000 characters.";
                        validation.ErrorTitle = "Error";
                        validation.ErrorMessage = "Invalid byte array length";
                        // Include byte validation
                        bool isValidByteA = IsValidByteA(columns[col - 1].DefaultValue, 1, 1000000); // Modify the length limits as needed
                        if (!isValidByteA)
                        {
                            // Data does not meet byte validation criteria
                            validation.ErrorMessage = "Invalid byte array format or length.";
                        }
                    }

                }
                for (int i = 3; i <= 65537; i++)
                {
                    string startindex = letter + i.ToString();
                    string endindex = lastletter + i.ToString();
                    CellRange lockrange = columnNamesWorksheet.Range[startindex + ":" + endindex];
                    lockrange.Style.Locked = false;
                }
            }
        }

        //Get Column names
        public string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
        }

        // Export Error Datas
        private async Task InsertDataIntoExcel(Worksheet columnNamesWorksheet, List<ColumnMetaDataDTO> columns, int? parentId)
        {
            try
            {
                var logChilds = await GetAllLogChildsByParentIDAsync(parentId.Value);
                int rowIndex = 3;
                foreach (var logChild in logChilds)
                {
                    string[] rows = logChild.Filedata.Split(';');

                    string errorMessage = logChild.ErrorMessage;
                    string errorrowNumber = logChild.ErrorRowNumber;

                    // Set the column name as "ErrorMessage" for the last column after processing all rows
                    columnNamesWorksheet.Range[rowIndex - 1, columns.Count + 1].Text = "ErrorMessage";
                    columnNamesWorksheet.Range[rowIndex - 1, columns.Count + 2].Text = "ErrorRowNumber";
                    // Split ErrorRowNumber into individual values
                    string[] errorRowNumbers = errorrowNumber.Split(',');

                    for (int i = 1; i < rows.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(rows[i]))
                        {
                            continue;
                        }
                        string cleanedRow = rows[i].TrimStart(';').Trim();
                        string[] values = cleanedRow.Split(',');

                        // Display each ErrorRowNumber on a separate row
                        columnNamesWorksheet.Range[rowIndex, 1].Text = values[0];  // Assuming id is the first column
                        columnNamesWorksheet.Range[rowIndex, 2].Text = values[1];  // Assuming username is the second column
                        columnNamesWorksheet.Range[rowIndex, columns.Count + 1].Text = errorMessage;
                        columnNamesWorksheet.Range[rowIndex, columns.Count + 2].Text = errorRowNumbers[i - 1]; // Use (i - 1) to get the corresponding ErrorRowNumber
                        rowIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Get Tbale name by Entity ID
        public int GetEntityIdByEntityName(string entityName)
        {
            // Assuming you have a list of EntityListMetadataModel instances
            var entityListMetadataModels = GetEntityListMetadataModels(); // Implement this method to fetch your metadata models

            // Use LINQ to find the entity Id
            int entityId = entityListMetadataModels
                .Where(model => model.EntityName == entityName)
                .Select(model => model.Id)
                .FirstOrDefault();

            if (entityId != 0) // Check if a valid entity Id was found
            {
                return entityId;
            }
            else
            {
                // Handle the case where the entity name is not found
                throw new Exception("Entity not found");
            }
        }

        // Get Entity names from meta table
        private List<TableMetaDataEntity> GetEntityListMetadataModels()
        {
            var sample = _context.TableMetaDataEntity.ToList();

            return sample;
        }

        // Read excel file
        public DataTable ReadExcelFromFormFile(IFormFile excelFile)
        {
            using (Stream stream = excelFile.OpenReadStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    DataTable dataTable = new DataTable();

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {

                        var firstCell = worksheet.Cells[2, col];

                        if (string.IsNullOrWhiteSpace(firstCell.Text))
                        {
                            continue;
                        }
                        dataTable.Columns.Add(firstCell.Text);
                    }


                    dataTable.Columns.Add("RowNumber", typeof(int)); // Add "RowNumber" column


                    for (int rowNumber = 3; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                    {
                        var dataRow = dataTable.NewRow();

                        // Set the "RowNumber" value for each row

                        int colIndex = 0;

                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {

                            // Check if this column should be included

                            if (dataTable.Columns.Contains(worksheet.Cells[2, col].Text))
                            {

                                dataRow[colIndex] = worksheet.Cells[rowNumber, col].Text;

                                colIndex++;

                            }
                        }

                        dataTable.Rows.Add(dataRow);
                    }

                    bool allRowsAreNull = dataTable.AsEnumerable()

                    .All(row => row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field.ToString())));

                    if (allRowsAreNull)
                    {
                        return null;
                    }

                    dataTable = dataTable.AsEnumerable()

                        .Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field.ToString())))

                        .CopyToDataTable();

                    dataTable = dataTable.AsEnumerable().Select((row, index) =>
                    {
                        row.SetField("RowNumber", index + 3);
                        return row;
                    }).CopyToDataTable();
                    return dataTable;
                }
            }
        }

        // Read excel file
        public List<Dictionary<string, string>> ReadDataFromExcel(Stream excelFileStream, int rowCount)
        {
            using (var package = new ExcelPackage(excelFileStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                rowCount = rowCount + 2;

                int colCount = worksheet.Dimension.Columns;

                var data = new List<Dictionary<string, string>>();

                var columnNames = new List<string>();

                var skipColumns = new List<bool>();

                for (int col = 1; col <= colCount; col++)
                {
                    var columnName = worksheet.Cells[2, col].Value?.ToString();

                    columnNames.Add(columnName);

                    skipColumns.Add(string.IsNullOrWhiteSpace(columnName));
                }

                // Read data rows

                for (int row = 3; row <= rowCount; row++)
                {
                    var rowData = new Dictionary<string, string>();

                    for (int col = 1; col <= colCount; col++)
                    {
                        if (!skipColumns[col - 1])
                        {

                            var columnName = columnNames[col - 1];

                            var cellValue = worksheet.Cells[row, col].Value?.ToString();

                            rowData[columnName] = cellValue;
                        }
                    }

                    // Include the row number as "RowNumber" in the dictionary

                    rowData["RowNumber"] = row.ToString();

                    data.Add(rowData);
                }
                return data;
            }
        }

        public bool IsValidByteA(string data, int minLength, int maxLength)
        {
            // Check if the input is a valid hexadecimal string
            if (!IsHexString(data))
            {
                return false;
            }

            // Check if the length is within acceptable limits
            if (data.Length < minLength || data.Length > maxLength)
            {
                return false;
            }
            // Add more specific checks if needed
            return true;
        }

        public bool IsHexString(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public IEnumerable<ColumnMetaDataDTO> GetColumnsForEntity(string entityName)
        {
            var entity = _context.TableMetaDataEntity.FirstOrDefault(e => e.EntityName == entityName);
            if (entity == null)
            {
                // Entity not found, return a 404 Not Found response
                return null;
            }
            var columnsDTO = _context.ColumnMetaDataEntity
                .Where(column => column.EntityId == entity.Id)
                .Select(column => new ColumnMetaDataDTO
                {
                    Id = column.Id,
                    ColumnName = column.ColumnName,
                    Datatype = column.Datatype,
                    Length = column.Length,
                    Description = column.Description,
                    IsNullable = column.IsNullable,
                    DefaultValue = column.DefaultValue,
                    IsPrimaryKey = column.IsPrimaryKey,
                    True = column.True,
                    False = column.False,
                    MinLength = column.MinLength,
                    MaxLength = column.MaxLength,
                    MinRange = column.MinRange,
                    MaxRange = column.MaxRange,
                    DateMinValue = column.DateMinValue,
                    DateMaxValue = column.DateMaxValue,
                    IsForeignKey = column.IsForeignKey,
                    ReferenceColumnID = column.ReferenceColumnID,
                    ReferenceEntityID = column.ReferenceEntityID,
                }).ToList();
            if (columnsDTO.Count == 0)
            {
                // No columns found, return a 404 Not Found response with an error message
                return null;
            }
            return columnsDTO;
        }

        //not Nul Validation
        public async Task<ValidationResultData> ValidateNotNull(DataTable excelData, List<ColumnMetaDataDTO> columnsDTO)
        {
            List<string> badRows = new List<string>();
            List<string> errorColumnNames = new List<string>();
            DataTable validRowsDataTable = excelData.Clone(); // Create a DataTable to store valid rows

            for (int row = 0; row < excelData.Rows.Count; row++)
            {
                bool rowValidationFailed = false;

                string badRow = string.Join(",", excelData.Rows[row].ItemArray);

                if (excelData.Columns.Contains("ErrorMessage"))
                {
                    for (int col = 0; col < excelData.Columns.Count - 3; col++)
                    {
                        string cellData = excelData.Rows[row][col].ToString();
                        ColumnMetaDataDTO columnDTO = columnsDTO[col];

                        if (columnDTO.IsNullable == true && string.IsNullOrEmpty(cellData))
                        {
                            rowValidationFailed = true;
                            badRows.Add(badRow);
                            if (!errorColumnNames.Contains(columnDTO.ColumnName))
                            {
                                errorColumnNames.Add(columnDTO.ColumnName);
                            }

                            break;
                        }
                    }
                }
                else
                {
                    for (int col = 0; col < excelData.Columns.Count - 2; col++)
                    {
                        string cellData = excelData.Rows[row][col].ToString();
                        ColumnMetaDataDTO columnDTO = columnsDTO[col];

                        if (columnDTO.IsNullable == true && string.IsNullOrEmpty(cellData))
                        {
                            rowValidationFailed = true;
                            badRows.Add(badRow);
                            if (!errorColumnNames.Contains(columnDTO.ColumnName))
                            {
                                errorColumnNames.Add(columnDTO.ColumnName);
                            }

                            break;
                        }
                    }

                }

                if (!rowValidationFailed)
                {
                    validRowsDataTable.Rows.Add(excelData.Rows[row].ItemArray);
                }
            }

            // Return both results
            return new ValidationResultData { BadRows = badRows, SuccessData = validRowsDataTable, errorcolumns = errorColumnNames, Column_Name = string.Empty };
        }

        //Primary Key validation
        public async Task<ValidationResultData> ValidatePrimaryKeyAsync(ValidationResultData validationResult, List<ColumnMetaDataDTO> columnsDTO, string tableName,DBConnectionDTO connectionDTO)
        {
            List<string> badRows = new List<string>();
            string columnName = string.Empty;
            DataTable validRowsDataTable = validationResult.SuccessData;
            DataTable successdata = validRowsDataTable.Clone();
            List<int> primaryKeyColumns = new List<int>();
            if (validRowsDataTable.Columns.Contains("ErrorMessage"))
            {
                for (int col = 0; col < validRowsDataTable.Columns.Count - 3; col++)
                {
                    ColumnMetaDataDTO columnDTO = columnsDTO[col];
                    if (columnDTO.IsPrimaryKey)
                    {
                        primaryKeyColumns.Add(col);
                        columnName = columnDTO.ColumnName; // Set the primary key column name
                    }
                }
            }
            else
            {
                for (int col = 0; col < validRowsDataTable.Columns.Count - 2; col++)
                {
                    ColumnMetaDataDTO columnDTO = columnsDTO[col];
                    if (columnDTO.IsPrimaryKey)
                    {
                        primaryKeyColumns.Add(col);
                        columnName = columnDTO.ColumnName; // Set the primary key column name
                    }
                }
            }


            HashSet<string> seenValues = new HashSet<string>();
            var ids = await GetAllIdsFromDynamicTable(connectionDTO,tableName);

            for (int row = 0; row < validRowsDataTable.Rows.Count; row++)
            {
                bool rowValidationFailed = false;
                //  var badRowData = new List<string>();

                for (int col = 0; col < primaryKeyColumns.Count; col++)
                {
                    int primaryKeyColumnIndex = primaryKeyColumns[col];
                    string cellData = validRowsDataTable.Rows[row][primaryKeyColumnIndex].ToString();

                    if (seenValues.Contains(cellData))
                    {
                        // Set the flag to indicate validation failure for this row
                        rowValidationFailed = true;
                        columnName = columnsDTO[primaryKeyColumnIndex].ColumnName;
                        badRows.Add(string.Join(",", validRowsDataTable.Rows[row].ItemArray)); // Store the row data
                        break; // Exit the loop as soon as a validation failure is encountered
                    }

                    if (ids.Contains(cellData))
                    {
                        rowValidationFailed = true;
                        columnName = columnsDTO[primaryKeyColumnIndex].ColumnName;
                        badRows.Add(string.Join(",", validRowsDataTable.Rows[row].ItemArray)); // Store the row data
                        break;
                    }

                    // Store the value for duplicate checking
                    seenValues.Add(cellData);
                }

                // If row validation succeeded, add the entire row to the successdata DataTable
                if (!rowValidationFailed)
                {
                    successdata.Rows.Add(validRowsDataTable.Rows[row].ItemArray);
                }
            }

            // Return both bad rows and success data using the custom class
            return new ValidationResultData { BadRows = badRows, SuccessData = successdata, Column_Name = columnName };
        }

        //Convert result type
        public async Task<ValidationResult> resultparams(ValidationResultData validationResult, string comma_separated_string)
        {
            string errorMessages = string.Empty;
            string values = string.Join(",", validationResult.BadRows.Select(row => row.Split(',').Last()));
            validationResult.BadRows.Insert(0, comma_separated_string);
            List<string> modifiedRows = validationResult.BadRows.Select(row => row.Substring(0, row.LastIndexOf(','))).ToList();
            validationResult.BadRows = modifiedRows;
            string delimiter = ";"; // Specify the delimiter you want
            string delimiter1 = ","; // Specify the delimiter you want   //chng
            string baddatas = string.Join(delimiter, validationResult.BadRows);
            string badcolumns = string.Join(delimiter1, validationResult.errorcolumns);
            errorMessages = "Null value found in column" + " " + badcolumns;
            // Return both results
            return new ValidationResult { ErrorRowNumber = values, Filedatas = baddatas, errorMessages = errorMessages };
        }

        //Convert primary key validation result to result type
        public async Task<ValidationResult> resultparamsforprimary(ValidationResultData validationResult, string comma_separated_string, string tableName)
        {

            var badRowsPrimaryKey = validationResult.BadRows;


            string columnName = validationResult.Column_Name;

            badRowsPrimaryKey = badRowsPrimaryKey.Where(x => x != "").ToList();
            string values = string.Join(",", badRowsPrimaryKey.Select(row => row.Split(',').Last()));

            badRowsPrimaryKey.Insert(0, comma_separated_string);

            List<string> modifiedRows = badRowsPrimaryKey.Select(row =>
            {
                int lastCommaIndex = row.LastIndexOf(',');
                if (lastCommaIndex >= 0)
                {
                    return row.Substring(0, lastCommaIndex);
                }
                else
                {
                    return row; // No comma found, keep the original string
                }
            }).Where(row => !string.IsNullOrEmpty(row)).ToList();
            badRowsPrimaryKey = modifiedRows;
            string delimiter = ";"; // Specify the delimiter you want
            string baddatas = string.Join(delimiter, badRowsPrimaryKey);
            string errorMessages = "Duplicate key value violates unique constraints in column " + columnName + " " + "in" + " " + tableName;

            // Return both results
            return new ValidationResult { ErrorRowNumber = values, Filedatas = baddatas, errorMessages = errorMessages };
        }

        //Convert range validation result to result type
        public async Task<ValidationResult> resultparamsforrange(ValidationResultData validationResult, string comma_separated_string, string tableName)
        {
            var badRowsPrimaryKey = validationResult.BadRows;

            string columnName = validationResult.Column_Name;

            badRowsPrimaryKey = badRowsPrimaryKey.Where(x => x != "").ToList();
            string values = string.Join(",", badRowsPrimaryKey.Select(row => row.Split(',').Last()));

            badRowsPrimaryKey.Insert(0, comma_separated_string);

            List<string> modifiedRows = badRowsPrimaryKey.Select(row =>
            {
                int lastCommaIndex = row.LastIndexOf(',');
                if (lastCommaIndex >= 0)
                {
                    return row.Substring(0, lastCommaIndex);
                }
                else
                {
                    return row; // No comma found, keep the original string
                }
            }).Where(row => !string.IsNullOrEmpty(row)).ToList();
            badRowsPrimaryKey = modifiedRows;
            string delimiter = ";"; // Specify the delimiter you want
            string baddatas = string.Join(delimiter, badRowsPrimaryKey);
            string errorMessages = "Incorrect Range Value on " + columnName + " " + "in" + " " + tableName;

            // Return both results
            return new ValidationResult { ErrorRowNumber = values, Filedatas = baddatas, errorMessages = errorMessages };
        }

        // Insert Log
        public async Task<LogDTO> Createlog(string tableName, List<string> filedata, string fileName, int successdata, List<string> errorMessage, int total_count, List<string> ErrorRowNumber)
        {
            var storeentity = await _context.TableMetaDataEntity.FirstOrDefaultAsync(x => x.EntityName.ToLower() == tableName.ToLower());

            LogParent logParent = new LogParent();

            logParent.FileName = fileName;

            logParent.User_Id = 1;

            logParent.Entity_Id = storeentity.Id;

            logParent.Timestamp = DateTime.UtcNow;

            logParent.PassCount = successdata;

            logParent.RecordCount = total_count;

            logParent.FailCount = total_count - successdata;

            // Insert the LogParent record

            _context.LogParents.Add(logParent);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            List<LogChild> logChildren = new List<LogChild>();

            for (int i = 0; i < errorMessage.Count; i++)
            {
                LogChild logChild = new LogChild();

                logChild.ParentID = logParent.ID; // Set the ParentId

                logChild.ErrorMessage = errorMessage[i];

                if (filedata.Count > 0)
                {
                    logChild.Filedata = filedata[i];
                }
                else
                {
                    logChild.Filedata = ""; // Set the filedata as needed
                }

                if (ErrorRowNumber.Count > 0)
                {
                    logChild.ErrorRowNumber = ErrorRowNumber[i];
                }
                else
                {
                    logChild.ErrorRowNumber = ""; // Set the filedata as needed
                }
                if (ErrorRowNumber.Count > 0)
                {
                    logChild.ErrorRowNumber = ErrorRowNumber[i];
                }
                else
                {
                    logChild.ErrorRowNumber = ""; // Set the filedata as needed
                }

                // Insert the LogChild record
                _context.LogChilds.Add(logChild);

                logChildren.Add(logChild);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine("Error: " + ex.Message);
            }
            LogDTO logDTO = new LogDTO()
            {
                LogParentDTOs = logParent,
                ChildrenDTOs = logChildren
            };
            return logDTO;
        }

        //Insert uploaded data into Client db
        public async Task InsertDataFromDataTableToPostgreSQL(DataTable data, string tableName, List<string> columns, IFormFile file,DBConnectionDTO connectionDTO)
        {
            try
            {
                var columnProperties = GetColumnsForEntity(tableName).ToList();
                List<ColumnMetaDataDTO> booleancolumns = columnProperties.Where(c => c.Datatype.ToLower() == "boolean").ToList();

                var listofvaluecolumns = columnProperties.Where(c => c.Datatype.ToLower() == "listofvalue").ToList();

                List<Dictionary<string, string>> convertedDataList = new List<Dictionary<string, string>>();

                foreach (DataRow row in data.Rows)
                {
                    Dictionary<string, string> convertedData = new Dictionary<string, string>();
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        string cellValue = row[i].ToString();

                        ColumnMetaDataDTO columnProperty = columnProperties.FirstOrDefault(col => col.ColumnName == data.Columns[i].ColumnName);

                        if (columnProperty != null)
                        {
                            // Use the column name from ColumnProperties as the key and the cell value as the value
                            convertedData[columnProperty.ColumnName] = cellValue;
                        }
                    }
                    convertedDataList.Add(convertedData);
                }

                // 'convertedDataList' is now a list of dictionaries, each representing a row in the desired format.
                var storeEntity = await _context.TableMetaDataEntity.FirstOrDefaultAsync(x => x.EntityName.ToLower() == tableName.ToLower());

                tableName = storeEntity.EntityName;

                var connectionString = _httpContextAccessor.HttpContext.Session.GetString("ConnectionString");

                // Create an HttpClient instance
                using (var httpClient = new HttpClient())
                {
                    // Set the base address for the external API
                    httpClient.BaseAddress = new Uri("https://localhost:7246");

                    var queryParams = $"?connectionDTO={JsonConvert.SerializeObject(connectionDTO)}" +
                          $"&convertedDataList={JsonConvert.SerializeObject(convertedDataList)}" +
                          $"&booleanColumns={JsonConvert.SerializeObject(booleancolumns)}" +
                          $"&tableName={tableName}";

                    // Call the external API
                    var response = await httpClient.PostAsync($"/EntityMigrate/InsertData{queryParams}", null);

                    // Check the response status
                    if (response.IsSuccessStatusCode)
                    {
                        // Continue with the rest of your method logic...
                    }
                    else
                    {
                        // Handle the error, log, or throw a new exception as needed
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Failed to insert data. Error: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log, or rethrow as needed
                throw new Exception($"Error in InsertDataFromDataTableToPostgreSQL: {ex.Message}");
            }
        }
        public int GetEntityIdByEntityNamefromui(string entityName)
        {
            // Assuming you have a list of EntityListMetadataModel instances
            List<TableMetaDataEntity> entityListMetadataModels = GetEntityListMetadataModelforlist(); // Implement this method to fetch your metadata models

            // Use LINQ to find the entity Id
            int entityId = entityListMetadataModels
                .Where(model => model.EntityName == entityName)
                .Select(model => model.Id)
                .FirstOrDefault();

            if (entityId != 0) // Check if a valid entity Id was found
            {
                return entityId;
            }
            else
            {
                // Handle the case where the entity name is not found
                throw new Exception("Entity not found");//return null
            }
        }

        public List<TableMetaDataEntity> GetEntityListMetadataModelforlist()
        {
            {
                List<TableMetaDataEntity> entityListMetadataModels = _context.TableMetaDataEntity.ToList();
                return entityListMetadataModels;
            }
        }

        public int? GetEntityIdFromTemplate(IFormFile file, int sheetIndex)
        {
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetIndex];
                int entityId;
                if (int.TryParse(worksheet.Cells[1, 1].Text, out entityId))
                {
                    return entityId;
                }
                return null;
            }
        }
        public string GetPrimaryKeyColumnForEntity(string entityName)
        {
            var entity = _context.TableMetaDataEntity.FirstOrDefault(e => e.EntityName == entityName);
            if (entity == null)
            {
                // Entity not found, return null or throw an exception
                return null;
            }
            var primaryKeyColumn = _context.ColumnMetaDataEntity
                .Where(column => column.EntityId == entity.Id && column.IsPrimaryKey)
                .Select(column => column.ColumnName)
                .FirstOrDefault();
            return primaryKeyColumn;
        }

        public async Task<List<dynamic>> GetAllIdsFromDynamicTable(DBConnectionDTO connectionDTO, string tableName)
        {
            try
            {
                // Create an HttpClient instance
                using (var httpClient = new HttpClient())
                {
                    // Set the base address for the API
                    httpClient.BaseAddress = new Uri("https://localhost:7246");

                    // Call the GetPrimaryColumnData API endpoint
                    var DBconnectionDTO = connectionDTO;
                    var response = await httpClient.GetAsync($"/EntityMigrate/GetPrimaryColumnData?Provider={connectionDTO.Provider}&HostName={connectionDTO.HostName}&DataBase={connectionDTO.DataBase}&UserName={connectionDTO.UserName}&Password={connectionDTO.Password}&tableName={tableName}");

                    // Check the response status
                    if (response.IsSuccessStatusCode)
                    {
                        var contentAsString = await response.Content.ReadAsStringAsync();
                        // Read the response content and convert it to a list of strings
                        var primaryColumnData = JsonConvert.DeserializeObject<List<dynamic>>(contentAsString);
                        var ids = primaryColumnData?.Select(item => item.ToString()).ToList();
                        return ids;
                    }
                    else
                    {
                        // Handle the error, log, or throw a new exception as needed
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Error fetching IDs from the specified table. Details: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching IDs from the specified table.", ex);
            }
        }

        public bool TableExists(string tableName)
        {
            try
            {
                //Implement IsTableExists APi
                return true;
            }
            catch (Exception ex)
            {
                // Log or print detailed information about the exception
                Console.WriteLine($"Error checking table existence. ConnectionString: TableName: {tableName}, Exception: {ex}");

                // Rethrow the exception
                throw;
            }
        }

        public (int EntityId, string EntityColumnName) GetAllEntityColumnData(int checklistEntityValue)
        {
            List<ColumnMetaDataEntity> allData = _context.ColumnMetaDataEntity
                .AsEnumerable()  // Bring data into memory
                .Where(c => c.Id == checklistEntityValue)
                .ToList();

            if (allData.Count > 0)
            {
                // Return the EntityId and EntityColumnName of the first item in the list
                return (allData[0].EntityId, allData[0].ColumnName);
            }
            else
            {
                // Return default values or handle as needed
                return (0, string.Empty);
            }
        }

        public List<dynamic> GetTableDataByChecklistEntityValue(DBConnectionDTO connectionDTO, int checklistEntityValue)
        {
            // Get the EntityId and EntityColumnName using the provided method
            var (entityId, entityColumnName) = GetAllEntityColumnData(checklistEntityValue);

            if (entityId == 0 || string.IsNullOrEmpty(entityColumnName))
            {
                // Handle the case where the EntityId or EntityColumnName is not found
                throw new InvalidOperationException("No entity metadata found");
            }

            // Use Entity Framework Core to get the table name
            var entityListMetadata = _context.TableMetaDataEntity.FirstOrDefault(entity => entity.Id == entityId);

            if (entityListMetadata == null)
            {
                // Log or print debug information
                throw new InvalidOperationException("No entity metadata found");
            }

            string tableName = entityListMetadata.EntityName;

            try
            {
                // Create an HttpClient instance
                using (var httpClient = new HttpClient())
                {
                    // Set the base address for the API
                    httpClient.BaseAddress = new Uri("https://localhost:7246");

                    // Call the GetTabledata API endpoint
                    var response = httpClient.GetAsync($"/EntityMigrate/GetPrimaryColumnData?Provider={connectionDTO.Provider}&HostName={connectionDTO.HostName}&DataBase={connectionDTO.DataBase}&UserName={connectionDTO.UserName}&Password={connectionDTO.Password}&tableName={tableName}").Result;

                    // Check the response status
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        var contentAsString = response.Content.ReadAsStringAsync().Result;

                        // Deserialize the string content to a list of dynamic objects
                        var tableData = JsonConvert.DeserializeObject<List<dynamic>>(contentAsString);

                        // Continue with the rest of your method logic...
                        return tableData;
                    }
                    else
                    {
                        // Handle the error, log, or throw a new exception as needed
                        var errorMessage = response.Content.ReadAsStringAsync().Result;
                        throw new Exception($"Error fetching table data. Details: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching table data: {ex.Message}");
            }
        }

        // range Validation
        public async Task<ValidationResultData> ValidateRange(ValidationResultData validationResult, List<ColumnMetaDataDTO> columnsDTO, string tableName)
        {
            List<string> badRows = new List<string>();
            List<string> errorColumnNames = new List<string>();
            var excelData = validationResult.SuccessData;
            DataTable validRowsDataTable = excelData.Clone(); // Create a DataTable to store valid rows
            for (int row = 0; row < excelData.Rows.Count; row++)
            {
                bool rowValidationFailed = false;

                string badRow = string.Join(",", excelData.Rows[row].ItemArray);

                if (excelData.Columns.Contains("ErrorMessage"))
                {
                    for (int col = 0; col < excelData.Columns.Count - 3; col++)
                    {
                        string cellData = excelData.Rows[row][col].ToString();
                        ColumnMetaDataDTO columnDTO = columnsDTO[col];

                        if (double.TryParse(cellData, out double numericValue))
                        {
                            if (columnDTO.MinLength > 0 && numericValue < columnDTO.MinLength)
                            {
                                // Your logic for when numericValue is less than MinLength
                                rowValidationFailed = true;
                                badRows.Add(badRow);
                                if (!errorColumnNames.Contains(columnDTO.ColumnName))
                                {
                                    errorColumnNames.Add(columnDTO.ColumnName);
                                }
                                break;
                            }

                            if (columnDTO.MaxLength > 0 && numericValue > columnDTO.MaxLength)
                            {
                                // Your logic for when numericValue is greater than MaxLength
                                rowValidationFailed = true;
                                badRows.Add(badRow);
                                if (!errorColumnNames.Contains(columnDTO.ColumnName))
                                {
                                    errorColumnNames.Add(columnDTO.ColumnName);
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int col = 0; col < excelData.Columns.Count - 2; col++)
                    {
                        string cellData = excelData.Rows[row][col].ToString();
                        ColumnMetaDataDTO columnDTO = columnsDTO[col];

                        if (double.TryParse(cellData, out double numericValue))
                        {
                            if (columnDTO.MinLength > 0 && numericValue < columnDTO.MinLength)
                            {
                                // Your logic for when numericValue is less than MinLength
                                rowValidationFailed = true;
                                badRows.Add(badRow);
                                if (!errorColumnNames.Contains(columnDTO.ColumnName))
                                {
                                    errorColumnNames.Add(columnDTO.ColumnName);
                                }
                                break;
                            }

                            if (columnDTO.MaxLength > 0 && numericValue > columnDTO.MaxLength)
                            {
                                // Your logic for when numericValue is greater than MaxLength
                                rowValidationFailed = true;
                                badRows.Add(badRow);
                                if (!errorColumnNames.Contains(columnDTO.ColumnName))
                                {
                                    errorColumnNames.Add(columnDTO.ColumnName);
                                }
                                break;
                            }
                        }
                    }
                }

                if (!rowValidationFailed)
                {
                    validRowsDataTable.Rows.Add(excelData.Rows[row].ItemArray);
                }
            }
            // Return both results
            return new ValidationResultData { BadRows = badRows, SuccessData = validRowsDataTable, errorcolumns = errorColumnNames, Column_Name = string.Empty };
        }

        public async Task<List<LogChild>> GetAllLogChildsByParentIDAsync(int parentID)
        {
            try
            {
                return await _context.LogChilds
                    .Where(c => c.ParentID == parentID)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
