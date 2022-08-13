using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using NUnit.Framework;
using spire = Spire.Pdf.PdfDocument;
using spirepdf =Spire.Pdf;
using System.Drawing;

namespace SeleniumTest.TestSuites._Tools
{
    internal class PDFActions
    {

        public static void AssertMultipleString(List<string>  web, List<string> pdf)
        {
            int count_list = web.Count();
            for (int count = 0; count < count_list; count++)
            {
                Console.WriteLine(web[count] + " vs " + pdf[count]);
                Assert.AreEqual(web[count], pdf[count], $"web: {web[count]} != pdf: {pdf[count]}");
            }
        }

        public static void AssertMultipleDouble(List<double> web, List<double> pdf)
        {
            int count_list = web.Count();
            for (int count = 0; count < count_list; count++)
            {
                Assert.AreEqual(web[count], pdf[count], $"web: {web[count]} != pdf: {pdf[count]}");
            }

        }

        public static void AssertImages(
            List<(string tag, string format)> web, 
            List<(string tag, string format)> pdf)
        {
            int count_list = web.Count();
            for (int count = 0; count < count_list; count++)
            {
                Assert.AreEqual(web[count].tag, pdf[count].tag, $"TAG {count} | web: {web[count].tag} != pdf: {pdf[count].tag}");
                Assert.AreEqual(web[count].format, pdf[count].format, $"FORMAT {count} | web: {web[count].format} != pdf: {pdf[count].format}");
            }
        }

        /// <summary>
        /// Get Absolute path of the file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns> String | Absolute path of the file </returns>
        /// <exception cref="Exception"></exception>
        public static string GetDownloadedFileLocation(string filename)
        {
            var test_path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var files = Directory.GetFiles(test_path, ".", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (file.Contains(filename))
                {
                    var file_location = $"{test_path}\\{Path.GetFileName(file)}";
                    return file_location;
                }
            }
            throw new Exception($"File {filename} is not found.");
        }

        /// <summary>
        /// Get all data in a row of the table
        /// </summary>
        /// <param name="file_location"></param>
        /// <param name="row">List of names from web app to be used to check each row in the table of pdf.</param>
        /// <returns></returns>
        public static List<string> GetRowFromTable(string file_location, List<string> row, int page_number = 1)
        {
            var pdfDocument = new PdfDocument(new PdfReader(file_location));
            // Get row count
            int column_headers_count = row.Count();
            // Check page 1 only and get all text
            var page = pdfDocument.GetPage(page_number);
            string text = PdfTextExtractor.GetTextFromPage(page, new SimpleTextExtractionStrategy());
            string clean_text = text.Replace("\n", "");
            //Console.WriteLine(clean_text);
            // List items fetch from pdf to be asserted
            var pdf_row_items = new List<string>();

            // Add all items to the pdf_row_items
            for (int column_number = 0; column_number < column_headers_count; column_number++)
            {
                try
                {
                    var column_header = row[column_number];
                    int index = clean_text.IndexOf(column_header);
                    pdf_row_items.Add(clean_text[index..(index + column_header.Length)]);
                }
                catch 
                {
                    throw new Exception($"There is something wrong with: {row[column_number]}");
                }
                
            }
            return pdf_row_items;
        }

        public static List<string> GetAllPartNumbers(string file_location, List<string> part_numbers)
        {
            var pdfDocument = new PdfDocument(new PdfReader(file_location));

            // Check page 1 only and get all text
            var page = pdfDocument.GetPage(1);
            string text = PdfTextExtractor.GetTextFromPage(page);

            // Part numbers list
            var part_numbers_list = new List<string>();

            foreach (var part in part_numbers)
            {
                try
                {
                    // Get total text length to use as last index
                    int part_text_length = part.Length;
                    // Start Index of the part number
                    int index = text.IndexOf(part);
                    part_numbers_list.Add(text[index..(index + part_text_length)]);
                }
                catch
                {
                    throw new Exception($"There is something wrong with part number: {part}.");
                }
            }
            return part_numbers_list;
        }

        /// <summary>
        /// Fetch All links in the pdf specifically shareable link and part-numbers link
        /// </summary>
        /// <param name="file_location"></param>
        /// <returns>Dictionary | key as shareable link and value as part numbers link</returns>
        public static List<string> FetchAllLinks(string file_location)
        {
            // Read File
            string[] rawFile = File.ReadAllLines(@file_location);

            // Get Shareable link
            var shareable_link = GetLinks(rawFile, link_name: "search");

            // Get all part numbers link
            var part_numbers_link = GetLinks(rawFile, link_name: "partNumber");

            // Modify part_numbers_link to a cleaner string using CleanLink
            var clean_part_numbers_link = part_numbers_link.Select(s => s.Replace(s, CleanLink(s))).ToList();

            // Collate all in a list
            var all_links_list = new List<string>();
            all_links_list.Add(CleanLink(shareable_link[0]));
            all_links_list.AddRange(clean_part_numbers_link);

            return all_links_list;
        }

