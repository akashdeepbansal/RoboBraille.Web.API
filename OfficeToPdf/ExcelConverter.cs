﻿/**
 *  OfficeToPDF command line PDF conversion for Office 2007/2010
 *  Copyright (C) 2011-2015 Cognidox Ltd
 *  http://www.cognidox.com/opensource/
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace OfficeToPDF
{
    /// <summary>
    /// Handle conversion of Excel files
    /// </summary>
    class ExcelConverter: Converter
    {
        public static new int Convert(String inputFile, String outputFile, Hashtable options)
        {
            Boolean running = (Boolean)options["noquit"];
            Microsoft.Office.Interop.Excel.Application app = null;
            Microsoft.Office.Interop.Excel.Workbooks workbooks = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;

            String tmpFile = null;
            object oMissing = System.Reflection.Missing.Value;
            Boolean nowrite = (Boolean)options["readonly"];
            try
            {
                app = new Microsoft.Office.Interop.Excel.Application()
                {
                    Visible = true,
                    DisplayAlerts = false,
                    AskToUpdateLinks = false,
                    AlertBeforeOverwriting = false,
                    EnableLargeOperationAlert = false,
                    Interactive = false,
                    FeatureInstall = Microsoft.Office.Core.MsoFeatureInstall.msoFeatureInstallNone
                };
                if ((Boolean)options["hidden"])
                {
                    // Try and at least minimise it
                    app.WindowState = XlWindowState.xlMinimized;
                    app.Visible = false;
                }

                String readPassword = "";
                if (!String.IsNullOrEmpty((String)options["password"]))
                {
                    readPassword = (String)options["password"];
                }
                Object oReadPass = (Object)readPassword;

                String writePassword = "";
                if (!String.IsNullOrEmpty((String)options["writepassword"]))
                {
                    writePassword = (String)options["writepassword"];
                }
                Object oWritePass = (Object)writePassword;

                // Check for password protection and no password
                if (Converter.IsPasswordProtected(inputFile) && String.IsNullOrEmpty(readPassword))
                {
                    Console.WriteLine("Unable to open password protected file");
                    return (int)ExitCode.PasswordFailure;
                }

                app.EnableEvents = (bool)options["excel_auto_macros"];
                workbooks = app.Workbooks;
                workbook = workbooks.Open(inputFile, true, nowrite, oMissing, oReadPass, oWritePass, true, oMissing, oMissing, oMissing, oMissing, oMissing, false, oMissing, oMissing);

                // Unable to open workbook
                if (workbook == null)
                {
                    return (int)ExitCode.FileOpenFailure;
                }

                if (app.EnableEvents)
                {
                    workbook.RunAutoMacros(XlRunAutoMacro.xlAutoOpen);
                }

                // Try and avoid xls files raising a dialog
                var temporaryStorageDir = Path.GetTempFileName();
                File.Delete(temporaryStorageDir);
                Directory.CreateDirectory(temporaryStorageDir);
                tmpFile = Path.Combine(temporaryStorageDir, Path.GetFileNameWithoutExtension(inputFile) + ".xls");

                // Set up the file save format
                XlFileFormat fmt = XlFileFormat.xlOpenXMLWorkbook;
                if (workbook.HasVBProject)
                {
                    fmt = XlFileFormat.xlOpenXMLWorkbookMacroEnabled;
                    tmpFile += "m";
                }
                else
                {
                    tmpFile += "x";
                }

                // Set up the print quality
                XlFixedFormatQuality quality = XlFixedFormatQuality.xlQualityStandard;
                if ((Boolean)options["screen"])
                {
                    quality = XlFixedFormatQuality.xlQualityMinimum;
                }

                // Remember - Never use 2 dots with COM objects!
                // Using more than one dot leaves wrapper objects left over
                var wbWin = workbook.Windows;
                var appWin = app.Windows;
                if ((Boolean)options["excel_show_formulas"])
                {
                    // Determine whether to show formulas
                    appWin[1].DisplayFormulas = true;
                }
                if (wbWin.Count > 0)
                {
                    wbWin[1].Visible = (Boolean)options["hidden"] ? false : true;
                    Converter.ReleaseCOMObject(wbWin);
                }
                if (appWin.Count > 0)
                {
                    appWin[1].Visible = (Boolean)options["hidden"] ? false : true;
                    Converter.ReleaseCOMObject(appWin);
                }

                // Large excel files may simply not print reliably - if the excel_max_rows
                // configuration option is set, then we must close up and forget about 
                // converting the file. However, if a print area is set in one of the worksheets
                // in the document, then assume the author knew what they were doing and
                // use the print area.
                var max_rows = (int)options[@"excel_max_rows"];

                // We may need to loop through all the worksheets in the document
                // depending on the options given. If there are maximum row restrictions
                // or formulas are being shown, then we need to loop through all the
                // worksheets
                if (max_rows > 0 || (Boolean)options["excel_show_formulas"] || (Boolean)options["excel_show_headings"])
                {
                    var row_count_check_ok = true;
                    var found_rows = 0;
                    var found_worksheet = "";
                    var worksheets = workbook.Worksheets;
                    foreach (var ws in worksheets)
                    {
                        if ((Boolean)options["excel_show_headings"])
                        {
                            var pageSetup = ((Microsoft.Office.Interop.Excel.Worksheet)ws).PageSetup;
                            pageSetup.PrintHeadings = true;
                            Converter.ReleaseCOMObject(pageSetup);
                        }

                        // If showing formulas, make things auto-fit
                        if ((Boolean)options["excel_show_formulas"])
                        {
                            ((Microsoft.Office.Interop.Excel.Worksheet)ws).Activate();
                            app.ActiveWindow.DisplayFormulas = true;
                            var cols = ((Microsoft.Office.Interop.Excel.Worksheet)ws).Columns;
                            cols.AutoFit();
                            Converter.ReleaseCOMObject(cols);
                        }

                        // If there is a maximum row count, make sure we check each worksheet
                        if (max_rows > 0)
                        {
                            // Check for a print area
                            var page_setup = ((Microsoft.Office.Interop.Excel.Worksheet)ws).PageSetup;
                            var print_area = page_setup.PrintArea;
                            Converter.ReleaseCOMObject(page_setup);
                            if (string.IsNullOrEmpty(print_area))
                            {
                                // There is no print area, check that the row count is <= to the
                                // excel_max_rows value. Note that we can't just take the range last
                                // row, as this may return a huge value, rather find the last non-blank
                                // row.
                                var row_count = 0;
                                var range = ((Microsoft.Office.Interop.Excel.Worksheet)ws).UsedRange;
                                if (range != null)
                                {
                                    var rows = range.Rows;
                                    if (rows != null && rows.Count > max_rows)
                                    {
                                        var cells = range.Cells;
                                        if (cells != null)
                                        {
                                            var cellSearch = cells.Find("*", oMissing, oMissing, oMissing, oMissing, Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious, false, oMissing, oMissing);
                                            // Make sure we actually get some results, since the worksheet may be totally blank
                                            if (cellSearch != null)
                                            {
                                                row_count = cellSearch.Row;
                                                found_worksheet = ((Microsoft.Office.Interop.Excel.Worksheet)ws).Name;
                                                Converter.ReleaseCOMObject(cellSearch);
                                            }
                                            Converter.ReleaseCOMObject(cells);
                                        }
                                        Converter.ReleaseCOMObject(rows);
                                    }
                                }
                                Converter.ReleaseCOMObject(range);

                                if (row_count > max_rows)
                                {
                                    // Too many rows on this worksheet - mark the workbook as unprintable
                                    row_count_check_ok = false;
                                    found_rows = row_count;
                                    Converter.ReleaseCOMObject(ws);
                                    break;
                                }
                            }
                        } // End of row check
                        Converter.ReleaseCOMObject(ws);
                    }
                    Converter.ReleaseCOMObject(worksheets);

                    // Make sure we are not converting a document with too many rows
                    if (row_count_check_ok == false)
                    {
                        throw new Exception(String.Format("Too many rows to process ({0}) on worksheet {1}", found_rows, found_worksheet));
                    }
                }

                Boolean includeProps = !(Boolean)options["excludeprops"];

                workbook.SaveAs(tmpFile, fmt, Type.Missing, Type.Missing, Type.Missing, false, XlSaveAsAccessMode.xlNoChange, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing);
                workbook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF,
                    outputFile, quality, includeProps, false, Type.Missing, Type.Missing, false, Type.Missing);
                return (int)ExitCode.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return (int)ExitCode.UnknownError;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }

                if (!running)
                {
                    if (workbooks != null)
                    {
                        workbooks.Close();
                    }

                    if (app != null)
                    {
                        ((Microsoft.Office.Interop.Excel._Application)app).Quit();
                    }
                }

                // Clean all the COM leftovers
                Converter.ReleaseCOMObject(workbook);
                Converter.ReleaseCOMObject(workbooks);
                Converter.ReleaseCOMObject(app);

                if (tmpFile != null && File.Exists(tmpFile))
                {
                    System.IO.File.Delete(tmpFile);
                    // Remove the temporary path to the temp file
                    Directory.Delete(Path.GetDirectoryName(tmpFile));
                }
            }
        }
    }
}
