using Microsoft.VisualBasic;
using System;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.IO;

namespace nsImportAndExportAllInOne
{

    // --------------------------------------------------------------------------------
    // Name: 
    // Abstract:
    // --------------------------------------------------------------------------------
    class CExportUtilities
    {
        // --------------------------------------------------------------------------------
        // Name: ExportPatientRecords
        // Abstract: Performs import procedures
        // --------------------------------------------------------------------------------
        public static bool ExportPatientRecords(string strFilePath)
        {
            bool blnResult = false;

            try
            {
                int intRecordCount = 0;
                int intIndex = 0;
                string strSelect = string.Empty;
                string[] strPatientData;

                // We use the TPatients Table becasue the number of lines in the Export document directly relates to the number of records in the parent table TPatients
                intRecordCount = CDatabaseUtilities.NumberofRecordsInTable("TPatients");

                // Create a string array of that size
                strPatientData = new string[intRecordCount];

                // We dont want to use the below method, thats why it is commented out. Using this method removes at least 207 calls to the database.
                // thus reducing complexity and system resource usage
                CDatabaseUtilities.PatientData(ref strPatientData);

                //strPatientData[intIndex] += CDatabaseUtilities.AllergyData(intIndex);
                //strPatientData[intIndex] += CDatabaseUtilities.AllergyMedicationData(intIndex);
                //strPatientData[intIndex] += CDatabaseUtilities.PatientConditionData(intIndex);
                //strPatientData[intIndex] += CDatabaseUtilities.ConditionMedicationData(intIndex);
                //strPatientData[intIndex] += CDatabaseUtilities.ProcedureData(intIndex);







                //// Read in each patient to the string array
                ////      Space each with the required number of spaces as laid out in the Import document
                //for (intIndex = 0; intIndex < intRecordCount; intIndex +=1)
                //{

                //    // We pass the index with each procedure call because the index is directly related to the ID of the patient.
                //    // It is generally bad practice to have an index to represent any kind of real world data outside of just counting or iterating
                //    // but in this case I believe it is okay since the number of records being Exported is finite, and any solution using anything 
                //    // other than the index would be convoluted and introduce complexity unnessecarily
                //    strPatientData[intIndex]  = CDatabaseUtilities.PatientData(intIndex);
                //    strPatientData[intIndex] += CDatabaseUtilities.AllergyData(intIndex);
                //    strPatientData[intIndex] += CDatabaseUtilities.AllergyMedicationData(intIndex);
                //    strPatientData[intIndex] += CDatabaseUtilities.PatientConditionData(intIndex);
                //    strPatientData[intIndex] += CDatabaseUtilities.ConditionMedicationData(intIndex);
                //    strPatientData[intIndex] += CDatabaseUtilities.ProcedureData(intIndex);
                //}

                /*        public const int FULL_NAME_START_LOCATION                   = 0;
                          public const int STREET_ADDRESS_LOCATION                    = 30;
                          public const int CITY_LOCATION                              = 50;
                          public const int STATE_LOCATION                             = 70;
                          public const int ZIP_LOCATION                               = 72;
                          public const int PRIMARY_PHONE_LOCATION                     = 81;
                          public const int SECONDARY_PHONE_LOCATION                   = 91;
                          public const int SMOKER_LOCATION                            = 106;          // SMOKER_STATUS_LOCATION
                          public const int PACK_YEARS_LOCATION                        = 107;
                          public const int HEAD_OF_HOUSEHOLD_LOCATION                 = 110;
                          public const int DATE_OF_BIRTH_LOCATION                     = 111;
                          public const int SEX_LOCATION                               = 122;
                          public const int ALLERGY_1_LOCATION                         = 123;
                          public const int ALLERGY_2_LOCATION                         = 139;
                          public const int ALLERGY_1_MEDICATION_1_LOCATION            = 154;
                          public const int ALLERGY_1_MEDICATION_2_LOCATION            = 169;
                          public const int ALLERGY_2_MEDICATION_1_LOCATION            = 184;
                          public const int ALLERGY_2_MEDICATION_2_LOCATION            = 199; */

                // Read each patients Allergy Data
                //      If Null insert the number of space needed in order to mirror the Import document
                //      Else Still insert the number of spaces required to mirror the Import document

                // Read each patients Allergy medication data
                //      If Null insert the number of space needed in order to mirror the Import document 
                //      Else still insert the number of spaces required to mirror the Import document

                // Read each patients Condition data
                //      If Null insert the number of space needed in order to mirror the Import document 
                //      Else still insert the number of spaces required to mirror the Import document

                // Read each patients Condition medication data
                //      If Null insert the number of space needed in order to mirror the Import document 
                //      Else still insert the number of spaces required to mirror the Import document

                // Read each procedure data
                //      If Null insert the number of space needed in order to mirror the Import document 
                //      Else still insert the number of spaces required to mirror the Import document

                // Write the Line to a Text Document

                // Move to the next record
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }
    }
}
