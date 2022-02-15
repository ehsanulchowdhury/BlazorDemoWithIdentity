using SecureDemoClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.ExcelToPdfConverter;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using System.Drawing;
using System.Web;

namespace SecureDemoClassLibrary.Services
{
    public class EmployeeService : IDataService<Employee>
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeeService(ApplicationDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException("dbContext");
        }

        public async Task<Employee?> GetByIdAsync(int id) => await _dbContext.Employees.FindAsync(id);

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _dbContext.Employees
            .OrderBy(a => a.FirstName)
            .ThenBy(a => a.LastName)
            .Select(a => new Employee
            {
                EmployeeId = a.EmployeeId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                DateOfBirth = a.DateOfBirth,
                DateOfJoining = a.DateOfJoining,
                PictureFileExtension = a.PictureFileExtension,
                AttachmentFileExtension = a.AttachmentFileExtension
            })
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
        {
            string term = searchTerm.ToLower();

            return await _dbContext
                        .Employees
                        .Where(a =>
                        a.FirstName!.ToLower().Contains(term) || a.LastName!.ToLower().Contains(term))
                        .OrderBy(a => a.FirstName)
                        .ThenBy(a => a.LastName)
                        .ToListAsync();
        }

        public async Task<Employee> Add(Employee employee)
        {
            ValidateModelData(employee);

            Employee emp = new()
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                DateOfJoining = employee.DateOfJoining,
                Attachment = employee.Attachment,
                Image = employee.Image,
                PictureFileExtension = employee.PictureFileExtension,
                AttachmentFileExtension = employee.AttachmentFileExtension
            };

