// -------------------------------------------------------------------------
// Module: CWriteLog
// Author: Patrick Callahan
// Abstract: Log messages to disk
//
// Revision        Owner   Changes:
// 2013/04/07      P.C.    For Book
// 2013/10/09      P.C.    C# version
// -------------------------------------------------------------------------

// -------------------------------------------------------------------------
// Imports
// -------------------------------------------------------------------------
using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using nsImportAndExportAllInOne;

// Must have "using nsUtilities" or change namespace here to match the rest of the project.
// Everything is static so you don//t need to make an instance to call.
// Sample procedure usage:
//
//  private void DoSomething( )
//  {
//      try
//      {
//          // Your code here
//      }
//      catch ( Exception excError )
//      {
//          CWriteLog.WriteLog( excError );
//      }
//  }

namespace nsImportAndExportAllInOne
{

    class CUtilities
    {

        // -------------------------------------------------------------------------
        //  Constants
        // -------------------------------------------------------------------------
        // What log file should we use
        private const String strLOG_FILE_EXTENSION = ".Log";

        // -------------------------------------------------------------------------
        //  Properties
        // -------------------------------------------------------------------------
        private static String m_strOldLogFilePath = "";        // Name of the last log file opened
        private static FileStream m_fsLogFile = null;          // File handle of the last log file opened
        private static enuEmailComplexity m_enuEmailComplexity = enuEmailComplexity.COMPLEX;