        /// <summary>
        /// Find and Get the sorted values in the pdf
        /// </summary>
        /// <param name="file_location"></param>
        /// <param name="column_name"></param>
        /// <param name="part_numbers"></param>
        /// <param name="row">All column header names in the table</param>
        /// <returns></returns>
        public static List<double> GetSortedValues(string file_location, string column_name, List<string> part_numbers ,List<string> row)
        {
            var pdfDocument = new PdfDocument(new PdfReader(file_location));
            
            // Check page 1 only and get all text
            var page = pdfDocument.GetPage(1);
            string text = PdfTextExtractor.GetTextFromPage(page);

            // Sorted values in the pdf
            var sorted_values = new List<double>();

            for (int count = 0; count < part_numbers.Count(); count++)
            {
                try
                {
                    // Get the starting index of the part number from part numbers
                    int first = text.IndexOf(part_numbers[count]);

                    // Get the last index of the next part number on the table, if it is the last part number, then last will be the total length of text in pdf
                    int last = (count < part_numbers.Count() - 1) ? text.IndexOf(part_numbers[count + 1]) - 1 : text.Length;

                    // Split the text from the starting index until the last index then convert it to lost
                    var list = text[first..last].Split(" ");
                    
                    // Get the index of the column_name inside the row list
                    var index = row.IndexOf(column_name);
                    sorted_values.Add(Convert.ToDouble(list[index]));
                }
                catch
                {
                    throw new Exception($"There is something wrong with: {part_numbers[count]}.");
                }
            }
            return sorted_values;
        }


        public static List<(string tag, string format)> GetImagesInPdf(string file_location)
        {
            // Load pdf file
            spire doc = new spire();
            doc.LoadFromFile(@file_location);

            // Image List container
            var image_list = new List<(string tag, string format)>();

            // Check each pages for images except page 1 which contains the data table
            for (int page_number = 2; page_number <= Convert.ToInt64(doc.Pages.Count); page_number++)
            {
                try
                {
                    spirepdf.PdfPageBase page = doc.Pages[page_number - 1];
                    Image[] images = page.ExtractImages();
                    foreach (Image image in images)
                    {
                        image_list.Add((
                            image.Tag.ToString(),
                            image.RawFormat.ToString().Replace("[ImageFormat: ", "").Replace("]", "")
                            ));
                    }
                }
                catch
                {
                    throw new Exception($"No image found in page {page_number}.");
                }
            }
            return image_list;
        }

        /// <summary>
        /// Get all image titles in the pdf
        /// </summary>
        /// <param name="file_location"></param>
        /// <param name="row">List of names from web app to be used to check each row in the table of pdf.</param>
        /// <returns></returns>
        public static List<string> GetImageTitles(string file_location, List<string> image_titles)
        {
            var pdfDocument = new PdfDocument(new PdfReader(file_location));

            // Containter for parsed image titles in the pdf
            var titles = new List<string>();
        
            // Check all pages except page 1 which contains the data table
            int total_pages = Convert.ToInt32(pdfDocument.GetNumberOfPages());
            for (int page_number = 2; page_number <= total_pages; page_number++)
            {
                var page = pdfDocument.GetPage(page_number);
                string text = PdfTextExtractor.GetTextFromPage(page, new SimpleTextExtractionStrategy());
                string clean_text = text.Replace("\n", "");

                // Check if image title is in the current page
                foreach (var item in image_titles)
                {
                    try
                    {
                        if (titles.Contains(item)) continue;
                        int index = clean_text.IndexOf(item);
                        titles.Add(clean_text[index..(index + item.Length)]);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            return titles;
        }


        /// <summary>
        /// Remove strings that is not part of the link
        /// </summary>
        /// <param name="link"></param>
        /// <returns> String | removed strings that is not part of the link </returns>
        public static string CleanLink(string link)
        {
            return link
                .Remove(startIndex: link.Length - 1) // Remove Close Parenthesis -> ")"
                .Remove(startIndex: 0, count: 6); // Remove Open Parenthesis and URI -> "/URI ("
        }

        /// <summary>
        /// Get shareable link or part-numbers link
        /// </summary>
        /// <param name="links"></param>
        /// <param name="link_name"></param>
        /// <returns> List | links but still with unnecessary strings </returns>
        public static List<string> GetLinks(string[] links, string link_name)
        {
            var get_all_link = links.ToList()
                .Where(x => x.Contains("URI") && x.Contains("https") && x.Contains(link_name));
            var shareable_link = new List<string>();
            if (link_name == "search")
            {
                shareable_link.Add(get_all_link.ToList().First()); // Get only the first item 
                return shareable_link;
            }
            return get_all_link.ToList();
        }
    }
}