            await _dbContext.Employees.AddAsync(emp);
            await _dbContext.SaveChangesAsync();
            return emp;
        }
        public async Task Update(Employee employee)
        {
            ValidateModelData(employee);

            Employee? emp = await GetByIdAsync(employee.EmployeeId);
            if (emp == null) throw new InvalidDataException("Invalid Employee.");
            emp.FirstName = employee.FirstName;
            emp.LastName = employee.LastName;
            emp.DateOfBirth = employee.DateOfBirth;
            emp.DateOfJoining = employee.DateOfJoining;
            emp.Attachment = employee.Attachment;
            emp.Image = employee.Image;
            emp.PictureFileExtension = employee.PictureFileExtension;
            emp.AttachmentFileExtension = employee.AttachmentFileExtension;

            await _dbContext.SaveChangesAsync();
        }
        public async Task Delete(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);
            if (employee == null) throw new InvalidDataException("Invalid Id");

            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();

        }

        private void ValidateModelData(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            if (string.IsNullOrWhiteSpace(employee.FirstName))
            {
                throw new ArgumentException("First Name is required.");
            }
            
            if (employee.FirstName.Length > 50)
            {
                throw new ArgumentException("First Name must be less or equal to 50 characters.");
            }

            if (string.IsNullOrWhiteSpace(employee.LastName))
            {
                throw new ArgumentException("Last Name is required.");
            }
            
            if (employee.LastName.Length > 50)
            {
                throw new ArgumentException("Last Name must be less or equal to 50 characters.");
            }
        }

        public static byte[]? GetEmployeeProfileAsWordDoc(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            string currentDir = Directory.GetCurrentDirectory();
            string templatePath = currentDir + @"\DocTemplates\template.dotx";
            
            byte[]? profileBytes = null;

            using (WordDocument document = new())
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (FileStream fileStream = File.OpenRead(templatePath))
                        {
                            fileStream.CopyTo(memoryStream);
                        }
                        document.Open(memoryStream, Syncfusion.DocIO.FormatType.Dotx);
                    }

                    if (document.Sections[0].Count>0 && document.Sections[0].Tables.Count > 0)
                    {
                        IWTable table = document.Sections[0].Tables[0];
                        table[1, 0].Paragraphs[0].Text = employee.FirstName;
                        table[1, 1].Paragraphs[0].Text = employee.LastName;
                        if (employee.DateOfBirth != null)
                        {
                            table[3, 0].Paragraphs[0].Text = ((DateTime)employee.DateOfBirth).ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            table[3, 0].Paragraphs[0].Text = "";
                        }
                        if (employee.DateOfJoining != null)
                        {
                            table[5, 0].Paragraphs[0].Text = ((DateTime)employee.DateOfJoining).ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            table[5, 0].Paragraphs[0].Text = "";
                        }

                        if (employee.Image != null)
                        {
                            IWPicture picture = table[0, 2].Paragraphs[0].AppendPicture(employee.Image);
                            //picture.HorizontalOrigin = 30;
                            //picture.VerticalPosition = 10;
                            picture.Width = 100;
                            picture.Height = 100;
                        }
                    }

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        document.Save(memoryStream, Syncfusion.DocIO.FormatType.Docx);
                        profileBytes = memoryStream.ToArray();
                    }
                }

                finally
                {
                    document.Close();
                }
            }
            return profileBytes;
        }

        public static byte[]? GetEmployeeProfileAsPdfDoc(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            byte[]? wordBytes = GetEmployeeProfileAsWordDoc(employee);

            byte[]? pdfBytes = null;

            if (wordBytes != null)
            {
                using (MemoryStream memoryStream = new(wordBytes))
                {
                    using (DocToPDFConverter converter = new())
                    {
                        PdfDocument pdfDoc = new();
                        try
                        {
                            pdfDoc = converter.ConvertToPDF(memoryStream);
                            using (MemoryStream outStream = new())
                            {
                                pdfDoc.Save(outStream);
                                pdfBytes = outStream.ToArray();
                                outStream.Close();
                            }
                        }
                        finally
                        {
                            memoryStream.Close();
                            pdfDoc.Dispose();
                        }
                    }
                }
            }

            return pdfBytes;
        }

        public static byte[]? GetEmployeeListAsExcelDoc(IList<Employee> employees)
        {
            if(employees == null) throw new ArgumentNullException(nameof (employees));
            byte[]? excelBytes  = null;

            using (ExcelEngine excelEngine = new ExcelEngine())
            {

                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;
                IWorkbook workbook = application.Workbooks.Create(0);
                IWorksheet sheet = workbook.Worksheets.Create("Employee List");

                //sheet.sets.Cells.Font.Name = "Calibri";

                //Merger columns, set title, allign center, font size and style
                sheet.Range["A1:H1"].Merge();
                Syncfusion.XlsIO.IStyle style = workbook.Styles.Add("NewStyle");
                style.Color = Color.LightGreen;
                style.FillPattern = ExcelPattern.DarkUpwardDiagonal;
                style.Font.FontName = "Calibri";
                style.Font.Size = 20;
                style.Font.Bold = true;
                style.Font.Underline = ExcelUnderline.Double;
                style.HorizontalAlignment = Syncfusion.XlsIO.ExcelHAlign.HAlignCenter;

                IRange range = sheet.Range["A1"];
                range.CellStyle = style;
                range.Text = "Employee List";

                //Set column widths
                sheet.Range["A:A"].ColumnWidth = 1;
                sheet.Range["B:B"].ColumnWidth = 6;
                sheet.Range["C:C"].ColumnWidth = 20;
                sheet.Range["D:D"].ColumnWidth = 20;
                sheet.Range["E:E"].ColumnWidth = 15;
                sheet.Range["F:F"].ColumnWidth = 15;

                //Column headers
                sheet.Range["B1"].Text = "Serial";
                sheet.Range["C1"].Text = "First Name";
                sheet.Range["D1"].Text = "Last Name";
                sheet.Range["E1"].Text = "Date of Birth";
                sheet.Range["F1"].Text = "Date of Joining";
                sheet.Range["G1"].Text = "Pic Ext";
                sheet.Range["H1"].Text = "Atch Ext";

                //Set date format to columns
                sheet.Range["E1:F10"].NumberFormat = "dd-MMM-yyyy";

                int row = 4;
                foreach (Employee emp in employees)
                {
                    sheet.Range[row, 2].Number = (row-3);
                    sheet.Range[row, 2].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    sheet.Range[row, 3].Text = emp.FirstName;
                    sheet.Range[row, 4].Text = emp.LastName;
                    if (emp.DateOfBirth != null)
                        sheet.Range[row, 5].DateTime = ((DateTime)emp.DateOfBirth);

                    if (emp.DateOfJoining != null)
                        sheet.Range[row, 6].DateTime = ((DateTime)emp.DateOfJoining);
                    sheet.Range[row, 7].Text = emp.PictureFileExtension ?? "";
                    sheet.Range[row, 8].Text = emp.AttachmentFileExtension ?? "";

                    row++;
                }

                using(MemoryStream outStream = new())
                {
                    workbook.SaveAs(outStream);
                    excelBytes = outStream.ToArray();
                }
            }

            return excelBytes;
        }

        public static byte[]? GetEmployeeListAsPdfDoc(IList<Employee> employees)
        {
            if (employees == null) throw new ArgumentNullException(nameof(employees));

            byte[]? excelBytes = GetEmployeeListAsExcelDoc(employees);

            byte[]? pdfBytes = null;

            if (excelBytes != null)
            {

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;
                    IWorkbook workbook;
                    IWorksheet worksheet;

                    using (MemoryStream inputStream = new(excelBytes))
                    {
                        workbook = application.Workbooks.Open(inputStream);
                        worksheet = workbook.Worksheets["Employee List"];
                        inputStream.Close();
                    }
                    ExcelToPdfConverterSettings converterSettings = new ExcelToPdfConverterSettings();
                    converterSettings.DisplayGridLines = GridLinesDisplayStyle.Visible;
                    converterSettings.LayoutOptions = LayoutOptions.FitAllColumnsOnOnePage;

                    PdfDocument? pdfDoc = null;
                    using (ExcelToPdfConverter converter = new(worksheet))
                    {
                        try
                        {
                            pdfDoc = converter.Convert(converterSettings);
                            using (MemoryStream outputStream = new())
                            {
                                pdfDoc.Save(outputStream);
                                pdfBytes = outputStream.ToArray();
                                outputStream.Close();
                            }
                        }
                        finally
                        {
                            if (pdfDoc != null)
                                pdfDoc.Dispose();
                        }
                    }
                }
            }

            return pdfBytes;
        }


    }

}