        // --------------------------------------------------------------------------------
        // Name: SetBusyCursor
        // Abstract: Enable/Disable the form and set the cursor to normal or busy.
        // --------------------------------------------------------------------------------
        public static void SetBusyCursor(Form frmForm, bool blnBusy)
        {
            try
            {
                // Busy?
                if (blnBusy == true)
                {
                    // Yes
                    frmForm.Cursor = Cursors.WaitCursor;
                    frmForm.Enabled = false;
                }
                else
                {// No
                    frmForm.Cursor = Cursors.Default;
                    frmForm.Enabled = true;
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

        }



        // --------------------------------------------------------------------------------
        // Name: SelectItemInComboBoxFromID
        // Abstract: Enable/Disable the form and set the cursor to normal or busy.
        // --------------------------------------------------------------------------------
        public static void SelectItemInComboBoxFromID(ref ComboBox cmbTarget, int intItemToSelectID)
        {
            try
            {
                int intIndex = 0;
                CListItem liCurrentItem = null;

                liCurrentItem = new CListItem();

                // Loop through all the list items
                for (intIndex = 0; intIndex < cmbTarget.Items.Count - 1; intIndex += 1)
                {
                    liCurrentItem = (CListItem)cmbTarget.Items[intIndex];

                    // do the ID's match?
                    if (intItemToSelectID == liCurrentItem.GetID())
                    {
                        // Yes, Select it
                        cmbTarget.SelectedIndex = intIndex;

                        // Stop searching
                        break;
                    }
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

        }



        // --------------------------------------------------------------------------------
        // Name: TrimAllFormTextBoxes
        // Abstract: Trim all the textboxes on the form.
        // --------------------------------------------------------------------------------
        public static void TrimAllFormTextBoxes(Form frmTarget)
        {
            try
            {
                TrimAllFormTextBoxes(frmTarget.Controls);
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }
        }




        // --------------------------------------------------------------------------------
        // Name: SetBusyCursor
        // Abstract: Enable/Disable the form and set the cursor to normal or busy.
        // --------------------------------------------------------------------------------
        public static void TrimAllFormTextBoxes(Control.ControlCollection ccTarget)
        {
            try
            {
                TextBox txtCurrentTextBox = null;

                // Loop thorugh each of the controls
                foreach (Control ctlCurrentControl in ccTarget)
                {
                    // Is it a Textbox?
                    if (ctlCurrentControl is TextBox)
                    {
                        // Yes, then trim it
                        txtCurrentTextBox = (TextBox)ctlCurrentControl;
                        txtCurrentTextBox.Text = txtCurrentTextBox.Text.Trim();
                    }
                    // Container control (e.g. groupbox, panel?)
                    else if (ctlCurrentControl.HasChildren == true)
                    {
                        // Yes, recursive
                        TrimAllFormTextBoxes(ctlCurrentControl.Controls);
                    }
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

        }



        // --------------------------------------------------------------------------------
        // Name: ToProperCase
        // Abstract: make the source propercase
        //
        //	Must have "using System.Globalization;" for CultureInfo.
        //
        // --------------------------------------------------------------------------------
        public static void ToProperCase(ref string strSource)
        {

            try
            {
                TextInfo tiEnglishUSA = new CultureInfo("en-US").TextInfo;

                strSource = tiEnglishUSA.ToTitleCase(strSource);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // -------------------------------------------------------------------------
        // Name: WriteLog
        // Abstract: Overload withd blnDisplay set to true
        // -------------------------------------------------------------------------
        public static void WriteLog(Exception excErrorToLog)
        {
            try
            {
                WriteLog(excErrorToLog.ToString(), true);
            }
            catch (Exception excError)
            {
                // Display error message
                MessageBox.Show("Error:\n" + excError.ToString(), Application.ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        // -------------------------------------------------------------------------
        // Name: WriteLog
        // Abstract: Overload withd blnDisplay set to true
        // -------------------------------------------------------------------------
        public static void WriteLog(Exception excErrorToLog,
                                     Boolean blnDisplayWarning)
        {
            try
            {

                WriteLog(excErrorToLog.ToString(), blnDisplayWarning);
            }
            catch (Exception excError)
            {
                // Display error message
                MessageBox.Show("Error:\n" + excError.ToString(), Application.ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        // -------------------------------------------------------------------------
        // Name: WriteLog
        // Abstract: Write a message to the error log.
        // -------------------------------------------------------------------------
        public static void WriteLog(String strMessageToLog)
        {
            try
            {
                WriteLog(strMessageToLog, true);
            }
            catch (Exception excError)
            {
                // Display error message
                MessageBox.Show("Error:\n" + excError.ToString(), Application.ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        // -------------------------------------------------------------------------
        // Name: WriteLog
        // Abstract: Write a message to the error log.
        // -------------------------------------------------------------------------
        public static void WriteLog(String strMessageToLog,
                                     Boolean blnDisplayWarning)
        {
            try
            {
                FileStream fsLogFile = null;
                System.Text.UTF8Encoding encConvertToByteArray = new System.Text.UTF8Encoding();

                // Warn the user?
                if (blnDisplayWarning == true)
                {
                    // Yes( ProductName is set in AssemblyInfo )
                    MessageBox.Show(strMessageToLog, Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Append a date/time stamp
                strMessageToLog = (DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss")
                                  + " - " + strMessageToLog + Environment.NewLine +
                                  Environment.NewLine;

                // Get a free file handle
                fsLogFile = GetLogFile();

                // Is the file OK?
                if (fsLogFile != null)
                {
                    // Yes, Log it
                    fsLogFile.Write(encConvertToByteArray.GetBytes(strMessageToLog),
                                     0, strMessageToLog.Length);

                    // Flush the buffer so we can immediately see results in file.  Very important.
                    // Otherwise we have to wait for flush which might be when application closes
                    // or we get another error.  Waiting for the application to close may not be
                    // a good idea if the application is in a production environment ( e.g. a web
                    //  app running on a remote server )
                    fsLogFile.Flush();
                }
            }
            catch (Exception excError)
            {
                // Display error message
                MessageBox.Show("Error:\n" + excError.ToString(), Application.ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        // -------------------------------------------------------------------------
        // Name: DeleteOldFiles
        // Abstract: Delete any files older than 10 days.
        // -------------------------------------------------------------------------
        private static void DeleteOldFiles()
        {
            try
            {
                String strLogFilePath = "";
                DirectoryInfo dirLogDirectory = null;
                DateTime dtmFileCreated = DateTime.Now;
                int intDaysOld = 0;

                // Path
                strLogFilePath = Application.StartupPath + "\\Log\\";

                // Look for any files
                dirLogDirectory = new DirectoryInfo(strLogFilePath);

                // Are there any?
                foreach (FileInfo finLogFile in dirLogDirectory.GetFiles("*" + strLOG_FILE_EXTENSION))
                {
                    // When was the file created?
                    dtmFileCreated = finLogFile.CreationTime;

                    // How old is the file?
                    intDaysOld = (dtmFileCreated.Subtract(DateTime.Now)).Days;

                    // Is the file older than 10 days?
                    if (intDaysOld > 10)
                    {
                        // Yes.  Delete it.
                        finLogFile.Delete();
                    }

                }

            }
            catch (Exception excError)
            {
                // Display error message
                MessageBox.Show("Error:\n" + excError.ToString(), Application.ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        // -------------------------------------------------------------------------
        // Name: GetLogFile
        // Abstract: Open the log file for writing.  Use today//s date as part of
        //           the file name.  Each day a new log file will be created.
        //           Makes debug easier.
        //           Use a filestream object so we can specify file read share
        //           during the open call.
        // -------------------------------------------------------------------------
        private static FileStream GetLogFile()
        {
            try
            {
                String strToday = (DateTime.Now).ToString("yyyyMMdd");
                String strLogFilePath = "";

                // Log everything in a log directory off of the current application directory
                strLogFilePath = Application.StartupPath + "\\Log\\" + strToday + strLOG_FILE_EXTENSION;

                // Is this a new day?
                if (m_strOldLogFilePath != strLogFilePath)
                {
                    // Save the log file name
                    m_strOldLogFilePath = strLogFilePath;

                    // Does the log directory exist?
                    if (Directory.Exists(Application.StartupPath + "\\Log") == false)
                    {
                        // No, so create it
                        Directory.CreateDirectory(Application.StartupPath + "\\Log");
                    }

                    // Close old log file( if there is one )
                    if (m_fsLogFile != null) m_fsLogFile.Close();

                    // Delete old log files
                    DeleteOldFiles();

                    // Does the file exist?
                    if (File.Exists(strLogFilePath) == false)
                    {
                        // No, create with shared read access so it can be read while application has it open
                        m_fsLogFile = new FileStream(strLogFilePath, FileMode.Create,
                                                      FileAccess.Write, FileShare.Read);
                    }
                    else
                    {
                        // Yes, append with shared read access so it can be read while application has it open
                        m_fsLogFile = new FileStream(strLogFilePath, FileMode.Append,
                                                      FileAccess.Write, FileShare.Read);

                    }

                }

            }
            catch (Exception excError)
            {
                // Display error message
                MessageBox.Show("Error:\n" + excError.ToString(), Application.ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Return result
            return m_fsLogFile;
        }



        // --------------------------------------------------------------------------------
        // Name: GetVersion
        // Abstract: Get the major, minor revision numbers. Format at M.m.
        // --------------------------------------------------------------------------------
        public static string GetVersion()
        {
            string strVersion = "";

            try
            {
                // Major
                strVersion += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();

                // Minor
                strVersion += "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();

                // or strVersion = Application.Version
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return strVersion;
        }



        // --------------------------------------------------------------------------------
        // Name: CheckForPreviousApplicationInstance
        // Abstract: Check to see if a previous instance exists( true )
        // --------------------------------------------------------------------------------
        public static bool CheckForPreviousApplicationInstance()
        {

            bool blnPreviousInstance = false;

            try
            {
                string strCurrentProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                Process[] aproInstanceList = Process.GetProcessesByName(strCurrentProcessName);

                // Does a previous version exist?
                if (aproInstanceList.Length > 1)
                {
                    // Yes
                    blnPreviousInstance = true;
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return blnPreviousInstance;
        }


        //--------------------------------------------------------------------------------
        // Name: Wait
        // Abstract: Wait a few seconds
        // --------------------------------------------------------------------------------
        public static void Wait(int intMilliSeconds)
        {
            try
            {
                DateTime dtmWaitUntil = DateTime.Now;

                dtmWaitUntil = dtmWaitUntil.Date;

                // Maximum of 10 second wait
                if (intMilliSeconds > 10000) intMilliSeconds = 10000;

                // Get the current
                dtmWaitUntil = dtmWaitUntil.AddMilliseconds(intMilliSeconds);

                // Wait
                do
                {
                    Application.DoEvents();

                } while (DateTime.Now < dtmWaitUntil);
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }
        }


        // --------------------------------------------------------------------------------
        // Name: HighlightNextItemInList
        // Abstract: Highlight the next closest item in the list
        // --------------------------------------------------------------------------------
        public static void HighlightNextItemInList(ref ListBox lstTarget, int intIndex)
        {
            try
            {
                int intItemCount = 0;

                // are there any items in the list (might have deleted the last one)?
                intItemCount = lstTarget.Items.Count;

                if (intItemCount > 0)
                {
                    // Yes
                    // Did we delete the last item?
                    if (intIndex >= intItemCount)
                    {
                        // Yes, move the index to the new last item
                        intIndex = intItemCount - 1;
                    }

                    // Select the next closest item
                    lstTarget.SelectedIndex = intIndex;
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: HighlightNextItemInList
        // Abstract: Highlight the next closest item in the list
        // --------------------------------------------------------------------------------
        public static void HighlightNextItemInList(ref ListView lvwTarget, int intSelectIndex)
        {
            try
            {
                int intItemCount = 0;
                int intIndex = 0;

                // Loop through currently selected items
                for (intIndex = lvwTarget.SelectedItems.Count - 1; intIndex > 0; intIndex -= 1)
                {
                    // and clear them
                    lvwTarget.SelectedItems[intIndex].Selected = false;

                    // are there any items in the list (might have deleted the last one)?
                    intItemCount = lvwTarget.Items.Count;

                    if (intItemCount > 0)
                    {
                        // Yes
                        // Did we delete the last item?
                        if (intIndex >= intItemCount)
                        {
                            // Yes, move the index to the new last item
                            intIndex = intItemCount - 1;
                        }

                        // Select the next closest item
                        lvwTarget.Items[intIndex].Selected = true;
                    }
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: GetSelectedListItemID
        // Abstract: Get the ID for the currently selected list item. Default to 0.
        // --------------------------------------------------------------------------------
        public static int GetSelectedListItemID(ListBox lstSource)
        {
            int intSelectedItemID = 0;

            try
            {
                CListItem clsSelectedItem = null;

                // Is an item selected?
                if (lstSource.SelectedIndex >= 0)
                {
                    // Yes, get the ID from the item
                    clsSelectedItem = (CListItem)lstSource.SelectedItem;
                    intSelectedItemID = clsSelectedItem.GetID();
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return intSelectedItemID;
        }



        // --------------------------------------------------------------------------------
        // Name: GetSelectedListItemID
        // Abstract: Get the ID for the currently selected list item. Default to 0.
        // --------------------------------------------------------------------------------
        public static int GetSelectedListItemID(ListView lvwSource)
        {
            int intSelectedItemID = 0;

            try
            {
                ListViewItem lviSelectedItem = null;

                // Is an item selected?
                if (lvwSource.SelectedItems.Count > 0)
                {
                    // Yes, get the ID from the tag property
                    lviSelectedItem = lvwSource.SelectedItems[0];
                    intSelectedItemID = (int)Conversion.Val(lviSelectedItem.Tag);
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return intSelectedItemID;
        }


        // --------------------------------------------------------------------------------
        // Name: IsValidZipCode
        // Abstract: Use regular expressions to validate the zip code: 
        //           5 digits or 5 plus 4 digits
        //           ##### or #####-####.
        // --------------------------------------------------------------------------------
        public static bool IsValidZipCode(string strZipCode)
        {
            bool blnIsValidZipCode = false;

            try
            {
                string strStart = "^";
                string strStop = "$";
                string strPattern1 = null;
                string strPattern2 = null;

                // #####
                strPattern1 = strStart + "\\d{5}" + strStop;

                // #####-####
                strPattern2 = strStart + "\\d{5}" + "\\-" + "\\d{4}" + strStop;

                // Does it match any of the formats?
                if (Regex.IsMatch(strZipCode, strPattern1) == true ||
                   Regex.IsMatch(strZipCode, strPattern2) == true)
                {
                    // Yes
                    blnIsValidZipCode = true;
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return blnIsValidZipCode;
        }


        // --------------------------------------------------------------------------------
        // Name: IsValidPhoneNumber
        // Abstract: Use regular expressions to validate the Phone Number: 
        //           ###-#### or (###) ###-####.
        // --------------------------------------------------------------------------------
        public static bool IsValidPhoneNumber(string strPhoneNumber)
        {
            bool blnIsValidPhoneNumber = false;

            try
            {
                string strStart = "^";
                string strStop = "$";
                string strSpaceOrDashOrDot = "[ \\-\\.]";
                string strOptionalSpaceOrDashOrDot = strSpaceOrDashOrDot + "?";
                string strPattern1 = null;
                string strPattern2 = null;
                string strPattern3 = null;
                // ###-####
                strPattern1 = strStart + "\\d{3}" + strSpaceOrDashOrDot + "\\d{4}" + strStop;

                // ###-###-####
                strPattern2 = strStart + "\\d{3}" + strSpaceOrDashOrDot + "\\d{3}" + strSpaceOrDashOrDot + "\\d{4}" + strStop;

                // (###)###-####
                strPattern3 = strStart + "\\(\\d{3}\\)" + strOptionalSpaceOrDashOrDot + "\\d{3}" + strSpaceOrDashOrDot + "\\d{4}" + strStop;

                // Does it match any of the formats?
                if (Regex.IsMatch(strPhoneNumber, strPattern1) == true ||
                    Regex.IsMatch(strPhoneNumber, strPattern2) == true ||
                    Regex.IsMatch(strPhoneNumber, strPattern3) == true)
                {
                    // Yes
                    blnIsValidPhoneNumber = true;
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return blnIsValidPhoneNumber;
        }



        // --------------------------------------------------------------------------------
        // Name: IsCurrency
        // Abstract: Use regular expressions to validate currency: 
        //           +/-$#,###.##.
        // --------------------------------------------------------------------------------
        public static bool IsCurrency(string strBuffer)
        {
            bool blnIsCurrency = false;

            try
            {
                string strStart = "^";
                string strStop = "$";
                string strOptionalPlusOrMinus = "[\\+\\-]?";
                string strOptionalDollarSign = "\\$?";
                string strOptionalDecimal = "(\\.\\d{2})?";
                string strPattern1 = null;
                string strPattern2 = null;

                // Optional +/-, optional $, one or more digits, optional decimal
                strPattern1 = strStart
                            + strOptionalPlusOrMinus
                            + strOptionalDollarSign
                            + "\\d+"
                            + strOptionalDecimal
                            + strStop;

                // Same as above but with commas every 3 digits (e.g 12,345).
                strPattern2 = strStart
                            + strOptionalPlusOrMinus
                            + strOptionalDollarSign
                            + "\\d{1,3}(,\\d{3})*"
                            + strOptionalDecimal
                            + strStop;

                // Does it match any of the formats?
                if (Regex.IsMatch(strBuffer, strPattern1) == true ||
                    Regex.IsMatch(strBuffer, strPattern2) == true)
                {
                    // Yes
                    blnIsCurrency = true;
                }
            }
            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return blnIsCurrency;
        }



        // --------------------------------------------------------------------------------
        // Name: IsValidDate
        // Abstract: Use regular expressions to verify that the string is in valid
        //			 date format: yyyy/mm/dd
        //			 Yes, this isn't the most rigorous test and there are better
        //			 regular expressions out there but I leave that as an 
        //			 exercise for the student. ;)
        // --------------------------------------------------------------------------------
        public static bool IsValidDate(string strDate)
        {

            bool blnIsValidDate = false;

            try
            {
                string strStart = "^";
                string strStop = "$";
                string strSlashOrDashOrDot = "[\\/\\-\\.]";
                string strPattern = null;

                // yyyy /-. mm /-. dd
                strPattern = strStart
                           + "\\d{4}"
                           + strSlashOrDashOrDot
                           + "\\d{2}"
                           + strSlashOrDashOrDot
                           + "\\d{2}"
                           + strStop;

                blnIsValidDate = IsValidDate(strDate, strPattern);
            }

            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return blnIsValidDate;
        }


        // --------------------------------------------------------------------------------
        // Name: IsValidDate
        // Abstract: Use regular expressions to verify that the string is in valid date format.
        // --------------------------------------------------------------------------------
        public static bool IsValidDate(string strDate, string strPattern)
        {

            bool blnIsValidDate = false;

            try
            {
                // Does it match the format
                if (Regex.IsMatch(strDate, strPattern) == true) blnIsValidDate = true;
            }

            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return blnIsValidDate;
        }



        // --------------------------------------------------------------------------------
        // Name: IsValidDate
        // Abstract: Use regular expressions to verify that the string is in valid
        //			 date format: yyyy/mm/dd
        //			 Yes, this isn't the most rigorous test and there are better
        //			 regular expressions out there but I leave that as an 
        //			 exercise for the student. ;)
        // --------------------------------------------------------------------------------
        public static bool IsValidEmailAddress(string strEmailAddress)
        {

            bool blnIsValidEmailAddress = false;

            try
            {
                string strStart = "^";
                string strStop = "$";
                string chrOR = "|";
                string strTopLevelDomainList = "aero|asia|biz|cat|cc|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|xxx";
                string strCountryCodes = "ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az" +
                                         "|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz" +
                                         "|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz" +
                                         "|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr" +
                                         "|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy" +
                                         "|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it" +
                                         "|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz" +
                                         "|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly" +
                                         "|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz" +
                                         "|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om" +
                                         "|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw" +
                                         "|sa|sb|sc|sd|se|sg|sh|si|sj|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz" +
                                         "|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz" +
                                         "|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw";
                string strComplexPattern = null;
                string strEasyPattern = null;

                // From http://www.regular-expressions.info/email.html at the end of the page
                strComplexPattern = strStart
                                  + "[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\\.)+"
                                  + "(" + strTopLevelDomainList + chrOR + strCountryCodes + ")"
                                  + strStop;

                // 1:letter, N:letters/numbers/dot/dash
                // 1:@
                // 1:letter, N:letters/numbers/dot/dash, 1:dot, 2-6:letters
                strEasyPattern = strStart
                               + "[a-zA-Z][a-zA-Z0-9\\.\\-]*"
                               + "@"
                               + "[a-zA-Z][a-zA-Z0-9\\.\\-]*\\.[a-zA-Z]{2,6}"
                               + strStop;

                // Check which validation method to use, then validate.
                switch (m_enuEmailComplexity)
                {
                    case enuEmailComplexity.EASY:
                        if (Regex.IsMatch(strEmailAddress, strEasyPattern) == true) blnIsValidEmailAddress = true;
                        break;

                    case enuEmailComplexity.COMPLEX:
                        if (Regex.IsMatch(strEmailAddress, strComplexPattern) == true) blnIsValidEmailAddress = true;
                        break;
                }
            }

            catch (Exception excError)
            {
                // log and display error
                CUtilities.WriteLog(excError);
            }

            return blnIsValidEmailAddress;
        }
    }
}

