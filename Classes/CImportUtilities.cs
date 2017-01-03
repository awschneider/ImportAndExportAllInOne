using Microsoft.VisualBasic;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Windows.Forms;

namespace nsImportAndExportAllInOne
{
    class CImportUtilities
    {
        // We assume the patient isn't in the database unles proved otherwise.
        // We use this flag so even if an individual patients information is rejected, doesnt mean all the data 
        //      is bad, we want to collect this data, and do so without adding relationships to a non existent patient.
        static bool blnPatientInDatabase = false;

        // --------------------------------------------------------------------------------
        // Name: ImportPatientRecords
        // Abstract: Performs import procedures
        // --------------------------------------------------------------------------------
        public static bool ImportPatientRecords(string strFilePath)
        {
            bool blnResult = false;

            try
            {
                StreamReader srPatientRecordFile = null;
                string strFileLines = "";

                // Read the file line by line
                srPatientRecordFile = new StreamReader(strFilePath);

                // Loop through the document assigning the line to a string variable.
                // Is it Null?
                //https://msdn.microsoft.com/en-us/library/94223t4d.aspx
                while ((strFileLines = srPatientRecordFile.ReadLine()) != null)
                {
                    // No?
                    // Break apart patient import data
                    BreakApartPatientImportData(strFileLines);

                    // Set the Success variable
                    blnResult = blnPatientInDatabase;
                }

                // Yes?
                // Close the file
                srPatientRecordFile.Close();
            }
            catch(Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // -------------------------------------------------------------------------
        // Name: BreakApartPatientImportData
        // Abstract: Brake apart the patient record data
        // -------------------------------------------------------------------------
        private static void BreakApartPatientImportData(string strFileLines)
        {

            try
            {
                udtPatientType udtPatient = new udtPatientType();

                // Get the ID
                GetPatientID(ref udtPatient);

                // Validate the patient data and Import to database if acceptable
                if (IsValidPatientImportData(ref udtPatient, strFileLines) == true)
                {
                    CDatabaseUtilities.AddPatientToDatabase(ref udtPatient);
                    blnPatientInDatabase = true;
                }
                else
                {
                    blnPatientInDatabase = false;
                }

                ExtractAllergies(ref udtPatient, strFileLines);
                ExtractConditions(udtPatient, strFileLines);
                ExtractProcedures(udtPatient, strFileLines);

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: GetPatientID
        // Abstract: Assign the patient an ID
        // --------------------------------------------------------------------------------
        private static void GetPatientID(ref udtPatientType udtPatient)
        {
            try
            {
                CDatabaseUtilities.GetNextHighestRecordID("intPatientID", "TPatients", ref udtPatient.intPatientID);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: IsValidPatientImportData
        // Abstract: Validate the patient import data
        // --------------------------------------------------------------------------------
        private static bool IsValidPatientImportData(ref udtPatientType udtPatient, string strRecordLine )
        {
            bool blnResult = true;
            string strFakeValue = null;

            try
            {
                blnResult = blnResult & ExtractFullName(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractHomeAddress(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractCity(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractState(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractZipCode(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractPrimaryPhone(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractSecondaryPhone(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractSmokerStatus(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractPackYears(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractHeadOfHouseHoldStatus(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractDateOfBirth(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractRecordCreator(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractSex(ref udtPatient, strRecordLine);
                blnResult = blnResult & ExtractEmailAddress(ref udtPatient, strRecordLine);


            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }

        #region "Patients"

        // --------------------------------------------------------------------------------
        // Name: ExtractFullName
        // Abstract: Extract the full name
        // --------------------------------------------------------------------------------
        private static bool ExtractFullName(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnSuccess = true;  // rare case where it is easier to assume data is good, and if not just turn it off
               // F, L M.
               // F L, Suffix
            try
            {

                string strFullName = null;
                int intWordCount = 0;
                int intSubstringLength = 0;
                int intLastIndexOfDot = 0;
                int intFirstIndexOfDot = 0;
                int intLength = 0;

                // Get the full name out
                strFullName = strFileLines.Substring(CConstants.FULL_NAME_START_LOCATION, CConstants.FULL_NAME_LENGTH);
                strFullName = strFullName.Trim();
                intWordCount = CountWordsInString(strFullName);

                // Check the format
                // Three Words and Any Commas? Any Dots?
                if (intWordCount == 3)
                {
                    // FML
                    // Any commas or dots?
                    if (strFullName.IndexOf(',') < 0 && strFullName.IndexOf('.') < 0)
                    {
                        // No.

                        // First
                        intSubstringLength = strFullName.IndexOf(' ');
                        udtPatient.strFirstName = strFullName.Substring(0, intSubstringLength);

                        // Middle
                        intSubstringLength = strFullName.LastIndexOf(' ') - strFullName.IndexOf(' ') - 1;
                        udtPatient.strMiddleName = strFullName.Substring(strFullName.IndexOf(' ') + 1, intSubstringLength);

                        // Last
                        udtPatient.strLastName = strFullName.Substring(strFullName.LastIndexOf(' ') + 1);
                    }

                    // FM.L 
                    // any dots? 
                    else if (strFullName.IndexOf(',') < 0 && strFullName.IndexOf('.') > 0)
                    {
                        // first occurance of a dot
                        intFirstIndexOfDot = strFullName.IndexOf('.');

                        // Last occurance of a dot
                        intLastIndexOfDot = strFullName.LastIndexOf('.');

                        // Get the length
                        intLength = strFullName.Length;

                        // Not L, FM. 
                        if (intLastIndexOfDot != intLength)
                        {
                            // First
                            intSubstringLength = strFullName.IndexOf(' ');
                            udtPatient.strFirstName = strFullName.Substring(0, intSubstringLength);

                            // Middle
                            intSubstringLength = strFullName.LastIndexOf('.') - strFullName.IndexOf(' ') - 1;
                            udtPatient.strMiddleName = strFullName.Substring(strFullName.IndexOf(' ') + 1, intSubstringLength);

                            // Last
                            udtPatient.strLastName = strFullName.Substring(strFullName.LastIndexOf(' ') + 1);
                        }
                    }
                    // L, FM
                    else if (strFullName.IndexOf(',') > 0 && strFullName.LastIndexOf('.') != strFullName.Length - 1)
                    {
                        // Last
                        intSubstringLength = strFullName.IndexOf(',');
                        udtPatient.strLastName = strFullName.Substring(0, intSubstringLength).Trim();

                        // First
                        udtPatient.strFirstName = strFullName.Substring(intSubstringLength + 1, strFullName.LastIndexOf(' ') - intSubstringLength).Trim();

                        // Middle
                        udtPatient.strMiddleName = strFullName.Substring(strFullName.LastIndexOf(' ')).Trim();

                        //// First
                        //intSubstringLength = strFullName.IndexOf(' ');
                        //udtPatient.strFirstName = strFullName.Substring(0, intSubstringLength);

                        //// Middle
                        //intSubstringLength = strFullName.LastIndexOf('.') - strFullName.IndexOf(' ') - 1;
                        //udtPatient.strMiddleName = strFullName.Substring(strFullName.IndexOf(' ') + 1, intSubstringLength);

                        //// Last
                        //udtPatient.strLastName = strFullName.Substring(strFullName.LastIndexOf(' ') + 1);
                    }
                    // L, FM.
                    else if (strFullName.IndexOf(',') > 0 && strFullName.IndexOf('.') > 0)
                    {
                        // Last
                        intSubstringLength = strFullName.IndexOf(',');
                        udtPatient.strLastName = strFullName.Substring(0, intSubstringLength).Trim();

                        // First
                        udtPatient.strFirstName = strFullName.Substring(intSubstringLength + 1, strFullName.LastIndexOf(' ') - intSubstringLength).Trim();

                        // Middle
                        udtPatient.strMiddleName = strFullName.Substring(strFullName.LastIndexOf(' ')).Trim();
                    }
                    else
                    {
                        blnSuccess = false;
                    }

                }
                else if (intWordCount == 2)
                {
                    // FL 
                    if (strFullName.IndexOf(',') < 0)
                    {
                        // First
                        intSubstringLength = strFullName.IndexOf(' ');
                        udtPatient.strFirstName = strFullName.Substring(0, intSubstringLength);

                        // Last
                        udtPatient.strLastName = strFullName.Substring(intSubstringLength + 1);
                    }
                    // L, F
                    else if (strFullName.IndexOf(',') > 0)
                    {
                        // Last
                        intSubstringLength = strFullName.IndexOf(',');
                        udtPatient.strLastName = strFullName.Substring(0, intSubstringLength);

                        // First
                        udtPatient.strFirstName = strFullName.Substring(intSubstringLength + 1);

                    } 
                    else
                    {
                        blnSuccess = false;
                    }
                }
                else
                {
                    blnSuccess = false;
                }
                //string strFullName = string.Empty;
                //int intWordCount = 0;
                //int intSubstringLength = 0;

                //udtPatient = new udtPatientType();

                //// Get the full name out
                //strFullName = strFileLines.Substring(CConstants.FULL_NAME_START_LOCATION, CConstants.FULL_NAME_LENGTH);
                //strFullName = strFullName.Trim();
                //intWordCount = CountWordsInString(strFullName);

                //// First Middle Last
                //if(intWordCount == 3 && strFullName.IndexOf(',') < 0 )
                //{
                //    // First
                //    intSubstringLength = strFullName.IndexOf(' ');
                //    udtPatient.strFirstName = strFullName.Substring(0, intSubstringLength);

                //    // Middle
                //    intSubstringLength = strFullName.LastIndexOf(' ') - strFullName.IndexOf(' ') - 1;
                //    udtPatient.strMiddleName = strFullName.Substring(strFullName.IndexOf(' ') + 1, intSubstringLength);

                //    // Last
                //    udtPatient.strLastName = strFullName.Substring(strFullName.LastIndexOf(' ') + 1);
                //}
                //// Last, First Middle
                //else if(intWordCount == 3 && strFullName.IndexOf(',') > 0)
                //{
                //    // Last Name
                //    intSubstringLength = strFullName.IndexOf(',');
                //    udtPatient.strLastName = strFullName.Substring(0, intSubstringLength);

                //    // First Name
                //    intSubstringLength = strFullName.LastIndexOf(' ') - strFullName.IndexOf(',') - 1;
                //    udtPatient.strFirstName = strFullName.Substring(strFullName.IndexOf(' ') + 1, intSubstringLength - 1);

                //    // Middle Name
                //    udtPatient.strMiddleName = strFullName.Substring(strFullName.LastIndexOf(' ') + 1);

                //    // Senior or Junior
                //    if(udtPatient.strMiddleName == "Sr." || udtPatient.strMiddleName == "Jr.")
                //    {
                //        udtPatient.strLastName = udtPatient.strLastName + " " + udtPatient.strMiddleName;
                //        udtPatient.strMiddleName = string.Empty;
                //    }
                //}
                //// First Last
                //else
                //{
                //    // First
                //    intSubstringLength = strFullName.IndexOf(' ');
                //    udtPatient.strFirstName = strFullName.Substring(0, intSubstringLength);

                //    // Last
                //    udtPatient.strLastName = strFullName.Substring(strFullName.LastIndexOf(' ') + 1);
                //}

                // Proper Case
                CUtilities.ToProperCase(ref udtPatient.strFirstName);
                CUtilities.ToProperCase(ref udtPatient.strMiddleName);
                CUtilities.ToProperCase(ref udtPatient.strLastName);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnSuccess;
        }



        // --------------------------------------------------------------------------------
        // Name: CountWordsInString
        // Abstract: Count the words in a string
        // --------------------------------------------------------------------------------
        private static int CountWordsInString(string strStringToCount)
        {
            int intWordCount = 0;

            try
            {
                char[] strStringCharArrayCopy;
                int intIndex = 0;
                int intLength = 0;
                char chrCurrentLetter = (char)0;
                char chrPreviousLetter = ' ';

                // Get the length
                intLength = strStringToCount.Length;

                // Allocate Space
                strStringCharArrayCopy = new char[intLength + 1];

                // Convert to a char array
                strStringCharArrayCopy = strStringToCount.ToCharArray();

                for (intIndex = 0; intIndex < intLength; intIndex += 1)
                {

                    chrCurrentLetter = strStringCharArrayCopy[intIndex];

                    // Word Boundary?
                    if (chrPreviousLetter == ' ' && chrCurrentLetter != ' ')
                    {
                        // Yes, Count it
                        intWordCount += 1;
                    }

                    // Save current character
                    chrPreviousLetter = chrCurrentLetter;
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
            return intWordCount;
        }


        // --------------------------------------------------------------------------------
        // Name: ExtractHomeAddress
        // Abstract: Extract the street address
        // --------------------------------------------------------------------------------
        private static bool ExtractHomeAddress(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strHomeAddress = string.Empty;

                // Get the Home address out
                strHomeAddress = strFileLines.Substring(CConstants.STREET_ADDRESS_LOCATION, CConstants.STREET_ADDRESS_LENGTH);
                udtPatient.strStreetAddress = strHomeAddress.Trim();

                // To proper case
                CUtilities.ToProperCase(ref udtPatient.strStreetAddress);

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        // --------------------------------------------------------------------------------
        // Name: ExtractCity
        // Abstract: Extract the City
        // --------------------------------------------------------------------------------
        private static bool ExtractCity(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strCity = string.Empty;

                // Get the City out
                strCity = strFileLines.Substring(CConstants.CITY_LOCATION, CConstants.CITY_LENGTH);
                udtPatient.strCity = strCity.Trim();

                // To proper case
                CUtilities.ToProperCase(ref udtPatient.strCity);

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        // --------------------------------------------------------------------------------
        // Name: ExtractState
        // Abstract: Extract the State
        // --------------------------------------------------------------------------------
        private static bool ExtractState(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strState = string.Empty;
                int intStateID = 0;

                // Get the State out
                strState = strFileLines.Substring(CConstants.STATE_LOCATION, CConstants.STATE_LENGTH);
                strState = strState.Trim();

                // Capitalize it
                strState = strState.ToUpper();

                // Did it work?
                if (CDatabaseUtilities.GetStateID(strState, ref intStateID) == true)
                {
                    // Yes, assign the ID
                    udtPatient.intStateID = intStateID;
                }
                else
                {
                    blnResult = false;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        // --------------------------------------------------------------------------------
        // Name: ExtractZip
        // Abstract: Extract the Zip
        // --------------------------------------------------------------------------------
        private static bool ExtractZipCode(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strZipCode = string.Empty;

                // Get the Zip Code out
                strZipCode = strFileLines.Substring(CConstants.ZIP_LOCATION, CConstants.ZIP_LENGTH);
                udtPatient.strZipCode = strZipCode.Trim();

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: ExtractPrimaryPhone
        // Abstract: Extract the Primary Phone Number
        // --------------------------------------------------------------------------------
        private static bool ExtractPrimaryPhone(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strPrimaryPhoneNumber = string.Empty;

                // Get the primary phone number out
                strPrimaryPhoneNumber = strFileLines.Substring(CConstants.PRIMARY_PHONE_LOCATION, CConstants.PRIMARY_PHONE_LENGTH);
                udtPatient.strPrimaryPhoneNumber = strPrimaryPhoneNumber.Trim();

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        // --------------------------------------------------------------------------------
        // Name: ExtractSecondaryPhone
        // Abstract: Extract the Primary Phone Number
        // --------------------------------------------------------------------------------
        private static bool ExtractSecondaryPhone(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strSecondaryPhoneNumber = string.Empty;

                // Get the secondary phone number out
                strSecondaryPhoneNumber = strFileLines.Substring(CConstants.SECONDARY_PHONE_LOCATION, CConstants.SECONDARY_PHONE_LENGTH);
                udtPatient.strSecondaryPhoneNumber = strSecondaryPhoneNumber.Trim();

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: ExtractSmokerStatus
        // Abstract: Extract the Smoker Status
        // --------------------------------------------------------------------------------
        private static bool ExtractSmokerStatus(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strSmokerStatus = string.Empty;

                // Get the smoker status out
                strSmokerStatus = strFileLines.Substring(CConstants.SMOKER_LOCATION, CConstants.SMOKER_LENGTH);
                strSmokerStatus = strSmokerStatus.Trim();

                // Capitalize it
                strSmokerStatus = strSmokerStatus.ToUpper();

                // Smoker?
                if (strSmokerStatus == "Y")
                {
                    // Yes
                    udtPatient.blnSmoker = true;
                }
                else
                {
                    // No
                    udtPatient.blnSmoker = false;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: ExtractPackYears
        // Abstract: Extract the Pack Years
        // --------------------------------------------------------------------------------
        private static bool ExtractPackYears(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strPackYears = string.Empty;

                // Get the Pack years out
                strPackYears = strFileLines.Substring(CConstants.PACK_YEARS_LOCATION, CConstants.PACK_YEARS_LENGTH);
                strPackYears = strPackYears.Trim();

                udtPatient.decPackYears = (decimal)Conversion.Val(strPackYears);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: ExtractHeadOfHouseHoldStatus
        // Abstract: Extract the Head Of Household Status
        // --------------------------------------------------------------------------------
        private static bool ExtractHeadOfHouseHoldStatus(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strHouseHoldStatus = string.Empty;

                // Get the full name out
                strHouseHoldStatus = strFileLines.Substring(CConstants.HEAD_OF_HOUSEHOLD_LOCATION, CConstants.HEAD_OF_HOUSEHOLD_LENGTH);
                strHouseHoldStatus = strHouseHoldStatus.Trim();

                // Capitalize it
                strHouseHoldStatus = strHouseHoldStatus.ToUpper();

                // Assign it
                // Head of house hold ?
                if (strHouseHoldStatus == "Y")
                {
                    // Yes, true
                    udtPatient.blnHeadOfHouseHold = true;
                }
                else
                {
                    // No, false
                    udtPatient.blnHeadOfHouseHold = false;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: ExtractDateOfBirth
        // Abstract: Extract the Date Of Birth
        // --------------------------------------------------------------------------------
        private static bool ExtractDateOfBirth(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;
            string strFakeValue = "";

            if (udtPatient.intPatientID == 77)
            {
                strFakeValue = "gazorpazorpfield";
            }

            try
            {
                string strDateOfBirth = string.Empty;
                int intDateOfBirthLength = 0;
                string strMonth = string.Empty;
                string strDay = string.Empty;
                string strYear = string.Empty;
                int intYear = 0;
                string strMinimumSQLYear = SqlDateTime.MinValue.ToString();
                int intMinimumSQLYear = 0;

                // 1753
                strMinimumSQLYear = strMinimumSQLYear.Substring(4, 4);

                // Get the Birthday
                strDateOfBirth = strFileLines.Substring(CConstants.DATE_OF_BIRTH_LOCATION, CConstants.DATE_OF_BIRTH_LENGTH);
                strDateOfBirth = strDateOfBirth.Trim();

                // Dashes?
                if (strDateOfBirth.IndexOf('-') > 0)
                {
                    // Yes, Then slashes
                    strDateOfBirth = strDateOfBirth.Replace("-", "/");
                }

                // Any slashes?
                if (strDateOfBirth.IndexOf('/') < 0)
                {
                    // No, How long?
                    intDateOfBirthLength = strDateOfBirth.Length;

                    //MMDDYYYY      Add the slashes
                    if (intDateOfBirthLength == 8)
                    {
                        strMonth = strDateOfBirth.Substring(0, 2);
                        strDay = strDateOfBirth.Substring(2, 2);
                        strYear = strDateOfBirth.Substring(4);

                        // do a conversion
                        intYear = (int)Conversion.Val(strYear);

                        // Check validity, and correct if nessecary
                        blnResult = IsValidDateData(ref intYear, ref strMonth, ref strDay, ref strYear, ref strDateOfBirth);

                        strDateOfBirth = strMonth + "/" + strDay + "/" + strYear;
                    }
                    else
                    {
                        strMonth = "01";
                        strDay = "01";
                        strYear = strDateOfBirth.Substring(0);

                        // do a conversion
                        intYear = (int)Conversion.Val(strYear);

                        // Check validity, and correct if nessecary
                        blnResult = IsValidDateData(ref intYear, ref strMonth, ref strDay, ref strYear, ref strDateOfBirth);

                        strDateOfBirth = strMonth + "/" + strDay + "/" + strYear;
                    }
                }
                else
                {
                    // Extract the Year
                    strYear = strDateOfBirth.Substring(6);

                    // do a conversion
                    intYear = (int)Conversion.Val(strYear);
                    intMinimumSQLYear = (int)Conversion.Val(strMinimumSQLYear);

                    strMonth = strDateOfBirth.Substring(0, 2);
                    strDay = strDateOfBirth.Substring(3, 2);

                    // check validity, and correct if nessecary
                    blnResult = IsValidDateData(ref intYear, ref strMonth, ref strDay, ref strYear, ref strDateOfBirth);

                    strDateOfBirth = strMonth + "/" + strDay + "/" + strYear;

                }

                // was our dates good?
                if(blnResult == true)
                {
                    // yes, Assign it
                    udtPatient.dtmDateOfBirth = DateTime.Parse(strDateOfBirth);
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: IsValidDateData
        // Abstract: Checks if any dates imported are the correct date in leap year &
        //                  Checks to see if the year is below the minimum SQL year value,
        //                  then corrects these issues if necessary. 
        // --------------------------------------------------------------------------------
        private static bool IsValidDateData(ref int intYear, ref string strMonth, ref string strDay, ref string strYear, ref string strDateOfBirth)
        {
            bool blnResult = true;

            try
            {
                string strMinimumSQLYear = SqlDateTime.MinValue.ToString();
                int intSQLMinimumYear = 0;
                int intMonth = 0;
                int intDay = 0;

                // 1753
                strMinimumSQLYear = strMinimumSQLYear.Substring(4, 4);
                intSQLMinimumYear = (int)Conversion.Val(strMinimumSQLYear);
                intMonth = (int)Conversion.Val(strMonth);
                intDay = (int)Conversion.Val(strDay);


                // Smaller than SQL's minimum accepted year?
                if (intYear < intSQLMinimumYear)
                {
                    // Yes, change it to SQL's minimum year
                    intYear = intSQLMinimumYear;

                    // Add it to the string for later parsing
                    strYear = intYear.ToString();
                }

                // Is it a leap year?
                if (DateTime.IsLeapYear(intYear) == false)
                {
                    // No, Is it an invalid date
                    if (strMonth == "02" && intDay > 28 || strMonth == "2" && intDay > 28)
                    {
                        //// Yes, correct it
                        //strMonth = "03";
                        //strDay = "01";
                        //strDateOfBirth = strMonth + "/" + strDay + "/" + strYear;
                        blnResult = false;
                    }
                }

                // Right Days?
                if(intMonth == 1 || intMonth == 3 || intMonth == 5  || 
                   intMonth == 7 || intMonth == 8 || intMonth == 10 ||
                   intMonth == 12)
                {
                    // More than 31? 
                    if(intDay > 31)
                    {
                        //// Yes? then change it
                        //strDay = "31";
                        blnResult = false;
                    }
                }

                // More than 30?
                else if(intMonth != 2 && intDay > 30)
                {
                    //// Yes, then change it
                    //strDay = "30";
                    blnResult = false;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: ExtractRecordCreator
        // Abstract: Extract the Record Creator
        // --------------------------------------------------------------------------------
        private static bool ExtractRecordCreator(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strRecordCreator = string.Empty;
                udtRecordCreatorType udtRecordCreator = null;
                int intLength = 0;

                if(udtPatient.intPatientID == 25)
                {
                    strRecordCreator = "You Dun Goofed";
                }

                udtRecordCreator = new udtRecordCreatorType();

                intLength = strFileLines.Length;

                // Get the full name out
                strRecordCreator = strFileLines.Substring(CConstants.RECORD_CREATED_BY_LOCATION);

                // trim it
                strRecordCreator = strRecordCreator.Trim();

                // Assign the name
                udtRecordCreator.strRecordCreator = strRecordCreator;

                // Get the ID
                CDatabaseUtilities.GetRecordCreatorID(ref udtRecordCreator);

                // Assign the ID
                udtPatient.intRecordCreatorID = udtRecordCreator.intRecordCreatorID;

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: ExtractSex
        // Abstract: Extract the Sex
        // --------------------------------------------------------------------------------
        private static bool ExtractSex(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strSex = string.Empty;

                // Get the full name out
                strSex = strFileLines.Substring(CConstants.SEX_LOCATION, CConstants.SEX_LENGTH);

                // trim it
                strSex = strSex.Trim();

                // Capatilize it
                strSex = strSex.ToUpper();

                // Female?
                if (strSex == "F")
                {
                    // Yes, Assign it
                    udtPatient.intSexID = 1;
                }
                else
                {
                    // No, Male
                    udtPatient.intSexID = 2;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: ExtractEmailAddress
        // Abstract: Extract the Email Address
        // --------------------------------------------------------------------------------
        private static bool ExtractEmailAddress(ref udtPatientType udtPatient, string strFileLines)
        {
            bool blnResult = true;

            try
            {
                string strEmailAddress = string.Empty;

                // Get the Email Address out
                strEmailAddress = strFileLines.Substring(CConstants.EMAIL_ADDRESS_LOCATION, CConstants.EMAIL_ADDRESS_LENGTH);

                // trim it
                strEmailAddress = strEmailAddress.Trim();

                // Add it
                udtPatient.strEmailAddress = strEmailAddress;

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }
        #endregion

        #region Allergies

        // --------------------------------------------------------------------------------
        // Name: ExtractAllergies
        // Abstract: Extract the Extract Allergy 1
        // --------------------------------------------------------------------------------
        private static void ExtractAllergies(ref udtPatientType udtPatient, string strFileLines)
        {
            try
            {
                ExtractAllergy1(udtPatient, strFileLines);
                ExtractAllergy2(udtPatient, strFileLines);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }

            #region Allergy1
            // --------------------------------------------------------------------------------
            // Name: ExtractAllergy1
            // Abstract: Extract the Extract Allergy 1
            // --------------------------------------------------------------------------------
            private static void ExtractAllergy1(udtPatientType udtPatient, string strFileLines)
            {
                try
                {
                    string strAllergy1 = string.Empty;
                    udtAllergyType udtAllergy = null;

                    udtAllergy = new udtAllergyType();

                    // Get the allergy out
                    strAllergy1 = strFileLines.Substring(CConstants.ALLERGY_1_LOCATION, CConstants.ALLERGY_1_LENGTH);

                    // trim it
                    strAllergy1 = strAllergy1.Trim();

                    CUtilities.ToProperCase(ref strAllergy1);

                    // Is there an allergy?
                    if (strAllergy1 != "")
                    {
                        udtAllergy.strAllergy = strAllergy1;

                        // yes, get the ID and insert it into the database
                        CDatabaseUtilities.GetAllergyID(ref udtAllergy);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            //Yes, then associate this allergy with this patient
                            CDatabaseUtilities.AssignPatientAllergyInDatabase(udtPatient, udtAllergy);
                        }

                        // get the medication associated with this allergy
                        ExtractAllergy1Medication1(strFileLines, udtPatient, udtAllergy);
                        ExtractAllergy1Medication2(strFileLines, udtPatient, udtAllergy);

                    }
                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }

                #region Old ExtractAllergy
                //// --------------------------------------------------------------------------------
                //// Name: ExtractAllergy1
                //// Abstract: Extract the Extract Allergy 1
                //// --------------------------------------------------------------------------------
                //private void ExtractAllergy1(string strFileLines, udtPatientType udtPatient)
                //{
                //    try
                //    {
                //        string strAllergy1 = string.Empty;
                //        udtAllergyType udtAllergy = null;

                //        udtAllergy = new udtAllergyType();

                //        // Get the full name out
                //        strAllergy1 = strFileLines.Substring(CConstants.ALLERGY_1_LOCATION, CConstants.ALLERGY_1_LENGTH);

                //        // trim it
                //        strAllergy1 = strAllergy1.Trim();

                //        CUtilities.ToProperCase(ref strAllergy1);

                //        udtAllergy.strAllergy = strAllergy1;

                //        // Is there an allergy?
                //        if (udtAllergy.strAllergy != "")
                //        {
                //            // yes, get the ID
                //            CDatabaseUtilities.GetAllergyID(ref udtAllergy);

                //            // Add the relationship
                //            CDatabaseUtilities.AssignPatientAllergyInDatabase(udtPatient, udtAllergy);
                //        }
                //        else
                //        {
                //            udtAllergy.intAllergyID = 1;
                //        }

                //        // Extract Allergy 1's Medication 1
                //        ExtractAllergy1Medication1(strFileLines, udtPatient, ref udtAllergy);
                //    }
                //    catch (Exception excError)
                //    {
                //        CUtilities.WriteLog(excError);
                //    }
                //}
                #endregion


            // --------------------------------------------------------------------------------
            // Name: ExtractAllergy1Medication1
            // Abstract: Extract the Extract The medication associated with the first allergy
            // --------------------------------------------------------------------------------
            private static void ExtractAllergy1Medication1(string strFileLines, udtPatientType udtPatient, udtAllergyType udtAllergy)
            {
                try
                {
                    udtMedicationType udtMedication = new udtMedicationType();
                    string strAllergy1Medication1 = string.Empty;

                    // Get the full name out
                    strAllergy1Medication1 = strFileLines.Substring(CConstants.ALLERGY_1_MEDICATION_1_LOCATION, CConstants.ALLERGY_1_MEDICATION_1_LENGTH);

                    // trim it
                    strAllergy1Medication1 = strAllergy1Medication1.Trim();

                    CUtilities.ToProperCase(ref strAllergy1Medication1);

                    // Is there an allergy?
                    if (strAllergy1Medication1 != "")
                    {
                        udtMedication.strMedication = strAllergy1Medication1;

                        // Yes, Get the ID
                        CDatabaseUtilities.GetMedicationID(ref udtMedication);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientAllergyMedication(udtPatient, udtAllergy, udtMedication);
                        }
                    }
                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }



            // --------------------------------------------------------------------------------
            // Name: ExtractAllergy1Medication2
            // Abstract: Extract the Extract The medication associated with the second allergy
            // --------------------------------------------------------------------------------
            private static void ExtractAllergy1Medication2(string strFileLines, udtPatientType udtPatient, udtAllergyType udtAllergy)
            {
                try
                {
                    string strAllergy1Medication2 = string.Empty;
                    udtMedicationType udtMedication = new udtMedicationType();

                    // Get the full name out
                    strAllergy1Medication2 = strFileLines.Substring(CConstants.ALLERGY_1_MEDICATION_2_LOCATION, CConstants.ALLERGY_1_MEDICATION_2_LENGTH);

                    // trim it
                    strAllergy1Medication2 = strAllergy1Medication2.Trim();

                    CUtilities.ToProperCase(ref strAllergy1Medication2);

                    // Is there an allergy?
                    if (strAllergy1Medication2 != "")
                    {
                        udtMedication.strMedication = strAllergy1Medication2;

                        // Yes, Get the ID
                        CDatabaseUtilities.GetMedicationID(ref udtMedication);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientAllergyMedication(udtPatient, udtAllergy, udtMedication);
                        }

                    }

                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }
        #endregion

            #region Allergy2
            // --------------------------------------------------------------------------------
            // Name: ExtractAllergy2
            // Abstract: Extract the Extract Allergy 2
            // --------------------------------------------------------------------------------
            private static void ExtractAllergy2(udtPatientType udtPatient, string strFileLines)
            {
                try
                {
                    udtAllergyType udtAllergy = new udtAllergyType();
                    string strAllergy2 = string.Empty;

                    // Get the full name out
                    strAllergy2 = strFileLines.Substring(CConstants.ALLERGY_2_LOCATION, CConstants.ALLERGY_2_LENGTH);

                    // trim it
                    strAllergy2 = strAllergy2.Trim();

                    CUtilities.ToProperCase(ref strAllergy2);

                    // Is there an allergy?
                    if (strAllergy2 != "")
                    {
                        udtAllergy.strAllergy = strAllergy2;

                        // Yes, Get the ID
                        CDatabaseUtilities.GetAllergyID(ref udtAllergy);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientAllergyInDatabase(udtPatient, udtAllergy);
                        }

                        // Extract the Allergy medications
                        ExtractAllergy2Medication1(strFileLines, udtPatient, udtAllergy);
                        ExtractAllergy2Medication2(strFileLines, udtPatient, udtAllergy);
                    }

                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }



            // --------------------------------------------------------------------------------
            // Name: ExtractAllergy2Medication1
            // Abstract: Extract the Extract The medication associated with the second allergy
            // --------------------------------------------------------------------------------
            private static void ExtractAllergy2Medication1(string strFileLines, udtPatientType udtPatient, udtAllergyType udtAllergy)
            {
                try
                {
                    udtMedicationType udtMedication = null;
                    string strAllergy2Medication1 = string.Empty;

                    udtMedication = new udtMedicationType();

                    // Get the full name out
                    strAllergy2Medication1 = strFileLines.Substring(CConstants.ALLERGY_2_MEDICATION_1_LOCATION, CConstants.ALLERGY_2_MEDICATION_1_LENGTH);

                    // trim it
                    strAllergy2Medication1 = strAllergy2Medication1.Trim();

                    CUtilities.ToProperCase(ref strAllergy2Medication1);

                    // Is there an allergy?
                    if (strAllergy2Medication1 != "")
                    {
                        udtMedication.strMedication = strAllergy2Medication1;

                        // Yes, Get the ID
                        CDatabaseUtilities.GetMedicationID(ref udtMedication);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientAllergyMedication(udtPatient, udtAllergy, udtMedication);
                        }
                    }
                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }



            // --------------------------------------------------------------------------------
            // Name: ExtractAllergy2Medication2
            // Abstract: Extract the Extract The medication associated with the second allergy
            // --------------------------------------------------------------------------------
            private static void ExtractAllergy2Medication2(string strFileLines, udtPatientType udtPatient, udtAllergyType udtAllergy)
            {
                try
                {
                    string strAllergy2Medication2 = string.Empty;
                    udtMedicationType udtMedication = new udtMedicationType();

                    // Get the full name out
                    strAllergy2Medication2 = strFileLines.Substring(CConstants.ALLERGY_2_MEDICATION_2_LOCATION, CConstants.ALLERGY_2_MEDICATION_2_LENGTH);

                    // trim it
                    strAllergy2Medication2 = strAllergy2Medication2.Trim();

                    CUtilities.ToProperCase(ref strAllergy2Medication2);

                    // Is there an allergy?
                    if (strAllergy2Medication2 != "")
                    {
                        udtMedication.strMedication = strAllergy2Medication2;

                        // Yes, Get the ID
                        CDatabaseUtilities.GetMedicationID(ref udtMedication);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientAllergyMedication(udtPatient, udtAllergy, udtMedication);
                        }
                    }

                    // Extract Primary Phone Number
                    // ExtractProcedure1(strFileLines, udtPatient);
                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }
            #endregion

        #endregion

        #region Conditions

        // --------------------------------------------------------------------------------
        // Name: ExtractConditions
        // Abstract: Extract the Conditions
        // --------------------------------------------------------------------------------
        private static void ExtractConditions(udtPatientType udtPatient, string strFileLines)
        {
            try
            {
                ExtractCondition1(udtPatient, strFileLines);
                ExtractCondition2(udtPatient, strFileLines);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }

            #region Condition1
            // --------------------------------------------------------------------------------
            // Name: ExtractCondition1
            // Abstract: Extract the Extract Condition 1
            // --------------------------------------------------------------------------------
            private static void ExtractCondition1(udtPatientType udtPatient, string strFileLines)
            {
                try
                {
                    string strCondition1 = string.Empty;
                    udtConditionType udtCondition = null;

                    udtCondition = new udtConditionType();

                    // Get the full name out
                    strCondition1 = strFileLines.Substring(CConstants.CONDITION_1_LOCATION, CConstants.CONDITION_1_LENGTH);

                    // trim it
                    strCondition1 = strCondition1.Trim();

                    CUtilities.ToProperCase(ref strCondition1);

                    udtCondition.strCondition = strCondition1;

                    // Is there an Condition?
                    if (udtCondition.strCondition != "")
                    {
                        // yes, get the ID
                        CDatabaseUtilities.GetConditionID(ref udtCondition);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientConditionInDatabase(udtPatient, udtCondition);
                        }

                        // Extract  Medications
                        ExtractCondition1Medication1(strFileLines, udtPatient, udtCondition);
                        ExtractCondition1Medication2(strFileLines, udtPatient, udtCondition);
                    }

                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }



            // --------------------------------------------------------------------------------
            // Name: ExtractCondition1Medication1
            // Abstract: Extract the Extract The medication associated with the first Condition
            // --------------------------------------------------------------------------------
            private static void ExtractCondition1Medication1(string strFileLines, udtPatientType udtPatient, udtConditionType udtCondition)
            {
                try
                {
                    udtMedicationType udtMedication = null;
                    string strCondition1Medication1 = string.Empty;

                    udtMedication = new udtMedicationType();

                    // Get the full name out
                    strCondition1Medication1 = strFileLines.Substring(CConstants.CONDITION_1_MEDICATION_1_LOCATION, CConstants.CONDITION_1_MEDICATION_1_LENGTH);

                    // trim it
                    strCondition1Medication1 = strCondition1Medication1.Trim();

                    CUtilities.ToProperCase(ref strCondition1Medication1);

                    udtMedication.strMedication = strCondition1Medication1;

                    // Is there an Condition?
                    if (udtMedication.strMedication != "")
                    {
                        // Yes, Get the ID
                        CDatabaseUtilities.GetMedicationID(ref udtMedication);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientConditionMedication(udtPatient, udtCondition, udtMedication);
                        }
                    }

                    // Extract Primary Phone Number
                    //ExtractCondition1Medication2(strFileLines, udtCondition, udtPatient, ref udtMedication);
                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }



            // --------------------------------------------------------------------------------
            // Name: ExtractCondition1Medication2
            // Abstract: Extract the Extract The medication associated with the second Condition
            // --------------------------------------------------------------------------------
            private static void ExtractCondition1Medication2(string strFileLines, udtPatientType udtPatient, udtConditionType udtCondition)
            {
                try
                {
                    udtMedicationType udtMedication = null;
                    string strCondition1Medication2 = string.Empty;

                    udtMedication = new udtMedicationType();

                    // Get the full name out
                    strCondition1Medication2 = strFileLines.Substring(CConstants.CONDITION_1_MEDICATION_2_LOCATION, CConstants.CONDITION_1_MEDICATION_2_LENGTH);

                    // trim it
                    strCondition1Medication2 = strCondition1Medication2.Trim();

                    CUtilities.ToProperCase(ref strCondition1Medication2);

                    udtMedication.strMedication = strCondition1Medication2;

                    // Is there an Condition?
                    if (udtMedication.strMedication != "")
                    {
                        // Yes, Get the ID
                        CDatabaseUtilities.GetMedicationID(ref udtMedication);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientConditionMedication(udtPatient, udtCondition, udtMedication);
                        }
                    }

                    // Extract Primary Phone Number
                    //ExtractCondition2(strFileLines, udtPatient, ref udtCondition);
                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }
            #endregion

            #region Condition2
            // --------------------------------------------------------------------------------
            // Name: ExtractCondition2
            // Abstract: Extract the Extract Condition 1
            // --------------------------------------------------------------------------------
            private static void ExtractCondition2(udtPatientType udtPatient, string strFileLines)
            {
                try
                {
                    string strCondition2 = string.Empty;
                    udtConditionType udtCondition = null;

                    udtCondition = new udtConditionType();

                    // Get the full name out
                    strCondition2 = strFileLines.Substring(CConstants.CONDITION_2_LOCATION, CConstants.CONDITION_2_LENGTH);

                    // trim it
                    strCondition2 = strCondition2.Trim();

                    CUtilities.ToProperCase(ref strCondition2);

                    udtCondition.strCondition = strCondition2;

                    // Is there an Condition?
                    if (udtCondition.strCondition != "")
                    {
                        // yes, get the ID
                        CDatabaseUtilities.GetConditionID(ref udtCondition);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientConditionInDatabase(udtPatient, udtCondition);
                        }

                        // Extract  Medications
                        ExtractCondition2Medication1(strFileLines, udtPatient, udtCondition);
                        ExtractCondition2Medication2(strFileLines, udtPatient, udtCondition);
                    }

                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }



            // --------------------------------------------------------------------------------
            // Name: ExtractCondition2Medication1
            // Abstract: Extract the Extract The medication associated with the first Condition
            // --------------------------------------------------------------------------------
            private static void ExtractCondition2Medication1(string strFileLines, udtPatientType udtPatient, udtConditionType udtCondition)
            {
                try
                {
                    udtMedicationType udtMedication = null;
                    string strCondition2Medication1 = string.Empty;

                    udtMedication = new udtMedicationType();

                    // Get the full name out
                    strCondition2Medication1 = strFileLines.Substring(CConstants.CONDITION_2_MEDICATION_1_LOCATION, CConstants.CONDITION_2_MEDICATION_1_LENGTH);

                    // trim it
                    strCondition2Medication1 = strCondition2Medication1.Trim();

                    CUtilities.ToProperCase(ref strCondition2Medication1);

                    udtMedication.strMedication = strCondition2Medication1;

                    // Is there an Condition?
                    if (udtMedication.strMedication != "")
                    {
                        // Yes, Get the ID
                        CDatabaseUtilities.GetMedicationID(ref udtMedication);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientConditionMedication(udtPatient, udtCondition, udtMedication);
                        }
                    }

                    // Extract Primary Phone Number
                    //ExtractCondition2Medication2(strFileLines, udtCondition, udtPatient, ref udtMedication);
                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }



            // --------------------------------------------------------------------------------
            // Name: ExtractCondition2Medication2
            // Abstract: Extract the Extract The medication associated with the second Condition
            // --------------------------------------------------------------------------------
            private static void ExtractCondition2Medication2(string strFileLines, udtPatientType udtPatient, udtConditionType udtCondition)
            {
                try
                {
                    udtMedicationType udtMedication = null;
                    string strCondition2Medication2 = string.Empty;

                    udtMedication = new udtMedicationType();

                    // Get the full name out
                    strCondition2Medication2 = strFileLines.Substring(CConstants.CONDITION_2_MEDICATION_2_LOCATION, CConstants.CONDITION_2_MEDICATION_2_LENGTH);

                    // trim it
                    strCondition2Medication2 = strCondition2Medication2.Trim();

                    CUtilities.ToProperCase(ref strCondition2Medication2);

                    udtMedication.strMedication = strCondition2Medication2;

                    // Is there an Condition?
                    if (udtMedication.strMedication != "")
                    {
                        // Yes, Get the ID
                        CDatabaseUtilities.GetMedicationID(ref udtMedication);

                        // Was the patient inserted into the Patients table
                        if (blnPatientInDatabase == true)
                        {
                            // Add the relationship
                            CDatabaseUtilities.AssignPatientConditionMedication(udtPatient, udtCondition, udtMedication);
                        }
                    }

                    // Extract Primary Phone Number
                    //ExtractCondition2(strFileLines, udtPatient, ref udtCondition);
                }
                catch (Exception excError)
                {
                    CUtilities.WriteLog(excError);
                }
            }
            #endregion

        #endregion

        #region Procedures
        // --------------------------------------------------------------------------------
        // Name: ExtractProcedures
        // Abstract: Extract the Procedures
        // --------------------------------------------------------------------------------
        private static void ExtractProcedures(udtPatientType udtPatient, string strFileLines)
        {
            try
            {
                ExtractProcedure1(udtPatient, strFileLines);
                ExtractProcedure2(udtPatient, strFileLines);
                ExtractProcedure3(udtPatient, strFileLines);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }

        // --------------------------------------------------------------------------------
        // Name: ExtractProcedure1
        // Abstract: Extract the first procedure
        // --------------------------------------------------------------------------------
        private static void ExtractProcedure1(udtPatientType udtPatient, string strFileLines)
        {
            try
            {
                string strProcedure = string.Empty;
                udtProcedureType udtProcedure = null;

                udtProcedure = new udtProcedureType();

                // Get the full name out
                strProcedure = strFileLines.Substring(CConstants.PROCEDURE_1_LOCATION, CConstants.PROCEDURE_1_LENGTH);

                // trim it
                strProcedure = strProcedure.Trim();

                CUtilities.ToProperCase(ref strProcedure);

                udtProcedure.strProcedure = strProcedure;

                // Is there an procedure?
                if (udtProcedure.strProcedure != "")
                {
                    // Yes, Get the ID
                    CDatabaseUtilities.GetProcedureID(ref udtProcedure);

                    if (blnPatientInDatabase == true)
                    {
                        // Add the relationship
                        CDatabaseUtilities.AssignPatientProcedure(udtPatient, udtProcedure);
                    }
                }


                // Extract Primary Phone Number
                //ExtractProcedure2(strFileLines, udtPatient, ref udtProcedure);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }


        // --------------------------------------------------------------------------------
        // Name: ExtractProcedure2
        // Abstract: Extract the second procedure
        // --------------------------------------------------------------------------------
        private static void ExtractProcedure2(udtPatientType udtPatient, string strFileLines)
        {
            try
            {
                string strProcedure = string.Empty;
                udtProcedureType udtProcedure = null;

                udtProcedure = new udtProcedureType();

                // Get the full name out
                strProcedure = strFileLines.Substring(CConstants.PROCEDURE_2_LOCATION, CConstants.PROCEDURE_2_LENGTH);

                // trim it
                strProcedure = strProcedure.Trim();

                CUtilities.ToProperCase(ref strProcedure);

                udtProcedure.strProcedure = strProcedure;

                // Is there an procedure?
                if (udtProcedure.strProcedure != "")
                {
                    // Yes, Get the ID
                    CDatabaseUtilities.GetProcedureID(ref udtProcedure);

                    if (blnPatientInDatabase == true)
                    {
                        // Add the relationship
                        CDatabaseUtilities.AssignPatientProcedure(udtPatient, udtProcedure);
                    }
                }

                // Extract Primary Phone Number
                //ExtractProcedure3(strFileLines, udtPatient, ref udtProcedure);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: ExtractProcedure3
        // Abstract: Extract the third procedure
        // --------------------------------------------------------------------------------
        private static void ExtractProcedure3(udtPatientType udtPatient, string strFileLines)
        {
            try
            {
                string strProcedure = string.Empty;
                udtProcedureType udtProcedure = null;

                udtProcedure = new udtProcedureType();

                // Get the full name out
                strProcedure = strFileLines.Substring(CConstants.PROCEDURE_3_LOCATION, CConstants.PROCEDURE_3_LENGTH);

                // trim it
                strProcedure = strProcedure.Trim();

                CUtilities.ToProperCase(ref strProcedure);

                udtProcedure.strProcedure = strProcedure;

                // Is there an procedure?
                if (udtProcedure.strProcedure != "")
                {
                    // Yes, Get the ID
                    CDatabaseUtilities.GetProcedureID(ref udtProcedure);

                    if (blnPatientInDatabase == true)
                    {
                        // Add the relationship
                        CDatabaseUtilities.AssignPatientProcedure(udtPatient, udtProcedure);
                    }
                }

                if (udtPatient.intPatientID == 26)
                {

                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }
        #endregion
    }

}

